using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace TZ
{
    public partial class Users : Form
    {
        private DataTable usersTable;
        private int selectedUserId = -1;
        private bool isInitialLoad = true;

        public Users()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;

            // Подписываемся на события валидации
            txtFIO.KeyPress += txtFIO_KeyPress;
            txtFIO.Leave += txtFIO_Leave;
            txtLogin.KeyPress += txtLogin_KeyPress;
            txtLogin.Leave += txtLogin_Leave;
            txtPassword.KeyPress += txtPassword_KeyPress;
            txtPassword.Leave += txtPassword_Leave;
        }

        #region Валидация ФИО (только русские буквы)

        private bool IsRussianLetter(char c)
        {
            return (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == 'Ё' || c == 'ё';
        }

        private void txtFIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!IsRussianLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
                errorProvider1.SetError(txtFIO, "Только русские буквы");
            }
            else
            {
                errorProvider1.SetError(txtFIO, "");
            }
        }

        private void txtFIO_Leave(object sender, EventArgs e)
        {
            string fio = txtFIO.Text.Trim();

            if (!string.IsNullOrEmpty(fio))
            {
                foreach (char c in fio)
                {
                    if (!IsRussianLetter(c) && !char.IsWhiteSpace(c))
                    {
                        errorProvider1.SetError(txtFIO, "ФИО должно содержать только русские буквы");
                        return;
                    }
                }
                errorProvider1.SetError(txtFIO, "");
            }
        }

        #endregion

        #region Валидация Логина (только английские буквы и цифры)

        private bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private void txtLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar) && !IsEnglishLetter(e.KeyChar))
            {
                e.Handled = true;
                errorProvider2.SetError(txtLogin, "Только латинские буквы и цифры");
            }
            else
            {
                errorProvider2.SetError(txtLogin, "");
            }
        }

        private void txtLogin_Leave(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();

            if (!string.IsNullOrEmpty(login))
            {
                foreach (char c in login)
                {
                    if (!char.IsDigit(c) && !IsEnglishLetter(c))
                    {
                        errorProvider2.SetError(txtLogin, "Логин должен содержать только латинские буквы и цифры");
                        return;
                    }
                }
                errorProvider2.SetError(txtLogin, "");
            }
        }

        #endregion

        #region Валидация Пароля (только английские буквы, цифры и спецсимволы)

        private bool IsValidPasswordChar(char c)
        {
            // Английские буквы
            if (IsEnglishLetter(c))
                return true;

            // Цифры
            if (char.IsDigit(c))
                return true;

            // Разрешенные спецсимволы
            string allowedSpecialChars = "!@#$%^&*()-_+=<>?/{}[]:;\"'\\|~`,.";
            if (allowedSpecialChars.Contains(c.ToString()))
                return true;

            return false;
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!IsValidPasswordChar(e.KeyChar))
            {
                e.Handled = true;
                errorProvider3.SetError(txtPassword, "Недопустимый символ в пароле");
            }
            else
            {
                errorProvider3.SetError(txtPassword, "");
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            if (!string.IsNullOrEmpty(password))
            {
                foreach (char c in password)
                {
                    if (!IsValidPasswordChar(c))
                    {
                        errorProvider3.SetError(txtPassword, "Пароль содержит недопустимые символы");
                        return;
                    }
                }
                errorProvider3.SetError(txtPassword, "");
            }
        }

        #endregion

        private void Users_Load(object sender, EventArgs e)
        {
            isInitialLoad = true;
            LoadRoles();
            LoadUsers();

            // Принудительный сброс выделения
            if (dgvUsers.Rows.Count > 0)
            {
                dgvUsers.CurrentCell = null;
            }

            dgvUsers.ClearSelection();
            selectedUserId = -1;
            ClearFields();
            isInitialLoad = false;
        }

        private void LoadRoles()
        {
            string sql = "SELECT id_role, role_name FROM Role ORDER BY id_role";
            DataTable dt = DatabaseHelper.GetData(sql);

            cmbRole.DataSource = dt;
            cmbRole.DisplayMember = "role_name";
            cmbRole.ValueMember = "id_role";
            cmbRole.SelectedIndex = -1;
        }

        private void LoadUsers()
        {
            string sql = @"
                SELECT 
                    u.id_user,
                    u.FIO,
                    u.Login,
                    r.role_name AS Роль,
                    u.id_role
                FROM User u
                INNER JOIN Role r ON u.id_role = r.id_role
                ORDER BY u.id_user";

            usersTable = DatabaseHelper.GetData(sql);
            dgvUsers.DataSource = usersTable;

            dgvUsers.Columns["id_user"].Visible = false;
            dgvUsers.Columns["id_role"].Visible = false;
            dgvUsers.Columns["FIO"].HeaderText = "ФИО";
            dgvUsers.Columns["Login"].HeaderText = "Логин";
            dgvUsers.Columns["Роль"].HeaderText = "Роль";
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (isInitialLoad) return;

            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0 || dgvUsers.SelectedRows.Count == 0)
            {
                ClearFields();
                return;
            }

            selectedUserId = Convert.ToInt32(dgvUsers.CurrentRow.Cells["id_user"].Value);
            txtFIO.Text = dgvUsers.CurrentRow.Cells["FIO"].Value?.ToString() ?? "";
            txtLogin.Text = dgvUsers.CurrentRow.Cells["Login"].Value?.ToString() ?? "";
            txtPassword.Clear();

            int idRole = Convert.ToInt32(dgvUsers.CurrentRow.Cells["id_role"].Value);
            cmbRole.SelectedValue = idRole;

            btnDelete.Enabled = (dgvUsers.CurrentRow.Cells["Роль"].Value?.ToString() != "Администратор");

            // Очищаем ошибки
            errorProvider1.SetError(txtFIO, "");
            errorProvider2.SetError(txtLogin, "");
            errorProvider3.SetError(txtPassword, "");
        }

        private bool IsLoginUnique(string login, int excludeUserId = -1)
        {
            string sql = "SELECT COUNT(*) FROM User WHERE Login = @login";
            if (excludeUserId != -1)
            {
                sql += " AND id_user != @userId";
            }

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@login", login)
            };

            if (excludeUserId != -1)
            {
                parameters.Add(new MySqlParameter("@userId", excludeUserId));
            }

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, parameters.ToArray()));
            return count == 0;
        }

        private bool ValidateUserInput()
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(txtFIO.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFIO.Focus();
                return false;
            }

            foreach (char c in txtFIO.Text.Trim())
            {
                if (!IsRussianLetter(c) && !char.IsWhiteSpace(c))
                {
                    MessageBox.Show("ФИО должно содержать только русские буквы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFIO.Focus();
                    txtFIO.SelectAll();
                    return false;
                }
            }

            // Проверка Логина
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                return false;
            }

            foreach (char c in txtLogin.Text.Trim())
            {
                if (!char.IsDigit(c) && !IsEnglishLetter(c))
                {
                    MessageBox.Show("Логин должен содержать только латинские буквы и цифры!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLogin.Focus();
                    txtLogin.SelectAll();
                    return false;
                }
            }

            // Проверка уникальности логина
            if (!IsLoginUnique(txtLogin.Text.Trim(), selectedUserId))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                txtLogin.SelectAll();
                return false;
            }

            // Проверка Пароля (при добавлении)
            if (selectedUserId == -1 && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            // Проверка пароля на допустимые символы (если введен)
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                foreach (char c in txtPassword.Text)
                {
                    if (!IsValidPasswordChar(c))
                    {
                        MessageBox.Show("Пароль содержит недопустимые символы!\nРазрешены: латинские буквы, цифры и спецсимволы",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Focus();
                        txtPassword.SelectAll();
                        return false;
                    }
                }
            }

            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите роль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateUserInput()) return;

            string sql = @"
                INSERT INTO User (FIO, Login, Password, id_role)
                VALUES (@fio, @login, @password, @roleId)";

            var parameters = new[]
            {
                new MySqlParameter("@fio", txtFIO.Text.Trim()),
                new MySqlParameter("@login", txtLogin.Text.Trim()),
                new MySqlParameter("@password", SimpleHash(txtPassword.Text)),
                new MySqlParameter("@roleId", cmbRole.SelectedValue)
            };

            try
            {
                int rows = DatabaseHelper.ExecuteNonQuery(sql, parameters);

                if (rows > 0)
                {
                    MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();

                    // Сбрасываем выделение
                    if (dgvUsers.Rows.Count > 0)
                    {
                        dgvUsers.CurrentCell = null;
                    }
                    dgvUsers.ClearSelection();
                    selectedUserId = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedUserId == -1)
            {
                MessageBox.Show("Выберите пользователя для изменения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateUserInput()) return;

            string sql = @"
                UPDATE User SET 
                    FIO = @fio,
                    Login = @login,
                    id_role = @roleId";

            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                sql += ", Password = @password";
            }

            sql += " WHERE id_user = @id";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@fio", txtFIO.Text.Trim()),
                new MySqlParameter("@login", txtLogin.Text.Trim()),
                new MySqlParameter("@roleId", cmbRole.SelectedValue),
                new MySqlParameter("@id", selectedUserId)
            };

            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                parameters.Add(new MySqlParameter("@password", SimpleHash(txtPassword.Text)));
            }

            try
            {
                int rows = DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());

                if (rows > 0)
                {
                    MessageBox.Show("Пользователь успешно обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();

                    // Сбрасываем выделение
                    if (dgvUsers.Rows.Count > 0)
                    {
                        dgvUsers.CurrentCell = null;
                    }
                    dgvUsers.ClearSelection();
                    selectedUserId = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId == -1)
            {
                MessageBox.Show("Выберите пользователя для удаления!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string role = dgvUsers.CurrentRow.Cells["Роль"].Value?.ToString();
            if (role == "Администратор")
            {
                MessageBox.Show("Администратора удалить нельзя!", "Запрещено",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить пользователя?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string sql = "DELETE FROM User WHERE id_user = @id";
            var param = new MySqlParameter("@id", selectedUserId);

            try
            {
                int rows = DatabaseHelper.ExecuteNonQuery(sql, param);

                if (rows > 0)
                {
                    MessageBox.Show("Пользователь успешно удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();

                    // Сбрасываем выделение
                    if (dgvUsers.Rows.Count > 0)
                    {
                        dgvUsers.CurrentCell = null;
                    }
                    dgvUsers.ClearSelection();
                    selectedUserId = -1;
                }
                else
                {
                    MessageBox.Show("Не удалось удалить пользователя (возможно, есть связанные заказы)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();

            if (dgvUsers.Rows.Count > 0)
            {
                dgvUsers.CurrentCell = null;
            }
            dgvUsers.ClearSelection();
            selectedUserId = -1;
        }

        private string SimpleHash(string input)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void ClearFields()
        {
            txtFIO.Clear();
            txtLogin.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;

            errorProvider1.SetError(txtFIO, "");
            errorProvider2.SetError(txtLogin, "");
            errorProvider3.SetError(txtPassword, "");
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Administrator i = new Administrator();
            i.ShowDialog();
            this.Visible = true;
        }
    }
}