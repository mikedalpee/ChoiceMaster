using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class DiscreteRangeScorer : Scorer
    {
        public long LowSetting { get; set; }
        public long HighSetting { get; set; }
        public bool HigherIsBetter { get; set; }

        private long setting;

        public long Setting
        {
            get
            {
                return setting;
            }
            set
            {
                long newSetting;

                if (value < LowSetting)
                {
                    newSetting = LowSetting;
                }
                else if (value > HighSetting)
                {
                    newSetting = HighSetting;
                }
                else
                {
                    newSetting = value;
                }

                if (newSetting != setting)
                {
                    setting = newSetting;

                    if (HigherIsBetter)
                    {
                        Rating = (double)(Setting - LowSetting) / (double)(HighSetting - LowSetting);
                    }
                    else
                    {
                        Rating = (double)(HighSetting - Setting) / (double)(HighSetting - LowSetting);
                    }
                }
            }
        }

        public override ScorerType ScorerType
        {
            get
            {
                return ScorerType.DiscreteRange;
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public DiscreteRangeScorer(
            long lowSetting,
            long highSetting,
            bool higherIsBetter = true)
        {
            LowSetting = lowSetting;
            HighSetting = highSetting;
            HigherIsBetter = higherIsBetter;
            Setting = Math.Max((HighSetting-LowSetting)/2,LowSetting);
        }
    }
}
