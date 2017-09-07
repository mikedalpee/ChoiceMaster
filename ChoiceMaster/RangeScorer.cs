using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class RangeScorer : Scorer
    {
        public double LowSetting { get; set; }
        public double HighSetting { get; set; }
        public bool HigherIsBetter { get; set; }

        private double setting;

        public double Setting
        {
            get
            {
                return setting;
            }
            set
            {
                double newSetting;

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
                        Rating = (Setting - LowSetting) / (HighSetting - LowSetting);
                    }
                    else
                    {
                        Rating = (HighSetting - Setting) / (HighSetting - LowSetting);
                    }
                }
            }
        }

        public override ScorerType ScorerType
        {
            get
            {
                return ScorerType.Range;
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public RangeScorer(
            double lowSetting,
            double highSetting,
            bool higherIsBetter = true)
        {
            LowSetting = lowSetting;
            HighSetting = highSetting;
            HigherIsBetter = higherIsBetter;
            Setting = Math.Max((HighSetting-LowSetting)/2.0,LowSetting);
        }
    }
}
