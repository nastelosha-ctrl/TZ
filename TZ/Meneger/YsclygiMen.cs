using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace TZ
{
    public partial class YsclygiMen : Form
    {
        private int selectedServiceId = -1;
        private bool isEditing = false;
        private DataTable originalData;
        private byte[] currentImageBytes = null;
        private string currentImageHash = null; // Для хранения хеша изображения

        public YsclygiMen()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 80;

            SetFieldsEnabled(false);
            LoadServices();
            LoadCategories();
        }

        private void LoadServices()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    // Сначала проверим, есть ли колонка
                    string checkColumnSql = "SHOW COLUMNS FROM Service LIKE 'service_image'";
                    MySqlCommand checkCmd = new MySqlCommand(checkColumnSql, conn);
                    conn.Open();
                    bool hasImageColumn = checkCmd.ExecuteScalar() != null;

                    string sql;
                    if (hasImageColumn)
                    {
                        sql = @"SELECT 
                                s.id_service,
                                s.service_name,
                                s.Description,
                                s.Price,
                                sc.category_name as CategoryName,
                                s.service_image
                            FROM service s
                            LEFT JOIN Service_Category sc ON s.id_serviceCategory = sc.id_serviceCategory
                            ORDER BY s.id_service";
                    }
                    else
                    {
                        MessageBox.Show("Колонка service_image отсутствует в таблице Service.\nДобавьте её через SQL запрос.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        sql = @"SELECT 
                    s.id_service,
                    s.service_name,
                    s.Description,
                    s.Price,
                    sc.category_name as CategoryName
                FROM Service s
                LEFT JOIN Service_Category sc ON s.id_serviceCategory = sc.id_serviceCategory
                ORDER BY s.id_service";
                    }

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    originalData = new DataTable();
                    da.Fill(originalData);

                    // Добавляем столбец для отображения изображения
                    if (!originalData.Columns.Contains("Изображение"))
                    {
                        originalData.Columns.Add("Изображение", typeof(Image));
                    }

                    // Если есть колонка с изображением, конвертируем
                    if (hasImageColumn && originalData.Columns.Contains("service_image"))
                    {
                        foreach (DataRow row in originalData.Rows)
                        {
                            if (row["service_image"] != DBNull.Value)
                            {
                                try
                                {
                                    byte[] bytes = (byte[])row["service_image"];
                                    if (bytes != null && bytes.Length > 0)
                                    {
                                        using (MemoryStream ms = new MemoryStream(bytes))
                                        {
                                            row["Изображение"] = Image.FromStream(ms);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Ошибка конвертации: {ex.Message}");
                                }
                            }
                        }
                    }

                    // Переименовываем колонки
                    originalData.Columns["service_name"].ColumnName = "Название";
                    originalData.Columns["Description"].ColumnName = "Описание";
                    originalData.Columns["Price"].ColumnName = "Цена";
                    if (originalData.Columns.Contains("CategoryName"))
                        originalData.Columns["CategoryName"].ColumnName = "Категория";

                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // МЕТОД УДАЛЕН, ТАК КАК НЕ ИСПОЛЬЗУЕТСЯ

        private void ApplyFilters()
        {
            if (originalData == null || originalData.Rows.Count == 0) return;

            DataView dv = originalData.DefaultView;
            List<string> filters = new List<string>();

            // Фильтр по категории
            if (comboCategory.SelectedIndex > 0 && comboCategory.SelectedItem != null)
            {
                string cat = comboCategory.SelectedItem.ToString().Replace("'", "''");
                filters.Add($"Категория = '{cat}'");
            }

            // Поиск по названию (с начала строки)
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.Trim().Replace("'", "''");
                filters.Add($"Название LIKE '{search}%'");
            }

            // Применяем фильтры
            if (filters.Count > 0)
            {
                dv.RowFilter = string.Join(" AND ", filters);
            }
            else
            {
                dv.RowFilter = "";
            }

            // Сортировка
            string sort = "";
            switch (comboSortOrder.SelectedIndex)
            {
                case 1: sort = "Название ASC"; break;
                case 2: sort = "Название DESC"; break;
            }

            if (!string.IsNullOrEmpty(sort))
                dv.Sort = sort;

            dataGridView1.DataSource = dv.ToTable();
            ConfigureDataGridView();
        }

        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT category_name FROM Service_Category ORDER BY category_name";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    comboCategory.Items.Clear();
                    comboCategory.Items.Add("Все категории");

                    while (reader.Read())
                    {
                        comboCategory.Items.Add(reader["category_name"].ToString());
                    }

                    comboCategory.SelectedIndex = 0;

                    comboSortOrder.Items.Clear();
                    comboSortOrder.Items.Add("Без сортировки");
                    comboSortOrder.Items.Add("По возрастанию");
                    comboSortOrder.Items.Add("По убыванию");
                    comboSortOrder.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) => ApplyFilters();
        private void comboCategory_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();
        private void comboSortOrder_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();

        private void ConfigureDataGridView()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Скрываем служебные столбцы
                if (dataGridView1.Columns.Contains("id_service"))
                    dataGridView1.Columns["id_service"].Visible = false;

                if (dataGridView1.Columns.Contains("service_image"))
                    dataGridView1.Columns["service_image"].Visible = false;

                // НАСТРОЙКА КОЛОНКИ С ИЗОБРАЖЕНИЕМ
                if (dataGridView1.Columns.Contains("Изображение"))
                {
                    DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["Изображение"];
                    imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    imageColumn.Width = 80;
                    imageColumn.HeaderText = "Фото";
                    imageColumn.DisplayIndex = 1;
                }

                // Настройка остальных колонок
                if (dataGridView1.Columns.Contains("Название"))
                {
                    dataGridView1.Columns["Название"].Width = 180;
                    dataGridView1.Columns["Название"].DisplayIndex = 2;
                }

                if (dataGridView1.Columns.Contains("Описание"))
                {
                    dataGridView1.Columns["Описание"].Width = 200;
                    dataGridView1.Columns["Описание"].DisplayIndex = 3;
                }

                if (dataGridView1.Columns.Contains("Цена"))
                {
                    dataGridView1.Columns["Цена"].Width = 80;
                    dataGridView1.Columns["Цена"].DefaultCellStyle.Format = "N2";
                    dataGridView1.Columns["Цена"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView1.Columns["Цена"].DisplayIndex = 4;
                }

                if (dataGridView1.Columns.Contains("Категория"))
                {
                    dataGridView1.Columns["Категория"].Width = 150;
                    dataGridView1.Columns["Категория"].DisplayIndex = 5;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            if (isEditing && selectedServiceId != -1)
            {
                DialogResult result = MessageBox.Show(
                    "Вы редактируете услугу. Сохранить изменения перед переходом?",
                    "Подтверждение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!SaveChanges())
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            selectedServiceId = Convert.ToInt32(row.Cells["id_service"].Value);
            LoadServiceData(selectedServiceId);
        }

        private void LoadServiceData(int serviceId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"SELECT 
                        s.service_name,
                        s.Description,
                        s.Price,
                        sc.category_name,
                        s.service_image
                    FROM Service s
                    LEFT JOIN Service_Category sc ON s.id_serviceCategory = sc.id_serviceCategory
                    WHERE s.id_service = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", serviceId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["service_name"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            txtPrice.Text = reader["Price"].ToString();

                            string category = reader["category_name"].ToString();
                            if (!string.IsNullOrEmpty(category))
                            {
                                comboCategory.SelectedItem = category;
                            }
                            else
                            {
                                comboCategory.SelectedIndex = -1;
                            }

                            if (reader["service_image"] != DBNull.Value)
                            {
                                currentImageBytes = (byte[])reader["service_image"];
                                currentImageHash = ComputeImageHash(currentImageBytes);
                                using (MemoryStream ms = new MemoryStream(currentImageBytes))
                                {
                                    pictureBox1.Image = Image.FromStream(ms);
                                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                                btnDeleteImage.Enabled = false; // Кнопка удаления отключена
                            }
                            else
                            {
                                pictureBox1.Image = null;
                                currentImageBytes = null;
                                currentImageHash = null;
                                btnDeleteImage.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных услуги: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вычисление хеша изображения для проверки уникальности
        private string ComputeImageHash(byte[] imageBytes)
        {
            if (imageBytes == null) return null;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(imageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Проверка уникальности изображения
        private bool IsImageUnique(byte[] imageBytes, int excludeServiceId = -1)
        {
            if (imageBytes == null) return true;

            string newImageHash = ComputeImageHash(imageBytes);

            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = "SELECT id_service, service_image FROM Service WHERE service_image IS NOT NULL";
                if (excludeServiceId != -1)
                {
                    sql += " AND id_service != @excludeId";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                if (excludeServiceId != -1)
                {
                    cmd.Parameters.AddWithValue("@excludeId", excludeServiceId);
                }

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[] existingImage = (byte[])reader["service_image"];
                        if (existingImage != null)
                        {
                            string existingHash = ComputeImageHash(existingImage);
                            if (existingHash == newImageHash)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == -1)
            {
                MessageBox.Show("Выберите услугу для редактирования!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isEditing)
            {
                isEditing = true;
                SetFieldsEnabled(true);
                btnEdit.Text = "Сохранить";
                btnAdd.Enabled = false;
                txtName.Focus();
            }
            else
            {
                if (SaveChanges())
                {
                    isEditing = false;
                    SetFieldsEnabled(false);
                    btnEdit.Text = "Изменить";
                    btnAdd.Enabled = true;
                    LoadServices();
                }
            }
        }

        private bool SaveChanges()
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Проверьте правильность ввода данных!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // Проверка уникальности изображения
                if (currentImageBytes != null)
                {
                    if (!IsImageUnique(currentImageBytes, selectedServiceId))
                    {
                        MessageBox.Show("Это изображение уже используется для другой услуги!",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                int categoryId = GetCategoryId(comboCategory.SelectedItem?.ToString());
                if (categoryId == -1)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    string sql = @"UPDATE Service SET
                        service_name = @name,
                        Description = @desc,
                        Price = @price,
                        id_serviceCategory = @catId";

                    // Добавляем обновление изображения только если оно было изменено
                    if (currentImageBytes != null)
                    {
                        sql += ", service_image = @image";
                    }

                    sql += " WHERE id_service = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());

                    // Правильный парсинг цены
                    string priceText = txtPrice.Text.Trim().Replace(',', '.');
                    if (!decimal.TryParse(priceText, System.Globalization.NumberStyles.Any,
                                          System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                    {
                        MessageBox.Show("Неверный формат цены!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.Parameters.AddWithValue("@catId", categoryId);

                    if (currentImageBytes != null)
                    {
                        cmd.Parameters.AddWithValue("@image", currentImageBytes);
                    }

                    cmd.Parameters.AddWithValue("@id", selectedServiceId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Услуга успешно обновлена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить услугу!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // КНОПКА "УДАЛИТЬ" - УБРАНА
        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Удаление услуг отключено!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                DialogResult result = MessageBox.Show(
                    "Вы редактируете услугу. Сохранить изменения перед добавлением новой?",
                    "Подтверждение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!SaveChanges())
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                isEditing = false;
                SetFieldsEnabled(false);
                btnEdit.Text = "Изменить";
            }

            DobavlenieYslygi form = new DobavlenieYslygi();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadServices();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            comboCategory.SelectedIndex = 0;
            comboSortOrder.SelectedIndex = 0;
            dataGridView1.ClearSelection();
            ClearFields();
            selectedServiceId = -1;
            ApplyFilters();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                txtPrice.Focus();
                return false;
            }

            string priceText = txtPrice.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(priceText, System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                return false;
            }

            if (comboCategory.SelectedIndex == -1)
            {
                return false;
            }

            return true;
        }

        private int GetCategoryId(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) return -1;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT id_serviceCategory FROM Service_Category WHERE category_name = @name";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", categoryName);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        private void SetFieldsEnabled(bool enabled)
        {
            txtName.Enabled = enabled;
            txtDescription.Enabled = enabled;
            txtPrice.Enabled = enabled;
            btnImage.Enabled = enabled;
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            comboCategory.SelectedIndex = -1;
            pictureBox1.Image = null;
            currentImageBytes = null;
            currentImageHash = null;
            btnDeleteImage.Enabled = false;
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if (e.KeyChar == ',' && ((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                DialogResult result = MessageBox.Show(
                    "Вы редактируете услугу. Отменить редактирование и очистить поля?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                isEditing = false;
                SetFieldsEnabled(false);
                btnEdit.Text = "Изменить";
            }

            ClearFields();
            selectedServiceId = -1;
            dataGridView1.ClearSelection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                DialogResult result = MessageBox.Show(
                    "Вы редактируете услугу. Сохранить изменения перед закрытием?",
                    "Подтверждение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!SaveChanges())
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            this.Close();
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == -1)
            {
                MessageBox.Show("Выберите услугу!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isEditing)
            {
                MessageBox.Show("Сначала нажмите 'Изменить'!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
            ofd.Title = "Выберите изображение для услуги";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image original = Image.FromFile(ofd.FileName))
                    {
                        Image resized = ResizeImage(original, 300, 300);

                        // Временное сохранение для проверки уникальности
                        byte[] tempImageBytes = ImageToByteArray(resized);

                        // Проверка уникальности изображения
                        if (!IsImageUnique(tempImageBytes, selectedServiceId))
                        {
                            MessageBox.Show("Это изображение уже используется для другой услуги!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            resized.Dispose();
                            return;
                        }

                        pictureBox1.Image = resized;
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        currentImageBytes = tempImageBytes;
                        currentImageHash = ComputeImageHash(tempImageBytes);
                        btnDeleteImage.Enabled = false; // Удаление отключено
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Кнопка удаления изображения - отключена
        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Удаление изображений отключено!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void YsclygiMen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isEditing)
            {
                DialogResult result = MessageBox.Show(
                    "Вы редактируете услугу. Сохранить изменения перед закрытием?",
                    "Подтверждение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (!SaveChanges())
                        e.Cancel = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}