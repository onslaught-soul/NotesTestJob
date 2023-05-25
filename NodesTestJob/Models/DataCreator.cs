using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NodesTestJob.Models
{
    public class DataCreator
    {
        private static object createChecker { get; set; }
        private readonly string _connectionString;
        private readonly DataManager _dataManager;
        public DataCreator(IConfiguration configuration, DataManager dataManager)
        {
            _connectionString = configuration.GetConnectionString("NotesData");
            _dataManager = dataManager;
        }
        public object GetCC() => createChecker;
        public object Create()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var checkDB = new SqlCommand(QuerySQL.CheckData, connection))
                    {
                        if ((int)checkDB.ExecuteScalar() == 0)
                        {
                            ExecuteQuery(QuerySQL.CreateData, connection);
                            ExecuteQuery(QuerySQL.CreateTable, connection);
                            //ExecuteQuery(QuerySQL.CreateUser, connection);
                        }
                        else
                        {
                            try
                            {
                                using (var checkTable = new SqlCommand(QuerySQL.CheckTableString, connection)) { checkTable.ExecuteScalar(); }
                                return true;
                            }
                            catch { ExecuteQuery(QuerySQL.CreateTable, connection); }
                        }
                        InsertTestString(new Dictionary<string, string> { { "Первая", "Первая заметка для теста" }, { "Вторая", "Вторая заметка для теста" }, { "Третья", "Третья заметка для теста" }, { "Четвертая", "Четвертая заметка для теста" }, { "Пятая", "Пятая заметка для теста" } });
                    }
                }
            }
            catch (Exception ex) { createChecker = ex.Message; return ex.Message; }
            createChecker = true;
            return true;
        }
        private void ExecuteQuery(string query, SqlConnection connection)
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        private void InsertTestString(Dictionary<string, string> dataParameters)
        {
            foreach (var parameter in dataParameters)
            {
                _dataManager.Insert(QuerySQL.InsertNote, new SqlParameter("@Title", parameter.Key), new SqlParameter("@Text", parameter.Value), new SqlParameter("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), new SqlParameter("@ChangeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), new SqlParameter("@IsChecked", false));
            }
        }
    }
}
