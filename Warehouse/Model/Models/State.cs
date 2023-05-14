namespace Warehouse.Model.Models
{
    /// <summary>
    /// Represents an one of the state of the <see cref="Product"/> from the database. A model for parsing data from the DB.  
    /// </summary>
    public class State
    {
        /// <summary>
        /// The unique ID for each state.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The unique name of the state
        /// </summary>
        public string Name { get; set; }
    }
}
