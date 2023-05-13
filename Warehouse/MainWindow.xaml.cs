using Accessibility;
using System.Windows;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProductsViewModel _productsViewModel;
        public MainWindow()
        {
            InitializeComponent();
            _productsViewModel = new ProductsViewModel();
            DataContext = _productsViewModel;
        }

        private void ButtonReception_Click(object sender, RoutedEventArgs e)
        {
            ReceiptionWindow receiptionWindow = new ReceiptionWindow(_productsViewModel);
            receiptionWindow.Show();
        }
        private void ButtonWarehouse_Click(object sender, RoutedEventArgs e)
        {
            OnWarehouseWindow warehouseWindow = new OnWarehouseWindow(_productsViewModel);
            warehouseWindow.Show();
        }
        private void ButtonSold_Click(object sender, RoutedEventArgs e) 
        { 
            SoldProductsWindow soldProducts = new SoldProductsWindow(_productsViewModel);
            soldProducts.Show();
        }
        
        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.Show();
        }
    }
}
