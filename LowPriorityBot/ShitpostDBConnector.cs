using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPriorityBot
{
    class ShitpostDBConnector
    {
        public static string connectionString = "Data Source=LowPriorityBot.sqlite3;Version=3;";

        public static void addLink(string link)
        {
            if (!isExists(link))
            {
                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection(ShitpostDBConnector.connectionString);
                m_dbConnection.Open();
                string sql = "INSERT INTO LP_Thread (Link) VALUES ($link)";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.Parameters.AddWithValue("$link", link);
                command.ExecuteNonQuery();
                m_dbConnection.Close();
            }
        }

        public static bool isExists(string link)
        {
            bool result = false;
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection(ShitpostDBConnector.connectionString);
            m_dbConnection.Open();
            string sql = "SELECT EXISTS(SELECT * FROM LP_Thread WHERE Link = $link)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.Parameters.AddWithValue("$link", link);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if ((long)reader[0] == 1)
                {
                    result = true;
                }
            }
            m_dbConnection.Close();

            return result;
        }
    }
}
