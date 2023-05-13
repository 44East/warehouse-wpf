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
using System.Windows.Shapes;
using Warehouse.Model.Models;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Interaction logic for OnWarehouseWindow.xaml
    /// </summary>
    public partial class OnWarehouseWindow : Window
    {
        private ProductsViewModel _productViewModel;
        public OnWarehouseWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
            _productViewModel.GetProductsByState(ProductStates.OnStorage);
            DataContext = _productViewModel;
        }
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
