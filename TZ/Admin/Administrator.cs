using System;
using System.Windows.Forms;

namespace TZ
{
    public partial class Administrator : Form
    {
        public Administrator()
        {
            InitializeComponent();
            this.FormClosed += Administrator_FormClosed;
            this.Load += Administrator_Load; // Добавляем обработчик загрузки формы
        }
        private void Administrator_Load(object sender, EventArgs e)
        {
            // Отображаем ФИО пользователя в label
            DisplayUserInfo();
        }

        private void DisplayUserInfo()
        {
            // Проверяем, авторизован ли пользователь
            if (CurrentUser.IsAuthenticated)
            {
                // Предполагаем, что на форме есть label с именем lblUserInfo
                // Если label называется по-другому, замените название
                lblUserInfo.Text = $"Пользователь: {CurrentUser.FIO}";

                // Можно также добавить роль для информации
                // lblUserInfo.Text = $"Администратор: {CurrentUser.FIO}";
            }
            else
            {
                lblUserInfo.Text = "Пользователь: не авторизован";
            }
        }

        private void Administrator_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Очищаем данные пользователя
            CurrentUser.Clear();
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
    }
}