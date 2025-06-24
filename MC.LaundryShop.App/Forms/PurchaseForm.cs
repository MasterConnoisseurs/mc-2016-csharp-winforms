using MC.LaundryShop.App.Class;
using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class PurchaseForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _formId;
        private readonly long? _userId;
        private readonly List<TransactionDeleted> _deletedInventoryPurchaseIds = new List<TransactionDeleted>();

        public PurchaseForm(long? formId, long? userId)
        {
            InitializeComponent();
            _formId = formId;
            _userId = userId;
            InitializeForm();
            if (_formId.HasValue && userId.HasValue)
            {
                label3.Text = @"Update your purchase record here.";
                submit_button.Text = @"Update";
                LoadData();
            }
            else
            {
                label3.Text = @"Save your purchase record here.";
                submit_button.Text = @"Save";
            }
        }

        private void InitializeForm()
        {
            //Load Dropdown
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT id, name FROM suppliers WHERE is_deleted = FALSE AND is_active = TRUE;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            comboBox1.DisplayMember = "Name";
                            while (reader.Read())
                            {
                                var loadedSupplierItem = new ItemDisplay()
                                {
                                    Id = reader.GetInt32("id"),
                                    Name = reader.GetString("name"),
                                };
                                comboBox1.Items.Add(loadedSupplierItem);
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Load Columns
            #region Inventory

            var idColumn = new DataGridViewTextBoxColumn();
            idColumn.DataPropertyName = "id";
            idColumn.Name = "id";
            idColumn.Visible = false;
            dataGridView1.Columns.Add(idColumn);

            var itemIdColumn = new DataGridViewTextBoxColumn();
            itemIdColumn.DataPropertyName = "item_id";
            itemIdColumn.Name = "item_id";
            itemIdColumn.Visible = false;
            dataGridView1.Columns.Add(itemIdColumn);

            var itemNameColumn = new DataGridViewTextBoxColumn();
            itemNameColumn.DataPropertyName = "item_name";
            itemNameColumn.Name = "item_name";
            itemNameColumn.HeaderText = @"Inventory";
            itemNameColumn.Width = 200;
            itemNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(itemNameColumn);

            var itemPriceIdColumn = new DataGridViewTextBoxColumn();
            itemPriceIdColumn.DataPropertyName = "item_price_id";
            itemPriceIdColumn.Name = "item_price_id";
            itemPriceIdColumn.Visible = false;
            dataGridView1.Columns.Add(itemPriceIdColumn);

            var itemPriceColumn = new DataGridViewTextBoxColumn();
            itemPriceColumn.DataPropertyName = "item_price";
            itemPriceColumn.Name = "item_price";
            itemPriceColumn.HeaderText = @"Price";
            itemPriceColumn.Width = 120;
            dataGridView1.Columns.Add(itemPriceColumn);

            var amountColumn = new DataGridViewTextBoxColumn();
            amountColumn.DataPropertyName = "amount";
            amountColumn.Name = "amount";
            amountColumn.HeaderText = @"Amount";
            amountColumn.Width = 120;
            dataGridView1.Columns.Add(amountColumn);

            var totalCostColumn = new DataGridViewTextBoxColumn();
            totalCostColumn.DataPropertyName = "total_cost";
            totalCostColumn.Name = "total_cost";
            totalCostColumn.HeaderText = @"Total Cost";
            totalCostColumn.Width = 120;
            dataGridView1.Columns.Add(totalCostColumn);

            var deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "DeleteColumn";
            deleteButtonColumn.HeaderText = @"Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            deleteButtonColumn.Width = 90;
            dataGridView1.Columns.Add(deleteButtonColumn);

            #endregion
        }
        private void UpdateTotalLabel()
        {
            decimal inventoryGrandTotal = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells["total_cost"].Value != null && decimal.TryParse(row.Cells["total_cost"].Value.ToString(), out var totalCost))
                {
                    inventoryGrandTotal += totalCost;
                }
            }
            customTextbox1.Text = inventoryGrandTotal.ToString(CultureInfo.InvariantCulture);

            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
            var total = inventoryGrandTotal + otherCost + tax - discount;
            customTextbox4.Text = $@"PHP {total:N2}";
        }
        private void customTextbox_KeyPress(object sender, KeyPressEventArgs e)
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
        private void customTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateTotalLabel();
        }

        private void LoadData()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT s.supplier_id, c.name as supplier_name, s.notes, s.other_cost, s.tax, s.discount FROM purchase AS s LEFT JOIN suppliers AS c ON c.id = s.supplier_id WHERE s.id = @id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return;
                            richTextBox1.Text = reader.IsDBNull(reader.GetOrdinal("notes"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("notes"));
                            customTextbox3.Text = reader.GetString(reader.GetOrdinal("other_cost"));
                            customTextbox6.Text = reader.GetString(reader.GetOrdinal("tax"));
                            customTextbox5.Text = reader.GetString(reader.GetOrdinal("discount"));
                            var loadedSupplierItem = new ItemDisplay()
                            {
                                Id = reader.GetInt64("supplier_id"),
                                Name = reader.GetString("supplier_name"),
                            };
                            var existingItem = comboBox1.Items.Cast<ItemDisplay>()
                                .FirstOrDefault(item => item.Id == loadedSupplierItem.Id);
                            if (existingItem == null)
                            {
                                comboBox1.Items.Add(loadedSupplierItem);
                                comboBox1.SelectedItem = loadedSupplierItem;
                            }
                            else
                            {
                                comboBox1.SelectedItem = existingItem;
                            }
                        }
                    }
                }

            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT s.id, ip.inventory_id as item_id, i.name as item_name, s.inventory_price_id as item_price_id, ip.price as item_price, " +
                                         "s.amount, s.total_cost FROM purchase_inventory AS s LEFT JOIN inventory_prices AS ip ON ip.Id = s.inventory_price_id " +
                                         "LEFT JOIN inventory AS i ON i.Id = ip.inventory_id WHERE s.purchase_id = @id AND s.is_deleted = FALSE;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = reader.GetInt64(reader.GetOrdinal("Id"));
                                var itemId = reader.GetInt64(reader.GetOrdinal("item_id"));
                                var itemName = reader.GetString(reader.GetOrdinal("item_name"));
                                var itemPriceId = reader.GetInt64(reader.GetOrdinal("item_price_id"));
                                var itemPrice = reader.GetDecimal(reader.GetOrdinal("item_price"));
                                var amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                                var totalCost = reader.GetDecimal(reader.GetOrdinal("total_cost"));
                                dataGridView1.Rows.Add(id, itemId, itemName, itemPriceId, itemPrice, amount, totalCost);
                            }
                        }
                    }
                }

            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateTotalLabel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var amountForm = new InventoryAmountForm())
            {
                var result = amountForm.ShowDialog();
                if (result != DialogResult.OK) return;
                var item = amountForm.Item;
                dataGridView1.Rows.Add(null, item.ItemId, item.ItemName, item.ItemPriceId, item.ItemPrice, item.Amount, item.TotalCost);
                UpdateTotalLabel();
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (e.ColumnIndex != dataGridView1.Columns["DeleteColumn"].Index || e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            var idString = row.Cells["id"]?.Value?.ToString();
            var itemIdString = row.Cells["item_id"]?.Value?.ToString();
            var amountString = row.Cells["amount"]?.Value?.ToString();
            if (!long.TryParse(idString, out var id))
            {
                return;
            }

            if (!long.TryParse(itemIdString, out var itemId))
            {
                return;
            }

            if (!decimal.TryParse(amountString, out var amount))
            {
                return;
            }
            var deletedTransaction = new TransactionDeleted
            {
                Id = id,
                ItemId = itemId,
                Amount = amount
            };
            _deletedInventoryPurchaseIds.Add(deletedTransaction);
            dataGridView1.Rows.RemoveAt(e.RowIndex);
            UpdateTotalLabel();
        }

        private bool SaveData()
        {
            var result = false;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    long savedPurchaseId = 0;
                    const string insertQuery = "INSERT INTO purchase (supplier_id, notes, inventory_total, other_cost, tax, discount, grand_total, created_by, created_datetime, modified_by, modified_datetime) " +
                                              "VALUES (@supplier_id, @notes, @inventory_total, @other_cost, @tax, @discount, @grand_total, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        UpdateTotalLabel();
                        if (comboBox1.SelectedItem is ItemDisplay selectedItem)
                        {
                            var currentDateTime = DateTime.Now;
                            var description = richTextBox1.Text.Trim();
                            var grandTotal = decimal.TryParse(customTextbox4.Text.Replace(",", "").Replace("PHP ", ""), out var parsedGrandTotal) ? parsedGrandTotal : 0;
                            var inventoryTotal = decimal.TryParse(customTextbox1.Text, out var parsedServiceTotal) ? parsedServiceTotal : 0;
                            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
                            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
                            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
                            command.Parameters.AddWithValue("@supplier_id", selectedItem.Id);
                            command.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                            command.Parameters.AddWithValue("@inventory_total", inventoryTotal);
                            command.Parameters.AddWithValue("@other_cost", otherCost);
                            command.Parameters.AddWithValue("@tax", tax);
                            command.Parameters.AddWithValue("@discount", discount);
                            command.Parameters.AddWithValue("@grand_total", grandTotal);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                            savedPurchaseId = command.LastInsertedId;
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show(@"Invalid supplier item selected.",Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        long itemPriceId = 0;
                        if (row.Cells["item_price_id"].Value != null && long.TryParse(row.Cells["item_price_id"].Value.ToString(), out var parsedItemPriceId))
                        {
                            itemPriceId = parsedItemPriceId;
                        }

                        long itemId = 0;
                        if (row.Cells["item_id"].Value != null && long.TryParse(row.Cells["item_id"].Value.ToString(), out var parsedItemId))
                        {
                            itemId = parsedItemId;
                        }

                        var amount = 0;
                        if (row.Cells["amount"].Value != null)
                            int.TryParse(row.Cells["amount"].Value.ToString(), out amount);

                        decimal totalCost = 0;
                        if (row.Cells["total_cost"].Value != null)
                            decimal.TryParse(row.Cells["total_cost"].Value.ToString(), out totalCost);

                        const string insertItemQuery = "INSERT INTO purchase_inventory (purchase_id, inventory_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                       "VALUES (@purchase_id, @inventory_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemQuery, connection))
                        {
                            var currentDateTime = DateTime.Now;
                            command.Parameters.AddWithValue("@purchase_id", savedPurchaseId);
                            command.Parameters.AddWithValue("@inventory_price_id", itemPriceId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@total_cost", totalCost);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }


                        const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, is_sales, created_by, created_datetime, modified_by, modified_datetime) " +
                                                       "VALUES (@reference_id, @inventory_id, @amount, 0, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                        {
                            var currentDateTime = DateTime.Now;
                            command.Parameters.AddWithValue("@reference_id", savedPurchaseId);
                            command.Parameters.AddWithValue("@inventory_id", itemId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }
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
                    const string updateQuery = "UPDATE purchase SET " +
                                               "supplier_id = @supplier_id, " +
                                               "notes = @notes, " +
                                               "inventory_total = @inventory_total, " +
                                               "other_cost = @other_cost, " +
                                               "tax = @tax, " +
                                               "discount = @discount, " +
                                               "grand_total = @grand_total, " +
                                               "modified_by = @modified_by, " +
                                               "modified_datetime = @modified_datetime " +
                                               "WHERE id = @id;";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        UpdateTotalLabel();
                        if (comboBox1.SelectedItem is ItemDisplay selectedItem)
                        {
                            var currentDateTime = DateTime.Now;
                            var description = richTextBox1.Text.Trim();
                            var grandTotal = decimal.TryParse(customTextbox4.Text.Replace(",", "").Replace("PHP ", ""), out var parsedGrandTotal) ? parsedGrandTotal : 0;
                            var inventoryTotal = decimal.TryParse(customTextbox1.Text, out var parsedServiceTotal) ? parsedServiceTotal : 0;
                            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
                            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
                            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
                            command.Parameters.AddWithValue("@supplier_id", selectedItem.Id);
                            command.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                            command.Parameters.AddWithValue("@inventory_total", inventoryTotal);
                            command.Parameters.AddWithValue("@other_cost", otherCost);
                            command.Parameters.AddWithValue("@tax", tax);
                            command.Parameters.AddWithValue("@discount", discount);
                            command.Parameters.AddWithValue("@grand_total", grandTotal);
                            command.Parameters.AddWithValue("@modified_by", _userId);
                            command.Parameters.AddWithValue("@modified_datetime", currentDateTime);
                            command.Parameters.AddWithValue("@id", _formId);
                            command.ExecuteNonQuery();
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show(@"Invalid supplier item selected.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow)
                            continue;
                        
                        long itemPriceId = 0;
                        if (row.Cells["item_price_id"].Value != null && long.TryParse(row.Cells["item_price_id"].Value.ToString(), out var parsedItemPriceId))
                        {
                            itemPriceId = parsedItemPriceId;
                        }

                        long itemId = 0;
                        if (row.Cells["item_id"].Value != null && long.TryParse(row.Cells["item_id"].Value.ToString(), out var parsedItemId))
                        {
                            itemId = parsedItemId;
                        }

                        var amount = 0;
                        if (row.Cells["amount"].Value != null)
                            int.TryParse(row.Cells["amount"].Value.ToString(), out amount);

                        decimal totalCost = 0;
                        if (row.Cells["total_cost"].Value != null)
                            decimal.TryParse(row.Cells["total_cost"].Value.ToString(), out totalCost);
                        

                        if (row.Cells["id"].Value != null && long.TryParse(row.Cells["id"].Value.ToString(), out _))
                        {

                        }
                        else
                        {
                            const string insertItemQuery = "INSERT INTO purchase_inventory (purchase_id, inventory_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                           "VALUES (@purchase_id, @inventory_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                            using (var command = new MySqlCommand(insertItemQuery, connection))
                            {
                                var currentDateTime = DateTime.Now;
                                command.Parameters.AddWithValue("@purchase_id", _formId);
                                command.Parameters.AddWithValue("@inventory_price_id", itemPriceId);
                                command.Parameters.AddWithValue("@amount", amount);
                                command.Parameters.AddWithValue("@total_cost", totalCost);
                                command.Parameters.AddWithValue("@user_id", _userId);
                                command.Parameters.AddWithValue("@current_date", currentDateTime);
                                command.ExecuteNonQuery();
                            }

                            const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, is_sales, created_by, created_datetime, modified_by, modified_datetime) " +
                                                                      "VALUES (@reference_id, @inventory_id, @amount, 0, @user_id, @current_date, @user_id, @current_date);";
                            using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                            {
                                var currentDateTime = DateTime.Now;
                                command.Parameters.AddWithValue("@reference_id", _formId);
                                command.Parameters.AddWithValue("@inventory_id", itemId);
                                command.Parameters.AddWithValue("@amount", amount);
                                command.Parameters.AddWithValue("@user_id", _userId);
                                command.Parameters.AddWithValue("@current_date", currentDateTime);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    foreach (var item in _deletedInventoryPurchaseIds)
                    {
                        var currentDateTime = DateTime.Now;
                        const string deleteQuery = "UPDATE purchase_inventory SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        using (var command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", item.Id);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }

                        const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, is_sales, created_by, created_datetime, modified_by, modified_datetime) " +
                                                                  "VALUES (@reference_id, @inventory_id, @amount, 0, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                        {
                            command.Parameters.AddWithValue("@reference_id", _formId);
                            command.Parameters.AddWithValue("@inventory_id", item.ItemId);
                            command.Parameters.AddWithValue("@amount", item.Amount * -1);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }

                        
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
            if (comboBox1.Text == "")
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
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "item_price")
            {
                if (e.Value != null && e.Value != DBNull.Value && e.Value is decimal price)
                {
                    e.Value = $@"PHP {price:N2}";
                    e.FormattingApplied = true;
                }
                else if (e.Value == DBNull.Value)
                {
                    e.Value = "PHP 0.00";
                    e.FormattingApplied = true;
                }
            }


            if (dataGridView1.Columns[e.ColumnIndex].Name == "amount")
            {
                if (e.Value != null && e.Value != DBNull.Value && e.Value is decimal amount)
                {
                    e.Value = $@"{amount:N2}";
                    e.FormattingApplied = true;
                }
                else if (e.Value == DBNull.Value)
                {
                    e.Value = "0.00";
                    e.FormattingApplied = true;
                }
            }


            if (dataGridView1.Columns[e.ColumnIndex].Name != "total_cost") return;
            if (e.Value != null && e.Value != DBNull.Value && e.Value is decimal totalCost)
            {
                e.Value = $@"PHP {totalCost:N2}";
                e.FormattingApplied = true;
            }
            else if (e.Value == DBNull.Value)
            {
                e.Value = "PHP 0.00";
                e.FormattingApplied = true;
            }
        }
    }
}