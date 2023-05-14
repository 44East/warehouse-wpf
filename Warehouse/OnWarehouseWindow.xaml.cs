using System.Windows;
using Warehouse.Model.Models;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Represents the warehouse window for products.
    /// </summary>
    public partial class OnWarehouseWindow : Window
    {
        private ProductsViewModel _productViewModel;
        /// <summary>
        /// Initializes a new instance of the OnWarehouseWindow class.
        /// </summary>
        /// <param name="viewModel">The view model containing products data.</param>
        public OnWarehouseWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
            _productViewModel.GetProductsByState(ProductStates.OnStorage);
            DataContext = _productViewModel;
        }
        /// <summary>
        /// Event handler for the ContextMenuButton click event.
        /// Updates the selected product's <see cref="State"/> to "Sold" and refreshes the products list.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_productViewModel.SelectedProduct != null)
            {
                Product selectedProduct = _productViewModel.SelectedProduct;
                var nextState = ProductStates.Sold;
                _productViewModel.UpdateProduct(selectedProduct, nextState);

                _productViewModel.GetProductsByState(ProductStates.OnStorage);
            }
        }

    }
}
