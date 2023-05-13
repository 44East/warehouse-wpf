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
    public class MovementsViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Movements> _movements;
        private ObservableCollection<Movements> _sortedMovements;
        private ObservableCollection<string> _states;
        private string _selectedState;
        private DateTime? _selectedFromDate;
        private DateTime? _selectedToDate;

        private readonly ProductsDataAccessLayer _dal;
        public ObservableCollection<Movements> SortedMovements
        {
            get => _sortedMovements;
            set
            {
                _sortedMovements = value;
                OnPropertyChanged("SortedMovements");
            }
        }
        public ObservableCollection<Movements> Movements
        {
            get => _movements ?? (_movements = new ObservableCollection<Movements>());
            set
            {
                _movements = value;
                OnPropertyChanged("Movements");
            }
        }
        public ObservableCollection<string> States
        {
            get => _states;
            set
            {
                _states = value;
                OnPropertyChanged();
            }
        }

        public string SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                OnPropertyChanged();
            }
        }

        public DateTime? SelectedFromDate
        {
            get => _selectedFromDate;
            set
            {
                _selectedFromDate = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                OnPropertyChanged();
            }
        }

        public DateTime? SelectedToDate
        {
            get => _selectedToDate;
            set
            {
                _selectedToDate = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                OnPropertyChanged();
            }
        }

        public ICommand ApplyFiltersCommand { get; set; }

        public MovementsViewModel()
        {
            _dal = new ProductsDataAccessLayer();
            Movements = _dal.GetAllMovements();
            SortedMovements = Movements;

            ApplyFiltersCommand = new RelayCommand(ApplyFilters);

            InitializeStatesList();
        }

        private void InitializeStatesList()
        {
            States = ProductStates.States;
            SelectedState = ProductStates.AllProducts;
        }

        private void ApplyFilters()
        {
            SortedMovements = GetSortedMovements(SelectedState, SelectedFromDate, SelectedToDate);
        }
        private ObservableCollection<Movements> GetSortedMovements(string state, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null)
            {
                fromDate = DateTime.MinValue;
            }
            if (toDate == null)
            {
                toDate = DateTime.MaxValue;
            }

            IEnumerable<Movements> filteredMovements;

            if (state.Equals(ProductStates.AllProducts))
            {
                filteredMovements = Movements.Where(m => m.DateStamp > fromDate && m.DateStamp < toDate).OrderBy(m => m.Product.Id);
            }
            else
            {
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
