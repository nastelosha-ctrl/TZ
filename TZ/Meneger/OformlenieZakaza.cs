using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TZ
{
    public partial class OformlenieZakaza : Form
    {
        private DataTable servicesTable;
        private DataTable basketTable;
        private int currentOrderId = -1;

        public OformlenieZakaza()
        {
            InitializeComponent();
            basketTable = new DataTable();
            basketTable.Columns.Add("id_service", typeof(int));
            basketTable.Columns.Add("service_name", typeof(string));
            basketTable.Columns.Add("price", typeof(decimal));

            mtxtPhone.Mask = "+7(000)000-00-00";
            mtxtPhone.PromptChar = '_';

            // Подписываемся на события валидации ФИО
            txtClientFIO.KeyPress += TxtClientFIO_KeyPress;
            txtClientFIO.Leave += TxtClientFIO_Leave;
        }

        #region Валидация ФИО клиента (только русские буквы)

        private bool IsRussianLetter(char c)
        {
            return (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == 'Ё' || c == 'ё';
        }

        private void TxtClientFIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    SelectNextControl(sender as Control, true, true, true, true);
                }
                return;
            }

            if (!IsRussianLetter(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '-')
            {
                e.Handled = true;
                if (!string.IsNullOrEmpty(errorProviderFIO?.GetError(txtClientFIO)))
                {
                    errorProviderFIO.SetError(txtClientFIO, "Только русские буквы, пробел и дефис");
                }
            }
            else
            {
                errorProviderFIO?.SetError(txtClientFIO, "");
            }
        }

        private void TxtClientFIO_Leave(object sender, EventArgs e)
        {
            string fio = txtClientFIO.Text.Trim();

            if (!string.IsNullOrEmpty(fio))
            {
                foreach (char c in fio)
                {
                    if (!IsRussianLetter(c) && c != ' ' && c != '-')
                    {
                        errorProviderFIO.SetError(txtClientFIO, "ФИО должно содержать только русские буквы, пробелы и дефис");
                        return;
                    }
                }
                errorProviderFIO.SetError(txtClientFIO, "");
            }
        }

        private bool ValidateClientFIO(string fio)
        {
            if (string.IsNullOrWhiteSpace(fio))
            {
                MessageBox.Show("Введите ФИО клиента!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            foreach (char c in fio.Trim())
            {
                if (!IsRussianLetter(c) && c != ' ' && c != '-')
                {
                    MessageBox.Show("ФИО должно содержать только русские буквы, пробелы и дефис!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClientFIO.Focus();
                    txtClientFIO.SelectAll();
                    return false;
                }
            }

            return true;
        }

        #endregion

        private void OformlenieZakaza_Load(object sender, EventArgs e)
        {
            dtpOrderDate.Value = DateTime.Today;
            dtpOrderDate.Enabled = false;
            dtpDueDate.Value = DateTime.Today.AddDays(3);

            GenerateOrderNumber();
            LoadServices();
            UpdateBasketInfo();
            ValidateForm();
        }

        private void GenerateOrderNumber()
        {
            string sql = "SELECT MAX(id_order) FROM `Order`";
            object maxIdObj = DatabaseHelper.ExecuteScalar(sql);

            int maxId = (maxIdObj == null || maxIdObj == DBNull.Value) ? 0 : Convert.ToInt32(maxIdObj);
            currentOrderId = maxId + 1;
            txtOrderNumber.Text = currentOrderId.ToString();
            txtOrderNumber.ReadOnly = true;
        }

        private void LoadServices()
        {
            string sql = @"
                SELECT id_service, service_name, Price
                FROM Service";
            servicesTable = DatabaseHelper.GetData(sql);
            dgvServices.DataSource = servicesTable;

            dgvServices.Columns["id_service"].Visible = false;
            dgvServices.Columns["service_name"].HeaderText = "Услуга";
            dgvServices.Columns["Price"].HeaderText = "Цена";
            dgvServices.Columns["Price"].DefaultCellStyle.Format = "N2";
        }

        private void txtSearchService_TextChanged(object sender, EventArgs e)
        {
            if (servicesTable == null) return;

            string filter = txtSearchService.Text.Trim().Replace("'", "''");
            DataView dv = servicesTable.DefaultView;
            dv.RowFilter = $"service_name LIKE '%{filter}%'";
            dgvServices.DataSource = dv.ToTable();
        }

        private void btnAddToBasket_Click(object sender, EventArgs e)
        {
            if (dgvServices.CurrentRow == null)
            {
                MessageBox.Show("Выберите услугу!", "Внимание");
                return;
            }

            DataRow row = basketTable.NewRow();
            row["id_service"] = dgvServices.CurrentRow.Cells["id_service"].Value;
            row["service_name"] = dgvServices.CurrentRow.Cells["service_name"].Value;
            row["price"] = dgvServices.CurrentRow.Cells["Price"].Value;
            basketTable.Rows.Add(row);

            UpdateBasketInfo();
            ValidateForm();
        }

        private void UpdateBasketInfo()
        {
            int count = basketTable.Rows.Count;
            decimal total = 0m;

            foreach (DataRow r in basketTable.Rows)
                total += Convert.ToDecimal(r["price"]);

            lblBasketCount.Text = $"Корзина: {count}";
            lblTotalSum.Text = $"Сумма: {total:N2} ₽";
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            if (basketTable.Rows.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка");
                return;
            }

            if (!ValidateClientFIO(txtClientFIO.Text))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(mtxtPhone.Text) || mtxtPhone.Text.Contains('_'))
            {
                MessageBox.Show("Введите корректный номер телефона!", "Ошибка");
                mtxtPhone.Focus();
                return;
            }

            ProsmotrZakaza preview = new ProsmotrZakaza(
                currentOrderId,
                txtClientFIO.Text.Trim(),
                mtxtPhone.Text,
                dtpOrderDate.Value,
                dtpDueDate.Value,
                basketTable.Copy()
            );

            if (preview.ShowDialog() == DialogResult.OK)
            {
                SaveOrder(preview.DiscountApplied);

                // Формируем чек в Word
                WordHelper.CreateReceipt(
                    currentOrderId,
                    txtClientFIO.Text.Trim(),
                    mtxtPhone.Text,
                    dtpOrderDate.Value,
                    dtpDueDate.Value,
                    basketTable,
                    preview.DiscountApplied
                );

                MessageBox.Show("Заказ успешно оформлен и чек сформирован!", "Успех");

                basketTable.Clear();
                txtClientFIO.Clear();
                mtxtPhone.Clear();
                UpdateBasketInfo();
                GenerateOrderNumber();
                ValidateForm();
            }
        }

        private void SaveOrder(bool applyDiscount)
        {
            int clientId = GetOrCreateClient(txtClientFIO.Text.Trim(), mtxtPhone.Text.Trim());
            int managerId = CurrentUser.Id;
            int statusId = 1; // Статус "Принят"

            int firstOrderId = -1;

            foreach (DataRow row in basketTable.Rows)
            {
                decimal price = Convert.ToDecimal(row["price"]);
                if (applyDiscount)
                    price *= 0.9m;

                string insertOrderSql = @"
                    INSERT INTO `Order` 
                    (id_service, Date_of_admission, Due_date, id_status, id_client, id_user, price)
                    VALUES (@service, @admission, @due, @status, @client, @user, @price);
                    SELECT LAST_INSERT_ID();";

                var parameters = new[]
                {
                    new MySqlParameter("@service",   row["id_service"]),
                    new MySqlParameter("@admission", dtpOrderDate.Value),
                    new MySqlParameter("@due",       dtpDueDate.Value),
                    new MySqlParameter("@status",    statusId),
                    new MySqlParameter("@client",    clientId),
                    new MySqlParameter("@user",      managerId),
                    new MySqlParameter("@price",     price)
                };

                object orderIdObj = DatabaseHelper.ExecuteScalar(insertOrderSql, parameters);

                if (orderIdObj != null && orderIdObj != DBNull.Value)
                {
                    int orderId = Convert.ToInt32(orderIdObj);

                    if (firstOrderId == -1)
                        firstOrderId = orderId;

                    string insertBasketSql = "INSERT INTO Basket (id_order) VALUES (@orderId)";
                    var basketParams = new[] { new MySqlParameter("@orderId", orderId) };
                    DatabaseHelper.ExecuteNonQuery(insertBasketSql, basketParams);
                }
            }

            currentOrderId = firstOrderId;
        }

        private int GetOrCreateClient(string fio, string phone)
        {
            string cleanPhone = phone.Replace("_", "").Replace("(", "").Replace(")", "").Replace("-", "").Trim();

            string sqlCheck = "SELECT id_client FROM Client WHERE FIO = @fio AND phone_number = @phone";
            var p = new[]
            {
                new MySqlParameter("@fio", fio),
                new MySqlParameter("@phone", cleanPhone)
            };

            object id = DatabaseHelper.ExecuteScalar(sqlCheck, p);

            if (id != null && id != DBNull.Value)
                return Convert.ToInt32(id);

            string insert = "INSERT INTO Client (FIO, phone_number) VALUES (@fio, @phone); SELECT LAST_INSERT_ID();";
            object newId = DatabaseHelper.ExecuteScalar(insert, p);

            return Convert.ToInt32(newId);
        }

        private void ValidateForm()
        {
            bool isValid = basketTable.Rows.Count > 0 &&
                          !string.IsNullOrWhiteSpace(txtClientFIO.Text) &&
                          !string.IsNullOrWhiteSpace(mtxtPhone.Text) &&
                          !mtxtPhone.Text.Contains('_');

            btnViewOrder.Enabled = isValid;
        }

        private void txtClientFIO_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void mtxtPhone_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}