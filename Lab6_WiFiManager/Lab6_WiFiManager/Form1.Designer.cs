namespace Lab6_WiFiManager
{
    partial class Form1
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
            this.Description = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectToNet = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.Label();
            this.ListOfNetworks = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Description
            // 
            this.Description.Location = new System.Drawing.Point(13, 252);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(259, 130);
            this.Description.TabIndex = 1;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(72, 388);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(200, 20);
            this.Password.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 394);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Password";
            // 
            // ConnectToNet
            // 
            this.ConnectToNet.Location = new System.Drawing.Point(105, 414);
            this.ConnectToNet.Name = "ConnectToNet";
            this.ConnectToNet.Size = new System.Drawing.Size(75, 23);
            this.ConnectToNet.TabIndex = 4;
            this.ConnectToNet.Text = "Connect";
            this.ConnectToNet.UseVisualStyleBackColor = true;
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(13, 414);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(0, 13);
            this.Status.TabIndex = 5;
            // 
            // ListOfNetworks
            // 
            this.ListOfNetworks.Location = new System.Drawing.Point(13, 17);
            this.ListOfNetworks.Name = "ListOfNetworks";
            this.ListOfNetworks.Size = new System.Drawing.Size(259, 202);
            this.ListOfNetworks.TabIndex = 6;
            this.ListOfNetworks.UseCompatibleStateImageBehavior = false;
            this.ListOfNetworks.View = System.Windows.Forms.View.Tile;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Description";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 444);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ListOfNetworks);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.ConnectToNet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.Description);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "WiFi Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox Description;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ConnectToNet;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.ListView ListOfNetworks;
        private System.Windows.Forms.Label label2;
    }
}

