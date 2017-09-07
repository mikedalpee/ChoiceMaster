using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public class ChoiceScorer : Scorer
    {
        public class Selection
        {
            public string Choice { get; set; }
            public double Setting { get; set; }
 
            public Selection(
                string choice,
                double setting)
            {
                Choice = choice;
                Setting = setting;
            }
        }
        public double Scale { get; set; }
        public Selection[] Selections { get; set; }
        public ChoiceScorer(
                double      scale,
                Selection[] selections)
        {
            Selections = (Selection[])selections.Clone();
            Scale = scale;

            foreach (var selection in Selections)
            {
                if (selection.Setting > Scale)
                {
                    selection.Setting = Scale;
                }
                else if (selection.Setting < 0.0)
                {
                    selection.Setting = 0.0;
                }
            }

            Setting = Math.Max(Selections.Length/2,1);
        }

        private int setting;
        public int Setting
        {
            get
            {
                return setting;
            }
            set
            {
                int newSetting;
                               
                if (value < 1)
                {
                    newSetting = 1;
                }
                else if (value > Selections.Length)
                {
                    newSetting = Selections.Length;
                }
                else
                {
                    newSetting = value;
                }

                if (newSetting != setting)
                {
                    setting = newSetting;

                    if (setting > Selections.Length-1)
                    {
                        Rating = 0.0;

                        return;
                    }

                    Rating = Selections[Setting - 1].Setting / Scale;
                }
            }
        }

        public override ScorerType ScorerType
        {
            get
            {
                return ScorerType.Choice;
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }
    }
}
