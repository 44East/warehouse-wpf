using System.Windows;
using Warehouse.Model.Models;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Add Product Window.
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private ProductsViewModel _productViewModel;
        /// <summary>
        /// Initializes a new instance of the AddProductWindow class.
        /// </summary>
        /// <param name="viewModel">The view model for products.</param>
        public AddProductWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
        }
        /// <summary>
        /// Event handler for the add button click event.
        /// Adds a new product based on the provided input and updates the products collection in a view model.
        /// Closes the window after adding the product.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string sku = txtSKU.Text;

            Product newProduct = new Product
            {
                Name = name,
                SKU = sku,
            };
            _productViewModel.InsertProduct(newProduct);
            _productViewModel.GetProductsByState(ProductStates.OnAcceptance);

            this.Close();
        }
    }
}
