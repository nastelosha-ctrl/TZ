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
    public partial class YchetZakazovAdmin : Form
    {
        private DataTable ordersTable;  // храним все заказы
        private DateTime minDate;       // минимальная дата в заказах
        private DateTime maxDate;       // максимальная дата в заказах
        public YchetZakazovAdmin()
        {
            InitializeComponent();
        }

        private void YchetZakazovAdmin_Load(object sender, EventArgs e)
        {
            LoadStatuses();
            LoadManagers();
            LoadOrders();           // загружаем все заказы

            // Получаем минимальную и максимальную дату из заказов
            GetMinMaxDates();

            // Устанавливаем даты на весь диапазон
            dtpFrom.Value = minDate;
            dtpTo.Value = maxDate;
            // ПОДПИСКА НА СОБЫТИЯ
            dtpFrom.ValueChanged += dtpFrom_ValueChanged;
            dtpTo.ValueChanged += dtpTo_ValueChanged;

            ApplyFilters();         // сразу применяем фильтр
        }
        /// <summary>
        /// Получает минимальную и максимальную дату из таблицы заказов
        /// </summary>
        private void GetMinMaxDates()
        {
            string sql = @"
                    SELECT 
                        MIN(Date_of_admission) as MinDate,
                        MAX(Date_of_admission) as MaxDate
                    FROM `Order`";

            DataTable dt = DatabaseHelper.GetData(sql);

            if (dt.Rows.Count > 0)
            {
                // Минимальная дата
                if (dt.Rows[0]["MinDate"] != DBNull.Value)
                    minDate = Convert.ToDateTime(dt.Rows[0]["MinDate"]);
                else
                    minDate = DateTime.Today; // если нет заказов

                // Максимальная дата
                if (dt.Rows[0]["MaxDate"] != DBNull.Value)
                    maxDate = Convert.ToDateTime(dt.Rows[0]["MaxDate"]);
                else
                    maxDate = DateTime.Today; // если нет заказов
            }
        }
        private void LoadStatuses()
        {
            DataTable dt = DatabaseHelper.GetData("SELECT id_status, status_name FROM Status ORDER BY id_status");
            cmbStatus.DataSource = dt;
            cmbStatus.DisplayMember = "status_name";
            cmbStatus.ValueMember = "id_status";
            cmbStatus.SelectedIndex = -1;   // пустой по умолчанию
        }
        private void LoadManagers()
        {
            string sql = @"
                    SELECT u.id_user, u.FIO 
                    FROM User u 
                    INNER JOIN Role r ON u.id_role = r.id_role 
                    WHERE r.role_name = 'Менеджер' 
                    ORDER BY u.FIO";

            DataTable dt = DatabaseHelper.GetData(sql);
            cmbManager.DataSource = dt;
            cmbManager.DisplayMember = "FIO";
            cmbManager.ValueMember = "id_user";
            cmbManager.SelectedIndex = -1;
        }
        private void LoadOrders()
        {
            string sql = @"
                    SELECT 
                        o.id_order AS 'Номер заказа',
                        o.Date_of_admission AS 'Дата приёма',
                        o.Due_date AS 'Срок выполнения',
                        s.status_name AS 'Статус',
                        c.FIO AS 'Клиент',
                        u.FIO AS 'Менеджер',
                        o.price AS 'Сумма',
                        serv.service_name AS 'Услуга'
                    FROM `Order` o
                    INNER JOIN Status s ON o.id_status = s.id_status
                    INNER JOIN Client c ON o.id_client = c.id_client
                    INNER JOIN User u ON o.id_user = u.id_user
                    INNER JOIN Service serv ON o.id_service = serv.id_service
                    ORDER BY o.id_order DESC";

            ordersTable = DatabaseHelper.GetData(sql);
            dgvOrders.DataSource = ordersTable;

            // скрываем лишние столбцы, если нужно
            if (dgvOrders.Columns.Contains("Номер заказа"))
                dgvOrders.Columns["Номер заказа"].Visible = true; // если не нужно показывать

            // форматирование дат
            if (dgvOrders.Columns.Contains("Дата приёма"))
                dgvOrders.Columns["Дата приёма"].DefaultCellStyle.Format = "dd.MM.yyyy";

            if (dgvOrders.Columns.Contains("Срок выполнения"))
                dgvOrders.Columns["Срок выполнения"].DefaultCellStyle.Format = "dd.MM.yyyy";
        }
        private void ApplyFilters()
        {
            if (ordersTable == null) return;

            DataView dv = ordersTable.DefaultView;

            List<string> filters = new List<string>();

            // 1. Период (американский формат + квадратные скобки)
            string fromDate = dtpFrom.Value.ToString("MM/dd/yyyy");
            string toDate = dtpTo.Value.ToString("MM/dd/yyyy");
            filters.Add($"[Дата приёма] >= #{fromDate}# AND [Дата приёма] <= #{toDate}#");

            // 2. Статус
            if (cmbStatus.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(cmbStatus.Text))
            {
                string status = cmbStatus.Text.Replace("'", "''");
                filters.Add($"[Статус] = '{status}'");
            }

            // 3. Менеджер
            if (cmbManager.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(cmbManager.Text))
            {
                string manager = cmbManager.Text.Replace("'", "''");
                filters.Add($"[Менеджер] = '{manager}'");
            }

            // 4. Живой поиск по номеру заказа (самый надёжный вариант)
            if (!string.IsNullOrWhiteSpace(txtSearchOrderNumber.Text))
            {
                string search = txtSearchOrderNumber.Text.Trim().Replace("'", "''");
                if (int.TryParse(search, out int num))
                {
                    filters.Add($"[Номер заказа] = {num}");
                }
                else
                {
                    filters.Add($"CONVERT([Номер заказа], 'System.String') LIKE '{search}%'");
                }
            }

            // Применяем фильтр
            dv.RowFilter = filters.Count > 0
                ? string.Join(" AND ", filters)
                : "";

            // ← ВАЖНО: НЕ делаем ToTable()!
            dgvOrders.DataSource = ordersTable;        // всегда привязываем оригинал
            dgvOrders.Refresh();
        }
        private void txtSearchOrderNumber_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFrom.Value > dtpTo.Value)
                dtpTo.Value = dtpFrom.Value;
            ApplyFilters();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTo.Value < dtpFrom.Value)
                dtpFrom.Value = dtpTo.Value;
            ApplyFilters();
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cmbManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchOrderNumber.Clear();
            cmbStatus.SelectedIndex = -1;
            cmbManager.SelectedIndex = -1;

            // Сбрасываем на реальные границы заказов
            dtpFrom.Value = minDate;
            dtpTo.Value = maxDate;

            ApplyFilters();
            dgvOrders.ClearSelection();
        }
        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null)
            {
                MessageBox.Show("Выберите заказ!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int orderId = Convert.ToInt32(dgvOrders.CurrentRow.Cells["Номер заказа"].Value);

            // открываем форму просмотра (передаём id заказа)
            ProcmotrsoderZakaza formView = new ProcmotrsoderZakaza(orderId);  // ← подставь реальное имя формы
            formView.ShowDialog();
        }
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();  // или this.Hide() — в зависимости от логики возврата
        }
    }
}
