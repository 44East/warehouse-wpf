using System.Collections.ObjectModel;
namespace Warehouse.Model.Models
{
    /// <summary>
    /// The static service class for storing product states in a string format. 
    /// </summary>
    public static class ProductStates
    {
        /// <summary>
        /// An <see cref="ObservableCollection{T}"/> collection for storing all the states in a string format
        /// </summary>
        public static ObservableCollection<string> States { get;  }
        /// <summary>
        /// All the products with all the states
        /// </summary>
        public static string AllProducts { get; } = "Все";
        /// <summary>
        /// The state for an acceptance goods
        /// </summary>
        public static string OnAcceptance { get; } = "Принят";
        /// <summary>
        /// The state for a goods on a storage
        /// </summary>
        public static string OnStorage { get; } = "На складе";
        /// <summary>
        /// The status for the sold products
        /// </summary>
        public static string Sold { get; } = "Продан";
        /// <summary>
        /// The static constructor initialize the static states collection
        /// </summary>
        static ProductStates()
        {
            States = new ObservableCollection<string>()
            {
                AllProducts, //IndexOf = 0
                OnAcceptance, //IndexOf = 1
                OnStorage, //IndexOf = 2
                Sold //IndexOf = 3
            };
        }
        
    }
}
