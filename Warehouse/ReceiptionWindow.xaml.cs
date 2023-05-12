
using System.Windows;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Interaction logic for ReceiptionWindow.xaml
    /// </summary>
    public partial class ReceiptionWindow : Window
    {
        private ProductsViewModel _productViewModel;
        public ReceiptionWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
            DataContext = _productViewModel;
        }
    }
}
