using InventoryManager.Models;
using System.Windows;

namespace InventoryManager.Views
{
    public partial class EditWindow : Window
    {
        private InventoryItem _item;

        public EditWindow(InventoryItem item)
        {
            InitializeComponent();
            _item = item;
            NameTextBox.Text = _item.Name;
            AmountTextBox.Text = _item.Amount.ToString();
            TypeTextBox.Text = _item.Type;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _item.Name = NameTextBox.Text;
            if (int.TryParse(AmountTextBox.Text, out int amount))
            {
                _item.Amount = amount;
            }
            _item.Type = TypeTextBox.Text;
            this.Close();
        }
    }
}