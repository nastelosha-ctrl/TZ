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
using System.Xml.Linq;

namespace TZ
{
    public partial class Spravochniki : Form
    {
        private string currentTable = ""; // Текущая выбранная таблица
        private int currentId = -1; // ID выбранной записи
        private bool isEditing = false; // Режим редактирования

        public Spravochniki()
        {
            InitializeComponent();
            LoadComboBox();
            SetControlsEnabled(false);

            // Подписываемся на событие KeyPress для поля ввода
            txtName.KeyPress += txtName_KeyPress;
            txtName.Leave += txtName_Leave;

            // КНОПКИ РАЗБЛОКИРОВАНЫ ПО УМОЛЧАНИЮ
            btnAdd.Enabled = true;
            btnEdit.Enabled = false; // Только когда выбрана запись
        }

        #region Валидация ввода (только русские буквы)

        /// <summary>
        /// Проверка, является ли буква русской
        /// </summary>
        private bool IsRussianLetter(char c)
        {
            // Русские буквы в верхнем и нижнем регистре
            return (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == 'Ё' || c == 'ё';
        }

        /// <summary>
        /// Ограничение ввода: только русские буквы
        /// </summary>
        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем управляющие символы (Backspace, Delete, Enter и т.д.)
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter && isEditing)
                {
                    btnEdit_Click(sender, e);
                }
                return;
            }

