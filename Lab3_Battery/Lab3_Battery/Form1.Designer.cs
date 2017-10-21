namespace Lab3_Battery
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectType = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AvailableTime = new System.Windows.Forms.Label();
            this.AvailablePower = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Тип подключения:";
            // 
            // ConnectType
            // 
            this.ConnectType.AutoSize = true;
            this.ConnectType.Location = new System.Drawing.Point(126, 13);
            this.ConnectType.Name = "ConnectType";
            this.ConnectType.Size = new System.Drawing.Size(0, 13);
            this.ConnectType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Заряд батареи:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Оставшееся время:";
            // 
            // AvailableTime
            // 
            this.AvailableTime.AutoSize = true;
            this.AvailableTime.Location = new System.Drawing.Point(126, 64);
            this.AvailableTime.Name = "AvailableTime";
            this.AvailableTime.Size = new System.Drawing.Size(0, 13);
            this.AvailableTime.TabIndex = 4;
            // 
            // AvailablePower
            // 
            this.AvailablePower.AutoSize = true;
            this.AvailablePower.Location = new System.Drawing.Point(126, 38);
            this.AvailablePower.Name = "AvailablePower";
            this.AvailablePower.Size = new System.Drawing.Size(0, 13);
            this.AvailablePower.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 106);
            this.Controls.Add(this.AvailablePower);
            this.Controls.Add(this.AvailableTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConnectType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "PowerManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ConnectType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label AvailableTime;
        private System.Windows.Forms.Label AvailablePower;
    }
}

