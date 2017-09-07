using System;
using System.Collections.Generic;
using System.Linq;
using PublishSubscribe;
using PublishSubscribe.IntraProcessPublishSubscribe;

namespace ChoiceMaster
{
    public class DecisionModel : ISubscriber<string>, IPublisher<string>, IDisposable
    {
        public class OrderedCriteriaChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(DecisionModel)+"."+nameof(OrderedCriteriaChange);

            public ICollection<Criterion> NewOrderedCriteria
            {
                get;
                private set;
            }
            public OrderedCriteriaChange(ICollection<Criterion> newOrderedCriteria)
            :
                base(SubjectIdentifier)
            {
                NewOrderedCriteria = newOrderedCriteria.ToList();
            }
        }
        public class ScoreChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(DecisionModel)+"."+nameof(ScoreChange);

            public double NewScore
            {
                get;
                private set;
            }
            public ScoreChange(double newScore)
            :
                base(SubjectIdentifier)
            {
                NewScore = newScore;
            }
        }
        public class DuplicateCriterionError : Exception
        {
        }
        public class UnknownCriterionError : Exception
        {
        }
        string IPublisher<string>.Name
        {
            get
            {
                return "DecisionModelPublisher";
            }
        }
        string ISubscriber<string>.Name
        {
            get
            {
                return "DecisionModelSubscriber";
            }
        }
        public SortedDictionary<string, CriterionOrdering> CriterionOrderings { get; protected set; }
        private LinkedList<Criterion> orderedCriteria;
        public LinkedList<Criterion> OrderedCriteria
        {
            get
            {
                return orderedCriteria;
            }

            set
            {
                if (value != orderedCriteria)
                {
                    orderedCriteria = value;

                    MyBroker.Instance.Publish(this, new OrderedCriteriaChange(orderedCriteria));
                }
            }
        }
        public SortedDictionary<string, Criterion> Criteria { get; protected set; }

        private bool isNormalizing = false;

        public double score = 0.0;
        public double Score
        {
            get
            {
                return score;
            }
            protected set
            {
                if (value != score)
                {
                    score = value;

                    MyBroker.Instance.Publish(this, new ScoreChange(score));
                }
            }
        }
        public double Scale { get; protected set; }
        public string Name { get; protected set; }

        private const double DefaultScale = 1000.0;

        public DecisionModel(
                string name,
                double scale = DefaultScale)
        {
            MyBroker.Instance.Register(this, DecisionModel.OrderedCriteriaChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, DecisionModel.ScoreChange.SubjectIdentifier);
            Name = name;
            Scale = scale <= 0.0 ? DefaultScale : scale;
            CriterionOrderings = new SortedDictionary<string, CriterionOrdering>();
            Criteria = new SortedDictionary<string, Criterion>();
            OrderedCriteria = new LinkedList<Criterion>();
        }
        public void
        CreateCriterion(
                string name,
                string description,
                Scorer scorer)
        {
            if (Criteria.ContainsKey(name))
            {
                throw new DuplicateCriterionError();
            }

            Criterion c = new Criterion(CriterionOrderings, name, description, scorer);

            MyBroker.Instance.Subscribe(this, Scorer.RatingChange.SubjectIdentifier, c);
            MyBroker.Instance.Subscribe(this, Criterion.RelatedCriterionChange.SubjectIdentifier, c);
            MyBroker.Instance.Subscribe(this, Criterion.RelationToChange.SubjectIdentifier, c);
            MyBroker.Instance.Subscribe(this, Criterion.ScorerChange.SubjectIdentifier, c);

            Criteria.Add(
                name,
                c);
        }
        public void
        DeleteCriterion(
            string name)
        {
            if (!Criteria.ContainsKey(name))
            {
                return;
            }

            Criterion c = Criteria[name];

            Criteria.Remove(name);

            MyBroker.Instance.Unsubscribe(this, c);

            OrderedCriteria.Remove(c);

            c.RemoveCriterionOrdering();

            c.Dispose();

            NormalizeCriteria();
        }
        public void
        CreateCriterionOrdering(
            string name1,
            CriterionOrdering.OrderingRelation relation,
            string name2)
        {
            if (!Criteria.ContainsKey(name1))
            {
                throw new UnknownCriterionError();
            }

            if (!Criteria.ContainsKey(name2))
            {
                throw new UnknownCriterionError();
            }

            Criteria[name1].AddCriterionOrdering(
                relation,
                Criteria[name2]);
        }
        public void
        DeleteCriterionOrdering(
            string name)
        {
            if (!Criteria.ContainsKey(name))
            {
                throw new UnknownCriterionError();
            }

            Criteria[name].RemoveCriterionOrdering();
        }
        public void
        NormalizeCriteria()
        {
            if (!isNormalizing)
            {
                isNormalizing = true;

                LinkedList<Criterion> orderedCriteria = new LinkedList<Criterion>();           
                    
                foreach (var kvp in Criteria)
                {
                    orderedCriteria.AddLast(
                            kvp.Value);
                }

                orderedCriteria = new LinkedList<Criterion>(orderedCriteria.OrderByDescending(x => x));

                AdjustWeights(
                    orderedCriteria);

                double score =
                    ComputeScore(
                        orderedCriteria);

                OrderedCriteria = orderedCriteria;
                Score           = score;

                isNormalizing = false;
            }
        }
        public Scorer GetScorer(
            string name)
        {
            if (!Criteria.ContainsKey(name))
            {
                throw new UnknownCriterionError();
            }

            return Criteria[name].Scorer;
        }
        protected void AdjustWeights(
           LinkedList<Criterion> orderedCriteria)
        {
            var weights = new double[orderedCriteria.Count];
            double currentWeight = 1.0;
            double totalWeight = 1.0;

            // compute relative weights of each criteria based on its ordering relationship
            // this assumes OrderedCriteria is ordered by decreasing weight

            weights[0] = currentWeight;
            int i = 1;
            var ascendingOrderedCriteria = orderedCriteria.Reverse<Criterion>();

            var last = ascendingOrderedCriteria.ElementAt<Criterion>(0);

            while (i < ascendingOrderedCriteria.Count<Criterion>())
            {
                Criterion current = ascendingOrderedCriteria.ElementAt<Criterion>(i);

                var relation = current.GetRelationTo(last);

                currentWeight += (double)relation;

                weights[i] = currentWeight;
                totalWeight += currentWeight;

                last = current;
                i++;
            }

            // Now go back an compute normalized weight so value is between 0 and 1

            i = 0;

            while (i < ascendingOrderedCriteria.Count<Criterion>())
            {
                ascendingOrderedCriteria.ElementAt<Criterion>(i).Weight = weights[i] / totalWeight;
                i++;
            }
        }
        protected double ComputeScore(
            LinkedList<Criterion> orderedCriteria)
        {
            double score = 0.0;

            foreach (var kwp in orderedCriteria)
            {
                score += kwp.Weight * Scale * kwp.Scorer.Rating;
            }

            return score;
        }
        public void Notify(ISubject<string> subject, IPublisher<string> publisher)
        {
            NormalizeCriteria();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    foreach (var pair in Criteria)
                    {
                        MyBroker.Instance.Unsubscribe(this, Scorer.RatingChange.SubjectIdentifier, pair.Value);
                        MyBroker.Instance.Unsubscribe(this, Criterion.RelatedCriterionChange.SubjectIdentifier, pair.Value);
                        MyBroker.Instance.Unsubscribe(this, Criterion.RelationToChange.SubjectIdentifier, pair.Value);
                        MyBroker.Instance.Unsubscribe(this, Criterion.ScorerChange.SubjectIdentifier, pair.Value);

                        pair.Value.Dispose();
                    }

                    MyBroker.Instance.Unregister(this, DecisionModel.OrderedCriteriaChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, DecisionModel.ScoreChange.SubjectIdentifier);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DecisionModel() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
