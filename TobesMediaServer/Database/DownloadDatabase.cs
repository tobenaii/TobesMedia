using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public class DownloadDatabase
    {
        SQLiteConnection m_dbConnection;

        public DownloadDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=DownloadDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "create table if not exists downloads (nzbID int, imdbID text)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void AddDownload(int nzbID, string imdbID)
        {
            string sql = $"insert into downloads (nzbID, imdbID) values ({nzbID}, '{imdbID}')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void RemoveDownload(int nzbID)
        {
            string sql = $"delete from downloads where nzbID='{nzbID}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public string GetImdbID(int nzbID)
        {
            string sql = $"select imdbID from downloads where nzbID='{nzbID}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            return command.ExecuteScalar().ToString();
        }
    }
}
