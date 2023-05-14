using System.Windows;
using Warehouse.Model.Models;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Represents the window for displaying sold products.
    /// </summary>
    public partial class SoldProductsWindow : Window
    {
        private ProductsViewModel _productViewModel;
        /// <summary>
        /// Initializes a new instance of the SoldProductsWindow class.
        /// </summary>
        /// <param name="productViewModel">The view model containing products data.</param>
        public SoldProductsWindow(ProductsViewModel productViewModel)
        {
            InitializeComponent();
            _productViewModel = productViewModel;
            _productViewModel.GetProductsByState(ProductStates.Sold);
            DataContext = _productViewModel;
        }
    }
}
