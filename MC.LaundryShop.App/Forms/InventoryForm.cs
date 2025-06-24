using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class InventoryForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _formId;
        private readonly long? _userId;

        public InventoryForm(long? formId, long? userId)
        {
            InitializeComponent();
            _formId = formId;
            _userId = userId;   
            if (_formId.HasValue && userId.HasValue)
            {
                label3.Text = @"Update your inventory record here.";
                submit_button.Text = @"Update";
                button2.Visible = true;
                LoadData();
            }
            else
            {
                label3.Text = @"Save your inventory record here.";
                submit_button.Text = @"Save";
            }
        }

        private void LoadData()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT name, is_active FROM inventory WHERE id = @id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return;
                            customTextbox1.Text = reader.GetString(reader.GetOrdinal("name"));
                            checkBox1.Checked = reader.GetBoolean(reader.GetOrdinal("is_active"));
                        }
                    }
                }

                #region Price

                var modifiedDatetimeColumn = new DataGridViewTextBoxColumn();
                modifiedDatetimeColumn.DataPropertyName = "modified_datetime";
                modifiedDatetimeColumn.Name = "modified_datetime";
                modifiedDatetimeColumn.HeaderText = @"Last Modified";
                modifiedDatetimeColumn.Width = 120;
                dataGridView1.Columns.Add(modifiedDatetimeColumn);

                var lastModifiedColumn = new DataGridViewTextBoxColumn();
                lastModifiedColumn.DataPropertyName = "modified_by_name";
                lastModifiedColumn.Name = "modified_by_name";
                lastModifiedColumn.HeaderText = @"Modified By";
                lastModifiedColumn.Width = 120;
                lastModifiedColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns.Add(lastModifiedColumn);

                var fullNameColumn = new DataGridViewTextBoxColumn();
                fullNameColumn.DataPropertyName = "price";
                fullNameColumn.Name = "price";
                fullNameColumn.HeaderText = @"Price";
                fullNameColumn.Width = 120;
                dataGridView1.Columns.Add(fullNameColumn);

                LoadPrices();

                #endregion

                #region History

                

                var referenceIdColumn = new DataGridViewTextBoxColumn();
                referenceIdColumn.DataPropertyName = "reference_id";
                referenceIdColumn.Name = "reference_id";
                referenceIdColumn.HeaderText = @"Reference";
                referenceIdColumn.Width = 120;
                referenceIdColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Columns.Add(referenceIdColumn);

                var modifiedDatetimeColumn1 = new DataGridViewTextBoxColumn();
                modifiedDatetimeColumn1.DataPropertyName = "modified_datetime";
                modifiedDatetimeColumn1.Name = "modified_datetime1";
                modifiedDatetimeColumn1.HeaderText = @"Last Modified";
                modifiedDatetimeColumn1.Width = 120;
                dataGridView2.Columns.Add(modifiedDatetimeColumn1);

                var amountColumn = new DataGridViewTextBoxColumn();
                amountColumn.DataPropertyName = "amount";
                amountColumn.Name = "amount";
                amountColumn.HeaderText = @"Amount";
                amountColumn.Width = 120;
                dataGridView2.Columns.Add(amountColumn);

                var isSalesColumn = new DataGridViewCheckBoxColumn();
                isSalesColumn.DataPropertyName = "is_sales";
                isSalesColumn.Name = "is_sales";
                isSalesColumn.Visible = false;
                dataGridView2.Columns.Add(isSalesColumn);

                LoadHistory();

                #endregion

            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadPrices()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT i.modified_datetime, e.full_name AS modified_by_name, i.price FROM inventory_prices AS i LEFT JOIN employees AS e ON i.modified_by = e.id WHERE i.inventory_id = @id ORDER BY i.modified_datetime DESC;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView1.DataSource = dataTable;
                            if (dataTable.Rows.Count > 0)
                            {
                                var currentPrice = Convert.ToDecimal(dataTable.Rows[0]["price"]);
                                label4.Text = $@"Current Price: PHP {currentPrice:N2}";
                            }
                            else
                            {
                                label4.Text = @"Current Price: N/A";
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
        private void LoadHistory()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT i.reference_id, i.modified_datetime, i.amount, i.is_sales FROM inventory_transactions AS i WHERE i.inventory_id = @id ORDER BY i.modified_datetime DESC;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView2.DataSource = dataTable;
                            decimal currentStock = 0;
                            if (dataTable.Rows.Count > 0)
                            {
                                currentStock = dataTable.AsEnumerable().Sum(row => row.Field<decimal>("amount"));
                            }
                            label5.Text = $@"Current Stock: {currentStock:N2}";
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SaveData()
        {
            var result = false;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var name = customTextbox1.Text.Trim();

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM inventory WHERE name = @name AND is_deleted = FALSE;";
                    using (var checkCommand = new MySqlCommand(checkDuplicateQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@name", name);
                        var existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            MessageBox.Show(Resources.Messages_Success_Duplicate, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    const string insertQuery = "INSERT INTO inventory (name, description, is_active, created_by, created_datetime, modified_by, modified_datetime) " +
                                              "VALUES (@name, @description, @is_active, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        var description = richTextBox1.Text.Trim();

                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        command.Parameters.AddWithValue("@is_active", checkBox1.Checked);
                        command.Parameters.AddWithValue("@user_id", _userId);
                        command.Parameters.AddWithValue("@current_date", currentDateTime);
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }
        private bool UpdateData()
        {
            var result = false;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var name = customTextbox1.Text.Trim();

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM inventory WHERE name = @name AND is_deleted = FALSE AND Id != @id ;";
                    using (var checkCommand = new MySqlCommand(checkDuplicateQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@name", name);
                        checkCommand.Parameters.AddWithValue("@id", _formId);
                        var existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            MessageBox.Show(Resources.Messages_Success_Duplicate, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    const string query = "UPDATE inventory SET " +
                                         "name = @name, " +
                                         "description = @description, " +
                                         "is_active = @is_active, " +
                                         "modified_by = @modified_by, " +
                                         "modified_datetime = @modified_datetime " +
                                         "WHERE id = @id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        var description = richTextBox1.Text.Trim();

                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        command.Parameters.AddWithValue("@is_active", checkBox1.Checked);
                        command.Parameters.AddWithValue("@modified_by", _userId);
                        command.Parameters.AddWithValue("@modified_datetime", currentDateTime);
                        command.Parameters.AddWithValue("@id", _formId);
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(customTextbox1.Text))
            {
                MessageBox.Show(@"Please complete required details marked with red *.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_formId.HasValue && _userId.HasValue)
            {
                if (!UpdateData())
                {
                    return;
                }
            }
            else
            {
                if (!SaveData())
                {
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            using (var amountForm = new AmountForm())
            {
                var result = amountForm.ShowDialog();
                if (result != DialogResult.OK) return;
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var receivedAmount = amountForm.EnteredAmount;
                    const string insertQuery = "INSERT INTO inventory_prices (inventory_id, price, created_by, created_datetime, modified_by, modified_datetime) " +
                                               "VALUES (@inventory_id, @price, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        command.Parameters.AddWithValue("@inventory_id", _formId);
                        command.Parameters.AddWithValue("@price", receivedAmount);
                        command.Parameters.AddWithValue("@user_id", _userId);
                        command.Parameters.AddWithValue("@current_date", currentDateTime);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPrices();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name != "price") return;
            if (e.Value != null && e.Value != DBNull.Value && e.Value is decimal grandTotal)
            {
                e.Value = $@"PHP {grandTotal:N2}";
                e.FormattingApplied = true;
            }
            else if (e.Value == DBNull.Value)
            {
                e.Value = "PHP 0.00";
                e.FormattingApplied = true;
            }
        }
        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = dataGridView2.Rows[e.RowIndex];

            if (dataGridView2.Columns[e.ColumnIndex].Name == "amount")
            {
                decimal grandTotal = 0;

                if (e.Value != null && e.Value != DBNull.Value && e.Value is decimal tempGrandTotal)
                {
                    grandTotal = tempGrandTotal;
                    e.Value = $@"{grandTotal:N2}";
                    e.FormattingApplied = true;
                }
                else if (e.Value == DBNull.Value)
                {
                    e.Value = "0.00";
                    e.FormattingApplied = true;
                }

                if (grandTotal < 0)
                {
                    row.Cells["amount"].Style.ForeColor = Color.Red;
                }
                else if (grandTotal > 0)
                {
                    row.Cells["amount"].Style.ForeColor = Color.Green;
                }
                else
                {
                    row.Cells["amount"].Style.ForeColor = Color.Black;
                }
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name != "reference_id") return;
            if (row.Cells["is_sales"].Value == null || row.Cells["is_sales"].Value == DBNull.Value) return;
            var isSales = Convert.ToBoolean(row.Cells["is_sales"].Value);
            var referenceId = e.Value?.ToString();
            if (string.IsNullOrEmpty(referenceId)) return;
            e.Value = isSales ? $"Sales - {referenceId}" : $"Purchase - {referenceId}";
            e.FormattingApplied = true;
        }
    }
}