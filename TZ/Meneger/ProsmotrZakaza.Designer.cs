namespace TZ
{
    partial class ProsmotrZakaza
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProsmotrZakaza));
            this.lblSum = new System.Windows.Forms.Label();
            this.lblDiscountSum = new System.Windows.Forms.Label();
            this.chkDiscount = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dgvBasketPreview = new System.Windows.Forms.DataGridView();
            this.contextMenuBasket = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuRemoveService = new System.Windows.Forms.ToolStripMenuItem();
            this.lblClient = new System.Windows.Forms.Label();
            this.lblAdmission = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBasketPreview)).BeginInit();
            this.contextMenuBasket.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSum
            // 
            this.lblSum.AutoSize = true;
            this.lblSum.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSum.Location = new System.Drawing.Point(14, 67);
            this.lblSum.Name = "lblSum";
            this.lblSum.Size = new System.Drawing.Size(83, 26);
            this.lblSum.TabIndex = 88;
            this.lblSum.Text = "Сумма:";
            // 
            // lblDiscountSum
            // 
            this.lblDiscountSum.AutoSize = true;
            this.lblDiscountSum.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDiscountSum.Location = new System.Drawing.Point(14, 127);
            this.lblDiscountSum.Name = "lblDiscountSum";
            this.lblDiscountSum.Size = new System.Drawing.Size(196, 26);
            this.lblDiscountSum.TabIndex = 89;
            this.lblDiscountSum.Text = "Сумма со скидкой:";
            // 
            // chkDiscount
            // 
            this.chkDiscount.AutoSize = true;
            this.chkDiscount.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkDiscount.Location = new System.Drawing.Point(18, 18);
            this.chkDiscount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkDiscount.Name = "chkDiscount";
            this.chkDiscount.Size = new System.Drawing.Size(199, 30);
            this.chkDiscount.TabIndex = 90;
            this.chkDiscount.Text = "10% - >1000 руб.";
            this.chkDiscount.UseVisualStyleBackColor = true;
            this.chkDiscount.CheckedChanged += new System.EventHandler(this.chkDiscount_CheckedChanged);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(12, 326);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(272, 50);
            this.button4.TabIndex = 91;
            this.button4.Text = "Сформировать заказ";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.btnConfirmOrder_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(12, 436);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(181, 50);
            this.button1.TabIndex = 94;
            this.button1.Text = "Меню";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(9, 270);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(275, 50);
            this.button3.TabIndex = 93;
            this.button3.Text = "К оформлению заказа";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.btnBackToOrder_Click);
            // 
            // dgvBasketPreview
            // 
            this.dgvBasketPreview.AllowUserToAddRows = false;
            this.dgvBasketPreview.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvBasketPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBasketPreview.ContextMenuStrip = this.contextMenuBasket;
            this.dgvBasketPreview.Location = new System.Drawing.Point(517, 3);
            this.dgvBasketPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvBasketPreview.Name = "dgvBasketPreview";
            this.dgvBasketPreview.RowHeadersWidth = 51;
            this.dgvBasketPreview.RowTemplate.Height = 24;
            this.dgvBasketPreview.Size = new System.Drawing.Size(659, 485);
            this.dgvBasketPreview.TabIndex = 95;
            // 
            // contextMenuBasket
            // 
            this.contextMenuBasket.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuBasket.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRemoveService});
            this.contextMenuBasket.Name = "contextMenuBasket";
            this.contextMenuBasket.Size = new System.Drawing.Size(181, 28);
            this.contextMenuBasket.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuBasket_Opening);
            // 
            // mnuRemoveService
            // 
            this.mnuRemoveService.Name = "mnuRemoveService";
            this.mnuRemoveService.Size = new System.Drawing.Size(180, 24);
            this.mnuRemoveService.Text = "Удалить услугу";
            this.mnuRemoveService.Click += new System.EventHandler(this.mnuRemoveService_Click);
            // 
            // lblClient
            // 
            this.lblClient.AutoSize = true;
            this.lblClient.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblClient.Location = new System.Drawing.Point(14, 182);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(94, 26);
            this.lblClient.TabIndex = 96;
            this.lblClient.Text = "Клиент: ";
            // 
            // lblAdmission
            // 
            this.lblAdmission.AutoSize = true;
            this.lblAdmission.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAdmission.Location = new System.Drawing.Point(17, 226);
            this.lblAdmission.Name = "lblAdmission";
            this.lblAdmission.Size = new System.Drawing.Size(146, 26);
            this.lblAdmission.TabIndex = 97;
            this.lblAdmission.Text = "Дата приема: ";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(12, 381);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(181, 50);
            this.button2.TabIndex = 92;
            this.button2.Text = "Чек";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnPrintCheck_Click);
            // 
            // ProsmotrZakaza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1180, 493);
            this.Controls.Add(this.lblAdmission);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.dgvBasketPreview);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.chkDiscount);
            this.Controls.Add(this.lblDiscountSum);
            this.Controls.Add(this.lblSum);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProsmotrZakaza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Просмотр заказа";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBasketPreview)).EndInit();
            this.contextMenuBasket.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblSum;
        private System.Windows.Forms.Label lblDiscountSum;
        private System.Windows.Forms.CheckBox chkDiscount;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dgvBasketPreview;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.Label lblAdmission;
        private System.Windows.Forms.ContextMenuStrip contextMenuBasket;
        private System.Windows.Forms.ToolStripMenuItem mnuRemoveService;
        private System.Windows.Forms.Button button2;
    }
}