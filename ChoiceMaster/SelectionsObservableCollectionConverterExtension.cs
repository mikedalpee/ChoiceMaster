using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ChoiceMaster
{
     [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class SelectionsObservableCollectionConverterExtension : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ChoiceScorer.Selection> selections = value as ObservableCollection<ChoiceScorer.Selection>;

            return selections.ToArray<ChoiceScorer.Selection>();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ChoiceScorer.Selection> selections = new ObservableCollection<ChoiceScorer.Selection>((ChoiceScorer.Selection[])values);

            return selections;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            ObservableCollection<ChoiceScorer.Selection> selections = value as ObservableCollection<ChoiceScorer.Selection>;

            return selections.ToArray<ChoiceScorer.Selection>();
        }
    }
}
