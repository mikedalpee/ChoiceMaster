using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for ChoiceScorerEditor.xaml
    /// </summary>
    public partial class ChoiceScorerEditor : UserControl
    {
        public ChoiceScorerEditor(
                ChoiceScorer scorer)
        {
            InitializeComponent();

            DataContext = new EditableChoiceScorer(scorer);
        }
        private void Selections_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            ChoiceScorer.Selection selection = e.Row.Item as ChoiceScorer.Selection;

            int index = dg.Items.IndexOf(selection);
        }

        private void Selections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;

            if (dg.SelectedItems.Count == 1)
            {
                ChoiceScorer.Selection selection = dg.SelectedItems[0] as ChoiceScorer.Selection;

            }
            else
            {
            }
        }
    }
}
