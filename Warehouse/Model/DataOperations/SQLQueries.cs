namespace Warehouse.Model.DataOperations
{
    internal static class SQLQueries
    {
        public static string WarehoueseDBCreation { get; } = @"USE [master]
                                                                    CREATE DATABASE [Warehouse]";
        public static string StatesTableStructure { get; } = @"USE [Warehouse]
                                                                    CREATE TABLE dbo.States (
                                                                        ID INT NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL,
                                                                        CONSTRAINT PK_States PRIMARY KEY (ID)
                                                                        );";
    
        public static string ProductsTableStructure { get; } = @"USE [Warehouse]
                                                                      CREATE TABLE dbo.Products (
                                                                        ID INT IDENTITY (1,1) NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL,
                                                                        SKU NVARCHAR(30) NOT NULL,
                                                                        StateID INT NOT NULL,
                                                                        CONSTRAINT PK_Products PRIMARY KEY (ID),
                                                                        CONSTRAINT FK_Products_States FOREIGN KEY (StateID) REFERENCES dbo.States (ID)
                                                                        );";
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
        public static string InsertProductProcedure { get; } = @"CREATE PROCEDURE dbo.InsertProductWithMovement
                                                                        @Name NVARCHAR(50),
                                                                        @SKU NVARCHAR(20),
                                                                        @StateID INT
                                                                    AS
                                                                    BEGIN
                                                                        SET NOCOUNT ON;

                                                                        INSERT INTO Products (Name, SKU, StateID)
                                                                        VALUES (@Name, @SKU, @StateID);

                                                                        DECLARE @ProductID INT;
                                                                        SET @ProductID = SCOPE_IDENTITY();

                                                                        INSERT INTO Movements (ProductID, StateID, DateStamp)
                                                                        VALUES (@ProductID, @StateID, GETDATE());
                                                                    END";
        public static string UpdatingMovementsProcedure { get; } = @"CREATE PROCEDURE dbo.UpdateMovements
                                                                        @ProductID INT,
                                                                        @StateID INT
                                                                    AS
                                                                    BEGIN
                                                                        IF EXISTS (SELECT 1 FROM dbo.Movements WHERE ProductID = @ProductID)
                                                                        BEGIN
                                                                            UPDATE dbo.Movements
                                                                            SET StateID = @StateID
                                                                            WHERE ProductID = @ProductID
                                                                        END
                                                                        ELSE
                                                                        BEGIN
                                                                            INSERT INTO dbo.Movements (ProductID, StateID, DateStamp)
                                                                            VALUES (@ProductID, @StateID, GETDATE())
                                                                        END
                                                                    END";
        public static string AddMovementAfterUpdateProcedure { get; } = @"CREATE PROCEDURE dbo.InsertMovementsOnStateChange
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
        public static string CreatingProductTrigger { get; } = @"CREATE TRIGGER Trigger_UpdateMovementsOnProductInsert
                                                                    ON Products
                                                                    AFTER INSERT
                                                                    AS
                                                                    BEGIN
                                                                        DECLARE @ProductID INT, @StateID INT;

                                                                        SELECT @ProductID = Id, @StateID = StateID FROM inserted;

                                                                        EXEC dbo.UpdateMovements @ProductID, @StateID;
                                                                    END";
        public static string CheckDBForExisting { get; } = @"SELECT name
                                                             FROM sys.databases
                                                             WHERE name = 'Warehouse';";
        public static string FillingStates { get; } = @"USE [Warehouse]
                                                        INSERT INTO dbo.States (ID, Name) VALUES                                                                
                                                               (1, N'Принят'),
                                                               (2, N'На складе'),
                                                               (3, N'Продан');";
        public static string FillingProducts { get; } = @"USE [Warehouse]
                                                          INSERT INTO dbo.Products (Name, SKU, StateID) VALUES
                                                                (N'Товар 1', N'123333', 1),
                                                                (N'Товар 2', N'433333', 2),
                                                                (N'Товар 3', N'3444455', 2),
                                                                (N'Товар 4', N'433333', 3);";
        public static string SelectMovements { get; } = @"USE [Warehouse]
                                                          SELECT M.ID, M.DateStamp, P.ID AS ProductId, P.Name AS ProductName, P.SKU, S.ID AS StateId, S.Name AS StateName
                                                          FROM dbo.Movements M
                                                          INNER JOIN dbo.Products P ON M.ProductId = P.ID
                                                          INNER JOIN dbo.States S ON M.StateId = S.ID";
        public static string SelectProducts { get; } = @"USE [Warehouse]
                                                         SELECT P.ID, P.Name, P.SKU, S.ID AS StateId, S.Name AS StateName
                                                         FROM dbo.Products P
                                                         INNER JOIN dbo.States S ON P.StateId = S.ID";
        public static string SelectStates { get; } = @"USE [Warehouse]
                                                       SELECT TOP (10) 
                                                       ID,
                                                       Name
                                                       FROM dbo.States";
        public static string InsertProduct { get; } = @"USE [Warehouse]
                                                        INSERT INTO dbo.Products (Name, SKU, StateID) VALUES
                                                        (N'{0}', N'{1}', {2})";
        public static string UpdateProductState { get; } = @"USE [Warehouse]
                                                             UPDATE dbo.Products SET StateId = {0} WHERE ID = {1}";
        public static string SelectProdyctByState { get; } = @"USE [Warehouse]
                                                             SELECT P.ID, P.Name, P.SKU, S.ID AS StateId, S.Name AS StateName
                                                             FROM dbo.Products P
                                                             INNER JOIN dbo.States S ON P.StateId = S.ID
														     WHERE S.Name = N'{0}';";
    }
}
