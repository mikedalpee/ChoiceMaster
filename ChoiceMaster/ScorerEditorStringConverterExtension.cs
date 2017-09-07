using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ChoiceMaster
{

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ScorerEditorStringConverterExtension : ScorerTypeStringConverterExtension,IValueConverter
    {
        public new object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string editor = (string)base.Convert(value, targetType, parameter, culture);

            if (String.IsNullOrEmpty(editor))
            {
                return DecisionModelVM.NO_SCORER_TYPE_SELECTED;
            }

            return editor + " Editor";
        }
    }
}
