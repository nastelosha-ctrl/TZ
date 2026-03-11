using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TZ
{
    public partial class ProcmotrsoderZakaza : Form
    {
        private int orderId;
        private int currentStatusId;
        public ProcmotrsoderZakaza(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId; // СОХРАНЯЕМ ID!

            LoadOrderDetails();
            LoadStatuses();
        }

        /// <summary>
        /// Загрузка деталей заказа
        /// </summary>
        private void LoadOrderDetails()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // Получаем ВСЕ услуги для этого номера заказа
                    // Фильтруем по id_order, id_client, Date_of_admission и т.д.
                    string itemsSql = @"
                SELECT 
                    s.service_name AS 'Услуга',
                    s.Description AS 'Описание',
                    o.price AS 'Цена',
                    o.id_order,
                    o.Date_of_admission,
                    o.id_client
                FROM `Order` o
                INNER JOIN Service s ON o.id_service = s.id_service
                WHERE o.id_order = @id
                ORDER BY o.id_order";

                    MySqlDataAdapter da = new MySqlDataAdapter(itemsSql, conn);
                    da.SelectCommand.Parameters.AddWithValue("@id", orderId);

                    DataTable orderItems = new DataTable();
                    da.Fill(orderItems);

                    // Если записей нет, попробуем найти другие заказы этого клиента с той же датой
                    if (orderItems.Rows.Count <= 1)
                    {
                        // Получаем информацию о текущем заказе
                        string orderInfoSql = @"
                    SELECT id_client, Date_of_admission 
                    FROM `Order` 
                    WHERE id_order = @id";

                        MySqlCommand cmd = new MySqlCommand(orderInfoSql, conn);
                        cmd.Parameters.AddWithValue("@id", orderId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int clientId = reader.GetInt32("id_client");
                                DateTime dateAdmission = reader.GetDateTime("Date_of_admission");
                                reader.Close();

                                // Ищем ВСЕ заказы этого клиента за эту дату
                                string allOrdersSql = @"
                            SELECT 
                                o.id_order,
                                s.service_name AS 'Услуга',
                                s.Description AS 'Описание',
                                o.price AS 'Цена'
                            FROM `Order` o
                            INNER JOIN Service s ON o.id_service = s.id_service
                            WHERE o.id_client = @clientId 
                            AND DATE(o.Date_of_admission) = DATE(@date)
                            ORDER BY o.id_order";

                                MySqlCommand cmd2 = new MySqlCommand(allOrdersSql, conn);
                                cmd2.Parameters.AddWithValue("@clientId", clientId);
                                cmd2.Parameters.AddWithValue("@date", dateAdmission);

                                MySqlDataAdapter da2 = new MySqlDataAdapter(cmd2);
                                orderItems = new DataTable();
                                da2.Fill(orderItems);
                            }
                        }
                    }

                    dgvServices.DataSource = orderItems;

                    // Настройка DataGridView
                    if (dgvServices.Columns.Count > 0)
                    {
                        if (dgvServices.Columns.Contains("id_order"))
                            dgvServices.Columns["id_order"].Visible = false;

                        dgvServices.Columns["Услуга"].Width = 200;
                        dgvServices.Columns["Описание"].Width = 250;
                        dgvServices.Columns["Цена"].Width = 100;
                        dgvServices.Columns["Цена"].DefaultCellStyle.Format = "N2";
                        dgvServices.Columns["Цена"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvServices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        dgvServices.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    }

                    // Показываем количество услуг
                    this.Text = $"Просмотр заказа № {orderId} (услуг: {orderItems.Rows.Count})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка статусов в ComboBox
        /// </summary>
        private void LoadStatuses()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT id_status, status_name FROM Status ORDER BY id_status";
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbStatus.DataSource = dt;
                    cmbStatus.DisplayMember = "status_name";
                    cmbStatus.ValueMember = "id_status";

                    // Устанавливаем текущий статус
                    cmbStatus.SelectedValue = currentStatusId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Изменение статуса заказа
        /// </summary>
        private void btnChangeStatus_Click(object sender, EventArgs e)
        {// ПРОВЕРКА 1: Есть ли выбранный статус
            if (cmbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newStatusId;

            // ПРОВЕРКА 2: Корректный ID статуса
            try
            {
                newStatusId = Convert.ToInt32(cmbStatus.SelectedValue);
            }
            catch
            {
                MessageBox.Show("Ошибка получения статуса!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ПРОВЕРКА 3: Статус действительно изменился
            if (newStatusId == currentStatusId)
            {
                MessageBox.Show("Выберите другой статус!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ПРОВЕРКА 4: Есть ли ID заказа
            if (orderId <= 0)
            {
                MessageBox.Show("Ошибка: не указан номер заказа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Подтверждение изменения
            DialogResult result = MessageBox.Show(
                $"Изменить статус заказа №{orderId} на \"{cmbStatus.Text}\"?",
                "Подтверждение изменения",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                UpdateOrderStatus(newStatusId);
            }
        }

        /// <summary>
        /// Обновление статуса в БД
        /// </summary>
        private void UpdateOrderStatus(int newStatusId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // ПРОВЕРКА: Существует ли такой заказ
                    string checkSql = "SELECT COUNT(*) FROM `Order` WHERE id_order = @id";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@id", orderId);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists == 0)
                    {
                        MessageBox.Show($"Заказ №{orderId} не найден в базе данных!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ОБНОВЛЕНИЕ статуса
                    string sql = "UPDATE `Order` SET id_status = @status WHERE id_order = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@status", newStatusId);
                    cmd.Parameters.AddWithValue("@id", orderId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        currentStatusId = newStatusId;
                        MessageBox.Show($"Статус заказа №{orderId} успешно изменен на \"{cmbStatus.Text}\"!",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось изменить статус!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения статуса: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Закрыть форму и вернуться в меню
        /// </summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
