namespace Warehouse.Model.DataOperations
{
    /// <summary>
    /// The static service class is responsible for handling all commands to the ADO.NET connection to the local database instance by MSSQL Server.
    /// It uses T-SQL version of the SQL variations
    /// </summary>
    internal static class SQLQueries
    {
        /// <summary>
        /// A query for creating a new database [Warehouse].
        /// </summary>
        public static string WarehoueseDBCreation { get; } = @"USE [master]
                                                                    CREATE DATABASE [Warehouse]";
        /// <summary>
        /// A query for creating a new table [dbo.States] to store all the states for products.
        /// </summary>
        public static string StatesTableStructure { get; } = @"USE [Warehouse]
                                                                    CREATE TABLE dbo.States (
                                                                        ID INT NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL,
                                                                        CONSTRAINT PK_States PRIMARY KEY (ID)
                                                                        );";
        /// <summary>
        /// A query for creating a new table [dbo.Products] to store all the products.
        /// </summary>
        public static string ProductsTableStructure { get; } = @"USE [Warehouse]
                                                                      CREATE TABLE dbo.Products (
                                                                        ID INT IDENTITY (1,1) NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL,
                                                                        SKU NVARCHAR(30) NOT NULL,
                                                                        StateID INT NOT NULL,
                                                                        CONSTRAINT PK_Products PRIMARY KEY (ID),
                                                                        CONSTRAINT FK_Products_States FOREIGN KEY (StateID) REFERENCES dbo.States (ID)
                                                                        );";
        /// <summary>
        /// A query for creating a new table [dbo.Movements] to store all the movements of products by datetime and state.
        /// </summary>
        public static string MovementsTableStructure { get; } = @"USE [Warehouse]
                                                                       CREATE TABLE dbo.Movements (
                                                                            ID INT IDENTITY (1,1) NOT NULL,
                                                                            ProductID INT NOT NULL,
                                                                            StateID INT NOT NULL,
                                                                            DateStamp DATETIME NOT NULL,
                                                                            CONSTRAINT PK_Movements PRIMARY KEY (ID),
                                                                            CONSTRAINT FK_Movements_Products FOREIGN KEY (ProductID) REFERENCES dbo.Products (ID),
                                                                            CONSTRAINT FK_Movements_States FOREIGN KEY (StateID) REFERENCES dbo.States (ID)
                                                                        );";
        
        /// <summary>
        /// A query for creating a stored procedure in the database to add a new movement record in the Movements table after updating a state of an existing product or if a new product was added.
        /// </summary>
        public static string AddMovementAfterCreateOrUpdateProcedure { get; } = @"CREATE PROCEDURE dbo.InsertMovementsOnStateChange
                                                                                        @ProductID INT,
                                                                                        @StateID INT
                                                                                    AS
                                                                                    BEGIN
                                                                                        IF EXISTS (SELECT 1 FROM dbo.Products WHERE ID = @ProductID)
                                                                                        BEGIN
                                                                                            INSERT INTO dbo.Movements (ProductID, StateID, DateStamp)
                                                                                            VALUES (@ProductID, @StateID, GETDATE())
                                                                                        END
                                                                                    END";
        /// <summary>
        /// A query for creating a new trigger to check all updates in the dbo.Products table. It captures all changes in the product state for each operation and sends them to a stored procedure to create a new movement record.
        /// </summary>
        public static string UpdatingProductTrigger { get; } = @"CREATE TRIGGER dbo.Trigger_UpdateMovementsOnStateChange
                                                                    ON dbo.Products
                                                                    AFTER UPDATE
                                                                    AS
                                                                    BEGIN
                                                                        IF UPDATE(StateID)
                                                                        BEGIN
                                                                            DECLARE @ProductID INT, @StateID INT;
        
                                                                            SELECT @ProductID = ID, @StateID = StateID FROM inserted;

                                                                            EXEC dbo.InsertMovementsOnStateChange @ProductID, @StateID;
                                                                        END
                                                                    END";
        /// <summary>
        /// A query for creating a new trigger for checking all creatings in the dbo.Products table. It captures all inserts in the product table for each operation and sends them to a stored procedure to create a new movement record.
        /// </summary>
        public static string CreatingProductTrigger { get; } = @"CREATE TRIGGER Trigger_UpdateMovementsOnProductInsert
                                                                    ON Products
                                                                    AFTER INSERT
                                                                    AS
                                                                    BEGIN
                                                                        DECLARE @ProductID INT, @StateID INT;

                                                                        SELECT @ProductID = Id, @StateID = StateID FROM inserted;

                                                                        EXEC dbo.InsertMovementsOnStateChange @ProductID, @StateID;
                                                                    END";
        /// <summary>
        /// A query for checking the existence of a database.
        /// </summary>
        public static string CheckDBForExisting { get; } = @"SELECT name
                                                             FROM sys.databases
                                                             WHERE name = 'Warehouse';";
        /// <summary>
        /// A query for filling the database a test data
        /// </summary>
        public static string FillingStates { get; } = @"USE [Warehouse]
                                                        INSERT INTO dbo.States (ID, Name) VALUES                                                                
                                                               (1, N'Принят'),
                                                               (2, N'На складе'),
                                                               (3, N'Продан');";
        /// <summary>
        /// A query for filling the database a test data
        /// </summary>
        public static string FillingProducts { get; } = @"USE [Warehouse]
                                                          INSERT INTO dbo.Products (Name, SKU, StateID) VALUES
                                                                (N'PlayStation 5', N'CF-2201', 1),
                                                                (N'Iphone 14 Pro', N'YAL-21', 2),
                                                                (N'Nokia 8.3', N'TAL-1243', 2),
                                                                (N'Intel core I9 13900KF', N'QES9582', 3);";
        /// <summary>
        /// A query for receive all the movements from the [dbo.Movements] table. This query binds data from the connected tables [dbo.Products] and [dbo.States]. 
        /// </summary>
        public static string SelectMovements { get; } = @"USE [Warehouse]
                                                          SELECT M.ID, M.DateStamp, P.ID AS ProductId, P.Name AS ProductName, P.SKU, S.ID AS StateId, S.Name AS StateName
                                                          FROM dbo.Movements M
                                                          INNER JOIN dbo.Products P ON M.ProductId = P.ID
                                                          INNER JOIN dbo.States S ON M.StateId = S.ID";
        /// <summary>
        /// A query for receive all the products from the [dbo.Products] table. This query binds data from the connected table [dbo.States]. 
        /// </summary>
        public static string SelectProducts { get; } = @"USE [Warehouse]
                                                         SELECT P.ID, P.Name, P.SKU, S.ID AS StateId, S.Name AS StateName
                                                         FROM dbo.Products P
                                                         INNER JOIN dbo.States S ON P.StateId = S.ID";
        /// <summary>
        ///  A query for receive all the products from the [dbo.States] table. 
        /// </summary>
        public static string SelectStates { get; } = @"USE [Warehouse]
                                                       SELECT TOP (10) 
                                                       ID,
                                                       Name
                                                       FROM dbo.States";
        /// <summary>
        /// A query for inserting a new product into database. It stores formating elements for the string.Format() method
        /// </summary>
        public static string InsertProduct { get; } = @"USE [Warehouse]
                                                        INSERT INTO dbo.Products (Name, SKU, StateID) VALUES
                                                        (N'{0}', N'{1}', {2})";
        /// <summary>
        /// A query for updating an existing product into database. It stores formating elements for the string.Format() method
        /// </summary>
        public static string UpdateProductState { get; } = @"USE [Warehouse]
                                                             UPDATE dbo.Products SET StateId = {0} WHERE ID = {1}";
    }
}
