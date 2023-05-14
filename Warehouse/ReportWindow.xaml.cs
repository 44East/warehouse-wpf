using System.Windows;
using Warehouse.ViewModel;

namespace Warehouse
{
    /// <summary>
    /// Represents the window for generating reports.
    /// </summary>
    public partial class ReportWindow : Window
    {
        private MovementsViewModel _movements;
        /// <summary>
        /// Initializes a new instance of the ReportWindow class. It uses <see cref="MovementsViewModel"/> for the provided data.
        /// </summary>
        public ReportWindow()
        {
            InitializeComponent();
            _movements = new MovementsViewModel();
            DataContext = _movements;
        }
    }
}
