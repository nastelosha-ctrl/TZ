using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace TZ
{
    public partial class Yslygi : Form
    {
        private DataTable servicesTable;
        private DataTable categoriesTable;

        public Yslygi()
        {
            InitializeComponent();
            LoadCategories();
            LoadServices();
            ConfigureDataGridView();
        }

        /// <summary>
        /// Загрузка категорий в ComboBox
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                string sql = "SELECT id_serviceCategory, category_name FROM Service_Category ORDER BY category_name";
                categoriesTable = DatabaseHelper.GetData(sql);

                // Добавляем "Все категории"
                DataRow allRow = categoriesTable.NewRow();
                allRow["id_serviceCategory"] = DBNull.Value;
                allRow["category_name"] = "Все категории";
                categoriesTable.Rows.InsertAt(allRow, 0);

                cmbCategory.DataSource = categoriesTable;
                cmbCategory.DisplayMember = "category_name";
                cmbCategory.ValueMember = "id_serviceCategory";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка услуг
        /// </summary>
        private void LoadServices()
        {
            try
            {
                string sql = @"
                    SELECT 
                        s.id_service,
                        s.service_name AS 'Наименование услуги',
                        s.Description AS 'Описание',
                        s.Price AS 'Цена',
                        sc.category_name AS 'Категория'
                    FROM Service s
                    LEFT JOIN Service_Category sc ON s.id_serviceCategory = sc.id_serviceCategory
                    ORDER BY s.id_service DESC";

                servicesTable = DatabaseHelper.GetData(sql);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Применение фильтров
        /// </summary>
        private void ApplyFilters()
        {
            if (servicesTable == null) return;

            DataView dv = servicesTable.DefaultView;
            List<string> filters = new List<string>();

            // Фильтр по категории
            if (cmbCategory.SelectedIndex > 0 && cmbCategory.SelectedValue != null && cmbCategory.SelectedValue != DBNull.Value)
            {
                string category = cmbCategory.Text.Replace("'", "''");
                filters.Add($"[Категория] = '{category}'");
            }

            // Живой поиск по наименованию (с начала строки)
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.Trim().Replace("'", "''");
                filters.Add($"[Наименование услуги] LIKE '{search}%'");
            }

            // Применяем фильтр
            try
            {
                dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
                dgvServices.DataSource = dv.ToTable();

                // ОБЯЗАТЕЛЬНО обновляем счетчик записей!
                UpdateRecordsCount();
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
            if (dgvServices.Columns.Count > 0)
            {
                // Скрываем ID
                if (dgvServices.Columns.Contains("id_service"))
                    dgvServices.Columns["id_service"].Visible = false;

                // Настройка ширины колонок
                if (dgvServices.Columns.Contains("Наименование услуги"))
                {
                    dgvServices.Columns["Наименование услуги"].Width = 250;
                    dgvServices.Columns["Наименование услуги"].HeaderText = "Наименование";
                }

                if (dgvServices.Columns.Contains("Описание"))
                {
                    dgvServices.Columns["Описание"].Width = 300;
                    dgvServices.Columns["Описание"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }

                if (dgvServices.Columns.Contains("Цена"))
                {
                    dgvServices.Columns["Цена"].Width = 100;
                    dgvServices.Columns["Цена"].DefaultCellStyle.Format = "N2";
                    dgvServices.Columns["Цена"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvServices.Columns["Цена"].HeaderText = "Цена (₽)";
                }

                if (dgvServices.Columns.Contains("Категория"))
                {
                    dgvServices.Columns["Категория"].Width = 150;
                }

                // Общие настройки
                dgvServices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvServices.MultiSelect = false;
                dgvServices.ReadOnly = true;
                dgvServices.AllowUserToAddRows = false;
                dgvServices.AllowUserToDeleteRows = false;
                dgvServices.RowHeadersVisible = false;

                // Чередование цветов строк
                dgvServices.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            }
        }

        /// <summary>
        /// Обновление счетчика записей - ИСПРАВЛЕНО!
        /// </summary>
        private void UpdateRecordsCount()
        {
            int count = dgvServices.Rows.Count;

            // Проверяем, есть ли строка для новой записи
            if (dgvServices.AllowUserToAddRows && count > 0)
                count--;

            lblRecordsCount.Text = $"Всего услуг: {count}";

            // Показываем информацию о фильтрации
            if (cmbCategory.SelectedIndex > 0 || !string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                int totalCount = servicesTable?.Rows.Count ?? 0;
                // Вычитаем строку для новой записи из общего количества
                if (servicesTable != null && servicesTable.Columns.Count > 0)
                    totalCount = servicesTable.Rows.Count;

                if (count < totalCount)
                {
                    lblRecordsCount.Text += $" (отфильтровано из {totalCount})";
                }
            }

            // Принудительно обновляем отображение
            lblRecordsCount.Refresh();
        }

        /// <summary>
        /// Живой поиск по наименованию
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Фильтр по категории
        /// </summary>
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Сброс фильтров
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            ApplyFilters();

            MessageBox.Show("Фильтры сброшены", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Кнопка "Меню"
        /// </summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ЭКСПОРТ В EXCEL - С ИСПОЛЬЗОВАНИЕМ БИБЛИОТЕКИ MICROSOFT.OFFICE.INTEROP.EXCEL
        /// </summary>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли данные для экспорта
            int rowCount = dgvServices.Rows.Count;
            if (dgvServices.AllowUserToAddRows)
                rowCount--;

            if (rowCount == 0)
            {
                MessageBox.Show("Нет данных для экспорта!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel файлы (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls";
            sfd.FileName = $"Услуги_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            sfd.Title = "Экспорт услуг в Excel";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Создаем приложение Excel
                    Excel.Application excelApp = new Excel.Application();
                    excelApp.DisplayAlerts = false;
                    excelApp.ScreenUpdating = false;

                    // Создаем книгу
                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.ActiveSheet;

                    // Название листа
                    worksheet.Name = "Услуги";

                    // ЗАГОЛОВКИ - БЕЗ ПРОБЛЕМНОГО ФОРМАТИРОВАНИЯ
                    string[] headers = new string[]
                    {
                "Наименование",
                "Категория",
                "Цена (₽)",
                "Описание"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1] = headers[i];

                        // ТОЛЬКО БАЗОВОЕ ФОРМАТИРОВАНИЕ
                        Excel.Range headerCell = worksheet.Cells[1, i + 1];
                        headerCell.Font.Bold = true;
                        headerCell.Interior.Color = System.Drawing.Color.FromArgb(0, 70, 131);
                        headerCell.Font.Color = System.Drawing.Color.White;
                    }

                    // ДАННЫЕ
                    int rowIndex = 2;
                    decimal totalSum = 0;

                    foreach (DataGridViewRow row in dgvServices.Rows)
                    {
                        // Пропускаем пустую строку для добавления
                        if (row.IsNewRow) continue;

                        // Наименование
                        worksheet.Cells[rowIndex, 1] = row.Cells["Наименование услуги"].Value?.ToString() ?? "";

                        // Категория
                        worksheet.Cells[rowIndex, 2] = row.Cells["Категория"]?.Value?.ToString() ?? "Без категории";

                        // Цена - ПРОСТО ЧИСЛО
                        if (row.Cells["Цена"].Value != null)
                        {
                            decimal price = Convert.ToDecimal(row.Cells["Цена"].Value);
                            worksheet.Cells[rowIndex, 3] = price;
                            totalSum += price;
                        }

                        // Описание
                        worksheet.Cells[rowIndex, 4] = row.Cells["Описание"]?.Value?.ToString() ?? "";

                        rowIndex++;
                    }

                    // ИТОГОВАЯ СТРОКА
                    int lastRow = rowIndex + 1;
                    worksheet.Cells[lastRow, 1] = "ИТОГО:";
                    worksheet.Cells[lastRow, 1].Font.Bold = true;

                    worksheet.Cells[lastRow, 2] = $"{rowCount} услуг";
                    worksheet.Cells[lastRow, 2].Font.Bold = true;

                    worksheet.Cells[lastRow, 3] = totalSum;
                    worksheet.Cells[lastRow, 3].Font.Bold = true;

                    // ИНФОРМАЦИЯ ОБ ОТЧЕТЕ
                    int infoRow = lastRow + 2;

                    worksheet.Cells[infoRow, 1] = "ИНФОРМАЦИЯ ОБ ОТЧЕТЕ:";
                    worksheet.Cells[infoRow, 1].Font.Bold = true;

                    worksheet.Cells[infoRow + 1, 1] = "Дата формирования:";
                    worksheet.Cells[infoRow + 1, 2] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                    worksheet.Cells[infoRow + 2, 1] = "Директор:";
                    worksheet.Cells[infoRow + 2, 2] = CurrentUser.FIO;

                    worksheet.Cells[infoRow + 3, 1] = "Категория:";
                    worksheet.Cells[infoRow + 3, 2] = cmbCategory.SelectedIndex > 0 ? cmbCategory.Text : "Все категории";

                    worksheet.Cells[infoRow + 4, 1] = "Поиск:";
                    worksheet.Cells[infoRow + 4, 2] = string.IsNullOrWhiteSpace(txtSearch.Text) ? "нет" : txtSearch.Text;

                    worksheet.Cells[infoRow + 5, 1] = "Всего записей:";
                    worksheet.Cells[infoRow + 5, 2] = rowCount;

                    // АВТОПОДБОР ШИРИНЫ
                    worksheet.Columns.AutoFit();

                    // СОХРАНЕНИЕ ФАЙЛА
                    workbook.SaveAs(sfd.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // ОСВОБОЖДЕНИЕ РЕСУРСОВ
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    MessageBox.Show($"✅ Отчет успешно сохранен!\n\nФайл: {sfd.FileName}\n\nЗаписей: {rowCount}\nСумма: {totalSum:N2} ₽",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ОТКРЫТЬ ФАЙЛ
                    if (MessageBox.Show("Открыть файл?", "Вопрос",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Ошибка экспорта в Excel:\n{ex.Message}\n\nУбедитесь, что Microsoft Excel установлен!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Двойной клик по строке - просмотр детальной информации
        /// </summary>
        private void dgvServices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvServices.Rows[e.RowIndex].IsNewRow) return;

            try
            {
                string serviceName = dgvServices.Rows[e.RowIndex].Cells["Наименование услуги"].Value.ToString();
                string description = dgvServices.Rows[e.RowIndex].Cells["Описание"]?.Value?.ToString() ?? "";
                string price = dgvServices.Rows[e.RowIndex].Cells["Цена"].Value.ToString();
                string category = dgvServices.Rows[e.RowIndex].Cells["Категория"]?.Value?.ToString() ?? "Без категории";

                string message = $"📌 Наименование: {serviceName}\n" +
                               $"📁 Категория: {category}\n" +
                               $"💰 Цена: {decimal.Parse(price):N2} ₽\n" +
                               $"📝 Описание: {description}";

                MessageBox.Show(message, "Детальная информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка формы
        /// </summary>
        private void YslygiDirector_Load(object sender, EventArgs e)
        {
            this.Text = "Услуги - Просмотр (Директор)";

            // Инициализация счетчика записей
            UpdateRecordsCount();
        }
    }
}