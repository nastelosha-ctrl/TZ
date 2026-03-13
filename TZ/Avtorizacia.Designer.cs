namespace TZ
{
    partial class Avtorizacia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Avtorizacia));
            this.txtEnter = new System.Windows.Forms.Button();
            this.txtClose = new System.Windows.Forms.Button();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBoxCaptcha = new System.Windows.Forms.GroupBox();
            this.pictureBoxCaptcha = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblCaptcha = new System.Windows.Forms.Label();
            this.txtCaptcha = new System.Windows.Forms.TextBox();
            this.btnRefreshCaptcha = new System.Windows.Forms.Button();
            this.lblBlockTimer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.groupBoxCaptcha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).BeginInit();
            this.SuspendLayout();
            // 
            // txtEnter
            // 
            this.txtEnter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.txtEnter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.txtEnter.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtEnter.ForeColor = System.Drawing.Color.Black;
            this.txtEnter.Location = new System.Drawing.Point(140, 286);
            this.txtEnter.Name = "txtEnter";
            this.txtEnter.Size = new System.Drawing.Size(182, 47);
            this.txtEnter.TabIndex = 0;
            this.txtEnter.Text = "Войти";
            this.txtEnter.UseVisualStyleBackColor = false;
            this.txtEnter.Click += new System.EventHandler(this.txtEnter_Click);
            // 
            // txtClose
            // 
            this.txtClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.txtClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.txtClose.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtClose.ForeColor = System.Drawing.Color.Black;
            this.txtClose.Location = new System.Drawing.Point(140, 351);
            this.txtClose.Name = "txtClose";
            this.txtClose.Size = new System.Drawing.Size(182, 47);
            this.txtClose.TabIndex = 1;
            this.txtClose.Text = "Выйти";
            this.txtClose.UseVisualStyleBackColor = false;
            this.txtClose.Click += new System.EventHandler(this.txtClose_Click);
            // 
            // txtLogin
            // 
            this.txtLogin.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtLogin.Location = new System.Drawing.Point(140, 88);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(195, 49);
            this.txtLogin.TabIndex = 2;
            this.txtLogin.Leave += new System.EventHandler(this.txtLogin_Leave);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtPassword.Location = new System.Drawing.Point(140, 191);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(195, 49);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(31, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "Логин";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(31, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 31);
            this.label2.TabIndex = 5;
            this.label2.Text = "Пароль";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::TZ.Properties.Resources.eye_open;
            this.pictureBox2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.InitialImage")));
            this.pictureBox2.Location = new System.Drawing.Point(356, 197);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(28, 28);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.picShowPassword_Click);
            this.pictureBox2.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TZ.Properties.Resources.free_icon_gear_1160356;
            this.pictureBox1.Location = new System.Drawing.Point(378, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            // 
            // groupBoxCaptcha
            // 
            this.groupBoxCaptcha.Controls.Add(this.lblBlockTimer);
            this.groupBoxCaptcha.Controls.Add(this.btnRefreshCaptcha);
            this.groupBoxCaptcha.Controls.Add(this.txtCaptcha);
            this.groupBoxCaptcha.Controls.Add(this.lblCaptcha);
            this.groupBoxCaptcha.Controls.Add(this.pictureBoxCaptcha);
            this.groupBoxCaptcha.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxCaptcha.Location = new System.Drawing.Point(458, 12);
            this.groupBoxCaptcha.Name = "groupBoxCaptcha";
            this.groupBoxCaptcha.Size = new System.Drawing.Size(496, 369);
            this.groupBoxCaptcha.TabIndex = 8;
            this.groupBoxCaptcha.TabStop = false;
            this.groupBoxCaptcha.Text = "Проверка CAPTCHA";
            this.groupBoxCaptcha.Visible = false;
            // 
            // pictureBoxCaptcha
            // 
            this.pictureBoxCaptcha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCaptcha.Location = new System.Drawing.Point(75, 30);
            this.pictureBoxCaptcha.Name = "pictureBoxCaptcha";
            this.pictureBoxCaptcha.Size = new System.Drawing.Size(314, 76);
            this.pictureBoxCaptcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCaptcha.TabIndex = 0;
            this.pictureBoxCaptcha.TabStop = false;
            // 
            // lblCaptcha
            // 
            this.lblCaptcha.AutoSize = true;
            this.lblCaptcha.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaptcha.Location = new System.Drawing.Point(25, 133);
            this.lblCaptcha.Name = "lblCaptcha";
            this.lblCaptcha.Size = new System.Drawing.Size(117, 24);
            this.lblCaptcha.TabIndex = 1;
            this.lblCaptcha.Text = "Введите код:";
            // 
            // txtCaptcha
            // 
            this.txtCaptcha.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCaptcha.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtCaptcha.Location = new System.Drawing.Point(173, 133);
            this.txtCaptcha.MaxLength = 4;
            this.txtCaptcha.Name = "txtCaptcha";
            this.txtCaptcha.Size = new System.Drawing.Size(216, 31);
            this.txtCaptcha.TabIndex = 2;
            // 
            // btnRefreshCaptcha
            // 
            this.btnRefreshCaptcha.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRefreshCaptcha.Location = new System.Drawing.Point(415, 133);
            this.btnRefreshCaptcha.Name = "btnRefreshCaptcha";
            this.btnRefreshCaptcha.Size = new System.Drawing.Size(39, 37);
            this.btnRefreshCaptcha.TabIndex = 3;
            this.btnRefreshCaptcha.Text = "⟳";
            this.btnRefreshCaptcha.UseVisualStyleBackColor = true;
            this.btnRefreshCaptcha.Click += new System.EventHandler(this.BtnRefreshCaptcha_Click);
            // 
            // lblBlockTimer
            // 
            this.lblBlockTimer.AutoSize = true;
            this.lblBlockTimer.ForeColor = System.Drawing.Color.Red;
            this.lblBlockTimer.Location = new System.Drawing.Point(35, 193);
            this.lblBlockTimer.Name = "lblBlockTimer";
            this.lblBlockTimer.Size = new System.Drawing.Size(59, 24);
            this.lblBlockTimer.TabIndex = 4;
            this.lblBlockTimer.Text = "label3";
            this.lblBlockTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Avtorizacia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(966, 447);
            this.Controls.Add(this.groupBoxCaptcha);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.txtClose);
            this.Controls.Add(this.txtEnter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Avtorizacia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.groupBoxCaptcha.ResumeLayout(false);
            this.groupBoxCaptcha.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button txtEnter;
        private System.Windows.Forms.Button txtClose;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private System.Windows.Forms.GroupBox groupBoxCaptcha;
        private System.Windows.Forms.PictureBox pictureBoxCaptcha;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnRefreshCaptcha;
        private System.Windows.Forms.TextBox txtCaptcha;
        private System.Windows.Forms.Label lblCaptcha;
        private System.Windows.Forms.Label lblBlockTimer;
    }
}

