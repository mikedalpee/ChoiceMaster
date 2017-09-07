using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class EditableSelection : ViewModelBase, IEditableObject, IDataErrorInfo
    {
        struct SelectionData
        {
            internal string Choice;
            internal double Setting;
        }
        private SelectionData selectionData;
        private SelectionData backupData;
        private bool inTransaction = false;
        public ChoiceScorer.Selection Selection { get; set; }
        public EditableSelection()
        {
            Selection = new ChoiceScorer.Selection("",0.0);
            selectionData.Choice = "";
            selectionData.Setting = 0.0;
        }
        public EditableSelection(
            ChoiceScorer.Selection selection)
        {
            Selection = selection;

            selectionData.Choice  = Selection.Choice;
            selectionData.Setting = Selection.Setting;
        }
        public string Choice
        {
            get
            {
                return selectionData.Choice;
            }
            set
            {
                if (selectionData.Choice != value)
                {
                    selectionData.Choice = value;
                    NotifyPropertyChanged("Choice");
                }
            }
        }
        public double Setting
        {
            get
            {
                return selectionData.Setting;
            }
            set
            {
                if (selectionData.Setting != value)
                {
                    selectionData.Setting = value;
                    NotifyPropertyChanged("Setting");
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

                backupData = selectionData;
                inTransaction = true;
            }
        }
        public void CancelEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("Cancel Edit");

                if (selectionData.Choice != backupData.Choice)
                {
                    selectionData.Choice = backupData.Choice;
                }
                if (selectionData.Setting != backupData.Setting)
                {
                    selectionData.Setting = backupData.Setting;
                }
                inTransaction = false;
            }
        }

        public void EndEdit()
        {
            if (inTransaction)
            {
                Debug.WriteLine("End Edit");

                if (selectionData.Choice != backupData.Choice)
                {
                    Selection.Choice = selectionData.Choice;
                }
                if (selectionData.Setting != backupData.Setting)
                {
                    Selection.Setting = selectionData.Setting;
                }

                inTransaction = false;

                NotifyPropertyChanged("Selection");
            }
        }
    }
}
