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
        private ProductsViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ProductsViewModel();
            DataContext = _viewModel;
        }
    }
}