            // Проверяем, является ли символ русской буквой
            if (!IsRussianLetter(e.KeyChar))
            {
                e.Handled = true; // Блокируем ввод

                // Показываем подсказку
                if (!string.IsNullOrEmpty(errorProvider1?.GetError(txtName)))
                {
                    errorProvider1.SetError(txtName, "Только русские буквы");
                }
            }
            else
            {
                errorProvider1?.SetError(txtName, "");
            }
        }

        /// <summary>
        /// Проверка при потере фокуса
        /// </summary>
        private void txtName_Leave(object sender, EventArgs e)
        {
            string text = txtName.Text.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text)
                {
                    if (!IsRussianLetter(c) && !char.IsWhiteSpace(c))
                    {
                        errorProvider1.SetError(txtName, "Только русские буквы");
                        return;
                    }
                }
                errorProvider1.SetError(txtName, "");
            }
        }

        #endregion

        /// <summary>
        /// Загрузка списка справочников в ComboBox
        /// </summary>
        private void LoadComboBox()
        {
            cmbSpravochniki.Items.Clear();
            cmbSpravochniki.Items.Add("Роли");
            cmbSpravochniki.Items.Add("Статус заказа");
            cmbSpravochniki.Items.Add("Категории услуг");
            cmbSpravochniki.SelectedIndex = 0;
        }

        /// <summary>
        /// Выбор справочника
        /// </summary>
        private void cmbSpravochniki_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbSpravochniki.SelectedIndex)
            {
                case 0:
                    currentTable = "Role";
                    LoadData("SELECT id_role, role_name AS 'Наименование' FROM Role ORDER BY id_role");
                    lblName.Text = "Наименование роли:";
                    break;
                case 1:
                    currentTable = "Status";
                    LoadData("SELECT id_status, status_name AS 'Наименование' FROM Status ORDER BY id_status");
                    lblName.Text = "Наименование статуса:";
                    break;
                case 2:
                    currentTable = "Service_Category";
                    LoadData("SELECT id_serviceCategory, category_name AS 'Наименование' FROM Service_Category ORDER BY id_serviceCategory");
                    lblName.Text = "Наименование категории:";
                    break;
            }

            ClearFields();
            currentId = -1;
            isEditing = false;
            btnEdit.Text = "Изменить";
            SetControlsEnabled(false);

            // КНОПКИ: Добавить всегда активна, Изменить/Удалить - не активны (нет выбранной записи)
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;

            errorProvider1?.SetError(txtName, ""); // Очищаем ошибку
        }

        /// <summary>
        /// Загрузка данных в DataGridView
        /// </summary>
        private void LoadData(string sql)
        {
            try
            {
                DataTable dt = DatabaseHelper.GetData(sql);
                dgvSpravochnik.DataSource = dt;

                // Настройка DataGridView
                if (dgvSpravochnik.Columns.Count > 0)
                {
                    // Скрываем колонку с ID
                    if (dgvSpravochnik.Columns[0].Name.Contains("id"))
                        dgvSpravochnik.Columns[0].Visible = false;

                    dgvSpravochnik.Columns["Наименование"].Width = 250;
                    dgvSpravochnik.Columns["Наименование"].HeaderText = "Наименование";

                    dgvSpravochnik.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvSpravochnik.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvSpravochnik.MultiSelect = false;
                    dgvSpravochnik.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Выбор строки в DataGridView
        /// </summary>
        private void dgvSpravochnik_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                // Получаем ID выбранной записи
                currentId = Convert.ToInt32(dgvSpravochnik.Rows[e.RowIndex].Cells[0].Value);
                string name = dgvSpravochnik.Rows[e.RowIndex].Cells["Наименование"].Value.ToString();

                txtName.Text = name;
                errorProvider1?.SetError(txtName, ""); // Очищаем ошибку

                // РАЗБЛОКИРУЕМ КНОПКИ
                btnEdit.Enabled = true;
                btnAdd.Enabled = true;

                // Если были в режиме редактирования - выходим
                if (isEditing)
                {
                    isEditing = false;
                    btnEdit.Text = "Изменить";
                    SetControlsEnabled(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора записи: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Кнопка "Добавить"
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentTable))
            {
                MessageBox.Show("Выберите справочник!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Очищаем поля и включаем редактирование
            ClearFields();
            currentId = -1;
            isEditing = true;
            btnEdit.Text = "Сохранить";

            // БЛОКИРУЕМ КНОПКИ В РЕЖИМЕ РЕДАКТИРОВАНИЯ
            btnAdd.Enabled = false;
            btnEdit.Enabled = true;

            SetControlsEnabled(true);
            txtName.Focus();
        }

        /// <summary>
        /// Кнопка "Изменить/Сохранить"
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentTable))
            {
                MessageBox.Show("Выберите справочник!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isEditing)
            {
                // РЕЖИМ РЕДАКТИРОВАНИЯ
                if (currentId == -1)
                {
                    MessageBox.Show("Выберите запись для редактирования!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                isEditing = true;
                btnEdit.Text = "Сохранить";

                // БЛОКИРУЕМ ДРУГИЕ КНОПКИ
                btnAdd.Enabled = false;

                SetControlsEnabled(true);
                txtName.Focus();
                txtName.SelectAll();
            }
            else
            {
                // РЕЖИМ СОХРАНЕНИЯ
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите наименование!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                // Проверяем, что введены только русские буквы
                foreach (char c in txtName.Text.Trim())
                {
                    if (!IsRussianLetter(c) && !char.IsWhiteSpace(c))
                    {
                        MessageBox.Show("Наименование должно содержать только русские буквы!",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtName.Focus();
                        txtName.SelectAll();
                        return;
                    }
                }

                if (currentId == -1)
                    AddRecord();
                else
                    UpdateRecord();
            }
        }

        /// <summary>
        /// Добавление записи
        /// </summary>
        private void AddRecord()
        {
            try
            {
                string sql = "";

                // Определяем SQL запрос для каждого справочника
                switch (currentTable)
                {
                    case "Role":
                        sql = "INSERT INTO Role (role_name) VALUES (@name)";
                        break;
                    case "Status":
                        sql = "INSERT INTO Status (status_name) VALUES (@name)";
                        break;
                    case "Service_Category":
                        sql = "INSERT INTO Service_Category (category_name) VALUES (@name)";
                        break;
                }

                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись успешно добавлена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Перезагружаем данные
                        cmbSpravochniki_SelectedIndexChanged(null, null);

                        isEditing = false;
                        btnEdit.Text = "Изменить";
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = false;
                        SetControlsEnabled(false);
                        ClearFields();
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry
                {
                    MessageBox.Show("Запись с таким наименованием уже существует!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обновление записи
        /// </summary>
        private void UpdateRecord()
        {
            try
            {
                string sql = "";

                // Определяем SQL запрос для каждого справочника
                switch (currentTable)
                {
                    case "Role":
                        sql = "UPDATE Role SET role_name = @name WHERE id_role = @id";
                        break;
                    case "Status":
                        sql = "UPDATE Status SET status_name = @name WHERE id_status = @id";
                        break;
                    case "Service_Category":
                        sql = "UPDATE Service_Category SET category_name = @name WHERE id_serviceCategory = @id";
                        break;
                }

                using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", currentId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись успешно обновлена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Перезагружаем данные
                        cmbSpravochniki_SelectedIndexChanged(null, null);

                        // Выходим из режима редактирования
                        isEditing = false;
                        btnEdit.Text = "Изменить";
                        btnAdd.Enabled = true;
                        btnEdit.Enabled = false;
                        SetControlsEnabled(false);
                        ClearFields();

                        currentId = -1;
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry
                {
                    MessageBox.Show("Запись с таким наименованием уже существует!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Кнопка "Удалить"
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentId == -1)
            {
                MessageBox.Show("Выберите запись для удаления!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ПОДТВЕРЖДЕНИЕ УДАЛЕНИЯ (по ТЗ)
            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись \"{txtName.Text}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string sql = "";

                    // Определяем SQL запрос для каждого справочника
                    switch (currentTable)
                    {
                        case "Role":
                            sql = "DELETE FROM Role WHERE id_role = @id";
                            break;
                        case "Status":
                            sql = "DELETE FROM Status WHERE id_status = @id";
                            break;
                        case "Service_Category":
                            sql = "DELETE FROM Service_Category WHERE id_serviceCategory = @id";
                            break;
                    }

                    using (MySqlConnection conn = new MySqlConnection(DatabaseHelper.ConnectionString))
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", currentId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно удалена!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Перезагружаем данные
                            cmbSpravochniki_SelectedIndexChanged(null, null);

                            ClearFields();
                            currentId = -1;
                            btnEdit.Enabled = false;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451) // Foreign key constraint
                    {
                        MessageBox.Show("Невозможно удалить запись, так как она используется в других таблицах!",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Кнопка "Меню" - выход
        /// </summary>
        private void btnMenu_Click(object sender, EventArgs e)
        {
            // Если в режиме редактирования, спрашиваем подтверждение
            if (isEditing)
            {
                DialogResult result = MessageBox.Show(
                    "Изменения не сохранены. Выйти?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            this.Close();
        }

        /// <summary>
        /// Включение/отключение полей ввода
        /// </summary>
        private void SetControlsEnabled(bool enabled)
        {
            txtName.Enabled = enabled;
        }

        /// <summary>
        /// Очистка полей
        /// </summary>
        private void ClearFields()
        {
            txtName.Clear();
            errorProvider1?.SetError(txtName, "");
        }
    }
}