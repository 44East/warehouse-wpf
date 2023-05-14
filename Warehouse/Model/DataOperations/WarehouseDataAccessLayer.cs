using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Warehouse.Model.Models;

namespace Warehouse.Model.DataOperations
{
    /// <summary>
    /// This class relies on the ADO.NET framework for performing CRUD operations and provides data access to <see cref="Product"/>, <see cref="State"/>, and <see cref="Movements"/> objects from a MSSQL database.
    /// </summary>
    public class WarehouseDataAccessLayer : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection = null;
        private bool _disposed;

        /// <summary>
        /// A service bollean property for monitoring the database state
        /// </summary>
        public bool IsDBExist { get; private set; }
        /// <summary>
        /// The open constructor creates a new instance of <see cref="WarehouseDataAccessLayer"/> then
        /// sets the connection string for <see cref="SqlConnection"/>
        /// and checks database for existing
        /// </summary>
        public WarehouseDataAccessLayer()
        {
            _connectionString = $@"Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";
            IsDBExist = CheckDataBaseStatus();
        }

        /// <summary>
        /// Opens an connection to the MSSQL database using the <see cref="SqlConnection"/> class.
        /// </summary>
        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection()
            {
                ConnectionString = _connectionString
            };
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
        }
        /// <summary>
        /// Closing the connection to the MSSQL database.
        /// </summary>
        private void CloseConnection()
        {
            if (_sqlConnection.State != ConnectionState.Closed)
            {
                _sqlConnection.Close();
            }
        }
        /// <summary>
        /// Checks the existence of the <see cref="SqlConnection"/>. If it exists, the method disposes of the <see cref="SqlConnection"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _sqlConnection.Dispose();
            _disposed = true;
        }
        /// <summary>
        /// Realize IDisposable method for disposing this class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #region CRUD Operatons

        #region Checking and Creation DB
        /// <summary>
        /// Checks whether the local database exists or not.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> value indicating whether the database exists or not.</returns>
        /// <remarks>This method executes a SQL script to check if the local database exists. If it does, the method returns true, otherwise false.</remarks>
        private bool IsTheLocalDBExists()
        {
            OpenConnection();
            //Use the service class of storage queries.
            var sql = SQLQueries.CheckDBForExisting;
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return ((string)reader["name"]).Equals("Warehouse");//The query must return full name of database [Warehouse]
                    }
                }
            }
            CloseConnection();

            return false;
        }
        /// <summary>
        /// Checks the status of the local database otherwise creates and fills the database.
        /// </summary>
        private bool CheckDataBaseStatus()
        {
            if (IsTheLocalDBExists())
            {
                return true;
            }
            else
            {
                //if the database doesn't exist, the next code creates database and sructure
                //and then fills it tests objects
                try
                {
                    CreationDB();
                    FillDataBase();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Creates the Warehouse database structure by executing SQL scripts stored in a service class <see cref="SQLQueries"/>.
        /// Uses the <see cref="Stack{T}"/> collection to makes the database by querys queue
        /// </summary>
        private void CreationDB()
        {
            OpenConnection();

            // Execute the CREATE DATABASE statement separately
            using (SqlCommand createDBCommand = new SqlCommand(SQLQueries.WarehoueseDBCreation, _sqlConnection))
            {
                createDBCommand.CommandType = CommandType.Text;
                createDBCommand.ExecuteNonQuery();
            }

            // Execute the remaining SQL commands in a transaction
            //If there are problems during a transaction, all changes will be rolled back.
            using (SqlTransaction structureTransaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    var queriesStack = new Stack<string>();
                    //Use the service class of storage queries.
                    queriesStack.Push(SQLQueries.CreatingProductTrigger);
                    queriesStack.Push(SQLQueries.UpdatingProductTrigger);
                    queriesStack.Push(SQLQueries.AddMovementAfterCreateOrUpdateProcedure);
                    queriesStack.Push(SQLQueries.MovementsTableStructure);
                    queriesStack.Push(SQLQueries.ProductsTableStructure);
                    queriesStack.Push(SQLQueries.StatesTableStructure);

                    while (queriesStack.Count > 0)
                    {
                        string query = queriesStack.Pop();
                        using (SqlCommand command = new SqlCommand(query, _sqlConnection))
                        {
                            command.Transaction = structureTransaction;
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Try to roll back all changes
                    try
                    {
                        structureTransaction?.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
                finally
                {
                    //If the transaction is successful, all changes will be committed to the database.
                    structureTransaction?.Commit();
                }

            }

            CloseConnection();
        }
        /// <summary>
        /// Fills the database with test data.
        /// Uses the <see cref="Stack{T}"/> collection to fills the database by querys queue.
        /// </summary>
        private void FillDataBase()
        {
            var queriesStack = new Stack<string>();
            //Pushing the SQL scripts in a specified sequence
            //Use the service class of storage queries.
            queriesStack.Push(SQLQueries.FillingProducts);
            queriesStack.Push(SQLQueries.FillingStates);

            OpenConnection();
            do
            {
                using (SqlCommand command = new SqlCommand(queriesStack.Pop(), _sqlConnection))
                {
                    //If there are problems during a transaction, all changes will be rolled back.
                    SqlTransaction transaction = null;
                    try
                    {
                        transaction = _sqlConnection.BeginTransaction();
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        try
                        {
                            //Try to roll back all changes
                            transaction?.Rollback();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        //If the transaction is successful, all changes will be committed to the database.
                        transaction?.Commit();
                    }
                }
            } while (queriesStack.Count > 0);
            CloseConnection();
        }
        #endregion

        #region Read
        /// <summary>
        /// Retrieves all <see cref="Movements"/> objects from the database, and binds <see cref="Product"/> and <see cref="State"/> instances for each <see cref="Movements"/>. 
        /// If the connection to the database  doesn't exist, returns an empty <see cref="ObservableCollection{T}"/> of <see cref="Movements"/>.
        /// </summary>
        /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="Movements"/></returns>
        public ObservableCollection<Movements> GetAllMovements()
        {
            OpenConnection();
            var listMovements = new ObservableCollection<Movements>();
            //Use the service class of storage queries.
            var sql = SQLQueries.SelectMovements;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listMovements.Add(new Movements
                        {
                            ID = (int)reader["ID"],
                            DateStamp = (DateTime)reader["DateStamp"],
                            //Bind a product and include the current state
                            Product = new Product
                            {
                                Id = (int)reader["ProductId"],
                                Name = (string)reader["ProductName"],
                                SKU = (string)reader["SKU"],
                                State = new State { Id = (int)reader["StateId"], Name = ((string)reader["StateName"]).Trim() }
                            },
                            //Bind the state for the current product and the current movement
                            State = new State { Id = (int)reader["StateId"], Name = ((string)reader["StateName"]).Trim() }

                        });
                    }
                }

                CloseConnection();
                return listMovements;
            }


        }
        /// <summary>
        /// Retrieves all <see cref="Product"/> objects from the database, and binds the current <see cref="State"/> instance for each <see cref="Product"/>. 
        /// If the connection to the database  doesn't exist, returns an empty <see cref="ObservableCollection{T}"/> of <see cref="Product"/>.
        /// </summary>
        /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="Product"/></returns>
        public ObservableCollection<Product> GetAllProducts()
        {
            OpenConnection();
            var products = new ObservableCollection<Product>();
            //Use the service class of storage queries.
            var sql = SQLQueries.SelectProducts;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = (int)reader["ID"],
                            Name = (string)reader["Name"],
                            SKU = (string)reader["SKU"],
                            //Bind the state for the current product 
                            State = new State { Id = (int)reader["StateId"], Name = ((string)reader["StateName"]).Trim() }
                        });

                    }
                }

                CloseConnection();
                return products;
            }


        }

        /// <summary>
        /// Retrieves all <see cref="State"/> objects from the database. 
        /// If the connection to the database  doesn't exist, returns an empty <see cref="ObservableCollection{T}"/> of <see cref="State"/>.
        /// </summary>
        /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="State"/></returns>
        public ObservableCollection<State> GetAllStates()
        {
            OpenConnection();
            var states = new ObservableCollection<State>();
            //Use the service class of storage queries.
            var sql = SQLQueries.SelectStates;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        states.Add(new State { Id = (int)reader["ID"], Name = ((string)reader["Name"]).Trim() });
                    }
                }

                CloseConnection();
                return states;
            }


        }
        #endregion

        #region Create
        /// <summary>
        /// Inserts a new <see cref="Product"/> record into the database.       
        /// If there are problems during a transaction, all changes will be rolled back.If the transaction is successful, all changes will be committed to the database.
        /// </summary>
        /// <param name="product"> The <see cref="Product"/> object to be inserted into the database.</param>
        public void InsertProduct(Product product)
        {
            OpenConnection();
            //Use the service class of storage queries.
            //Formating the string in a specified format
            var sql = string.Format(SQLQueries.InsertProduct, product.Name, product.SKU, product.State.Id);
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                //If there are problems during a transaction, all changes will be rolled back.
                SqlTransaction transaction = null;
                try
                {
                    transaction = _sqlConnection.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    try
                    {
                        //Try to roll back all changes
                        transaction?.Rollback();
                    }
                    catch (TransactionException ex2)
                    {
                        throw;
                    }
                }
                finally
                {
                    //If the transaction is successful, all changes will be committed to the database.
                    transaction?.Commit();
                }
            }
            CloseConnection();
        }
        #endregion

        #region Update
        /// <summary>
        /// Updats an existing <see cref="Product"/> record into the database.       
        /// If there are problems during a transaction, all changes will be rolled back.If the transaction is successful, all changes will be committed to the database.
        /// </summary>
        /// <param name="product"> The <see cref="Product"/> object to be updated into the database.</param>
        public void UpdateProduct(Product product)
        {
            OpenConnection();
            //Use the service class of storage queries.
            //Formating the string in a specified format
            var sql = string.Format(SQLQueries.UpdateProductState, product.State.Id, product.Id);
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                //If there are problems during a transaction, all changes will be rolled back.
                SqlTransaction transaction = null;
                try
                {
                    transaction = _sqlConnection.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    try
                    {
                        //Try to roll back all changes
                        transaction?.Rollback();
                    }
                    catch (TransactionException ex2)
                    {
                        throw;
                    }
                }
                finally
                {
                    //If the transaction is successful, all changes will be committed to the database.
                    transaction?.Commit();
                }
            }
            CloseConnection();
        }
        #endregion
    }
}
#endregion
