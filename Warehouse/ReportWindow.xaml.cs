using System.Windows;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private MovementsViewModel _movements;
        public ReportWindow()
        {
            InitializeComponent();
            _movements = new MovementsViewModel();
            DataContext = _movements;
        }
    }
}
