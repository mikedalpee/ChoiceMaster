using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PublishSubscribe;
using PublishSubscribe.IntraProcessPublishSubscribe;

namespace ChoiceMaster
{
    public class DecisionModelVM : ViewModelBase, ISubscriber<string>, INotifyPropertyChanged, IDisposable
    {
        public const string NO_SCORER_TYPE_SELECTED = "<<NO SCORER TYPE SELECTED>>";
        public const string UNNAMED_MODEL = "<<UNNAMED MODEL>>";

        public DecisionModelVM()
        {
            DecisionModel = new DecisionModel(Name);

            ScorerEditor = new UndefinedScorerEditor();
        }

        private DecisionModel decisionModel;
        public DecisionModel DecisionModel
        {
            get
            {
                return decisionModel;
            }

            set
            {
                if (decisionModel != value)
                {
                    decisionModel = value;

                    Name            = decisionModel.Name;
                    Score           = decisionModel.Score;
                    OrderedCriteria = CreateEditableCriteria(decisionModel.OrderedCriteria);

                    MyBroker.Instance.Subscribe(this, DecisionModel.OrderedCriteriaChange.SubjectIdentifier, value);
                    MyBroker.Instance.Subscribe(this, DecisionModel.ScoreChange.SubjectIdentifier, value);

                    NotifyPropertyChanged("DecisionModel");
                }
            }
        }
        private ObservableCollection<EditableCriterion> CreateEditableCriteria(ICollection<Criterion> newOrderedCriteria)
        {
            ObservableCollection<EditableCriterion> orderedCriteria =
                new ObservableCollection<EditableCriterion>(
                        newOrderedCriteria.Select(item => new EditableCriterion(item)));

           return orderedCriteria;
        }

        private ObservableCollection<EditableCriterion> orderedCriteria;
        public ObservableCollection<EditableCriterion> OrderedCriteria
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

                    NotifyPropertyChanged("OrderedCriteria");
                }
            }
        }
        public string name = UNNAMED_MODEL;
        public String Name
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

                    NotifyPropertyChanged("Name");
                }
            }
        }
        public double score = 0.0;
        public double Score
        {
            get
            {
                return score;
            }
            set
            {
                if (value != score)
                {
                    score = value;

                    NotifyPropertyChanged("Score");
                }
            }
        }
        public ScorerType scorerType = ChoiceMaster.ScorerType.Undefined;
        public ScorerType ScorerType
        {
            get
            {
                if (Scorer == null)
                {
                    return ScorerType.Undefined;
                }

                return Scorer.ScorerType;
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

                    NotifyPropertyChanged("Scorer");
                    NotifyPropertyChanged("ScorerType");

                    if (scorer != null)
                    {
                        switch (scorer.ScorerType)
                        {
                            case ScorerType.Boolean:
                                ScorerEditor =  new BooleanScorerEditor((BooleanScorer)scorer);
                                return;
                            case ScorerType.Choice:
                                ScorerEditor = new ChoiceScorerEditor((ChoiceScorer)scorer);
                                return;
                            case ScorerType.Range:
                                ScorerEditor = new RangeScorerEditor((RangeScorer)scorer);
                                return;
                            case ScorerType.DiscreteRange:
                                ScorerEditor = new DiscreteRangeScorerEditor((DiscreteRangeScorer)scorer);
                                return;
                        }
                    }

                    ScorerEditor = new UndefinedScorerEditor();
                }
            }
        }
        private UserControl scorerEditor;
        public UserControl ScorerEditor
        {
            get
            {
                return scorerEditor;
            }

            set
            {
                if (value != scorerEditor)
                {
                    scorerEditor = value;

                    NotifyPropertyChanged("ScorerEditor");
                }
            }
        }
        public void Notify(ISubject<string> subject,IPublisher<string> publisher)
        {
            switch (subject.Identifier)
            {
                case DecisionModel.OrderedCriteriaChange.SubjectIdentifier:
                    {
                        OrderedCriteria = CreateEditableCriteria(((DecisionModel.OrderedCriteriaChange)subject).NewOrderedCriteria);
                        break;
                    }
                case DecisionModel.ScoreChange.SubjectIdentifier:
                    {
                        Score = ((DecisionModel.ScoreChange)subject).NewScore;
                        break;
                    }
            }
        }
        public void EditableCriterion_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case "Criterion":
                    {
                        EditableCriterion ec = sender as EditableCriterion;

                        int index = orderedCriteria.IndexOf(ec);

                        if (index == -1 || !ec.IsNewEditableCriterion)
                        {
                            return;
                        }

                        // Finished editing a new Criterion - need to add it to the DecisionModel

                        DecisionModel dm = DecisionModel;

                        dm.CreateCriterion(
                            ec.Name,
                            ec.Description,
                            ec.Scorer);

                        if (orderedCriteria.Count != 1)
                        {
                            Criterion lastCriterion = dm.OrderedCriteria.Last.Value;
                            Criterion newCriterion = dm.Criteria[ec.Name];

                            dm.CreateCriterionOrdering(
                                lastCriterion.Name,
                                CriterionOrdering.OrderingRelation.IsEqualTo,
                                newCriterion.Name);
                        }

                        dm.NormalizeCriteria();
                        break;
                    }
                case "Scorer":
                    {
                        EditableCriterion ec = sender as EditableCriterion;

                        Scorer = ec.Scorer;
                        break;
                    }
            }
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
                    MyBroker.Instance.Unsubscribe(this, DecisionModel.OrderedCriteriaChange.SubjectIdentifier, DecisionModel);
                    MyBroker.Instance.Unsubscribe(this, DecisionModel.ScoreChange.SubjectIdentifier, DecisionModel);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DecisionModelVM() {
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
