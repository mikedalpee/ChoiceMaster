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
    public class EnumStringConverterExtension<STRINGS> : MarkupExtension, IValueConverter where STRINGS : List<string>, new()
    {
        private STRINGS values = new STRINGS();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enums = Enum.GetValues(value.GetType());

            int index = Array.IndexOf(enums, value);

            if ((int)enums.GetValue(index) == Int32.MinValue || index >= values.Count)
            {
                return "";
            }

            return values[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value as string;

            if (v == null || v.Length == 0)
            {
                return Int32.MinValue;
            }

            int index = values.IndexOf(v);

            if (index < 0)
            {
                return Int32.MinValue;
            }

            var enums = Enum.GetValues(targetType);

            if (index >= enums.Length)
            {
                return Int32.MinValue;
            }

            return
                enums.GetValue(index);
        }
    }
}
