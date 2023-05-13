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
    /// Interaction logic for SoldProductsWindow.xaml
    /// </summary>
    public partial class SoldProductsWindow : Window
    {
        private ProductsViewModel _productViewModel;
        public SoldProductsWindow(ProductsViewModel productViewModel)
        {
            InitializeComponent();
            _productViewModel = productViewModel;
            _productViewModel.GetProductsByState(ProductStates.Sold);
            DataContext = _productViewModel;
        }
    }
}
