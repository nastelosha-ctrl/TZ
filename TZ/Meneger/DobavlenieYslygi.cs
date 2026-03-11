using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TZ
{
    public partial class DobavlenieYslygi : Form
    {
        private string imagePath = "";
        private int serviceId = -1;
        private byte[] currentImageBytes = null;
        private string currentImageHash = null;

        public DobavlenieYslygi()
        {
            InitializeComponent();
            LoadCategories();
            SetupValidation();
            ValidateForm();
        }

        public DobavlenieYslygi(int id)
        {
            InitializeComponent();
            serviceId = id;
            LoadCategories();
            LoadServiceData(id);
            btnAdd.Text = "Сохранить";
            SetupValidation();
            ValidateForm();
        }

        private void SetupValidation()
        {
            txtName.TextChanged += (s, e) => ValidateForm();
            txtPrice.TextChanged += (s, e) => ValidateForm();
            comboCategory.SelectedIndexChanged += (s, e) => ValidateForm();
        }

        private void LoadCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = "SELECT id_serviceCategory, category_name FROM Service_Category";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                comboCategory.Items.Clear();
                while (reader.Read())
                {
                    comboCategory.Items.Add(new KeyValuePair<int, string>(
                        reader.GetInt32("id_serviceCategory"),
                        reader.GetString("category_name")
                    ));
                }

                comboCategory.DisplayMember = "Value";
                comboCategory.ValueMember = "Key";
            }
        }

        private void LoadServiceData(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                string sql = @"SELECT service_name, Description, Price, id_serviceCategory, service_image 
                               FROM Service WHERE id_service = @id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtName.Text = reader["service_name"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        txtPrice.Text = reader["Price"].ToString();

                        if (reader["service_image"] != DBNull.Value)
                        {
                            currentImageBytes = (byte[])reader["service_image"];
                            currentImageHash = ComputeImageHash(currentImageBytes);
                            using (MemoryStream ms = new MemoryStream(currentImageBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            btnDeleteImage.Enabled = false; // Удаление отключено
                        }

                        int categoryId = Convert.ToInt32(reader["id_serviceCategory"]);
                        foreach (KeyValuePair<int, string> item in comboCategory.Items)
                        {
                            if (item.Key == categoryId)
                            {
                                comboCategory.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Вычисление хеша изображения
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
                                int existingId = reader.GetInt32("id_service");
                                if (existingId != excludeServiceId)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imagePath = ofd.FileName;
                    using (Image original = Image.FromFile(imagePath))
                    {
                        Image resized = ResizeImage(original, 300, 300);

                        // Временное сохранение для проверки уникальности
                        byte[] tempImageBytes = ImageToByteArray(resized);

                        // Проверка уникальности изображения
                        if (!IsImageUnique(tempImageBytes, serviceId))
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
            ValidateForm();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (serviceId == -1)
                {
                    AddService();
                }
                else
                {
                    UpdateService();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddService()
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                string sql = @"INSERT INTO Service 
                    (service_name, Description, Price, id_serviceCategory, service_image)
                    VALUES (@name, @desc, @price, @cat, @image)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());

                string priceText = txtPrice.Text.Trim().Replace(',', '.');
                cmd.Parameters.AddWithValue("@price", decimal.Parse(priceText, System.Globalization.CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@cat", ((KeyValuePair<int, string>)comboCategory.SelectedItem).Key);

                if (currentImageBytes != null)
                {
                    cmd.Parameters.AddWithValue("@image", currentImageBytes);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@image", DBNull.Value);
                }

                cmd.ExecuteNonQuery();

                MessageBox.Show("Услуга успешно добавлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateService()
        {
            using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();

                string sql = @"UPDATE Service SET
                    service_name = @name,
                    Description = @desc,
                    Price = @price,
                    id_serviceCategory = @cat";

                if (currentImageBytes != null)
                {
                    sql += ", service_image = @image";
                }

                sql += " WHERE id_service = @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());

                string priceText = txtPrice.Text.Trim().Replace(',', '.');
                cmd.Parameters.AddWithValue("@price", decimal.Parse(priceText, System.Globalization.CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@cat", ((KeyValuePair<int, string>)comboCategory.SelectedItem).Key);

                if (currentImageBytes != null)
                {
                    cmd.Parameters.AddWithValue("@image", currentImageBytes);
                }

                cmd.Parameters.AddWithValue("@id", serviceId);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Услуга успешно обновлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
                return false;

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
                return false;

            string priceText = txtPrice.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(priceText, System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price <= 0)
                return false;

            if (comboCategory.SelectedIndex == -1)
                return false;

            return true;
        }

        private void ValidateForm()
        {
            btnAdd.Enabled = ValidateInput();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            comboCategory.SelectedIndex = -1;
            pictureBox1.Image = null;
            imagePath = "";
            currentImageBytes = null;
            currentImageHash = null;
            ValidateForm();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // Кнопка удаления изображения - отключена
        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Удаление изображений отключено!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}