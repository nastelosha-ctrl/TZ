using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration; 

namespace TZ
{
    public partial class Avtorizacia : Form
    {
        private DatabaseHelper _dbHelper;
        private bool passwordVisible = false;

        public Avtorizacia()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();

            this.AcceptButton = txtEnter;
            txtPassword.UseSystemPasswordChar = true;

            // Очищаем данные пользователя при запуске
            CurrentUser.Clear();

            // Подписываемся на события
            txtLogin.KeyPress += txtLogin_KeyPress;
            txtLogin.Leave += txtLogin_Leave;
            txtPassword.KeyPress += txtPassword_KeyPress;
            txtPassword.Leave += txtPassword_Leave;
        }

        #region Валидация логина (только английские буквы и цифры)

        private void txtLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем управляющие символы (Backspace, Delete и т.д.)
            if (char.IsControl(e.KeyChar))
                return;

            // Проверяем, является ли символ английской буквой или цифрой
            if (!char.IsLetterOrDigit(e.KeyChar) ||
                (char.IsLetter(e.KeyChar) && !IsEnglishLetter(e.KeyChar)))
            {
                e.Handled = true;
                errorProvider1.SetError(txtLogin, "Только латинские буквы и цифры");
            }
            else
            {
                errorProvider1.SetError(txtLogin, "");
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
                        errorProvider1.SetError(txtLogin, "Логин должен содержать только латинские буквы и цифры");
                        return;
                    }
                }
                errorProvider1.SetError(txtLogin, "");
            }
        }

        #endregion

        #region Валидация пароля (английские буквы, цифры и спецсимволы)

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем управляющие символы (Backspace, Enter и т.д.)
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    txtEnter_Click(sender, e);
                }
                return;
            }

            // Разрешенные символы для пароля:
            // - английские буквы A-Z, a-z
            // - цифры 0-9
            // - спецсимволы: ! @ # $ % ^ & * ( ) - _ + = { } [ ] : ; " ' < > , . ? / | \ ~ ` 
            string allowedSpecialChars = "!@#$%^&*()-_+=<>?/{}[]:;\"'\\|~`,.";

            // Проверяем допустимые символы
            bool isValidChar = false;

            // Проверка на английскую букву
            if (char.IsLetter(e.KeyChar) && IsEnglishLetter(e.KeyChar))
                isValidChar = true;
            // Проверка на цифру
            else if (char.IsDigit(e.KeyChar))
                isValidChar = true;
            // Проверка на разрешенный спецсимвол
            else if (allowedSpecialChars.Contains(e.KeyChar.ToString()))
                isValidChar = true;

            if (!isValidChar)
            {
                e.Handled = true;
                errorProvider2.SetError(txtPassword, "Недопустимый символ в пароле");
            }
            else
            {
                errorProvider2.SetError(txtPassword, "");
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            if (!string.IsNullOrEmpty(password))
            {
                // Проверяем каждый символ
                foreach (char c in password)
                {
                    if (!IsValidPasswordChar(c))
                    {
                        errorProvider2.SetError(txtPassword, "Пароль содержит недопустимые символы");
                        return;
                    }
                }
                errorProvider2.SetError(txtPassword, "");
            }
        }

        /// <summary>
        /// Проверка допустимости символа для пароля
        /// </summary>
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

        #endregion

        /// <summary>
        /// Проверка, является ли буква английской
        /// </summary>
        private bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private void picShowPassword_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                txtPassword.UseSystemPasswordChar = false;
                pictureBox2.Image = Properties.Resources.eye_open;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                pictureBox2.Image = Properties.Resources.eye_closed;
            }

            txtPassword.Focus();
            txtPassword.SelectionStart = txtPassword.Text.Length;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SettingsForm i = new SettingsForm();
            i.ShowDialog(this);
        }

        private void txtEnter_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            // Валидация ввода
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                return;
            }

            // Проверяем логин на допустимые символы
            foreach (char c in login)
            {
                if (!char.IsDigit(c) && !IsEnglishLetter(c))
                {
                    MessageBox.Show("Логин может содержать только латинские буквы и цифры!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLogin.Focus();
                    txtLogin.SelectAll();
                    return;
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Проверяем пароль на допустимые символы
            foreach (char c in password)
            {
                if (!IsValidPasswordChar(c))
                {
                    MessageBox.Show("Пароль содержит недопустимые символы!\nРазрешены: латинские буквы, цифры и спецсимволы",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    txtPassword.SelectAll();
                    return;
                }
            }

            // Проверяем подключение к базе
            if (!DatabaseHelper.TestConnection())
            {
                return;
            }

            try
            {
                User user = AuthenticateUser(login, password);

                if (user != null)
                {
                    HandleSuccessfulLogin(user);
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private User AuthenticateUser(string login, string password)
        {
            // Проверка специального администратора из App.config
            string importLogin = ConfigurationManager.AppSettings["ImportAdminLogin"];
            string importPassword = ConfigurationManager.AppSettings["ImportAdminPassword"];
            string importName = ConfigurationManager.AppSettings["ImportAdminName"];

            if (login == importLogin && password == importPassword)
            {
                return new User
                {
                    Id = -1,
                    Login = login,
                    FullName = importName,
                    Role = "ImportAdmin"
                };
            }

            // Обычная проверка в БД
            string query = @"
                SELECT u.id_user, u.FIO, u.Login, u.Password, r.role_name 
                FROM User u 
                INNER JOIN Role r ON u.id_role = r.id_role 
                WHERE u.Login = @Login";

            using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader["Password"].ToString();
                        string userRole = reader["role_name"].ToString();
                        string fullName = reader["FIO"].ToString();
                        int userId = Convert.ToInt32(reader["id_user"]);

                        bool isPasswordValid = VerifyAndUpdatePassword(password, storedPassword, userId);

                        if (isPasswordValid)
                        {
                            return new User
                            {
                                Id = userId,
                                Login = login,
                                FullName = fullName,
                                Role = userRole
                            };
                        }
                    }
                }
            }

            return null;
        }

        private bool VerifyAndUpdatePassword(string password, string storedPassword, int userId)
        {
            if (storedPassword.Length == 44 && storedPassword.Contains("="))
            {
                string hashedInput = SimpleHash(password);
                return hashedInput == storedPassword;
            }
            else
            {
                if (password == storedPassword)
                {
                    UpdatePasswordHash(userId, password);
                    return true;
                }
                return false;
            }
        }

        private string SimpleHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void UpdatePasswordHash(int userId, string password)
        {
            try
            {
                string hashedPassword = SimpleHash(password);

                using (var connection = new MySqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE User SET Password = @Password WHERE id_user = @UserId";
                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Password", hashedPassword);
                        updateCommand.Parameters.AddWithValue("@UserId", userId);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении пароля: {ex.Message}");
            }
        }

        private void HandleSuccessfulLogin(User user)
        {
            CurrentUser.Id = user.Id;
            CurrentUser.FIO = user.FullName;
            CurrentUser.Login = user.Login;
            CurrentUser.Role = user.Role;

            MessageBox.Show($"Добро пожаловать, {user.FullName}!\nРоль: {user.Role}");

            Form mainForm = null;

            switch (user.Role.ToLower())
            {
                case "администратор":
                    mainForm = new Administrator();
                    break;
                case "менеджер":
                    mainForm = new Menedcher();
                    break;
                case "директор":
                    mainForm = new Director();
                    break;
                case "importadmin": // Специальная учётка из конфига
                    mainForm = new DatabaseAdminForm();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль");
                    CurrentUser.Clear();
                    return;
            }

            if (mainForm != null)
            {
                mainForm.FormClosed += (s, args) =>
                {
                    this.Show();
                    this.txtLogin.Clear();
                    this.txtPassword.Clear();
                    this.txtLogin.Focus();
                    errorProvider1.SetError(txtLogin, "");
                    errorProvider2.SetError(txtPassword, "");
                };

                this.Hide();
                mainForm.Show();
            }
        }

        private void txtClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Cursor = Cursors.Hand;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing && !CurrentUser.IsAuthenticated)
            {
                // Разрешаем закрытие формы
            }
        }
    }
}