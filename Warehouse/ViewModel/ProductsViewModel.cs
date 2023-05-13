using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Warehouse.Model.DataOperations;
using Warehouse.Model.Models;

namespace Warehouse.ViewModel
{
    public class ProductsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _productsByState;
        private Product _selectedProduct;
        private readonly ProductsDataAccessLayer _dal;
        public ObservableCollection<State> States { get; init; }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public ObservableCollection<Product> Products
        {
            get => _products ?? (_products = new ObservableCollection<Product>());
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public ObservableCollection<Product> ProductsByState
        {
            get => _productsByState ?? (_productsByState = new ObservableCollection<Product>());
            set
            {
                _productsByState = value;
                OnPropertyChanged(nameof(ProductsByState));
            }
        }
        public ProductsViewModel()
        {
            _dal = new ProductsDataAccessLayer();
            Products = _dal.GetAllProducts();
            States = _dal.GetAllStates();
        }

        public void InsertProduct(Product product)
        {
            if (product != null)
            {
                product.State = States.Where(s => s.Name.Equals(ProductStates.OnReceiption)).FirstOrDefault() ?? new State { Name = ProductStates.OnReceiption, Id = ProductStates.States.IndexOf(ProductStates.OnReceiption) };
                _dal.InsertProduct(product);
                //Refresh the props data after inserting from the database
                Products = _dal.GetAllProducts();
            }
            return;
        }

        public void UpdateProduct(Product product, string nextState)
        {
            if (States.Where(s => s.Name == nextState).Any())
            {
                product.State = States.Where(s => s.Name == nextState).Select(s => s).FirstOrDefault();
                _dal.UpdateProduct(product);
                //Refresh the props data after inserting from the database
                Products = _dal.GetAllProducts();
            }
            return;
        }

        public void GetProductsByState(string state)
        {
            ProductsByState = _dal.GetProductsByState(state);
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
