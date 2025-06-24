namespace MC.LaundryShop.App.Forms
{
    partial class ChangePasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
            this.customTextbox1 = new MC.LaundryShop.App.Helper.UserControls.CustomTextbox();
            this.customTextbox5 = new MC.LaundryShop.App.Helper.UserControls.CustomTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.submit_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.customTextbox2 = new MC.LaundryShop.App.Helper.UserControls.CustomTextbox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // customTextbox1
            // 
            this.customTextbox1.BackColor = System.Drawing.Color.Transparent;
            this.customTextbox1.LabelText = "Current Password";
            this.customTextbox1.Location = new System.Drawing.Point(29, 124);
            this.customTextbox1.MaximumSize = new System.Drawing.Size(1920, 64);
            this.customTextbox1.MinimumSize = new System.Drawing.Size(298, 64);
            this.customTextbox1.Name = "customTextbox1";
            this.customTextbox1.PasswordChar = '•';
            this.customTextbox1.PlaceholderText = "Enter password";
            this.customTextbox1.Size = new System.Drawing.Size(298, 64);
            this.customTextbox1.TabIndex = 1;
            // 
            // customTextbox5
            // 
            this.customTextbox5.BackColor = System.Drawing.Color.Transparent;
            this.customTextbox5.LabelText = "New Password";
            this.customTextbox5.Location = new System.Drawing.Point(29, 199);
            this.customTextbox5.MaximumSize = new System.Drawing.Size(1920, 64);
            this.customTextbox5.MinimumSize = new System.Drawing.Size(298, 64);
            this.customTextbox5.Name = "customTextbox5";
            this.customTextbox5.PasswordChar = '•';
            this.customTextbox5.PlaceholderText = "Enter new password";
            this.customTextbox5.Size = new System.Drawing.Size(298, 64);
            this.customTextbox5.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Poppins SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(26, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 51);
            this.label2.TabIndex = 7;
            this.label2.Text = "Change Password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Poppins", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(29, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(298, 27);
            this.label3.TabIndex = 8;
            this.label3.Text = "Update your password here.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // submit_button
            // 
            this.submit_button.BackColor = System.Drawing.Color.DodgerBlue;
            this.submit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submit_button.Font = new System.Drawing.Font("Poppins SemiBold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.submit_button.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.submit_button.Location = new System.Drawing.Point(33, 433);
            this.submit_button.Name = "submit_button";
            this.submit_button.Size = new System.Drawing.Size(294, 39);
            this.submit_button.TabIndex = 0;
            this.submit_button.Text = "Save";
            this.submit_button.UseVisualStyleBackColor = false;
            this.submit_button.Click += new System.EventHandler(this.Submit_button_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Poppins SemiBold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.Location = new System.Drawing.Point(33, 388);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(294, 39);
            this.button1.TabIndex = 8;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Location = new System.Drawing.Point(34, 371);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(293, 1);
            this.panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(176, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 25);
            this.label1.TabIndex = 18;
            this.label1.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(215, 274);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 25);
            this.label4.TabIndex = 19;
            this.label4.Text = "*";
            // 
            // customTextbox2
            // 
            this.customTextbox2.BackColor = System.Drawing.Color.Transparent;
            this.customTextbox2.LabelText = "Confirm New Password";
            this.customTextbox2.Location = new System.Drawing.Point(29, 274);
            this.customTextbox2.MaximumSize = new System.Drawing.Size(1920, 64);
            this.customTextbox2.MinimumSize = new System.Drawing.Size(298, 64);
            this.customTextbox2.Name = "customTextbox2";
            this.customTextbox2.PasswordChar = '•';
            this.customTextbox2.PlaceholderText = "Enter new password";
            this.customTextbox2.Size = new System.Drawing.Size(298, 64);
            this.customTextbox2.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(147, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 25);
            this.label5.TabIndex = 20;
            this.label5.Text = "*";
            // 
            // ChangePasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(362, 504);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.submit_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.customTextbox5);
            this.Controls.Add(this.customTextbox2);
            this.Controls.Add(this.customTextbox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Helper.UserControls.CustomTextbox customTextbox1;
        private Helper.UserControls.CustomTextbox customTextbox5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button submit_button;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private Helper.UserControls.CustomTextbox customTextbox2;
        private System.Windows.Forms.Label label5;
    }
}

