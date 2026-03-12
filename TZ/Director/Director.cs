using System;
using System.Windows.Forms;

namespace TZ
{
    public partial class Director : Form
    {
        public Director()
        {
            InitializeComponent();
            this.FormClosed += Director_FormClosed;
            this.Load += Director_Load; // Добавляем обработчик загрузки формы

            // Инициализируем таймер бездействия
            InactivityTimer.Initialize(this, OnInactivity);
        }

        private void Director_Load(object sender, EventArgs e)
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

        private void Director_FormClosed(object sender, FormClosedEventArgs e)
        {
            InactivityTimer.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YchetZakazovDirector i = new YchetZakazovDirector();
            i.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Yslygi i = new Yslygi();
            i.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e) // Выход
        {
            this.Close();
        }
    }
}