using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Warehouse.Model.DataOperations;
using Warehouse.Model.Models;

namespace Warehouse.ViewModel
{
    internal class ProductsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        private ObservableCollection<Movements> _movements;
        private readonly ProductsDataAccessLayer _dal;

        public ObservableCollection<State> States { get; init; }
        public ObservableCollection<Product> Products
        {
            get => _products ?? (_products = new ObservableCollection<Product>());
            set
            {
                _products = value;
                OnPropertyChanged("Products");
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
        public ProductsViewModel()
        {
            _dal = new ProductsDataAccessLayer();
            Products = _dal.GetAllProducts();
            States = _dal.GetAllStates();
            Movements = _dal.GetAllMovements();
        }

        public void InsertProduct(Product product)
        {
            _dal.InsertProduct(product);
            //Refresh the props data after inserting from the database
            Products = _dal.GetAllProducts();            
            Movements = _dal.GetAllMovements();
        }

        public void UpdateProduct(Product product)
        {
            _dal.UpdateProduct(product);
            //Refresh the props data after inserting from the database
            Products = _dal.GetAllProducts();
            Movements = _dal.GetAllMovements();
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
