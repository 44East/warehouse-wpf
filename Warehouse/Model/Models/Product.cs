namespace Warehouse.Model.Models
{
    /// <summary>
    /// Represents an one of the product in the warehouse. A model for parsing data from the DB. The model stores an instance of a current <see cref="Models.State"/> for the current product. 
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The unique ID for each product.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the product/model in a public format (PlayStation 5, OnePlus 10 etc.)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An unpublic model name, it could be an internal model name
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// The current <see cref="Models.State"/> for the current product object.
        /// </summary>
        public State State { get; set; }

    }
}
