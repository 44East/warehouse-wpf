using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Warehouse.Model.DataOperations;
using Warehouse.Model.Models;

namespace Warehouse.ViewModel
{
    /// <summary>
    /// View model for managing movements with sorting and filtering capabilities.
    /// Implements the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public class MovementsViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Movements> _movements;
        private ObservableCollection<Movements> _sortedMovements;
        private ObservableCollection<string> _states;
        private string _selectedState;
        private DateTime? _selectedFromDate;
        private DateTime? _selectedToDate;

        private readonly WarehouseDataAccessLayer _dal;
        /// <summary>
        /// The sorted movements collection after filtering.
        /// </summary>
        public ObservableCollection<Movements> SortedMovements
        {
            get => _sortedMovements;
            set
            {
                _sortedMovements = value;
                OnPropertyChanged(nameof(SortedMovements));
            }
        }
        /// <summary>
        /// All the movements collection from the database.
        /// </summary>
        public ObservableCollection<Movements> Movements
        {
            get => _movements ?? (_movements = new ObservableCollection<Movements>());
            set
            {
                _movements = value;
                OnPropertyChanged(nameof(Movements));
            }
        }
        /// <summary>
        /// All the existing states in a string format
        /// </summary>
        public ObservableCollection<string> States
        {
            get => _states;
            set
            {
                _states = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Stores a selected state
        /// </summary>
        public string SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// The start date for filtering
        /// </summary>
        public DateTime? SelectedFromDate
        {
            get => _selectedFromDate;
            set
            {
                //It sets the time format to 23:59:59 to include the whole day
                _selectedFromDate = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// The end date for filtering
        /// </summary>
        public DateTime? SelectedToDate
        {
            get => _selectedToDate;
            set
            {
                _selectedToDate = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// The command for applying filtering variables
        /// </summary>
        public ICommand ApplyFiltersCommand { get; set; }
        /// <summary>
        /// Initializes a new instance of the MovementsViewModel class.
        /// </summary>
        public MovementsViewModel()
        {
            _dal = new WarehouseDataAccessLayer();
            Movements = _dal.GetAllMovements();
            SortedMovements = Movements;

            ApplyFiltersCommand = new RelayCommand(ApplyFilters);

            InitializeStatesList();
        }
        /// <summary>
        /// Initializes the states list from the service class <see cref="ProductStates"/>
        /// </summary>
        private void InitializeStatesList()
        {
            States = ProductStates.States;
            SelectedState = ProductStates.AllProducts;
        }
        /// <summary>
        /// Applies the selected filters to the movements collection.
        /// </summary>
        private void ApplyFilters()
        {
            SortedMovements = GetSortedMovements(SelectedState, SelectedFromDate, SelectedToDate);
        }
        /// <summary>
        /// Retrieves the sorted movements based on the selected filters.
        /// </summary>
        /// <param name="state">The selected state filter.</param>
        /// <param name="fromDate">The selected from date filter.</param>
        /// <param name="toDate">The selected to date filter.</param>
        /// <returns>The sorted <see cref="Model.Models.Movements"/> collection.</returns>
        private ObservableCollection<Movements> GetSortedMovements(string state, DateTime? fromDate, DateTime? toDate)
        {
            //if the started date doesn't set in the filtering params, this code sets a min date value 
            if (fromDate == null)
            {
                fromDate = DateTime.MinValue;
            }
            //if the finished date doesn't set in the filtering params, this code sets a max date value 
            if (toDate == null)
            {
                toDate = DateTime.MaxValue;
            }
            //This collection will be stores the transition value
            IEnumerable<Movements> filteredMovements;

            if (state.Equals(ProductStates.AllProducts))
            {
                //Sorting movements only by datetime
                filteredMovements = Movements.Where(m => m.DateStamp > fromDate && m.DateStamp < toDate).OrderBy(m => m.Product.Id);
            }
            else
            {
                //Sorting movements by datetime and the current state
                filteredMovements = Movements.Where(m => m.State.Name.Equals(state) && m.DateStamp > fromDate && m.DateStamp < toDate).OrderBy(m => m.Product.Id);
            }

            return new ObservableCollection<Movements>(filteredMovements);
        }


        /// <summary>
        /// Notification property for the [View] part the app
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
