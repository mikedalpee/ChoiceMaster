using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChoiceMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MaxHeight = SystemParameters.PrimaryScreenHeight * 0.75;
            MaxWidth = SystemParameters.PrimaryScreenWidth * 0.75;

            InitializeComponent();

            DecisionModelVM = new DecisionModelVM();

            ModelName.DataContext = DecisionModelVM;

            Criteria.Drop += new DragEventHandler(Criteria_Drop);
            Criteria.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler(Criteria_PreviewMouseLeftButtonDown);
        }
        public DecisionModelVM DecisionModelVM { get; set; }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            Model_New_Popup.IsOpen = true;
        }

        private void Model_New_Name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string name = (sender as TextBox).Text;
                Model_New_Popup.IsOpen = false;

                DecisionModel dm = new DecisionModel(name);

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

                dm.NormalizeCriteria();

                DecisionModelVM.DecisionModel = dm;

                e.Handled = true;
            }
        }

        private int fromRowIndex = -1;
        void Criteria_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;

            fromRowIndex = GetDataGridItemCurrentRowIndex(dg, e.GetPosition);

            if (fromRowIndex < 0 || fromRowIndex >= dg.Items.Count - 1)
                return;

            dg.SelectedIndex = fromRowIndex;

            Criterion selectedCriterion = dg.Items[fromRowIndex] as Criterion;

            if (selectedCriterion == null)
                return;

            //Now Create a Drag Rectangle with Mouse Drag-Effect
            //Here you can select the Effect as per your choice

            DragDropEffects dragdropeffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(Criteria, selectedCriterion, dragdropeffects)
                                != DragDropEffects.None)
            {
                //Now This Item will be dropped at new location and so the new Selected Item
                Criteria.SelectedItem = selectedCriterion;
            }
        }
        void Criteria_Drop(object sender, DragEventArgs e)
        {
            DataGrid dg = sender as DataGrid;

            if (fromRowIndex < 0)
                return;

            int toRowIndex = this.GetDataGridItemCurrentRowIndex(dg, e.GetPosition);

            //The current Rowindex is -1 (No selected)
            if (toRowIndex < 0)
                return;
            //If Drag-Drop Location are same
            if (toRowIndex == fromRowIndex || (toRowIndex == dg.Items.Count - 1 && fromRowIndex == dg.Items.Count - 2))
                return;

            DecisionModel dm = DecisionModelVM.DecisionModel;
            LinkedList<Criterion> oc = dm.OrderedCriteria;

            //Fix up criterion orderings

            //First fix criterion ordering for criterion being moved

            LinkedListNode<Criterion> fromCriterionNode = oc.GetNodeAt<Criterion>(fromRowIndex);

            if (fromRowIndex == 0)
            {
                // moving first criterion somewhere else - delete its current ordering

                dm.DeleteCriterionOrdering(fromCriterionNode.Value.Name);
            }
            else
            {
                // moving criterion in body of orderings - delete the previous criterion's ordering to it

                CriterionOrdering previousCriterionOrdering = dm.CriterionOrderings[fromCriterionNode.Previous.Value.Name];

                dm.DeleteCriterionOrdering(fromCriterionNode.Previous.Value.Name);

                if (fromRowIndex != oc.Count - 1)
                {
                    // not moving the last criterion - so must create ordering between previous criterion and next criterion

                    CriterionOrdering fromCriterionOrdering = dm.CriterionOrderings[fromCriterionNode.Value.Name];

                    dm.DeleteCriterionOrdering(fromCriterionNode.Value.Name);

                    dm.CreateCriterionOrdering(
                        previousCriterionOrdering.Left.Name,
                        previousCriterionOrdering.Relation,
                        fromCriterionOrdering.Right.Name);
                }
            }

            fromCriterionNode.Remove();

            if (toRowIndex > fromRowIndex)
            {
                toRowIndex--;
            }

            //Now fix criterion ordering where criterion is being dropped

            LinkedListNode<Criterion> toCriterionNode = oc.GetNodeAt<Criterion>(toRowIndex);

            if (toRowIndex == 0)
            {
                // moving criterion to first position - just add a criterion ordering for the moved criterion to the first criterion

                dm.CreateCriterionOrdering(
                    fromCriterionNode.Value.Name,
                    CriterionOrdering.OrderingRelation.IsEqualTo,
                    toCriterionNode.Value.Name);
            }
            else
            {
                // not moving to first position

                if (toRowIndex < oc.Count)
                {
                    // Not moving below last criteria, so create ordering between previous criterion and moved criterion 

                    CriterionOrdering previousCriterionOrdering = dm.CriterionOrderings[toCriterionNode.Previous.Value.Name];

                    dm.DeleteCriterionOrdering(toCriterionNode.Previous.Value.Name);

                    dm.CreateCriterionOrdering(
                        previousCriterionOrdering.Left.Name,
                        previousCriterionOrdering.Relation,
                        fromCriterionNode.Value.Name);

                    // Add ordering for the moved criterion

                    dm.CreateCriterionOrdering(
                        fromCriterionNode.Value.Name,
                        CriterionOrdering.OrderingRelation.IsEqualTo,
                        toCriterionNode.Value.Name);
                }
                else
                {
                    // Moving below last criteria, so just create ordering between last criteria and moved criteria

                    LinkedListNode<Criterion> lastCriterionNode = oc.Last;

                    dm.CreateCriterionOrdering(
                        lastCriterionNode.Value.Name,
                        CriterionOrdering.OrderingRelation.IsEqualTo,
                        fromCriterionNode.Value.Name);
                }
            }

            dm.NormalizeCriteria();
        }
        public delegate Point GetDragDropPosition(IInputElement theElement);
        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }
        private DataGridRow GetDataGridRowItem(DataGrid dg, int index)
        {
            if (dg.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return dg.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        private int GetDataGridItemCurrentRowIndex(DataGrid dg, GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dg.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(dg, i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
        private void Model_New_Name_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TextBox t = sender as TextBox;
                Keyboard.Focus(t);
                t.SelectAll();
            }
        }
        private void RelationToNextComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = Locator.FindParent<DataGrid>(sender as DependencyObject);

            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1 && e.AddedItems[0] != e.RemovedItems[0])
            {
//                dataGrid.CommitEdit(DataGridEditingUnit.Cell, false);
//                dataGrid.CommitEdit(DataGridEditingUnit.Row, false);

//                e.Handled = true;
            }
        }
        private void ScorerTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = Locator.FindParent<DataGrid>(sender as DependencyObject);

            if (e.AddedItems.Count == 1 && (e.RemovedItems.Count == 0 || (e.RemovedItems.Count == 1 && e.AddedItems[0] != e.RemovedItems[0])))
            {
                ScorerType        scorerType = (ScorerType)(new ScorerEditorStringConverterExtension().ConvertBack(e.AddedItems[0], new ScorerType().GetType(), null, null));
                EditableCriterion ec = dg.SelectedItem as EditableCriterion;

                ec.ScorerType = scorerType;
            }
        }

        private void Criteria_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var dg = sender as DataGrid;
            var ec = e.Row.Item as EditableCriterion;

            // Don't allow editing the Name cell unless it is a new Criterion

            if (e.Column.DisplayIndex == 0 && !ec.IsNewEditableCriterion)
            {
                e.Cancel = true;

                return;
            }
            // Don't allow selecting a relation if this is a new Criterion or it's the last Criterion

            if (e.Column.DisplayIndex == 3 && (ec.IsNewEditableCriterion || dg.Items.IndexOf(ec) == dg.Items.Count-2))
            {
                e.Cancel = true;

                return;
            }
        }

        private void Criteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;

            if (dg.SelectedItems.Count == 1)
            {
                var currentRowIndex = dg.Items.IndexOf(dg.CurrentItem);

                var ec = dg.SelectedItems[0] as EditableCriterion;

                if (ec != null)
                {
                    DecisionModelVM.Scorer = ec.Scorer;

                    return;
                }
            }

            DecisionModelVM.Scorer = null;
        }

        private void Scorer_GotFocus(object sender, RoutedEventArgs e)
        {
            var scorer = sender as GroupBox;
            var cse = scorer.Content as ChoiceScorerEditor;

            if (cse != null)
            {
                var ecs = cse.DataContext as EditableChoiceScorer;

                ecs.BeginEdit();
            }
        }

        private void Scorer_LostFocus(object sender, RoutedEventArgs e)
        {
            var scorer = sender as GroupBox;
            var cse = scorer.Content as ChoiceScorerEditor;

            if (cse != null)
            {
                var ecs = cse.DataContext as EditableChoiceScorer;

                ecs.EndEdit();
            }
        }
    }
}
