using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ChoiceMaster
{
    public class ScorerTypeStrings : List<string>
    {
        public ScorerTypeStrings()
        {
            Add("Boolean");
            Add("Choice");
            Add("Range");
            Add("Discrete Range");
        }
    }

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ScorerTypeStringConverterExtension : EnumStringConverterExtension<ScorerTypeStrings>
    {
    }
}
