using MC.LaundryShop.App.Properties;
using System;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class AmountForm : Form
    {
        public decimal EnteredAmount { get; private set; }

        public AmountForm()
        {
            InitializeComponent();
        }

        private void Submit_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(customTextbox1.Text))
            {
                MessageBox.Show(@"Please complete required details marked with red *.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var value = Convert.ToDecimal(customTextbox1.Text);
                EnteredAmount = value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show(@"Please enter a valid numeric amount.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (OverflowException)
            {
                MessageBox.Show(@"The entered amount is too large or too small.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}