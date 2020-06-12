using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public class MovieDatabase
    {
        SQLiteConnection m_dbConnection;

        public MovieDatabase()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "create table movies (name text, nzbID int, imdbID text, downloaded int, progress int, directory text, fileName text)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();            
        }

        public void AddMovie(string name, int nzbID, string imdbID)
        {
            string sql = $"insert into movies (name, nzbID, imdbID, downloaded, progress, directory, fileName) values ('{name}', {nzbID}, '{imdbID}', 0, 0, 'undefined', 'undefined')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        ~MovieDatabase()
        {
            m_dbConnection.Close();
        }
    }
}
