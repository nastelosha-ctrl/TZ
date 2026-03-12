using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace TZ.Admin
{
    public partial class InactivitySettingsForm : Form
    {
        public InactivitySettingsForm()
        {
            InitializeComponent();
            LoadSettings();
            this.StartPosition = FormStartPosition.CenterParent;
            numericTimeout.ValueChanged += NumericTimeout_ValueChanged;
        }

        private void LoadSettings()
        {
            try
            {
                string timeoutStr = ConfigurationManager.AppSettings["InactivityTimeoutSeconds"];
                if (int.TryParse(timeoutStr, out int timeout))
                {
                    NumericUpDown numeric = this.Controls.Find("numericTimeout", true)[0] as NumericUpDown;
                    if (numeric != null)
                    {
                        numeric.Value = timeout;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NumericTimeout_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numeric = sender as NumericUpDown;
            Label preview = this.Controls.Find("lblPreviewText", true)[0] as Label;

            if (preview != null && numeric != null)
            {
                preview.Text = $"Система заблокируется через {numeric.Value} секунд бездействия";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown numeric = this.Controls.Find("numericTimeout", true)[0] as NumericUpDown;

                if (numeric != null)
                {
                    // Сохраняем в App.config
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    if (config.AppSettings.Settings["InactivityTimeoutSeconds"] != null)
                    {
                        config.AppSettings.Settings["InactivityTimeoutSeconds"].Value = numeric.Value.ToString();
                    }
                    else
                    {
                        config.AppSettings.Settings.Add("InactivityTimeoutSeconds", numeric.Value.ToString());
                    }

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");

                    // Обновляем таймер, если он уже инициализирован
                    InactivityTimer.UpdateTimeout((int)numeric.Value);

                    MessageBox.Show("Настройки успешно сохранены!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
