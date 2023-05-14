using System.Windows;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// The main window shows all the products in the database
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
        /// <summary>
        /// Event handler for the ButtonAcceptance click event.
        /// Opens the <see cref="AcceptanceWindow"/> with the provided products <see cref="ProductsViewModel"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonAcceptance_Click(object sender, RoutedEventArgs e)
        {
            AcceptanceWindow acceptanceWindow = new AcceptanceWindow(_productsViewModel);
            acceptanceWindow.Show();
        }
        /// <summary>
        /// Event handler for the ButtonWarehouse click event.
        /// Opens the <see cref="OnWarehouseWindow"/> with the provided products <see cref="ProductsViewModel"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonWarehouse_Click(object sender, RoutedEventArgs e)
        {
            OnWarehouseWindow warehouseWindow = new OnWarehouseWindow(_productsViewModel);
            warehouseWindow.Show();
        }
        /// <summary>
        /// Event handler for the ButtonSold click event.
        /// Opens the <see cref="SoldProductsWindow"/> with the provided products <see cref="ProductsViewModel"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonSold_Click(object sender, RoutedEventArgs e) 
        { 
            SoldProductsWindow soldProducts = new SoldProductsWindow(_productsViewModel);
            soldProducts.Show();
        }
        /// <summary>
        /// Event handler for the ButtonReport click event.
        /// Opens the <see cref="ReportWindow"/> with the provided products <see cref="MovementsViewModel"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.Show();
        }
    }
}
