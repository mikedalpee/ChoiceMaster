using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ChoiceMaster
{
    public class EditableCriterion : ViewModelBase, IEditableObject, IDataErrorInfo
    {
        struct CriterionData
        {
            internal string Name;
            internal string Description;
            internal double Weight;
            internal CriterionOrdering.OrderingRelation RelationTo;
            internal Scorer Scorer;
        }
        private CriterionData criterionData;
        private CriterionData backupData;
        private bool inTransaction = false;
        public DecisionModel DecisionModel { get; set; }
        public Criterion Criterion { get; set; }
        public bool IsNewEditableCriterion {get; protected set; }
        public EditableCriterion()
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            DecisionModel = mw.DecisionModelVM.DecisionModel;
            Criterion = new Criterion(DecisionModel.CriterionOrderings,"","",null);
            IsNewEditableCriterion = true;
            criterionData.Name = "";
            criterionData.Description = "";
            criterionData.RelationTo = CriterionOrdering.OrderingRelation.IsInvalid;
            criterionData.Scorer = null;
            PropertyChanged += new PropertyChangedEventHandler(mw.DecisionModelVM.EditableCriterion_PropertyChanged);
        }
        public EditableCriterion(
            Criterion criterion)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            DecisionModel = mw.DecisionModelVM.DecisionModel;
            Criterion = criterion;
            IsNewEditableCriterion = false;

            criterionData.Name = criterion.Name;
            criterionData.Description = criterion.Description;
            criterionData.Weight = criterion.Weight;
            criterionData.RelationTo = criterion.RelationTo;
            criterionData.Scorer = criterion.Scorer;
            PropertyChanged += new PropertyChangedEventHandler(mw.DecisionModelVM.EditableCriterion_PropertyChanged);
        }
        public string Name
        {
            get
            {
                return criterionData.Name;
            }
            set
            {
                if (criterionData.Name != value)
                {
                    Debug.WriteLine("Setting Name to: \"" + value+"\"");
                    criterionData.Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public string Description
        {
            get
            {
                return criterionData.Description;
            }
            set
            {
                if (criterionData.Description != value)
                {
                    criterionData.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }
        public double Weight
        {
            get
            {
                return criterionData.Weight;
            }
            set
            {
                if (criterionData.Weight != value)
                {
                    criterionData.Weight = value;
                    NotifyPropertyChanged("Weight");
                }
            }
        }
        public CriterionOrdering.OrderingRelation RelationTo
        {
            get
            {
                return criterionData.RelationTo;
            }
            set
            {
                if (criterionData.RelationTo != value)
                {
                    criterionData.RelationTo = value;
                    NotifyPropertyChanged("RelationTo");
                }
            }
        }
        public ScorerType ScorerType
        {
            get
            {
                if (criterionData.Scorer == null)
                {
                    return ScorerType.Undefined;
                }

                return criterionData.Scorer.ScorerType;
            }
            set
            {
                if ((criterionData.Scorer == null && value != ScorerType.Undefined) ||
                    criterionData.Scorer == null || criterionData.Scorer.ScorerType != value)
                {
                    switch (value)
                    {
                        case ScorerType.Boolean:
                            Scorer = new BooleanScorer();
                            return;
                        case ScorerType.Choice:
                            Scorer = new ChoiceScorer(10,new ChoiceScorer.Selection[] {});
                            return;
                        case ScorerType.Range:
                            Scorer = new RangeScorer(1, 10);
                            return;
                        case ScorerType.DiscreteRange:
                            Scorer = new DiscreteRangeScorer(1, 10);
                            return;
                    }

                    Scorer = null;
                }
            }
        }
        public Scorer Scorer
        {
            get
            {
                return criterionData.Scorer;
            }
            set
            {
                if (criterionData.Scorer != value)
                {
                    criterionData.Scorer = value;

                    NotifyPropertyChanged("Scorer");
                    NotifyPropertyChanged("ScorerType");
                }
            }
        }
        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];

                    if (propertyError != string.Empty)
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                return error.ToString();
            }
        }

        private readonly Regex nameEx = new Regex(@"^[A-Za-z][A-Za-z0-9_]*$");
        public string this[string columnName]
        {
            get
            {
                Debug.WriteLine("Validating: "+columnName);

                if (columnName == "Name")
                {
                    Debug.WriteLine("Name: \"" + Name+ "\"");

                    if (Name == null || Name == string.Empty)
                    {
                        return "Name cannot be null or empty";
                    }

                    if (!nameEx.Match(Name).Success)
                    {
                        return "Name must start with a letter and contain only letters, digits, and underscores";
                    }

                    if (IsNewEditableCriterion)
                    {
                        if (DecisionModel.Criteria.ContainsKey(Name))
                        {
                            return "Name must be unique";
                        }
                    }
                    return "";
                }

                if (columnName == "Description")
                {
                    Debug.WriteLine("Description: " + Description);

                    if (Description == null || Description == string.Empty)
                    {
                        return "Description cannot be null or empty";
                    }

                    return "";
                }

                return "";
            }
        }

        public void BeginEdit()
        {
            if (!inTransaction)
            {
                Debug.WriteLine("Begin Edit");

                backupData = criterionData;
                inTransaction = true;
            }
        }
        public void CancelEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("Cancel Edit");

                if (criterionData.Name != backupData.Name)
                {
                    criterionData.Name = backupData.Name;
                }
                if (criterionData.Description != backupData.Description)
                {
                    criterionData.Description = backupData.Description;
                }
                if (criterionData.Weight != backupData.Weight)
                {
                    criterionData.Weight = backupData.Weight;
                }
                if (criterionData.RelationTo != backupData.RelationTo)
                {
                    criterionData.RelationTo = backupData.RelationTo;
                }
                if (criterionData.Scorer != backupData.Scorer)
                {
                    criterionData.Scorer = backupData.Scorer;
                }
                inTransaction = false;
            }
        }

        public void EndEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("End Edit");

                if (criterionData.Name != backupData.Name)
                {
                    Criterion.Name = criterionData.Name;
                }
                if (criterionData.Description != backupData.Description)
                {
                    Criterion.Description = criterionData.Description;
                }
                if (criterionData.Weight != backupData.Weight)
                {
                    Criterion.Weight = criterionData.Weight;
                }
                if (criterionData.RelationTo != backupData.RelationTo)
                {
                    Criterion.RelationTo = criterionData.RelationTo;
                }
                if (criterionData.Scorer != backupData.Scorer)
                {
                    Criterion.Scorer = criterionData.Scorer;
                }

                inTransaction = false;

                NotifyPropertyChanged("Criterion");
            }
        }
    }
}
