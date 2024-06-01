using InventoryManager.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace InventoryManager.Views
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<InventoryItem> Inventory { get; set; }

        private ICollectionView _inventoryView;

        public MainWindow()
        {
            InitializeComponent();
            Inventory = new ObservableCollection<InventoryItem>();
            InventoryDataGrid.ItemsSource = Inventory;
            _inventoryView = CollectionViewSource.GetDefaultView(Inventory);
            LoadData();
        }


        private void InventoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem selectedItem)
            {
                var editWindow = new EditWindow(selectedItem);
                editWindow.ShowDialog();
                InventoryDataGrid.Items.Refresh();
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new InventoryItem { Name = "New Item", Amount = 0 };
            Inventory.Add(newItem);
            var editWindow = new EditWindow(newItem);
            editWindow.ShowDialog();
            InventoryDataGrid.Items.Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(Inventory));
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Inventory.Clear();
                var items = JsonConvert.DeserializeObject<ObservableCollection<InventoryItem>>(File.ReadAllText(openFileDialog.FileName));
                foreach (var item in items)
                {
                    Inventory.Add(item);
                }
            }
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            var report = string.Join("\n", Inventory.Select(item => $"Name: {item.Name}, Amount: {item.Amount}"));
            MessageBox.Show(report, "Inventory Report");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTypeComboBox.SelectedItem != null)
            {
                string searchType = (SearchTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                _inventoryView.Filter = item =>
                {
                    var inventoryItem = item as InventoryItem;
                    if (inventoryItem == null)
                        return false;

                    switch (searchType)
                    {
                        case "Search by Name":
                            return inventoryItem.Name.ToLower().Contains(SearchTextBox.Text.ToLower());
                        case "Search by Type":
                            return inventoryItem.Type.ToLower().Contains(SearchTextBox.Text.ToLower());
                        default:
                            return false;
                    }
                };
            }
        }

        private void LoadData()
        {
            string dataFilePath = "Data/InventoryData.json";
            if (File.Exists(dataFilePath))
            {
                var items = JsonConvert.DeserializeObject<ObservableCollection<InventoryItem>>(File.ReadAllText(dataFilePath));
                foreach (var item in items)
                {
                    Inventory.Add(item);
                }
            }
        }
        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (SortByComboBox.SelectedItem as ComboBoxItem)?.Content as string;
            if (selectedItem != null)
            {
                switch (selectedItem)
                {
                    case "Name (A-Z)":
                        InventoryDataGrid.ItemsSource = Inventory.OrderBy(item => item.Name);
                        break;
                    case "Name (Z-A)":
                        InventoryDataGrid.ItemsSource = Inventory.OrderByDescending(item => item.Name);
                        break;
                    case "Quantity (Low to High)":
                        InventoryDataGrid.ItemsSource = Inventory.OrderBy(item => item.Amount);
                        break;
                    case "Quantity (High to Low)":
                        InventoryDataGrid.ItemsSource = Inventory.OrderByDescending(item => item.Amount);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}