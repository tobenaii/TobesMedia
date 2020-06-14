using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public class MovieDatabase
    {
        SQLiteConnection m_dbConnection;

        public MovieDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MovieDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "create table if not exists movies (name text, imdbID text, fileDir text)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void AddMovie(string name, string imdbID, string fileDir)
        {
            string sql = $"insert into movies (name, imdbID, fileDir) values ('{name}', '{imdbID}', '{fileDir}')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public async Task<bool> MovieExistsAsync(string imdbID)
        {
            string sql = $"select imdbID from movies where imdbID='{imdbID}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            DbDataReader exists = await command.ExecuteReaderAsync();
            return exists.HasRows;
        }

        public async Task<string> GetMovieDirectoryAsync(string imdbID)
        {
            string sql = $"select fileDir from movies where imdbID='{imdbID}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            object dir = await command.ExecuteScalarAsync();
            return dir.ToString();
        }

        ~MovieDatabase()
        {
            m_dbConnection.Close();
        }
    }
}
