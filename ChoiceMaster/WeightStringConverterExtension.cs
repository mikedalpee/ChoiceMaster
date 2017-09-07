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
    public class WeightStringConverterExtension : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double weight = (double)value;

            if (weight == 0.0)
            {
                return "";

            }

            return String.Format("{0:0.000}", weight);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
