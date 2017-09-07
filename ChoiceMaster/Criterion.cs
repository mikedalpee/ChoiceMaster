using System;
using System.Collections.Generic;
using PublishSubscribe;
using PublishSubscribe.IntraProcessPublishSubscribe;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class Criterion : ISubscriber<string>, IPublisher<string>, IComparable,IDisposable
    {
        public class RelationshipError : Exception
        {
        }
        public class WeightChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(WeightChange);

            public double NewWeight
            {
                get;
                private set;
            }
            public WeightChange(double newWeight)
            :
                base(SubjectIdentifier)
            {
                NewWeight = newWeight;
            }
        }
        private double weight = 0.0;
        public double Weight
        {
            get
            {
                return weight;
            }

            set
            {
                if (value != weight)
                {
                    weight = value;

                    MyBroker.Instance.Publish(this, new WeightChange(value));
                }
            }
        }
        public class NameChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(NameChange);

            public string NewName
            {
                get;
                private set;
            }
            public NameChange(string newName)
            :
                base(SubjectIdentifier)
            {
                NewName = newName;
            }
        }
        string IPublisher<string>.Name
        {
            get
            {
                return "CriterionPublisher";
            }
        }
        string ISubscriber<string>.Name
        {
            get
            {
                return "CriterionSubscriber";
            }
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value != name)
                {
                    name = value;

                    MyBroker.Instance.Publish(this, new NameChange(value));
                }
            }
        }
        public class DescriptionChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(DescriptionChange);

            public string NewDescription
            {
                get;
                private set;
            }
            public DescriptionChange(string newDescription)
            :
                base(SubjectIdentifier)
            {
                NewDescription = newDescription;
            }
        }
        private string description;
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                if (value != description)
                {
                    description = value;

                    MyBroker.Instance.Publish(this, new DescriptionChange(value));
                }
            }
        }
        public class ScorerChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(ScorerChange);

            public Scorer NewScorer
            {
                get;
                private set;
            }
            public ScorerChange(Scorer newScorer)
            :
                base(SubjectIdentifier)
            {
                NewScorer = newScorer;
            }
        }
        private Scorer scorer;
        public Scorer Scorer
        {
            get
            {
                return scorer;
            }

            set
            {
                if (value != scorer)
                {
                    scorer = value;

                    MyBroker.Instance.Publish(this, new ScorerChange(value));
                }
            }
        }
        public CriterionOrdering CriterionOrdering
        {
            get
            {
                if (!CriterionOrderings.ContainsKey(Name))
                {
                    return new CriterionOrdering(null,CriterionOrdering.OrderingRelation.IsInvalid,null);
                }

                return CriterionOrderings[Name];
            }
        }
        public class RelationToChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(RelationToChange);

            public Criterion NewRelationTo
            {
                get;
                private set;
            }
            public RelationToChange(Criterion newRelationTo)
            :
                base(SubjectIdentifier)
            {
                NewRelationTo = newRelationTo;
            }
        }
        public CriterionOrdering.OrderingRelation RelationTo
        {
            get
            {
                if (CriterionOrderings == null || !CriterionOrderings.ContainsKey(Name))
                {
                    return CriterionOrdering.OrderingRelation.IsInvalid;
                }

                return CriterionOrderings[Name].Relation;
            }
            set
            {
                if (CriterionOrderings != null && CriterionOrderings.ContainsKey(Name))
                {
                    CriterionOrdering co = CriterionOrderings[Name];

                    if (value != co.Relation)
                    {
                        co.Relation = value;

                        MyBroker.Instance.Publish(this, new RelationToChange(this));
                    }
                }
            }
        }
        public class RelatedCriterionChange : Subject<string>
        {
            public const string SubjectIdentifier = nameof(Criterion) + "." + nameof(RelatedCriterionChange);

            public Criterion NewRelatedCriterion
            {
                get;
                private set;
            }
            public RelatedCriterionChange(Criterion newRelatedCriterion)
            :
                base(SubjectIdentifier)
            {
                NewRelatedCriterion = newRelatedCriterion;
            }
        }
        public Criterion RelatedCriterion
        {
            get
            {
                if (!CriterionOrderings.ContainsKey(Name))
                {
                    return null;
                }

                return CriterionOrderings[Name].Right;

            }
            set
            {
                if (CriterionOrderings.ContainsKey(Name))
                {
                    CriterionOrdering co = CriterionOrderings[Name];

                    if (value != co.Right)
                    {
                        co.Right = value;

                        MyBroker.Instance.Publish(this, new RelatedCriterionChange(this));
                    }
                }
            }
        }
        public SortedDictionary<string, CriterionOrdering> CriterionOrderings { get; protected set; }
        public Criterion(
                SortedDictionary<string, CriterionOrdering> criterionOrderings,
                string name,
                string description,
                Scorer scorer)
        {
            MyBroker.Instance.Register(this, Criterion.DescriptionChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Criterion.NameChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Criterion.RelatedCriterionChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Criterion.RelationToChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Criterion.ScorerChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Criterion.WeightChange.SubjectIdentifier);
            MyBroker.Instance.Register(this, Scorer.RatingChange.SubjectIdentifier);
            Name = name;
            Description = description;
            CriterionOrderings = criterionOrderings;
            Scorer = scorer;
            MyBroker.Instance.Subscribe(this, Scorer.RatingChange.SubjectIdentifier, Scorer);
        }
        public void AddCriterionOrdering(
            CriterionOrdering.OrderingRelation relation,
            Criterion criterion)
        {
            if (!CriterionOrderings.Equals(criterion.CriterionOrderings))
            {
                throw new RelationshipError();
            }

            if (CriterionOrderings.ContainsKey(Name))
            {
                throw new RelationshipError();
            }

            CriterionOrderings.Add(
                Name,
                new CriterionOrdering(this, relation, criterion));
       }
        public void RemoveCriterionOrdering()
        {
            if (CriterionOrderings.ContainsKey(Name))
            {
                CriterionOrderings.Remove(
                    Name);
            }
        }
        public CriterionOrdering GetCriterionOrdering(
            string name)
        {
            if (!CriterionOrderings.ContainsKey(name))
            {
                throw new RelationshipError();
            }

            return CriterionOrderings[name];
        }
        public CriterionOrdering.OrderingRelation GetRelationTo(
            Criterion c)
        {
            if (CriterionOrderings != c.CriterionOrderings)
            {
                throw new RelationshipError();
            }

            // First check relation between this and c

            CriterionOrdering.OrderingRelation greatestRelation = CriterionOrdering.OrderingRelation.IsEqualTo;

            string Next = Name;

            while (true)
            {
                if (!CriterionOrderings.ContainsKey(Next))
                {
                    break;
                }

                CriterionOrdering co = CriterionOrderings[Next];

                if (co.Relation > greatestRelation)
                {
                    greatestRelation = co.Relation;
                }

                if (co.Right.Name == c.Name)
                {
                    return greatestRelation;
                }

                Next = co.Right.Name;
            }

            throw new RelationshipError();
        }
        public static bool operator <(Criterion l, Criterion r)
        {
            if ((object)l == (object)r)
            {
                return false;
            }

            if ((object)l == null)
            {
                return true;
            }

            if ((object)r == null)
            {
                return false;
            }

            try
            {
                CriterionOrdering.OrderingRelation relation = l.GetRelationTo(r);

                return false;
            }
            catch (RelationshipError)
            {
                CriterionOrdering.OrderingRelation relation = r.GetRelationTo(l);

                return true;
            }
        }
        public static bool operator >(Criterion l, Criterion r)
        {
            if ((object)l == (object)r)
            {
                return false;
            }

            if ((object)l == null)
            {
                return false;
            }

            if ((object)r == null)
            {
                return true;
            }

            try
            {
                CriterionOrdering.OrderingRelation relation = l.GetRelationTo(r);

                return true;
            }
            catch (RelationshipError)
            {
                CriterionOrdering.OrderingRelation relation = r.GetRelationTo(l);

                return false;
            }
        }
        public static bool operator ==(Criterion l, Criterion r)
        {
            if ((object)l == (object)r)
            {
                return true;
            }

            if ((object)l == null)
            {
                return false;
            }

            if ((object)r == null)
            {
                return false;
            }

            return l.CriterionOrderings == r.CriterionOrderings &&
                   l.CriterionOrderings.ContainsKey(l.Name) && r.CriterionOrderings.ContainsKey(r.Name) &&
                   l.Name == r.Name;
        }
        public static bool operator !=(Criterion l, Criterion r)
        {
            return !(l == r);
        }
        public override bool Equals(object o)
        {
            if (o as Criterion == null)
            {
                return false;
            }
            try
            {
                return (bool)(this == o as Criterion);
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
        public int CompareTo(object obj)
        {
            if (this < (Criterion)obj)
            {
                return -1;
            }

            if (this > (Criterion)obj)
            {
                return 1;
            }

            return 0;
        }
        public void Notify(ISubject<string> subject, IPublisher<string> publisher)
        {
            MyBroker.Instance.Publish(this,subject);
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
                    MyBroker.Instance.Unregister(this, Criterion.DescriptionChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Criterion.NameChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Criterion.RelatedCriterionChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Criterion.RelationToChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Criterion.ScorerChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Scorer.RatingChange.SubjectIdentifier);
                    MyBroker.Instance.Unregister(this, Criterion.WeightChange.SubjectIdentifier);
                    MyBroker.Instance.Unsubscribe(this, Scorer.RatingChange.SubjectIdentifier, Scorer);

                    Scorer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Criterion() {
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