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
        }

        private void Menedcher_FormClosed(object sender, FormClosedEventArgs e)
        {
            CurrentUser.Clear();
        }

        private void Menedcher_Load(object sender, EventArgs e)
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