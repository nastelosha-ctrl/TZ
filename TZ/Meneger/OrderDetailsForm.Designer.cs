namespace TZ
{
    partial class OrderDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderDetailsForm));
            this.lblOrderNumber = new System.Windows.Forms.Label();
            this.lblOrderNumberValue = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblDueDateValue = new System.Windows.Forms.Label();
            this.lblDueDate = new System.Windows.Forms.Label();
            this.lblOrderDateValue = new System.Windows.Forms.Label();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClientDetails = new System.Windows.Forms.Button();
            this.lblPhoneValue = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblMiddleNameValue = new System.Windows.Forms.Label();
            this.lblMiddleName = new System.Windows.Forms.Label();
            this.lblFirstNameValue = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblLastNameValue = new System.Windows.Forms.Label();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.SuspendLayout();
            // 
            // lblOrderNumber
            // 
            this.lblOrderNumber.AutoSize = true;
            this.lblOrderNumber.Location = new System.Drawing.Point(6, 36);
            this.lblOrderNumber.Name = "lblOrderNumber";
            this.lblOrderNumber.Size = new System.Drawing.Size(123, 24);
            this.lblOrderNumber.TabIndex = 0;
            this.lblOrderNumber.Text = "Номер заказа:";
            // 
            // lblOrderNumberValue
            // 
            this.lblOrderNumberValue.AutoSize = true;
            this.lblOrderNumberValue.Location = new System.Drawing.Point(171, 36);
            this.lblOrderNumberValue.Name = "lblOrderNumberValue";
            this.lblOrderNumberValue.Size = new System.Drawing.Size(123, 24);
            this.lblOrderNumberValue.TabIndex = 1;
            this.lblOrderNumberValue.Text = "Номер заказа:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblDueDateValue);
            this.groupBox1.Controls.Add(this.lblDueDate);
            this.groupBox1.Controls.Add(this.lblOrderDateValue);
            this.groupBox1.Controls.Add(this.lblOrderDate);
            this.groupBox1.Controls.Add(this.lblOrderNumber);
            this.groupBox1.Controls.Add(this.lblOrderNumberValue);
            this.groupBox1.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 286);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Информация о заказе";
            // 
            // lblDueDateValue
            // 
            this.lblDueDateValue.AutoSize = true;
            this.lblDueDateValue.Location = new System.Drawing.Point(186, 143);
            this.lblDueDateValue.Name = "lblDueDateValue";
            this.lblDueDateValue.Size = new System.Drawing.Size(108, 24);
            this.lblDueDateValue.TabIndex = 5;
            this.lblDueDateValue.Text = "Дата заказа:";
            // 
            // lblDueDate
            // 
            this.lblDueDate.AutoSize = true;
            this.lblDueDate.Location = new System.Drawing.Point(6, 143);
            this.lblDueDate.Name = "lblDueDate";
            this.lblDueDate.Size = new System.Drawing.Size(161, 24);
            this.lblDueDate.TabIndex = 4;
            this.lblDueDate.Text = "Срок выполнения:";
            // 
            // lblOrderDateValue
            // 
            this.lblOrderDateValue.AutoSize = true;
            this.lblOrderDateValue.Location = new System.Drawing.Point(186, 93);
            this.lblOrderDateValue.Name = "lblOrderDateValue";
            this.lblOrderDateValue.Size = new System.Drawing.Size(108, 24);
            this.lblOrderDateValue.TabIndex = 3;
            this.lblOrderDateValue.Text = "Дата заказа:";
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(6, 93);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(108, 24);
            this.lblOrderDate.TabIndex = 2;
            this.lblOrderDate.Text = "Дата заказа:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnClientDetails);
            this.groupBox2.Controls.Add(this.lblPhoneValue);
            this.groupBox2.Controls.Add(this.lblPhone);
            this.groupBox2.Controls.Add(this.lblMiddleNameValue);
            this.groupBox2.Controls.Add(this.lblMiddleName);
            this.groupBox2.Controls.Add(this.lblFirstNameValue);
            this.groupBox2.Controls.Add(this.lblFirstName);
            this.groupBox2.Controls.Add(this.lblLastName);
            this.groupBox2.Controls.Add(this.lblLastNameValue);
            this.groupBox2.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(12, 317);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(519, 286);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Информация о клиенте";
            // 
            // btnClientDetails
            // 
            this.btnClientDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnClientDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClientDetails.Location = new System.Drawing.Point(6, 238);
            this.btnClientDetails.Name = "btnClientDetails";
            this.btnClientDetails.Size = new System.Drawing.Size(161, 33);
            this.btnClientDetails.TabIndex = 8;
            this.btnClientDetails.Text = "Детали клиента";
            this.btnClientDetails.UseVisualStyleBackColor = false;
            this.btnClientDetails.Click += new System.EventHandler(this.btnClientDetails_Click);
            // 
            // lblPhoneValue
            // 
            this.lblPhoneValue.AutoSize = true;
            this.lblPhoneValue.Location = new System.Drawing.Point(186, 191);
            this.lblPhoneValue.Name = "lblPhoneValue";
            this.lblPhoneValue.Size = new System.Drawing.Size(92, 24);
            this.lblPhoneValue.TabIndex = 7;
            this.lblPhoneValue.Text = "Отчество:";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(6, 191);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(92, 24);
            this.lblPhone.TabIndex = 6;
            this.lblPhone.Text = "Телефон:";
            // 
            // lblMiddleNameValue
            // 
            this.lblMiddleNameValue.AutoSize = true;
            this.lblMiddleNameValue.Location = new System.Drawing.Point(186, 143);
            this.lblMiddleNameValue.Name = "lblMiddleNameValue";
            this.lblMiddleNameValue.Size = new System.Drawing.Size(108, 24);
            this.lblMiddleNameValue.TabIndex = 5;
            this.lblMiddleNameValue.Text = "Дата заказа:";
            // 
            // lblMiddleName
            // 
            this.lblMiddleName.AutoSize = true;
            this.lblMiddleName.Location = new System.Drawing.Point(6, 143);
            this.lblMiddleName.Name = "lblMiddleName";
            this.lblMiddleName.Size = new System.Drawing.Size(92, 24);
            this.lblMiddleName.TabIndex = 4;
            this.lblMiddleName.Text = "Отчество:";
            // 
            // lblFirstNameValue
            // 
            this.lblFirstNameValue.AutoSize = true;
            this.lblFirstNameValue.Location = new System.Drawing.Point(186, 93);
            this.lblFirstNameValue.Name = "lblFirstNameValue";
            this.lblFirstNameValue.Size = new System.Drawing.Size(108, 24);
            this.lblFirstNameValue.TabIndex = 3;
            this.lblFirstNameValue.Text = "Дата заказа:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(6, 93);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(49, 24);
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "Имя:";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(6, 36);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(89, 24);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Фамилия:";
            // 
            // lblLastNameValue
            // 
            this.lblLastNameValue.AutoSize = true;
            this.lblLastNameValue.Location = new System.Drawing.Point(186, 36);
            this.lblLastNameValue.Name = "lblLastNameValue";
            this.lblLastNameValue.Size = new System.Drawing.Size(123, 24);
            this.lblLastNameValue.TabIndex = 1;
            this.lblLastNameValue.Text = "Номер заказа:";
            // 
            // dgvServices
            // 
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new System.Drawing.Point(346, 2);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.RowHeadersWidth = 51;
            this.dgvServices.RowTemplate.Height = 24;
            this.dgvServices.Size = new System.Drawing.Size(761, 317);
            this.dgvServices.TabIndex = 4;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTotal.Location = new System.Drawing.Point(840, 334);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(78, 24);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "ИТОГО:";
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTotalValue.Location = new System.Drawing.Point(939, 334);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(78, 24);
            this.lblTotalValue.TabIndex = 6;
            this.lblTotalValue.Text = "ИТОГО:";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Comic Sans MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClose.Location = new System.Drawing.Point(906, 406);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(201, 33);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // OrderDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1111, 611);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTotalValue);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvServices);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Информация о заказе";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOrderNumber;
        private System.Windows.Forms.Label lblOrderNumberValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblDueDateValue;
        private System.Windows.Forms.Label lblDueDate;
        private System.Windows.Forms.Label lblOrderDateValue;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClientDetails;
        private System.Windows.Forms.Label lblPhoneValue;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.Label lblMiddleNameValue;
        private System.Windows.Forms.Label lblMiddleName;
        private System.Windows.Forms.Label lblFirstNameValue;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblLastNameValue;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Button btnClose;
    }
}