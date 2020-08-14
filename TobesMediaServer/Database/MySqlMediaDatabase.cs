using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Database
{
    public class MySqlMediaDatabase : IMediaDatabase
    {
        SQLiteConnection m_dbConnection;

        public MySqlMediaDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MediaDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
        }

        private void TableCheck(string table)
        {
            string sql = $"create table if not exists {table} (id text, value text)";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void AddMedia(string table, string id, string value = "")
        {
            TableCheck(table);
            string sql = $"insert into {table}(id, value) values ('{id}', '{value}')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void RemoveMedia(string table, string id)
        {
            TableCheck(table);
            string sql = $"delete from {table} where id='{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public async Task<bool> MediaExistsAsync(string table, string id)
        {
            TableCheck(table);
            string sql = $"select id from {table} where id='{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            DbDataReader exists = await command.ExecuteReaderAsync();
            return exists.HasRows;
        }

        public async Task<List<string>> GetAllIdsAsync(string table)
        {
            TableCheck(table);
            List<string> ids = new List<string>();
            try
            {
                string sql = $"select id from {table}";
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

        public async Task<string> GetValueAsync(string table, string id)
        {
            try
            {
                TableCheck(table);
                string sql = $"select value from {table} where id='{id}'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                DbDataReader value = await command.ExecuteReaderAsync();
                return value.GetString(0);
            }
            catch
            {
                return string.Empty;
            }
        }

        ~MySqlMediaDatabase()
        {
            m_dbConnection.Close();
        }
    }
}
