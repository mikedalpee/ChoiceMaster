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
    public class CellValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
                                         CultureInfo cultureInfo)
        {
            // obtain the bound business object
            BindingExpression expression = value as BindingExpression;

            if (expression != null)
            {
                IDataErrorInfo info = expression.DataItem as IDataErrorInfo;

                if (info != null)
                {
                    // determine the binding path
                    string boundProperty = expression.ParentBinding.Path.Path;

                    // obtain any errors relating to this bound property
                    string error = info[boundProperty];

                    if (!string.IsNullOrEmpty(error))
                    {
                        return new ValidationResult(false, error);
                    }
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
