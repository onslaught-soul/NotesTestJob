using Microsoft.Data.SqlClient;

namespace NodesTestJob.Models
{
    static public class QuerySQL
    {
        #region Настройка Базы Данных
        static public readonly string CreateData = "CREATE DATABASE NotesData";
        static public readonly string CreateTable = "CREATE TABLE NotesData.dbo.Notes (Id INT PRIMARY KEY IDENTITY, Title NVARCHAR(MAX), Text NVARCHAR(MAX), CreatedDate NVARCHAR(19), ChangeDate NVARCHAR(19), IsChecked BIT)";
        static public readonly string CreateUser = "USE NotesData;CREATE USER [IIS APPPOOL\\NoteSite] FROM LOGIN [IIS APPPOOL\\NoteSite];EXEC sp_addrolemember 'db_owner', 'IIS APPPOOL\\NoteSite';";
        #endregion

        #region Проверки
        static public readonly string CheckData = "SELECT COUNT(*) FROM sys.databases WHERE name = 'NotesData'";
        static public readonly string CheckTable = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notes' AND TABLE_CATALOG = 'NotesData'";
        static public readonly string CheckTableString = "SELECT COUNT(*) FROM NotesData.dbo.Notes";
        #endregion

        #region Вывод
        static public readonly string GetFullNotes = "SELECT Id, Title, Text, ChangeDate, IsChecked FROM NotesData.dbo.Notes";
        static public readonly string GetCheckNotes = "SELECT Id, Title, Text, ChangeDate FROM NotesData.dbo.Notes WHERE IsChecked = 0";
        #endregion

        #region Добавление
        static public readonly string InsertNote = "INSERT INTO NotesData.dbo.Notes (Title, Text, CreatedDate, ChangeDate, IsChecked) VALUES (@Title, @Text, @CreatedDate, @ChangeDate, @IsChecked)";
        #endregion

        #region Обновление
        static public readonly string UpdateNoteTitle = "UPDATE NotesData.dbo.Notes SET Title = @noteText, ChangeDate = @changeDate WHERE Id = @noteId";
        static public readonly string UpdateNoteText = "UPDATE NotesData.dbo.Notes SET Text = @noteText, ChangeDate = @changeDate WHERE Id = @noteId";
        static public readonly string UpdateNoteCheck = "UPDATE NotesData.dbo.Notes SET IsChecked = '1', ChangeDate = @changeDate WHERE Id = @noteId";
        #endregion

        #region Удаление
        static public readonly string DeleteNote = "DELETE FROM NotesData.dbo.Notes WHERE Id = @noteId";
        #endregion
    }
}
