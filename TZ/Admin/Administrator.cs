using System;
using System.Windows.Forms;
using TZ.Admin;

namespace TZ
{
    public partial class Administrator : Form
    {
        public Administrator()
        {
            InitializeComponent();
            this.FormClosed += Administrator_FormClosed;
            this.Load += Administrator_Load; // Добавляем обработчик загрузки формы

            // Инициализируем таймер бездействия
            InactivityTimer.Initialize(this, OnInactivity);
        }
        private void Administrator_Load(object sender, EventArgs e)
        {
            DisplayUserInfo();
        }

        private void DisplayUserInfo()
        {
            if (CurrentUser.IsAuthenticated)
            {
                lblUserInfo.Text = $"Пользователь: {CurrentUser.FIO}";
            }
            else
            {
                lblUserInfo.Text = "Пользователь: не авторизован";
            }
        }

        /// <summary>
        /// Действие при бездействии пользователя
        /// </summary>
        private void OnInactivity()
        {
            // Останавливаем таймер
            InactivityTimer.Stop();

            // Очищаем данные пользователя
            CurrentUser.Clear();

            // Создаем и показываем форму авторизации
            Avtorizacia loginForm = new Avtorizacia();
            loginForm.Show();

            // Закрываем текущую форму
            this.Close();
        }

        private void Administrator_FormClosed(object sender, FormClosedEventArgs e)
        {
            InactivityTimer.Stop();
        }
        private void button3_Click(object sender, EventArgs e) // Выход
        {
            // Просто закрываем форму - FormClosed сам создаст новую авторизацию
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Users i = new Users();
            i.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YchetZakazovAdmin i = new YchetZakazovAdmin();
            i.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Spravochniki i = new Spravochniki();
            i.ShowDialog();
        }
        private void btnDatabaseAdmin_Click(object sender, EventArgs e)
        {
            DatabaseAdminForm form = new DatabaseAdminForm();
            form.ShowDialog();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            InactivitySettingsForm settingsForm = new InactivitySettingsForm();
            settingsForm.ShowDialog(this);
        }
    }
}