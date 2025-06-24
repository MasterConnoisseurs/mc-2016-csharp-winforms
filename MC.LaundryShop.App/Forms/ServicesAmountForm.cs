using MC.LaundryShop.App.Class;
using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class ServicesAmountForm : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        public ItemValue Item { get; private set; }

        public ServicesAmountForm()
        {
            InitializeComponent();
            LoadDropDownItems();
        }

        private void LoadDropDownItems()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = @"
                        SELECT
                            i.Id AS service_id,
                            i.name AS service_name,
                            ip.Id AS price_id,
                            ip.price AS price_price
                        FROM
                            services AS i
                        INNER JOIN 
                            services_prices AS ip ON i.Id = ip.service_id
                        WHERE
                            i.is_deleted = FALSE
                            AND i.is_active = TRUE
                            AND ip.Id = (SELECT ip2.Id 
                                         FROM services_prices AS ip2
                                         WHERE ip2.service_id = i.Id
                                         ORDER BY ip2.modified_datetime DESC
                                         LIMIT 1)
                        ORDER BY
                            i.name ASC;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            comboBox1.DisplayMember = "ItemName";
                            while (reader.Read())
                            {
                                var item = new ItemValue()
                                {
                                    ItemId = reader.GetInt32("service_id"),
                                    ItemName = reader.GetString("service_name"),
                                    ItemPriceId = reader.GetInt64("price_id"),
                                    ItemPrice = reader.GetDecimal("price_price"),
                                };
                                comboBox1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateTotalLabel()
        {
            if (comboBox1.SelectedItem is ItemValue selectedItem &&
                decimal.TryParse(customTextbox1.Text, out var quantity))
            {
                var total = quantity * selectedItem.ItemPrice;
                label2.Text = $@"Total PHP {total:N2}";
            }
            else
            {
                label2.Text = @"Total PHP 0.00";
            }
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(customTextbox1.Text) || comboBox1.Text == "")
            {
                MessageBox.Show(@"Please complete required details marked with red *.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateTotalLabel();
                if (comboBox1.SelectedItem is ItemValue selectedItem)
                {
                    selectedItem.Amount = Convert.ToDecimal(customTextbox1.Text);
                    selectedItem.TotalCost = Convert.ToDecimal(label2.Text.Replace(",", "").Replace("Total PHP ", ""));
                    Item = selectedItem;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(@"Invalid service item selected.",
                        Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(@"Please enter a valid numeric amount.",
                    Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (OverflowException)
            {
                MessageBox.Show(@"The entered amount is too large or too small.",
                    Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.Messages_Error_Fatal,
                    Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void customTextbox1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void customTextbox1_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateTotalLabel();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTotalLabel();
        }
    }
}