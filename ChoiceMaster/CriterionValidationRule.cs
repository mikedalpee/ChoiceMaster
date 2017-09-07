using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChoiceMaster
{
    public class CriterionValidationRule : ValidationRule
    {
        public override ValidationResult 
        Validate(
            object      value, 
            CultureInfo cultureInfo)
        {
            BindingGroup group = (BindingGroup)value;

            StringBuilder error = new StringBuilder();
            foreach (var item in group.Items)
            {
                // aggregate errors
                IDataErrorInfo info = item as IDataErrorInfo;

                if (info != null)
                {
                    string propertyError = info.Error;

                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }
            }

            if (error.Length != 0)
                return new ValidationResult(false, error.ToString());

            return ValidationResult.ValidResult;
        }
    }
}
