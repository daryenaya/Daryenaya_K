using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MyApplication.Tests
{
    [TestClass]
    public class DatabaseManagerTests
    {
        private DatabaseManager _dbManager;
        private Mock<SqlConnection> _mockConnection;

        [TestInitialize]
        public void Setup()
        {
            _mockConnection = new Mock<SqlConnection>();
            _dbManager = new DatabaseManager();
        }

        [TestMethod]
        public void Connect_ShouldOpenConnection()
        {
            // Arrange
            string connectionString = "Data Source=TestServer;Initial Catalog=TestDatabase;Integrated Security=True;";

            // Act
            _dbManager.Connect(connectionString);

            // Assert
            Assert.IsNotNull(_dbManager.GetConnection());
            Assert.AreEqual(System.Data.ConnectionState.Open, _dbManager.GetConnection().State);
        }

        [TestMethod]
        public void Close_ShouldCloseConnection()
        {
            // Arrange
            string connectionString = "Data Source=TestServer;Initial Catalog=TestDatabase;Integrated Security=True;";
            _dbManager.Connect(connectionString);
            Assert.AreEqual(System.Data.ConnectionState.Open, _dbManager.GetConnection().State);

            // Act
            _dbManager.Close();

            // Assert
            Assert.AreEqual(System.Data.ConnectionState.Closed, _dbManager.GetConnection().State);
        }
    }

    [TestClass]
    public class RequestManagerTests
    {
        private Mock<DatabaseManager> _mockDbManager;
        private RequestManager _requestManager;

        [TestInitialize]
        public void Setup()
        {
            _mockDbManager = new Mock<DatabaseManager>();
            _requestManager = new RequestManager(_mockDbManager.Object);
        }

        [TestMethod]
        public void AddRequest_ShouldExecuteInsertCommand()
        {
            // Arrange
            var request = new Request("1", DateTime.Now, "Стиральная машина", "Не включается", "В процессе", null, "Запчасти", "Мастер ФИО", "Клиент ФИО", "Сообщение");

            string commandText = $"INSERT INTO Request (RequestID, StartDate, HomeTechType, ProblemDescription, RequestStatus, CompletionDate, RepairParts, MasterFIO, ClientFIO, Message) " +
                                 $"VALUES (@RequestID, @StartDate, @HomeTechType, @ProblemDescription, @RequestStatus, @CompletionDate, @RepairParts, @MasterFIO, @ClientFIO, @Message)";

            _mockDbManager.Setup(db => db.ExecuteCommand(It.IsAny<string>())).Verifiable("ExecuteCommand was not called");

            // Act
            _requestManager.AddRequest(request);

            // Assert
            _mockDbManager.Verify(db => db.ExecuteCommand(It.Is<string>(cmd => cmd.StartsWith("INSERT"))), Times.Once);
        }

        [TestMethod]
        public void DeleteRequest_ShouldExecuteDeleteCommand()
        {
            // Arrange
            string requestId = "1";
            string commandText = "DELETE FROM Request WHERE RequestID = @RequestID";

            _mockDbManager.Setup(db => db.ExecuteCommand(It.IsAny<string>())).Verifiable("ExecuteCommand was not called");

            // Act
            _requestManager.DeleteRequest(requestId);

            // Assert
            _mockDbManager.Verify(db => db.ExecuteCommand(It.Is<string>(cmd => cmd.Contains("DELETE"))), Times.Once);
        }

        [TestMethod]
        public void GetRequestById_ShouldReturnRequest()
        {
            // Arrange
            string requestId = "1";
            // Здесь можно дополнительно настроить поведение моков, если необходимо.
            // Например, можно настроить возвращаемое значение для ExecuteReader и т.п.

            // Act
            var request = _requestManager.GetRequestById(requestId);

            // Assert
            Assert.IsNotNull(request);
            // Дополнительно можно проверить свойства request, если на мок-составлялись данные
        }
    }
}
