using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Warehouse.Model.Models;

namespace Warehouse.Model.DataOperations
{
    public class ProductsDataAccessLayer : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection = null;
        private bool _disposed;

        public bool IsDBExist { get; set; }
        public ProductsDataAccessLayer()
        {
            _connectionString = $@"Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";
            IsDBExist = CheckDataBaseStatus();
        }

        private void OpenConnection()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection = new SqlConnection()
                {
                    ConnectionString = _connectionString
                };
                _sqlConnection.Open();
            }
        }
        private void CloseConnection()
        {
            if (_sqlConnection.State != ConnectionState.Closed)
            {
                _sqlConnection.Close();
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _sqlConnection.Dispose();
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #region CRUD Operatons

        #region Checking and Creation DB
        private bool IsTheLocalDBExists()
        {
            OpenConnection();           
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

        public bool CheckDataBaseStatus()
        {
            if (IsTheLocalDBExists())
            {
                return true;
            }
            else
            {
                
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

        private void CreationDB()
        {
            var queriesStack = new Stack<string>();
            //Pushing the SQL scripts in a specified sequence
            queriesStack.Push(SQLQueries.CreatingProductTrigger);
            queriesStack.Push(SQLQueries.UpdatingProductTrigger);
            queriesStack.Push(SQLQueries.UpdatingMovementsProcedure);
            queriesStack.Push(SQLQueries.MovementsTableStructure);
            queriesStack.Push(SQLQueries.ProductsTableStructure);
            queriesStack.Push(SQLQueries.StatesTableStructure);
            queriesStack.Push(SQLQueries.WarehoueseDBCreation);

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
                        catch (TransactionException ex2)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        //If the transaction is successful, all changes will be committed to the database.
                        transaction.Commit();
                    }
                }
            } while (queriesStack.Count > 0);
            CloseConnection();
        }

        private void FillDataBase()
        {
            var queriesStack = new Stack<string>();
            //Pushing the SQL scripts in a specified sequence
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
                        catch (TransactionException ex2)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        //If the transaction is successful, all changes will be committed to the database.
                        transaction.Commit();
                    }
                }
            } while (queriesStack.Count > 0);
            CloseConnection();
        }
        #endregion

        public ObservableCollection<Movements> GetAllMovements()
        {
            OpenConnection();
            var listMovements = new ObservableCollection<Movements>();
            var sql = SQLQueries.SelectMovements;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listMovements.Add(new Movements
                    {
                        ID = (int)reader["ID"],
                        DateStamp = (DateTime)reader["DateStamp"],
                        Product = new Product
                        {
                            Id = (int)reader["ProductId"],
                            Name = (string)reader["ProductName"],
                            SKU = (string)reader["SKU"],
                            State = new State { Id = (int)reader["StateId"], Name = (string)reader["StateName"] }
                        },
                        State = new State { Id = (int)reader["StateId"], Name = (string)reader["StateName"] }

                    });
                    reader.Close();
                }

                CloseConnection();
                return listMovements;
            }


        }

        public ObservableCollection<Product> GetAllProducts()
        {
            OpenConnection();
            var products = new ObservableCollection<Product>();
            var sql = SQLQueries.SelectMovements;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                        {
                            Id = (int)reader["ProductId"],
                            Name = (string)reader["ProductName"],
                            SKU = (string)reader["SKU"],
                            State = new State { Id = (int)reader["StateId"], Name = (string)reader["StateName"] }
                        });
                    reader.Close();
                }

                CloseConnection();
                return products;
            }


        }

        public ObservableCollection<State> GetAllStates()
        {
            OpenConnection();
            var states = new ObservableCollection<State>();
            var sql = SQLQueries.SelectStates;

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    states.Add(new State { Id = (int)reader["ID"], Name = (string)reader["Name"] });
                    reader.Close();
                }

                CloseConnection();
                return states;
            }


        }
    }
}
#endregion
