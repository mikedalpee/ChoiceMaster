using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChoiceMaster;
using System.Diagnostics;

namespace ChoiceMasterUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DecisionModel dm = new DecisionModel("MyDecisionModel");

            dm.CreateCriterion("C1", "Color",
                                new ChoiceScorer(
                                        10,
                                        new[] {
                                            new ChoiceScorer.Selection("Red", 10),
                                            new ChoiceScorer.Selection("Green", 5),
                                            new ChoiceScorer.Selection("Other", 3) }));
            dm.CreateCriterion("C2", "MPG", new RangeScorer(10.0, 40.0));
            dm.CreateCriterion("C3", "Warranty Period Years", new RangeScorer(1.0, 5.0));
            dm.CreateCriterion("C4", "Price", new RangeScorer(30000.0, 150000.0));
            dm.CreateCriterion("C5", "Backup Camera", new BooleanScorer());
            dm.CreateCriterion("C6", "Craftsmanship", new DiscreteRangeScorer(1, 10));
            dm.CreateCriterion("C7", "Distance to Dealer", new RangeScorer(4.0, 50.0, higherIsBetter: false));
            dm.CreateCriterion("C8", "Horsepower", new RangeScorer(200.0, 900.0));
            dm.CreateCriterion("C9", "Manufacturer",
                                new ChoiceScorer(
                                        10, new[] {
                                                new ChoiceScorer.Selection("Porsche", 10),
                                                new ChoiceScorer.Selection("Audi", 8),
                                                new ChoiceScorer.Selection("BMW", 6),
                                                new ChoiceScorer.Selection("Lexus", 4),
                                                new ChoiceScorer.Selection("Nissan", 3),
                                                new ChoiceScorer.Selection("Ford", 2),
                                                new ChoiceScorer.Selection("Other", 0),}));

            dm.CreateCriterionOrdering("C8", CriterionOrdering.OrderingRelation.IsEqualTo, "C7");
            dm.CreateCriterionOrdering("C7", CriterionOrdering.OrderingRelation.IsSignificantlyBetterThan, "C6");
            dm.CreateCriterionOrdering("C6", CriterionOrdering.OrderingRelation.IsModeratelyBetterThan, "C9");
            dm.CreateCriterionOrdering("C9", CriterionOrdering.OrderingRelation.IsSlightlyBetterThan, "C5");
            dm.CreateCriterionOrdering("C5", CriterionOrdering.OrderingRelation.IsSlightlyBetterThan, "C4");
            dm.CreateCriterionOrdering("C4", CriterionOrdering.OrderingRelation.IsSlightlyBetterThan, "C3");
            dm.CreateCriterionOrdering("C3", CriterionOrdering.OrderingRelation.IsEqualTo, "C2");
            dm.CreateCriterionOrdering("C2", CriterionOrdering.OrderingRelation.IsEqualTo, "C1");

            Debug.WriteLine("Criterion Orderings");

            foreach (var kvp1 in dm.CriterionOrderings)
            {
                CriterionOrdering co = kvp1.Value;

                Debug.WriteLine(co.ToString());
            }

            dm.NormalizeCriteria();

            Debug.WriteLine("Ordered Criteria Weights");

            foreach (var c in dm.OrderedCriteria)
            {
                Debug.WriteLine("{0}: {1}", c.Name, c.Weight);
            }

            Debug.WriteLine("Score: {0}", dm.Score);

            Scorer scorer = dm.GetScorer("C8");

            ((RangeScorer)scorer).Setting = 850.0;

            Debug.WriteLine("Score: {0}", dm.Score);

            scorer = dm.GetScorer("C7");

            ((RangeScorer)scorer).Setting = 45.0;

            Debug.WriteLine("Score: {0}", dm.Score);
        }
    }
}
