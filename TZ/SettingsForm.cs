using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TZ
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["Host"] = textBox1.Text;
            Properties.Settings.Default["Database"] = textBox2.Text;
            Properties.Settings.Default["Username"] = textBox3.Text;
            Properties.Settings.Default["Password"] = textBox4.Text;
            Properties.Settings.Default.Save();

            MessageBox.Show("Настройки сохранены");
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

            textBox1.Text = Properties.Settings.Default.host;
            textBox2.Text = Properties.Settings.Default.Database;
            textBox3.Text = Properties.Settings.Default.Username;
            textBox4.Text = Properties.Settings.Default.Password;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
