using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace TZ
{
    public partial class YchetZakazovDirector : Form
    {
        private DataTable ordersTable;
        private DateTime minDate;
        private DateTime maxDate;

        public YchetZakazovDirector()
        {
            InitializeComponent();
            LoadStatuses();
            LoadManagers();
            LoadOrders();
            GetMinMaxDates();
            SetDateRange();
            CalculateTotalRevenue();
        }

        /// <summary>
        /// Загрузка статусов в ComboBox
        /// </summary>
        private void LoadStatuses()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetData("SELECT id_status, status_name FROM Status ORDER BY id_status");

                // Добавляем "Все статусы"
                DataRow allRow = dt.NewRow();
                allRow["id_status"] = DBNull.Value;
                allRow["status_name"] = "Все статусы";
                dt.Rows.InsertAt(allRow, 0);

                cmbStatus.DataSource = dt;
                cmbStatus.DisplayMember = "status_name";
                cmbStatus.ValueMember = "id_status";
                cmbStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Загрузка менеджеров в ComboBox
        /// </summary>
        private void LoadManagers()
        {
            try
            {
                string sql = @"
                    SELECT u.id_user, u.FIO 
                    FROM User u 
                    INNER JOIN Role r ON u.id_role = r.id_role 
                    WHERE r.role_name = 'Менеджер' 
                    ORDER BY u.FIO";

                DataTable dt = DatabaseHelper.GetData(sql);

                // Добавляем "Все менеджеры"
                DataRow allRow = dt.NewRow();
                allRow["id_user"] = DBNull.Value;
                allRow["FIO"] = "Все менеджеры";
                dt.Rows.InsertAt(allRow, 0);

                cmbManager.DataSource = dt;
                cmbManager.DisplayMember = "FIO";
                cmbManager.ValueMember = "id_user";
                cmbManager.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки менеджеров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Получение минимальной и максимальной даты заказов
        /// </summary>
        private void GetMinMaxDates()
        {
            try
            {
                string sql = @"
                    SELECT 
                        MIN(Date_of_admission) as MinDate,
                        MAX(Date_of_admission) as MaxDate
                    FROM `Order`";

                DataTable dt = DatabaseHelper.GetData(sql);

                if (dt.Rows.Count > 0)
                {
                    minDate = dt.Rows[0]["MinDate"] != DBNull.Value
                        ? Convert.ToDateTime(dt.Rows[0]["MinDate"])
                        : DateTime.Now.AddMonths(-1);

                    maxDate = dt.Rows[0]["MaxDate"] != DBNull.Value
                        ? Convert.ToDateTime(dt.Rows[0]["MaxDate"])
                        : DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения диапазона дат: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                minDate = DateTime.Now.AddMonths(-1);
                maxDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Установка диапазона дат
        /// </summary>
        private void SetDateRange()
        {
            dtpFrom.Value = minDate;
            dtpTo.Value = maxDate;

            dtpFrom.MaxDate = DateTime.Now;
            dtpTo.MaxDate = DateTime.Now;
        }
        /// <summary>
        /// Загрузка заказов
        /// </summary>
        private void LoadOrders()
        {
            try
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
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            if (ordersTable == null) return;

            DataView dv = ordersTable.DefaultView;
            List<string> filters = new List<string>();

            // Фильтр по периоду
            string fromDate = dtpFrom.Value.ToString("MM/dd/yyyy");
            string toDate = dtpTo.Value.ToString("MM/dd/yyyy");
            filters.Add($"[Дата приёма] >= #{fromDate}# AND [Дата приёма] <= #{toDate}#");

            // Фильтр по статусу
            if (cmbStatus.SelectedIndex > 0 && cmbStatus.SelectedValue != null && cmbStatus.SelectedValue != DBNull.Value)
            {
                string status = cmbStatus.Text.Replace("'", "''");
                filters.Add($"[Статус] = '{status}'");
            }

            // Фильтр по менеджеру
            if (cmbManager.SelectedIndex > 0 && cmbManager.SelectedValue != null && cmbManager.SelectedValue != DBNull.Value)
            {
                string manager = cmbManager.Text.Replace("'", "''");
                filters.Add($"[Менеджер] = '{manager}'");
            }

            // Поиск по номеру заказа
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
            try
            {
                dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
                dgvOrders.DataSource = dv.ToTable();

                // Настройка DataGridView
                ConfigureDataGridView();

                // Пересчет выручки
                CalculateTotalRevenue();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Настройка DataGridView
        /// </summary>
        private void ConfigureDataGridView()
        {
            if (dgvOrders.Columns.Count > 0)
            {
                // Скрываем лишние колонки если нужно

                // Настройка ширины колонок
                if (dgvOrders.Columns.Contains("Номер заказа"))
                    dgvOrders.Columns["Номер заказа"].Width = 80;

                if (dgvOrders.Columns.Contains("Дата приёма"))
                {
                    dgvOrders.Columns["Дата приёма"].Width = 90;
                    dgvOrders.Columns["Дата приёма"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    dgvOrders.Columns["Дата приёма"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvOrders.Columns.Contains("Срок выполнения"))
                {
                    dgvOrders.Columns["Срок выполнения"].Width = 90;
                    dgvOrders.Columns["Срок выполнения"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    dgvOrders.Columns["Срок выполнения"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgvOrders.Columns.Contains("Статус"))
                    dgvOrders.Columns["Статус"].Width = 100;

                if (dgvOrders.Columns.Contains("Клиент"))
                    dgvOrders.Columns["Клиент"].Width = 150;

                if (dgvOrders.Columns.Contains("Менеджер"))
                    dgvOrders.Columns["Менеджер"].Width = 150;

                if (dgvOrders.Columns.Contains("Сумма"))
                {
                    dgvOrders.Columns["Сумма"].Width = 90;
                    dgvOrders.Columns["Сумма"].DefaultCellStyle.Format = "N2";
                    dgvOrders.Columns["Сумма"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dgvOrders.Columns.Contains("Услуга"))
                    dgvOrders.Columns["Услуга"].Width = 200;

                dgvOrders.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvOrders.MultiSelect = false;
                dgvOrders.ReadOnly = true;
            }
        }
        /// <summary>
        /// Подсчет общей выручки
        /// </summary>
        private void CalculateTotalRevenue()
        {
            try
            {
                decimal total = 0;

                foreach (DataGridViewRow row in dgvOrders.Rows)
                {
                    if (row.Cells["Сумма"].Value != null)
                    {
                        total += Convert.ToDecimal(row.Cells["Сумма"].Value);
                    }
                }

                lblTotalRevenue.Text = $"Выручка: {total:N2} ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подсчета выручки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTotalRevenue.Text = "Выручка: 0.00 ₽";
            }
        }
        /// <summary>
        /// Сброс фильтров
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearchOrderNumber.Clear();
            cmbStatus.SelectedIndex = 0;
            cmbManager.SelectedIndex = 0;
            dtpFrom.Value = minDate;
            dtpTo.Value = maxDate;

            ApplyFilters();
        }
        /// <summary>
        /// Формирование отчета в Excel
        /// </summary>
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvOrders.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для формирования отчета!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel файлы (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls";
                sfd.FileName = $"Отчет_по_заказам_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                sfd.Title = "Сохранить отчет";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Создаем приложение Excel
                    Excel.Application excelApp = new Excel.Application();
                    excelApp.DisplayAlerts = false;

                    // Создаем книгу
                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.ActiveSheet;

                    // Заголовки
                    for (int i = 0; i < dgvOrders.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dgvOrders.Columns[i].HeaderText;
                        // Жирный шрифт для заголовков
                        worksheet.Cells[1, i + 1].Font.Bold = true;
                        // Цвет фона
                        worksheet.Cells[1, i + 1].Interior.Color = Excel.XlRgbColor.rgbLightBlue;
                    }

                    // Данные
                    for (int i = 0; i < dgvOrders.Rows.Count; i++)
                    {
                        if (!dgvOrders.Rows[i].IsNewRow)
                        {
                            for (int j = 0; j < dgvOrders.Columns.Count; j++)
                            {
                                if (dgvOrders.Rows[i].Cells[j].Value != null)
                                {
                                    worksheet.Cells[i + 2, j + 1] = dgvOrders.Rows[i].Cells[j].Value.ToString();
                                }
                            }
                        }
                    }

                    // Форматирование дат
                    if (dgvOrders.Columns.Contains("Дата приёма"))
                    {
                        int colIndex = dgvOrders.Columns["Дата приёма"].Index + 1;
                        Excel.Range dateRange = worksheet.Range[worksheet.Cells[2, colIndex], worksheet.Cells[dgvOrders.Rows.Count + 1, colIndex]];
                        dateRange.NumberFormat = "dd.MM.yyyy";
                    }

                    if (dgvOrders.Columns.Contains("Срок выполнения"))
                    {
                        int colIndex = dgvOrders.Columns["Срок выполнения"].Index + 1;
                        Excel.Range dateRange = worksheet.Range[worksheet.Cells[2, colIndex], worksheet.Cells[dgvOrders.Rows.Count + 1, colIndex]];
                        dateRange.NumberFormat = "dd.MM.yyyy";
                    }

                    // Форматирование сумм
                    if (dgvOrders.Columns.Contains("Сумма"))
                    {
                        int colIndex = dgvOrders.Columns["Сумма"].Index + 1;
                        Excel.Range priceRange = worksheet.Range[worksheet.Cells[2, colIndex], worksheet.Cells[dgvOrders.Rows.Count + 1, colIndex]];
                        priceRange.NumberFormat = "#,##0.00 ₽";
                        priceRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    }

                    // Автоподбор ширины колонок
                    Excel.Range usedRange = worksheet.UsedRange;
                    usedRange.Columns.AutoFit();

                    // Добавляем итоговую строку
                    int lastRow = dgvOrders.Rows.Count + 3;
                    worksheet.Cells[lastRow, 1] = "ИТОГО:";
                    worksheet.Cells[lastRow, 1].Font.Bold = true;

                    // Сумма выручки
                    int sumColIndex = dgvOrders.Columns.Contains("Сумма") ? dgvOrders.Columns["Сумма"].Index + 1 : 1;
                    worksheet.Cells[lastRow, sumColIndex] = lblTotalRevenue.Text.Replace("Выручка:", "").Trim();
                    worksheet.Cells[lastRow, sumColIndex].Font.Bold = true;

                    // Информация о периоде
                    worksheet.Cells[lastRow + 1, 1] = $"Период: с {dtpFrom.Value:dd.MM.yyyy} по {dtpTo.Value:dd.MM.yyyy}";
                    worksheet.Cells[lastRow + 2, 1] = $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                    worksheet.Cells[lastRow + 3, 1] = $"Менеджер: {CurrentUser.FIO}";

                    // Сохраняем файл
                    workbook.SaveAs(sfd.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // Освобождаем ресурсы
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    MessageBox.Show($"Отчет успешно сохранен!\n{sfd.FileName}", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Спрашиваем, открыть ли файл
                    if (MessageBox.Show("Открыть файл?", "Вопрос",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Живой поиск по номеру заказа
        /// </summary>
        private void txtSearchOrderNumber_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Изменение даты начала
        /// </summary>
        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFrom.Value > dtpTo.Value)
                dtpTo.Value = dtpFrom.Value;
            ApplyFilters();
        }

        /// <summary>
        /// Изменение даты окончания
        /// </summary>
        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTo.Value < dtpFrom.Value)
                dtpFrom.Value = dtpTo.Value;
            ApplyFilters();
        }

        /// <summary>
        /// Изменение статуса
        /// </summary>
        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Изменение менеджера
        /// </summary>
        private void cmbManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Кнопка "Меню"
        /// </summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
