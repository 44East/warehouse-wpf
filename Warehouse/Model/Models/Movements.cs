using System;

namespace Warehouse.Model.Models
{
    /// <summary>
    /// Represents an one of the <see cref="Models.Product"/> movement in the warehouse by <see cref="Models.State"/>. A model for parsing data from the DB.
    /// </summary>
    public class Movements
    {
        /// <summary>
        /// The unique ID for each product.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// A specific instance of the <see cref="Models.Product"/> model uses Many-To-Many relationships.
        /// </summary>
        public Product Product { get; set; }
        /// <summary>
        /// A specific instance of the <see cref="Models.State"/> model uses Many-To-Many relationships.
        /// </summary>
        public State State { get; set; }
        /// <summary>
        /// Date and time of a creation the movement record in the database
        /// </summary>
        public DateTime DateStamp { get; set; }
    }
}
