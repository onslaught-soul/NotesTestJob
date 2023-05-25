using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NodesTestJob.Models
{
    public class DataManager
    {
        private readonly string _connectionString;
        public DataManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("NotesData");
        }
        public string Select(string query)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            var json = JsonConvert.SerializeObject(dataTable);
                            return json;
                        }
                    }
                }
            }
            catch (Exception ex) { return "Ошибка: " + ex.Message; }
        }
        public object Insert(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { return ex.Message; }
            return true;
        }
        public object Update(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { return ex.Message; }
            return true;
        }
        public object Delete(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { return ex.Message; }
            return true;
        }
        public string TriggerCheck(string query)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    List<POCO.NoteCheck> notes = new List<POCO.NoteCheck>();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.GetString(1);
                                string text = reader.GetString(2);

                                if (Update(QuerySQL.UpdateNoteCheck, new SqlParameter("@noteId", id), new SqlParameter("@changeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) is string) continue;

                                POCO.NoteCheck note = new POCO.NoteCheck
                                {
                                    Id = id.ToString(),
                                    Title = title,
                                    Text = text
                                };

                                notes.Add(note);
                            }
                        }
                    }
                    string json = JsonConvert.SerializeObject(notes);
                    return json;
                }
            }
            catch (Exception ex) { return "Ошибка: " + ex.Message; }
        }
    }
}
