
using System.Collections.ObjectModel;
namespace Warehouse.Model.Models
{
    public static class ProductStates
    {
        public static ObservableCollection<string> States { get;  }

        public static string AllProducts { get; } = "Все";
        public static string OnReceiption { get; } = "Принят";
        public static string OnStorage { get; } = "На складе";
        public static string Sold { get; } = "Продан";
        static ProductStates()
        {
            States = new ObservableCollection<string>()
            {
                "Все", //IndexOf = 0
                "Принят", //IndexOf = 1
                "На складе", //IndexOf = 2
                "Продан" //IndexOf = 3
            };
        }
        
    }
}
