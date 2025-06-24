namespace MC.LaundryShop.App.Forms
{
    partial class AmountForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmountForm));
            this.customTextbox1 = new MC.LaundryShop.App.Helper.UserControls.CustomTextbox();
            this.submit_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // customTextbox1
            // 
            this.customTextbox1.BackColor = System.Drawing.Color.Transparent;
            this.customTextbox1.LabelText = "Amount";
            this.customTextbox1.Location = new System.Drawing.Point(29, 25);
            this.customTextbox1.MaximumSize = new System.Drawing.Size(1920, 64);
            this.customTextbox1.MinimumSize = new System.Drawing.Size(298, 64);
            this.customTextbox1.Name = "customTextbox1";
            this.customTextbox1.PasswordChar = '\0';
            this.customTextbox1.PlaceholderText = "Enter amount";
            this.customTextbox1.Size = new System.Drawing.Size(298, 64);
            this.customTextbox1.TabIndex = 1;
            this.customTextbox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CustomTextbox1_KeyPress);
            // 
            // submit_button
            // 
            this.submit_button.BackColor = System.Drawing.Color.DodgerBlue;
            this.submit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submit_button.Font = new System.Drawing.Font("Poppins SemiBold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.submit_button.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.submit_button.Location = new System.Drawing.Point(223, 106);
            this.submit_button.Name = "submit_button";
            this.submit_button.Size = new System.Drawing.Size(104, 39);
            this.submit_button.TabIndex = 0;
            this.submit_button.Text = "OK";
            this.submit_button.UseVisualStyleBackColor = false;
            this.submit_button.Click += new System.EventHandler(this.Submit_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(102, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 25);
            this.label1.TabIndex = 18;
            this.label1.Text = "*";
            // 
            // AmountForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(362, 165);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.submit_button);
            this.Controls.Add(this.customTextbox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AmountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Helper.UserControls.CustomTextbox customTextbox1;
        private System.Windows.Forms.Button submit_button;
        private System.Windows.Forms.Label label1;
    }
}

