using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class CriterionOrdering
    {
        public enum OrderingRelation : int
        {
            IsInvalid = Int32.MinValue,
            IsEqualTo = 0,
            IsSlightlyBetterThan = 2,
            IsModeratelyBetterThan = 4,
            IsSignificantlyBetterThan = 8,
        }

        public Criterion Left { get; set; }
        public OrderingRelation Relation { get; set; }
        public Criterion Right { get; set; }

        public CriterionOrdering()
        {
            Left = null;
            Relation = OrderingRelation.IsInvalid;
            Right = null;
        }
        public CriterionOrdering(
            Criterion left,
            OrderingRelation relation,
            Criterion right)
        {
            Left = left;
            Relation = relation;
            Right = right;
        }
        public override string ToString()
        {
            return Left.ToString()+" "+Relation.ToString()+" "+Right.ToString();
        }
    }
}
