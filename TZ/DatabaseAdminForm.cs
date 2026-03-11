using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TZ
{
    public partial class DatabaseAdminForm : Form
    {
        public DatabaseAdminForm()
        {
            InitializeComponent();
            this.Text = "Администрирование базы данных";
            LoadTables();
        }

        private void LoadTables()
        {
            cmbTables.Items.Clear();
            cmbTables.Items.AddRange(new string[] {
                "Role", "Status", "Service_Category", "Client",
                "User", "Service", "Order", "Basket"
            });
            cmbTables.SelectedIndex = 0;
        }

        // Восстановление структуры БД
        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "Это действие удалит все существующие таблицы и создаст их заново!\n" +
                    "ВСЕ ДАННЫЕ БУДУТ ПОТЕРЯНЫ!\n\nПродолжить?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    RestoreDatabase();
                    MessageBox.Show("Структура базы данных успешно восстановлена!",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoreDatabase()
        {
            string[] queries = new string[]
            {
                "DROP TABLE IF EXISTS Basket;",
                "DROP TABLE IF EXISTS `Order`;",
                "DROP TABLE IF EXISTS Service;",
                "DROP TABLE IF EXISTS User;",
                "DROP TABLE IF EXISTS Client;",
                "DROP TABLE IF EXISTS Service_Category;",
                "DROP TABLE IF EXISTS Status;",
                "DROP TABLE IF EXISTS Role;",

                @"CREATE TABLE Role (
                    id_role INT AUTO_INCREMENT PRIMARY KEY,
                    role_name VARCHAR(50) NOT NULL
                );",

                @"CREATE TABLE Status (
                    id_status INT AUTO_INCREMENT PRIMARY KEY,
                    status_name VARCHAR(50) NOT NULL
                );",

                @"CREATE TABLE Service_Category (
                    id_serviceCategory INT AUTO_INCREMENT PRIMARY KEY,
                    category_name VARCHAR(100) NOT NULL
                );",

                @"CREATE TABLE Client (
                    id_client INT AUTO_INCREMENT PRIMARY KEY,
                    FIO VARCHAR(150) NOT NULL,
                    phone_number VARCHAR(20) NOT NULL
                );",

                @"CREATE TABLE User (
                    id_user INT AUTO_INCREMENT PRIMARY KEY,
                    FIO VARCHAR(150) NOT NULL,
                    Login VARCHAR(50) NOT NULL UNIQUE,
                    Password VARCHAR(255) NOT NULL,
                    id_role INT NOT NULL,
                    FOREIGN KEY (id_role) REFERENCES Role(id_role)
                );",

                @"CREATE TABLE Service (
                    id_service INT AUTO_INCREMENT PRIMARY KEY,
                    service_name VARCHAR(200) NOT NULL,
                    Description TEXT,
                    Price DECIMAL(10,2) NOT NULL,
                    id_serviceCategory INT NOT NULL,
                    service_image LONGBLOB,
                    FOREIGN KEY (id_serviceCategory) REFERENCES Service_Category(id_serviceCategory)
                );",

                @"CREATE TABLE `Order` (
                    id_order INT AUTO_INCREMENT PRIMARY KEY,
                    id_service INT NOT NULL,
                    Date_of_admission DATE NOT NULL,
                    Due_date DATE,
                    id_status INT NOT NULL,
                    id_client INT NOT NULL,
                    id_user INT NOT NULL,
                    price DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (id_service) REFERENCES Service(id_service),
                    FOREIGN KEY (id_status) REFERENCES Status(id_status),
                    FOREIGN KEY (id_client) REFERENCES Client(id_client),
                    FOREIGN KEY (id_user) REFERENCES User(id_user)
                );",

                @"CREATE TABLE Basket (
                    id_basket INT AUTO_INCREMENT PRIMARY KEY,
                    id_order INT NOT NULL,
                    FOREIGN KEY (id_order) REFERENCES `Order`(id_order)
                );"
            };

            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string query in queries)
                        {
                            if (!string.IsNullOrWhiteSpace(query))
                            {
                                MySqlCommand cmd = new MySqlCommand(query, conn, transaction);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Выбор CSV файла
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            ofd.Title = "Выберите CSV файл для импорта";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = ofd.FileName;
                btnImport.Enabled = true;
            }
        }

        // Импорт данных
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                MessageBox.Show("Выберите файл!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Файл не существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbTables.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string tableName = cmbTables.SelectedItem.ToString();
                string[] lines = File.ReadAllLines(txtFilePath.Text, Encoding.UTF8);

                if (lines.Length == 0)
                {
                    MessageBox.Show("Файл пуст!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем структуру таблицы
                DataTable schema = GetTableSchema(tableName);
                int expectedColumns = schema.Columns.Count;

                var records = new List<string[]>();
                int lineNumber = 0;
                int skippedHeader = 0;

                foreach (string line in lines)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] values = ParseCsvLine(line);

                    // Пропускаем заголовок
                    if (lineNumber == 1 && values.Length == expectedColumns && IsHeaderLine(values))
                    {
                        skippedHeader++;
                        continue;
                    }

                    if (values.Length != expectedColumns)
                    {
                        MessageBox.Show($"Ошибка в строке {lineNumber}:\n" +
                            $"Ожидалось {expectedColumns} полей, найдено {values.Length}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    records.Add(values);
                }

                if (records.Count == 0)
                {
                    MessageBox.Show(skippedHeader > 0 ?
                        "В файле только заголовок" : "Нет данных для импорта",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int imported = ImportData(tableName, records);
                MessageBox.Show($"Импортировано записей: {imported}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetTableSchema(string tableName)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                // Экранируем название таблицы, особенно для Order
                string sql;
                if (tableName == "Order")
                    sql = "SELECT * FROM `Order` WHERE 1=0";
                else
                    sql = $"SELECT * FROM {tableName} WHERE 1=0";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            values.Add(current.ToString().Trim());
            return values.ToArray();
        }

        private bool IsHeaderLine(string[] values)
        {
            return values.All(v => !string.IsNullOrEmpty(v) &&
                v.Any(char.IsLetter) && !v.All(char.IsDigit));
        }

        private int ImportData(string tableName, List<string[]> records)
        {
            int imported = 0;

            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                // Отключаем проверку внешних ключей
                MySqlCommand cmdFK = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0", conn);
                cmdFK.ExecuteNonQuery();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string placeholders = string.Join(",",
                            Enumerable.Range(0, records[0].Length).Select(i => "@p" + i));

                        // Экранируем имя таблицы
                        string safeTableName = tableName == "Order" ? "`Order`" : tableName;
                        string sql = $"INSERT INTO {safeTableName} VALUES ({placeholders})";

                        MySqlCommand cmd = new MySqlCommand(sql, conn, transaction);

                        foreach (string[] values in records)
                        {
                            cmd.Parameters.Clear();
                            for (int i = 0; i < values.Length; i++)
                            {
                                cmd.Parameters.AddWithValue($"@p{i}", values[i]);
                            }
                            cmd.ExecuteNonQuery();
                            imported++;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка");
                        throw;
                    }
                    finally
                    {
                        // Включаем проверку обратно
                        cmdFK = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 1", conn);
                        cmdFK.ExecuteNonQuery();
                    }
                }
            }

            return imported;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}