using System;
using System.Data;
using System.Windows.Forms;

namespace TZ
{
    public partial class ProsmotrZakaza : Form
    {
        private int orderId;
        private string clientFIO;
        private string clientPhone;
        private DateTime dateAdmission;
        private DateTime dateDue;
        private DataTable basket;

        public bool DiscountApplied { get; private set; }

        public ProsmotrZakaza(int orderId, string fio, string phone, DateTime admission, DateTime due, DataTable basketData)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.clientFIO = fio;
            this.clientPhone = phone;
            this.dateAdmission = admission;
            this.dateDue = due;
            this.basket = basketData;


            LoadPreview();
        }

        private void LoadPreview()
        {
            this.Text = $"Просмотр заказа № {orderId}";

            lblClient.Text = $"Клиент: {clientFIO}";
            lblAdmission.Text = $"Дата приёма: {dateAdmission:dd.MM.yyyy}";

            dgvBasketPreview.DataSource = basket;
            dgvBasketPreview.Columns["id_service"].Visible = false;
            dgvBasketPreview.Columns["service_name"].HeaderText = "Услуга";
            dgvBasketPreview.Columns["price"].HeaderText = "Цена";
            dgvBasketPreview.Columns["price"].DefaultCellStyle.Format = "N2";
            dgvBasketPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            CalculateTotals();
        }

        private void CalculateTotals()
        {
            decimal total = 0m;
            foreach (DataRow row in basket.Rows)
            {
                total += Convert.ToDecimal(row["price"]);
            }

            lblSum.Text = $"Сумма: {total:N2} руб.";
            chkDiscount.Checked = total > 1000;
            chkDiscount.Enabled = total > 1000; // Блокируем чекбокс если сумма меньше 1000

            decimal final = chkDiscount.Checked ? total * 0.9m : total;
            lblDiscountSum.Text = $"Сумма со скидкой: {final:N2} руб.";
        }

        private void chkDiscount_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            DiscountApplied = chkDiscount.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnBackToOrder_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void contextMenuBasket_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = (dgvBasketPreview.CurrentRow == null);
        }

        private void mnuRemoveService_Click(object sender, EventArgs e)
        {
            if (dgvBasketPreview.CurrentRow == null) return;

            DialogResult res = MessageBox.Show(
                "Удалить эту услугу из корзины?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                basket.Rows.RemoveAt(dgvBasketPreview.CurrentRow.Index);
                CalculateTotals();
            }
        }

        // Удаляем старую кнопку печати чека, так как чек теперь формируется после оформления
        private void btnPrintCheck_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Чек будет сформирован после подтверждения заказа.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}