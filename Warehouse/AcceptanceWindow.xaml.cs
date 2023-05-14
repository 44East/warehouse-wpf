using System.Windows;
using Warehouse.Model.Models;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Represents the acceptance window for products.
    /// </summary>
    public partial class AcceptanceWindow : Window
    {
        private ProductsViewModel _productViewModel;
        /// <summary>
        /// Initializes a new instance of the AcceptanceWindow class.
        /// </summary>
        /// <param name="viewModel">The view model containing products data.</param>
        public AcceptanceWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
            _productViewModel.GetProductsByState(ProductStates.OnAcceptance);
            DataContext = _productViewModel;
        }
        /// <summary>
        /// Event handler for the ContextMenuButton click event.
        /// Updates the selected product's <see cref="State"/>  to "OnStorage" and update ot onto database through the <see cref="ProductsViewModel"/> and then refreshes the products list.
        /// </summary>
        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_productViewModel.SelectedProduct != null)
            {
                Product selectedProduct = _productViewModel.SelectedProduct;
                var nextState = ProductStates.OnStorage;
                _productViewModel.UpdateProduct(selectedProduct, nextState);

                _productViewModel.GetProductsByState(ProductStates.OnAcceptance);
            }
        }
        /// <summary>
        /// Event handler for the ButtonCreateNewProduct click event.
        /// Opens the <see cref="AddProductWindow"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonCreateNewProduct_Click(object sender, RoutedEventArgs e) 
        {
            AddProductWindow addProduct = new AddProductWindow(_productViewModel);
            addProduct.Show();
        }

    }
}
