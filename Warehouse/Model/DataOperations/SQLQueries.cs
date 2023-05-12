using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Model.DataOperations
{
    internal static class SQLQueries
    {
        public static string WarehoueseDBCreation { get; } = @"USE [master]
                                                                    CREATE DATABASE [Warehouse]
                                                                    ALTER DATABASE [Warehouse] SET READ_COMMITTED_SNAPSHOT ON";
        public static string StatesTableStructure { get; } = @"USE [Warehouse]
                                                                    CREATE TABLE dbo.States (
                                                                        ID INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL
                                                                        );";
    
        public static string ProductsTableStructure { get; } = @"USE [Warehouse]
                                                                      CREATE TABLE dbo.Products (
                                                                        ID INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
                                                                        Name NVARCHAR(30) NOT NULL,
                                                                        SKU NVARCHAR(30) NOT NULL,
                                                                        StateID INT NOT NULL,
                                                                        CONSTRAINT PK_Products PRIMARY KEY (ID),
                                                                        CONSTRAINT FK_Products_States FOREIGN KEY (StateID) REFERENCES dbo.States (ID)
                                                                        );";
        public static string MovementsTableStructure { get; } = @"USE [Warehouse]
                                                                       CREATE TABLE dbo.Movements (
                                                                            ID INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
                                                                            ProductID INT NOT NULL,
                                                                            StateID INT NOT NULL,
                                                                            DateStamp DATE NOT NULL,
                                                                            CONSTRAINT PK_Movements PRIMARY KEY (ID),
                                                                            CONSTRAINT FK_Movements_Products FOREIGN KEY (ProductID) REFERENCES dbo.Products (ID),
                                                                            CONSTRAINT FK_Movements_States FOREIGN KEY (StateID) REFERENCES dbo.Statuses (ID)
                                                                        );";
        public static string UpdatingMovementsProcedure { get; } = @"USE [Warehouse]
                                                                    CREATE PROCEDURE dbo.UpdateMovements
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
        public static string UpdatingProductTrigger { get; } =      @"USE [Warehouse]
                                                                    CREATE TRIGGER dbo.Trigger_UpdateMovementsOnStateChange
                                                                    ON dbo.Products
                                                                    AFTER UPDATE
                                                                    AS
                                                                    BEGIN
                                                                        IF UPDATE(StateID)
                                                                        BEGIN
                                                                            DECLARE @ProductID INT, @StateID INT;
        
                                                                            SELECT @ProductID = ID, @StateID = StateID FROM inserted;

                                                                            EXEC dbo.UpdateMovements @ProductID, @StateID;
                                                                        END
                                                                    END";
        public static string CreatingProductTrigger { get; } = @"USE [Warehouse]
                                                                    CREATE TRIGGER dbo.Trigger_UpdateMovementsOnProductInsert
                                                                    ON dbo.Products
                                                                    AFTER INSERT
                                                                    AS
                                                                    BEGIN
                                                                        DECLARE @ProductID INT, @StateID INT;
        
                                                                        SELECT @ProductID = ID, @StateID = StateID FROM inserted;

                                                                        EXEC dbo.UpdateMovements @ProductID, @StateID;
                                                                    END";
        public static string CheckDBForExisting { get; } = @"USE [Warehouse]
                                                             SELECT name
                                                             FROM sys.databases
                                                             WHERE name = 'Warehouse';";
        public static string FillingStates { get; } = @"USE [Warehouse]
                                                        INSERT INTO dbo.States (Name) VALUES                                                                
                                                               ('Принят'),
                                                               ('На складе'),
                                                               ('Продан');";
        public static string FillingProducts { get; } = @"USE [Warehouse]
                                                          INSERT INTO dbo.Products (Name, SKU, StateID) VALUES
                                                                ('Товар 1', '123333' 1),
                                                                ('Товар 2', '433333', 2),
                                                                ('Товар 3', '3444455', 2),
                                                                ('Товар 4', '433333', 3);";
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
    }
}
