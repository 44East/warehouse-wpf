using System.Windows;
using Warehouse.Model.Models;
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
            _productViewModel.GetProductsByState(ProductStates.OnReceiption);
            DataContext = _productViewModel;
        }
        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_productViewModel.SelectedProduct != null)
            {
                Product selectedProduct = _productViewModel.SelectedProduct;
                var nextState = ProductStates.OnStorage;
                _productViewModel.UpdateProduct(selectedProduct, nextState);

                _productViewModel.GetProductsByState(ProductStates.OnReceiption);
            }
        }

        private void ButtonCreateNewProduct_Click(object sender, RoutedEventArgs e) 
        {
            AddProductWindow addProduct = new AddProductWindow(_productViewModel);
            addProduct.Show();
        }

    }
}
