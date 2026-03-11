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
        }
        private void Director_Load(object sender, EventArgs e)
        {
            // Отображаем ФИО пользователя в label
            DisplayUserInfo();
        }
        private void DisplayUserInfo()
        {
            // Проверяем, авторизован ли пользователь
            if (CurrentUser.IsAuthenticated)
            {
                lblUserInfo.Text = $"Пользователь: {CurrentUser.FIO}";
            }
            else
            {
                lblUserInfo.Text = "Пользователь: не авторизован";
            }
        }

        private void Director_FormClosed(object sender, FormClosedEventArgs e)
        {
            CurrentUser.Clear();
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