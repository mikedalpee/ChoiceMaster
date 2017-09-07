using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ChoiceMaster
{
    public class EditableChoiceScorer : ViewModelBase, IEditableObject, IDataErrorInfo
    {
        struct ChoiceScorerData
        {
            internal double Scale;
            internal ObservableCollection<EditableSelection> Selections;
        }
        private ChoiceScorerData choiceScorerData;
        private ChoiceScorerData backupData;
        private bool inTransaction = false;
        public ChoiceScorer ChoiceScorer { get; set; }
        public EditableChoiceScorer()
        {
            ChoiceScorer = new ChoiceScorer(0.0,new ChoiceScorer.Selection[] { });
            choiceScorerData.Scale = 0.0;
            choiceScorerData.Selections = new ObservableCollection<EditableSelection>();
        }
        public EditableChoiceScorer(
            ChoiceScorer choiceScorer)
        {
            ChoiceScorer = choiceScorer;
 
            choiceScorerData.Scale = choiceScorer.Scale;
            choiceScorerData.Selections = 
                new ObservableCollection<EditableSelection>(
                    choiceScorer.Selections.Select(
                        item=>new EditableSelection(item)));
        }
        public double Scale
        {
            get
            {
                return choiceScorerData.Scale;
            }
            set
            {
                if (choiceScorerData.Scale != value)
                {
                    choiceScorerData.Scale = value;
                    NotifyPropertyChanged("Scale");
                }
            }
        }
        public ObservableCollection<EditableSelection> Selections
        {
            get
            {
                return choiceScorerData.Selections;
            }
            set
            {
                if (choiceScorerData.Selections != value)
                {
                    choiceScorerData.Selections = value;
                    NotifyPropertyChanged("Selections");
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

        public string this[string columnName]
        {
            get
            {
                return "";
            }
        }

        public void BeginEdit()
        {
            if (!inTransaction)
            {
                Debug.WriteLine("Begin Edit");

                backupData = choiceScorerData;
                inTransaction = true;
            }
        }
        public void CancelEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("Cancel Edit");

                if (choiceScorerData.Scale != backupData.Scale)
                {
                    choiceScorerData.Scale = backupData.Scale;
                }
                if (choiceScorerData.Selections != backupData.Selections)
                {
                    choiceScorerData.Selections = backupData.Selections;
                }
                inTransaction = false;
            }
        }

        public void EndEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("End Edit");

                if (choiceScorerData.Scale != backupData.Scale)
                {
                    ChoiceScorer.Scale = choiceScorerData.Scale;
                }

                ChoiceScorer.Selections = choiceScorerData.Selections.Select(item=>item.Selection).ToArray();

                inTransaction = false;

                NotifyPropertyChanged("ChoiceScorer");
            }
        }
    }
}
