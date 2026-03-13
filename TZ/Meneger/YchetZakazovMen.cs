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
    public partial class YchetZakazovMen : Form
    {
        private DataTable ordersTable;  // храним все заказы
        private DataTable filteredTable; // отфильтрованные данные для пагинации
        private DateTime minDate;       // минимальная дата в заказах
        private DateTime maxDate;       // максимальная дата в заказах

        // Переменные для пагинации
        private int currentPage = 1;
        private int pageSize = 20;
        private int totalRecords = 0;
        private int totalPages = 1;
        private List<Button> pageButtons = new List<Button>();
        public YchetZakazovMen()
        {
            InitializeComponent();
            InitializePaginationControls();
        }
        #region Методы пагинации

        private void InitializePaginationControls()
        {
            // ComboBox для выбора размера страницы
            cmbPageSize.Items.Clear();
            cmbPageSize.Items.Add(10);
            cmbPageSize.Items.Add(20);
            cmbPageSize.Items.Add(50);
            cmbPageSize.SelectedIndex = 1; // 20 по умолчанию
            cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;

            // Кнопки пагинации
            btnFirstPage.Click += BtnFirstPage_Click;
            btnPrevPage.Click += BtnPrevPage_Click;
            btnNextPage.Click += BtnNextPage_Click;
            btnLastPage.Click += BtnLastPage_Click;

            // Создаем вертикальную легенду
            CreateColorLegend();
        }

        private void CreateColorLegend()
        {
            // Очищаем панель легенды
            panelLegend.Controls.Clear();
            panelLegend.AutoScroll = true;
            panelLegend.BorderStyle = BorderStyle.None;

            // Заголовок легенды
            Label lblLegendTitle = new Label();
            lblLegendTitle.Text = "Цветовая легенда:";
            lblLegendTitle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            lblLegendTitle.AutoSize = true;
            lblLegendTitle.Location = new Point(5, 5);
            panelLegend.Controls.Add(lblLegendTitle);

            int yPos = 30;

            // Легенда для статуса "Принят"
            AddLegendItem("Принят", Color.FromArgb(255, 255, 200), ref yPos);

            // Легенда для статуса "В работе"
            AddLegendItem("В работе", Color.FromArgb(200, 230, 255), ref yPos);

            // Легенда для статуса "Готов"
            AddLegendItem("Готов", Color.FromArgb(200, 255, 200), ref yPos);

            // Легенда для статуса "Выдан"
            AddLegendItem("Выдан", Color.FromArgb(240, 240, 240), ref yPos);

            // Легенда для статуса "Отменен"
            AddLegendItem("Отменен", Color.FromArgb(255, 200, 200), ref yPos);
        }

        private void AddLegendItem(string text, Color color, ref int yPos, bool isBold = false)
        {
            // Цветной прямоугольник
            Panel colorBox = new Panel();
            colorBox.Size = new Size(20, 20);
            colorBox.BackColor = color;
            colorBox.BorderStyle = BorderStyle.FixedSingle;
            colorBox.Location = new Point(5, yPos);
            panelLegend.Controls.Add(colorBox);

            // Текст описания
            Label lblText = new Label();
            lblText.Text = text;
            lblText.Location = new Point(30, yPos);
            lblText.Size = new Size(100, 20);
            lblText.TextAlign = ContentAlignment.MiddleLeft;
            panelLegend.Controls.Add(lblText);

            yPos += 25;
        }

        private void UpdatePaginationControls()
        {
            // Обновляем информацию о страницах
            lblPageInfo.Text = $"Страница {currentPage} из {totalPages}";
            lblRecordInfo.Text = $"Показано {GetCurrentPageRecords()} из {totalRecords} записей";

            // Включаем/отключаем кнопки
            btnFirstPage.Enabled = currentPage > 1;
            btnPrevPage.Enabled = currentPage > 1;
            btnNextPage.Enabled = currentPage < totalPages;
            btnLastPage.Enabled = currentPage < totalPages;

            // Обновляем кнопки с номерами страниц
            UpdatePageNumberButtons();
        }

        private int GetCurrentPageRecords()
        {
            if (filteredTable == null) return 0;

            int start = (currentPage - 1) * pageSize;
            int end = Math.Min(start + pageSize, filteredTable.Rows.Count);
            return Math.Max(0, end - start);
        }

        private void UpdatePageNumberButtons()
        {
            // Удаляем старые кнопки
            foreach (Button btn in pageButtons)
            {
                btn.Click -= PageButton_Click;
            }
            pageButtons.Clear();
            flowPageNumbers.Controls.Clear();

            if (filteredTable == null || filteredTable.Rows.Count == 0)
                return;

            // Определяем диапазон отображаемых номеров страниц
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, startPage + 4);

            if (endPage - startPage < 4)
            {
                startPage = Math.Max(1, endPage - 4);
            }

            // Создаем кнопки для каждой страницы
            for (int i = startPage; i <= endPage; i++)
            {
                Button btn = new Button();
                btn.Text = i.ToString();
                btn.Size = new Size(35, 30);
                btn.Tag = i;

                // Выделяем текущую страницу
                if (i == currentPage)
                {
                    btn.BackColor = Color.FromArgb(0, 120, 215);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
                else
                {
                    btn.BackColor = SystemColors.Control;
                    btn.ForeColor = SystemColors.ControlText;
                }

                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.Gray;
                btn.Click += PageButton_Click;

                pageButtons.Add(btn);
                flowPageNumbers.Controls.Add(btn);
            }
        }

        private void PageButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int page = (int)btn.Tag;
                if (page != currentPage)
                {
                    currentPage = page;
                    DisplayCurrentPage();
                    UpdatePaginationControls();
                }
            }
        }

        private void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageSize = (int)cmbPageSize.SelectedItem;
            currentPage = 1;
            UpdateFilteredData();
            DisplayCurrentPage();
            UpdatePaginationControls();
        }

        private void BtnFirstPage_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            DisplayCurrentPage();
            UpdatePaginationControls();
        }

        private void BtnPrevPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayCurrentPage();
                UpdatePaginationControls();
            }
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                DisplayCurrentPage();
                UpdatePaginationControls();
            }
        }

        private void BtnLastPage_Click(object sender, EventArgs e)
        {
            currentPage = totalPages;
            DisplayCurrentPage();
            UpdatePaginationControls();
        }

        private void DisplayCurrentPage()
        {
            if (filteredTable == null || filteredTable.Rows.Count == 0)
            {
                dgvOrders.DataSource = null;
                return;
            }

            // Создаем DataTable для текущей страницы
            DataTable pageTable = filteredTable.Clone();

            int start = (currentPage - 1) * pageSize;
            int end = Math.Min(start + pageSize, filteredTable.Rows.Count);

            for (int i = start; i < end; i++)
            {
                pageTable.ImportRow(filteredTable.Rows[i]);
            }

            dgvOrders.DataSource = pageTable;

            // Применяем условное форматирование
            ApplyConditionalFormatting();
        }

        private void UpdateFilteredData()
        {
            if (ordersTable == null) return;

            // Применяем текущие фильтры к данным
            DataView dv = ordersTable.DefaultView;

            List<string> filters = new List<string>();

            // 1. Период
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

            // 4. Поиск по номеру заказа
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

            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";

            filteredTable = dv.ToTable();
            totalRecords = filteredTable.Rows.Count;
            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (currentPage > totalPages) currentPage = totalPages > 0 ? totalPages : 1;
        }

        #endregion

        #region Условное форматирование (только для колонки "Статус")

        private void ApplyConditionalFormatting()
        {
            if (dgvOrders.Rows.Count == 0) return;

            // Сбрасываем форматирование для всех ячеек
            foreach (DataGridViewRow row in dgvOrders.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.White;
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.Font = dgvOrders.Font;
                }

                // Получаем статус заказа
                if (row.Cells["Статус"].Value != null)
                {
                    string status = row.Cells["Статус"].Value.ToString();

                    // Применяем цвет только к ячейке статуса
                    DataGridViewCell statusCell = row.Cells["Статус"];

                    switch (status)
                    {
                        case "Принят":
                            statusCell.Style.BackColor = Color.FromArgb(255, 255, 200); // Светло-желтый
                            statusCell.Style.ForeColor = Color.Black;
                            statusCell.Style.Font = dgvOrders.Font;
                            break;

                        case "В работе":
                            statusCell.Style.BackColor = Color.FromArgb(200, 230, 255); // Светло-голубой
                            statusCell.Style.ForeColor = Color.Black;
                            statusCell.Style.Font = dgvOrders.Font;
                            break;

                        case "Готов":
                            statusCell.Style.BackColor = Color.FromArgb(200, 255, 200); // Светло-зеленый
                            statusCell.Style.ForeColor = Color.Black;
                            statusCell.Style.Font = dgvOrders.Font;
                            break;

                        case "Выдан":
                            statusCell.Style.BackColor = Color.FromArgb(240, 240, 240); // Светло-серый
                            statusCell.Style.ForeColor = Color.Black;
                            statusCell.Style.Font = dgvOrders.Font;
                            break;

                        case "Отменен":
                            statusCell.Style.BackColor = Color.FromArgb(255, 200, 200); // Светло-красный
                            statusCell.Style.ForeColor = Color.Black;
                            statusCell.Style.Font = new Font(dgvOrders.Font, FontStyle.Strikeout);
                            break;
                    }
                }
            }

            // Принудительно обновляем DataGridView
            dgvOrders.Refresh();
        }

        private void YchetZakazovMen_Load(object sender, EventArgs e)
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

            // Инициализируем отфильтрованные данные
            UpdateFilteredData();
            currentPage = 1;
            DisplayCurrentPage();
            UpdatePaginationControls();
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
            c.phone_number AS 'Телефон',
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

            // ПРИМЕНЯЕМ МАСКИРОВАНИЕ
            foreach (DataRow row in ordersTable.Rows)
            {
                string fullName = row["Клиент"].ToString();
                row["Клиент"] = DataMasker.MaskFIO(fullName);

                string phone = row["Телефон"].ToString();
                row["Телефон"] = DataMasker.MaskPhone(phone);
            }
        }

        private void ApplyFilters()
        {
            UpdateFilteredData();
            currentPage = 1;
            DisplayCurrentPage();
            UpdatePaginationControls();
        }

        private void txtSearchOrderNumber_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int orderId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["Номер заказа"].Value);
            OrderDetailsForm detailsForm = new OrderDetailsForm(orderId);

            // Подписываемся на событие закрытия формы деталей
            detailsForm.FormClosed += (s, args) =>
            {
                // Обновляем данные после закрытия формы деталей
                RefreshOrderData();
            };

            detailsForm.ShowDialog();
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

            // Создаем форму просмотра
            ProcmotrsoderZakaza formView = new ProcmotrsoderZakaza(orderId);

            // Подписываемся на событие закрытия формы
            formView.FormClosed += (s, args) =>
            {
                // Обновляем данные после закрытия формы просмотра
                RefreshOrderData();
            };

            formView.ShowDialog();
        }
        /// <summary>
        /// Обновление данных заказа после изменения статуса
        /// </summary>
        public void RefreshOrderData()
        {
            // Перезагружаем все заказы
            string sql = @"
        SELECT 
            o.id_order AS 'Номер заказа',
            o.Date_of_admission AS 'Дата приёма',
            o.Due_date AS 'Срок выполнения',
            s.status_name AS 'Статус',
            c.FIO AS 'Клиент',
            c.phone_number AS 'Телефон',
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

            // ПРИМЕНЯЕМ МАСКИРОВАНИЕ
            foreach (DataRow row in ordersTable.Rows)
            {
                string fullName = row["Клиент"].ToString();
                row["Клиент"] = DataMasker.MaskFIO(fullName);

                string phone = row["Телефон"].ToString();
                row["Телефон"] = DataMasker.MaskPhone(phone);
            }

            // Обновляем фильтрованные данные и отображение
            UpdateFilteredData();
            currentPage = 1;
            DisplayCurrentPage();
            UpdatePaginationControls();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
#endregion