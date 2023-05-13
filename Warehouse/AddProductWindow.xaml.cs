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
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private ProductsViewModel _productViewModel;

        public AddProductWindow(ProductsViewModel viewModel)
        {
            InitializeComponent();
            _productViewModel = viewModel;
        }

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
            _productViewModel.GetProductsByState(ProductStates.OnReceiption);

            this.Close();
        }
    }
}
