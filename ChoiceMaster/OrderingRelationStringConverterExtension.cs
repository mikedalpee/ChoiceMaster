using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class OrderingRelationStrings : List<string>
    {
        public OrderingRelationStrings()
        {
            Add("Is Equal To");
            Add("Is Slightly Better Than");
            Add("Is Moderately Better Than");
            Add("Is Significantly Better Than");
        }
    }

    public class OrderingRelationStringCoverterExtension : EnumStringConverterExtension<OrderingRelationStrings>
    {
    }
}
