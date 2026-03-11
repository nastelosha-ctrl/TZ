using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TZ
{
    public partial class OrderDetailsForm : Form
    {
        private int orderId;
        private int clientId;
        private string clientFIO;
        private string clientPhone;

        /// <summary>
        /// Конструктор формы детального просмотра заказа
        /// </summary>
        /// <param name="orderId">Номер заказа</param>
        public OrderDetailsForm(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadOrderData();
        }

        /// <summary>
        /// Загрузка данных о заказе
        /// </summary>
        private void LoadOrderData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Загружаем информацию о заказе и клиенте
                    string sql = @"
                        SELECT 
                            o.id_order,
                            o.Date_of_admission,
                            o.Due_date,
                            o.id_client,
                            c.FIO,
                            c.phone_number
                        FROM `Order` o
                        INNER JOIN Client c ON o.id_client = c.id_client
                        WHERE o.id_order = @id
                        LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", orderId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Номер заказа
                            lblOrderNumberValue.Text = reader["id_order"].ToString();

                            // Даты
                            DateTime admission = Convert.ToDateTime(reader["Date_of_admission"]);
                            DateTime due = Convert.ToDateTime(reader["Due_date"]);
                            lblOrderDateValue.Text = admission.ToString("dd.MM.yyyy");
                            lblDueDateValue.Text = due.ToString("dd.MM.yyyy");

                            // Данные клиента
                            clientId = Convert.ToInt32(reader["id_client"]);
                            clientFIO = reader["FIO"].ToString();
                            clientPhone = reader["phone_number"].ToString();

                            // Разбираем ФИО на части
                            string[] nameParts = clientFIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            lblLastNameValue.Text = nameParts.Length > 0 ? nameParts[0] : "";
                            lblFirstNameValue.Text = nameParts.Length > 1 ? nameParts[1] : "";
                            lblMiddleNameValue.Text = nameParts.Length > 2 ? nameParts[2] : "";

                            // Маскируем телефон для отображения
                            lblPhoneValue.Text = MaskPhone(clientPhone);
                        }
                        else
                        {
                            MessageBox.Show($"Заказ №{orderId} не найден!", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                        }
                    }

                    // Загружаем услуги заказа
                    LoadOrderServices();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка услуг, входящих в заказ
        /// </summary>
        private void LoadOrderServices()
        {
            string sql = @"
                SELECT 
                    s.service_name AS 'Услуга',
                    o.price AS 'Цена'
                FROM `Order` o
                INNER JOIN Service s ON o.id_service = s.id_service
                WHERE o.id_order = @id
                ORDER BY s.service_name";

            var parameters = new[] { new MySqlParameter("@id", orderId) };
            DataTable services = DatabaseHelper.GetData(sql, parameters);

            dgvServices.DataSource = services;

            // Настройка таблицы
            dgvServices.Columns["Услуга"].Width = 250;
            dgvServices.Columns["Услуга"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvServices.Columns["Цена"].Width = 100;
            dgvServices.Columns["Цена"].DefaultCellStyle.Format = "N2";
            dgvServices.Columns["Цена"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvServices.Columns["Цена"].HeaderText = "Цена (руб)";

            dgvServices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvServices.ReadOnly = true;
            dgvServices.AllowUserToAddRows = false;
            dgvServices.AllowUserToDeleteRows = false;
            dgvServices.RowHeadersVisible = false;

            // Подсчет общей суммы
            CalculateTotal(services);
        }

        /// <summary>
        /// Подсчет общей суммы заказа
        /// </summary>
        private void CalculateTotal(DataTable services)
        {
            decimal total = 0;
            foreach (DataRow row in services.Rows)
            {
                total += Convert.ToDecimal(row["Цена"]);
            }

            lblTotalValue.Text = $"{total:N2} руб.";
        }

        /// <summary>
        /// Маскирование телефона (оставляем код и последние 3 цифры)
        /// </summary>
        private string MaskPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "";

            // Оставляем только цифры
            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 11)
            {
                string code = digits.Substring(1, 3);      // код оператора
                string lastDigits = digits.Substring(8, 3); // последние 3 цифры
                return $"+7 ({code}) ***-**-{lastDigits}";
            }

            return phone; // если формат не распознан, возвращаем как есть
        }

        /// <summary>
        /// Кнопка "Детали клиента" - открывает форму с полными данными
        /// </summary>
        private void btnClientDetails_Click(object sender, EventArgs e)
        {
            // Создаем форму с полными данными клиента
            Form clientDetailsForm = new Form();
            clientDetailsForm.Text = "Полные данные клиента";
            clientDetailsForm.Size = new System.Drawing.Size(400, 200);
            clientDetailsForm.StartPosition = FormStartPosition.CenterParent;
            clientDetailsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            clientDetailsForm.MaximizeBox = false;
            clientDetailsForm.MinimizeBox = false;

            // Добавляем метки с данными
            Label lblFIO = new Label()
            {
                Text = $"ФИО: {clientFIO}",
                Location = new System.Drawing.Point(20, 20),
                Width = 350,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10)
            };

            Label lblPhone = new Label()
            {
                Text = $"Телефон: {clientPhone}",
                Location = new System.Drawing.Point(20, 50),
                Width = 350,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10)
            };

            Button btnClose = new Button()
            {
                Text = "Закрыть",
                Location = new System.Drawing.Point(150, 100),
                Size = new System.Drawing.Size(100, 30)
            };

            // ИСПРАВЛЕНО: используем другое имя параметра (args вместо e)
            btnClose.Click += (sender2, args) => clientDetailsForm.Close();

            clientDetailsForm.Controls.Add(lblFIO);
            clientDetailsForm.Controls.Add(lblPhone);
            clientDetailsForm.Controls.Add(btnClose);

            clientDetailsForm.ShowDialog(this);
        }

        /// <summary>
        /// Кнопка закрытия формы
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}