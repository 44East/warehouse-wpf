using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Warehouse.Model.DataOperations;
using Warehouse.Model.Models;

namespace Warehouse.ViewModel
{
    /// <summary>
    /// View model for managing products with state-based operations.
    /// Implements the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public class ProductsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _productsByState;
        private Product _selectedProduct;
        private readonly WarehouseDataAccessLayer _dal;
        /// <summary>
        /// Gets the collection of all available states.
        /// </summary>
        public ObservableCollection<State> States { get; init; }
        /// <summary>
        /// The property for managing the selectable products 
        /// </summary>
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
        /// <summary>
        /// The collection with all the products from the database
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get => _products ?? (_products = new ObservableCollection<Product>());
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        /// <summary>
        /// The filtered products by state for an any View
        /// </summary>
        public ObservableCollection<Product> ProductsByState
        {
            get => _productsByState ?? (_productsByState = new ObservableCollection<Product>());
            set
            {
                _productsByState = value;
                OnPropertyChanged(nameof(ProductsByState));
            }
        }
        /// <summary>
        /// Initializes a new instance of the ProductsViewModel class.
        /// </summary>
        public ProductsViewModel()
        {
            _dal = new WarehouseDataAccessLayer();
            Products = _dal.GetAllProducts();
            States = _dal.GetAllStates();
        }
        /// <summary>
        /// Inserts a new product into the database.
        /// </summary>
        /// <param name="product">The product to insert.</param>
        public void InsertProduct(Product product)
        {
            if (product != null)
            {
                //Sets the Acceptance state object for the product State property from the States collection or, if it doesn't available create a new instance
                product.State = States.Where(s => s.Name.Equals(ProductStates.OnAcceptance)).FirstOrDefault() ?? new State { Name = ProductStates.OnAcceptance, Id = ProductStates.States.IndexOf(ProductStates.OnAcceptance) };
                _dal.InsertProduct(product);
                //Refresh the props data after inserting from the database
                Products = _dal.GetAllProducts();
            }
            return;
        }
        /// <summary>
        /// Updates a product's state to the specified next state. 
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <param name="nextState">The name of the next state.</param>
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
        /// <summary>
        /// Retrieves products filtered by state ftom the all <see cref="Products"/> collection.
        /// </summary>
        /// <param name="state">The state to filter by.</param>

        public void GetProductsByState(string state)
        {
            ProductsByState = new ObservableCollection<Product>(Products.Where(p => p.State.Name.Equals(state)).Select(p => p).AsEnumerable());
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
