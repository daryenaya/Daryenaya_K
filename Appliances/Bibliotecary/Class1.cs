using System;
using System.Data.SqlClient;

namespace MyApplication
{
    public class DatabaseManager
    {
        private SqlConnection _connection;

        public DatabaseManager(SqlConnection @object)
        {
        }

        public DatabaseManager()
        {
        }

        public void Connect(string connectionString)
        {
            try
            {
                _connection = new SqlConnection(connectionString);
                _connection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Ошибка доступа к базе данных: {ex.Message}");
                throw; // Лучше прокинуть исключение дальше для обработки
            }
        }

        public void Close()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return _connection;
        }

        // Метод для выполнения запросов (например, с параметрами)
        public void ExecuteCommand(string commandText, SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(commandText, _connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                command.ExecuteNonQuery();
            }
        }

        public void ExecuteCommand(string v)
        {
            throw new NotImplementedException();
        }

        public void ExecuteReader(string v)
        {
            throw new NotImplementedException();
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // Здесь можно добавить хэширование пароля

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class Request
    {
        public string RequestID { get; set; }
        public DateTime StartDate { get; set; }
        public string HomeTechType { get; set; }
        public string ProblemDescription { get; set; }
        public string RequestStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string RepairParts { get; set; }
        public string MasterFIO { get; set; }
        public string ClientFIO { get; set; }
        public string Message { get; set; }

        public Request(string requestID, DateTime startDate, string homeTechType, string problemDescription,
                       string requestStatus, DateTime? completionDate, string repairParts, string masterFIO,
                       string clientFIO, string message)
        {
            RequestID = requestID;
            StartDate = startDate;
            HomeTechType = homeTechType;
            ProblemDescription = problemDescription;
            RequestStatus = requestStatus;
            CompletionDate = completionDate;
            RepairParts = repairParts;
            MasterFIO = masterFIO;
            ClientFIO = clientFIO;
            Message = message;
        }
    }

    public class RequestManager
    {
        private readonly DatabaseManager _dbManager;

        public RequestManager(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        // Метод для добавления запроса
        public void AddRequest(Request request)
        {
            var commandText = @"INSERT INTO Request (RequestID, StartDate, HomeTechType, ProblemDescription, 
                                                       RequestStatus, CompletionDate, RepairParts, 
                                                       MasterFIO, ClientFIO, Message) 
                                VALUES (@RequestID, @StartDate, @HomeTechType, @ProblemDescription, 
                                        @RequestStatus, @CompletionDate, @RepairParts, 
                                        @MasterFIO, @ClientFIO, @Message)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@RequestID", request.RequestID),
                new SqlParameter("@StartDate", request.StartDate),
                                new SqlParameter("@HomeTechType", request.HomeTechType),
                new SqlParameter("@ProblemDescription", request.ProblemDescription),
                new SqlParameter("@RequestStatus", request.RequestStatus),
                new SqlParameter("@CompletionDate", (object)request.CompletionDate ?? DBNull.Value),
                new SqlParameter("@RepairParts", request.RepairParts),
                new SqlParameter("@MasterFIO", request.MasterFIO),
                new SqlParameter("@ClientFIO", request.ClientFIO),
                new SqlParameter("@Message", request.Message)
            };

            try
            {
                if (_dbManager.GetConnection().State != System.Data.ConnectionState.Open)
                {
                    _dbManager.Connect("your_connection_string"); // Лучше передавать строку подключения через конструктор
                }

                _dbManager.ExecuteCommand(commandText, parameters);
            }
            catch (Exception ex)
            {
                // Обработайте ошибку (например, логирование)
                Console.WriteLine($"Ошибка при добавлении запроса: {ex.Message}");
                throw; // Прокиньте исключение дальше, если требуется
            }
            finally
            {
                // Не закрывайте соединение тут, это может быть необходимо для дальнейших операций
            }
        }

        public void DeleteRequest(string requestId)
        {
            throw new NotImplementedException();
        }

        public object GetRequestById(string requestId)
        {
            throw new NotImplementedException();
        }

        // Добавьте дополнительные методы для работы с запросами здесь
    }
}
