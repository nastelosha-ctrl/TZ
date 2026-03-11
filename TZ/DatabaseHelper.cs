using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TZ
{
    public  class DatabaseHelper
    {
        public static string host { get; set; } = Properties.Settings.Default.host;
        public static string Database { get; set; } = Properties.Settings.Default.Database;
        public static string Username { get; set; } = Properties.Settings.Default.Username;
        public static string Password { get; set; } = Properties.Settings.Default.Password;

        public static string ConnectionString =>
            $"Server={host};Database={Database};Uid={Username};Pwd={Password}";

        public static bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Автоматически открываем настройки при ошибке
                ShowConnectionError(ex.Message);
                return false;
            }
        }

        private static void ShowConnectionError(string errorMessage)
        {
            DialogResult result = MessageBox.Show(
                $"Ошибка подключения к базе данных:\n{errorMessage}\n\nХотите настроить подключение?",
                "Ошибка подключения",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error
            );

            if (result == DialogResult.Yes)
            {
                // Автоматически открываем форму настроек
                OpenSettingsForm();
            }
        }

        private static void OpenSettingsForm()
        {
            var settingsForm = new SettingsForm();
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                // После сохранения настроек проверяем соединение снова
                TestConnection();
            }
        }
        public static DataTable GetData(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var adapter = new MySqlDataAdapter(query, connection))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных:\n{ex.Message}\n\nЗапрос:\n{query}",
                    "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
        public static int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                            command.Parameters.AddRange(parameters);

                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения команды:\n{ex.Message}\n\nЗапрос:\n{query}",
                    "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
        public static object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении скалярного запроса:\n{ex.Message}\n\nЗапрос:\n{query}",
                    "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static DataTable GetData(string query, params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}\n\nЗапрос:\n{query}",
                    "Ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
    }
}
