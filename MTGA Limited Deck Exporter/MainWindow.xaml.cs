using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace MTGA_Limited_Deck_Exporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Utils.Load();

            label.Content = String.Format("{0} cards in database", Utils.CardCount);

            FindLogs();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FindLogs();
        }

        private void FindLogs()
        {
            var allPools = new Dictionary<int, Pool>();
            var files = Directory.GetFiles(textBox.Text);
            foreach (var file in files)
            {
                if (Utils.FileHasSealedPool(file))
                {
                    var pools = Utils.FindSealedPools(file);
                    foreach (var pool in pools) {
                        if (!allPools.ContainsKey(pool.Id))
                        {
                            allPools.Add(pool.Id, pool);
                        }
                    }
                }
            }

            dataGrid.ItemsSource = new ObservableCollection<Pool>(allPools.Values.ToList());
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSave.IsEnabled = dataGrid.SelectedItem != null;
            btnCopy.IsEnabled = dataGrid.SelectedItem != null;
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Utils.GetExportString(dataGrid.SelectedItem));
            MessageBox.Show(this, "Copied to clipboard.", "Success");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault(false))
            {
                using (StreamWriter outputFile = new StreamWriter(dialog.FileName))
                {
                    outputFile.Write(Utils.GetExportString(dataGrid.SelectedItem));
                }
                MessageBox.Show(this, "File was saved.", "Success");
            }
        }
    }
}
