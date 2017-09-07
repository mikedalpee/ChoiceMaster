using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class BooleanScorer : Scorer
    {
        public bool TrueIsBetter { get; set; }
        public bool InitialSetting { get; set; }

        private bool setting;
        public bool Setting
        {
            get
            {
                return setting;
            }
            set
            {
                if (value != setting)
                {
                    setting = value;

                    if (TrueIsBetter)
                    {
                        Rating = Setting ? 1.0 : 0.0;
                    }
                    else
                    {
                        Rating = Setting ? 0.0 : 1.0;
                    }
                }
            }
        }

        public override ScorerType ScorerType
        {
            get
            {
                return ScorerType.Boolean;
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public BooleanScorer(
                bool initialSetting = true,
                bool trueIsBetter   = true)
        {
            InitialSetting = initialSetting;
            TrueIsBetter   = trueIsBetter;
            Setting        = initialSetting;
        }
    }
}
