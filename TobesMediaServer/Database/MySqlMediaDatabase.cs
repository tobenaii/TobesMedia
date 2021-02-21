using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public class MySqlMediaDatabase : IMediaPipelineDatabase, ILocalMediaDatabase, IDownloadDatabase
    {
        SQLiteConnection m_dbConnection;
        private const string m_table = "Data";

        public MySqlMediaDatabase(string name)
        {
            m_dbConnection = new SQLiteConnection($"Data Source={name}.sqlite;Version=3;");
            m_dbConnection.Open();
            TableCheck();
        }

        private void TableCheck()
        {
            string sql = $"create table if not exists {m_table} (id text, value text)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void AddMedia(string id, string value = "")
        {
            string sql = $"insert into {m_table}(id, value) values ('{id}', '{value}')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void RemoveMedia(string id)
        {
            string sql = $"delete from {m_table} where id='{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public async Task<bool> MediaExistsAsync(string id)
        {
            string sql = $"select id from {m_table} where id='{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            DbDataReader exists = await command.ExecuteReaderAsync();
            return exists.HasRows;
        }

        public async Task<string> GetFilePathAsync(string id)
        {
            string sql = $"select value from {m_table} where id='{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            object dir = await command.ExecuteScalarAsync();
            return dir.ToString();
        }

        public async Task<List<string>> GetAllIdsAsync()
        {
            List<string> ids = new List<string>();
            try
            {
                string sql = $"select id from {m_table}";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                DbDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ids.Add(Convert.ToString(reader["id"]));
                }
                return ids;
            }
            catch
            {
                return ids;
            }
        }

        public async Task<string> GetValueAsync(string id)
        {
            try
            {
                string sql = $"select value from {m_table} where id='{id}'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                object obj = await command.ExecuteScalarAsync();
                if (obj != null)
                    return obj.ToString();
                else
                    return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        ~MySqlMediaDatabase()
        {
            m_dbConnection.Close();
        }
    }
}
