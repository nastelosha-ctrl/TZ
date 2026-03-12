using System;
using System.Windows.Forms;

namespace TZ
{
    public partial class Menedcher : Form
    {
        public Menedcher()
        {
            InitializeComponent();
            this.FormClosed += Menedcher_FormClosed;
            this.Load += Menedcher_Load; // Добавляем обработчик загрузки формы

            // Инициализируем таймер бездействия
            InactivityTimer.Initialize(this, OnInactivity);
        }

        private void Menedcher_Load(object sender, EventArgs e)
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
        { // Останавливаем таймер
            InactivityTimer.Stop();

            // Очищаем данные пользователя
            CurrentUser.Clear();

            // Создаем и показываем форму авторизации
            Avtorizacia loginForm = new Avtorizacia();
            loginForm.Show();

            // Закрываем текущую форму
            this.Close();
        }

        private void Menedcher_FormClosed(object sender, FormClosedEventArgs e)
        {
            InactivityTimer.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            YchetZakazovMen i = new YchetZakazovMen();
            i.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            YsclygiMen i = new YsclygiMen();
            i.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OformlenieZakaza i = new OformlenieZakaza();
            i.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e) // Выход
        {
            this.Close();
        }
    }
}