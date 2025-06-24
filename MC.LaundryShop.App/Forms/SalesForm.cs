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
    public partial class SalesForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _formId;
        private readonly long? _userId;
        private readonly List<TransactionDeleted> _deletedInventorySalesIds = new List<TransactionDeleted>();
        private readonly List<long> _deletedServiceSalesIds = new List<long>();

        public SalesForm(long? formId, long? userId)
        {
            InitializeComponent();
            _formId = formId;
            _userId = userId;
            InitializeForm();
            if (_formId.HasValue && userId.HasValue)
            {
                label3.Text = @"Update your sales record here.";
                submit_button.Text = @"Update";
                LoadData();
            }
            else
            {
                label3.Text = @"Save your sales record here.";
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
                    const string query = "SELECT id, full_name FROM customers WHERE is_deleted = FALSE AND is_active = TRUE;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            comboBox1.DisplayMember = "Name";
                            while (reader.Read())
                            {
                                var loadedCustomerItem = new ItemDisplay()
                                {
                                    Id = reader.GetInt32("id"),
                                    Name = reader.GetString("full_name"),
                                };
                                comboBox1.Items.Add(loadedCustomerItem);
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

            #region Services

            var idColumn1 = new DataGridViewTextBoxColumn();
            idColumn1.DataPropertyName = "id";
            idColumn1.Name = "id1";
            idColumn1.Visible = false;
            dataGridView2.Columns.Add(idColumn1);

            var itemIdColumn1 = new DataGridViewTextBoxColumn();
            itemIdColumn1.DataPropertyName = "item_id";
            itemIdColumn1.Name = "item_id1";
            itemIdColumn1.Visible = false;
            dataGridView2.Columns.Add(itemIdColumn1);

            var itemNameColumn1 = new DataGridViewTextBoxColumn();
            itemNameColumn1.DataPropertyName = "item_name";
            itemNameColumn1.Name = "item_name1";
            itemNameColumn1.HeaderText = @"Inventory";
            itemNameColumn1.Width = 200;
            itemNameColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns.Add(itemNameColumn1);

            var itemPriceIdColumn1 = new DataGridViewTextBoxColumn();
            itemPriceIdColumn1.DataPropertyName = "item_price_id";
            itemPriceIdColumn1.Name = "item_price_id1";
            itemPriceIdColumn1.Visible = false;
            dataGridView2.Columns.Add(itemPriceIdColumn1);

            var itemPriceColumn1 = new DataGridViewTextBoxColumn();
            itemPriceColumn1.DataPropertyName = "item_price";
            itemPriceColumn1.Name = "item_price1";
            itemPriceColumn1.HeaderText = @"Price";
            itemPriceColumn1.Width = 120;
            dataGridView2.Columns.Add(itemPriceColumn1);

            var amountColumn1 = new DataGridViewTextBoxColumn();
            amountColumn1.DataPropertyName = "amount";
            amountColumn1.Name = "amount1";
            amountColumn1.HeaderText = @"Amount";
            amountColumn1.Width = 120;
            dataGridView2.Columns.Add(amountColumn1);

            var totalCostColumn1 = new DataGridViewTextBoxColumn();
            totalCostColumn1.DataPropertyName = "total_cost";
            totalCostColumn1.Name = "total_cost1";
            totalCostColumn1.HeaderText = @"Total Cost";
            totalCostColumn1.Width = 120;
            dataGridView2.Columns.Add(totalCostColumn1);

            var deleteButtonColumn1 = new DataGridViewButtonColumn();
            deleteButtonColumn1.Name = "DeleteColumn1";
            deleteButtonColumn1.HeaderText = @"Delete";
            deleteButtonColumn1.Text = "Delete";
            deleteButtonColumn1.UseColumnTextForButtonValue = true;
            deleteButtonColumn1.Width = 90;
            dataGridView2.Columns.Add(deleteButtonColumn1);

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

            decimal servicesGrandTotal = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells["total_cost1"].Value != null && decimal.TryParse(row.Cells["total_cost1"].Value.ToString(), out var totalCost))
                {
                    servicesGrandTotal += totalCost;
                }
            }
            customTextbox2.Text = servicesGrandTotal.ToString(CultureInfo.InvariantCulture);

            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
            var total = inventoryGrandTotal + servicesGrandTotal + otherCost + tax - discount;
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
                    const string query = "SELECT s.customer_id, c.full_name as customer_name, s.notes, s.other_cost, s.tax, s.discount FROM sales AS s LEFT JOIN customers AS c ON c.id = s.customer_id WHERE s.id = @id;";
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
                            var loadedCustomerItem = new ItemDisplay()
                            {
                                Id = reader.GetInt64("customer_id"),
                                Name = reader.GetString("customer_name"),
                            };
                            var existingItem = comboBox1.Items.Cast<ItemDisplay>()
                                .FirstOrDefault(item => item.Id == loadedCustomerItem.Id);
                            if (existingItem == null)
                            {
                                comboBox1.Items.Add(loadedCustomerItem);
                                comboBox1.SelectedItem = loadedCustomerItem;
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
                                         "s.amount, s.total_cost FROM sales_inventory AS s LEFT JOIN inventory_prices AS ip ON ip.Id = s.inventory_price_id " +
                                         "LEFT JOIN inventory AS i ON i.Id = ip.inventory_id WHERE s.sales_id = @id AND s.is_deleted = FALSE;";
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

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT s.id, ip.service_id as item_id, i.name as item_name, s.service_price_id as item_price_id, ip.price as item_price, " +
                                         "s.amount, s.total_cost FROM sales_service AS s LEFT JOIN services_prices AS ip ON ip.Id = s.service_price_id " +
                                         "LEFT JOIN services AS i ON i.Id = ip.service_id WHERE s.sales_id = @id AND s.is_deleted = FALSE;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = reader.GetInt64(reader.GetOrdinal("id"));
                                var itemId = reader.GetInt64(reader.GetOrdinal("item_id"));
                                var itemName = reader.GetString(reader.GetOrdinal("item_name"));
                                var itemPriceId = reader.GetInt64(reader.GetOrdinal("item_price_id"));
                                var itemPrice = reader.GetDecimal(reader.GetOrdinal("item_price"));
                                var amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                                var totalCost = reader.GetDecimal(reader.GetOrdinal("total_cost"));
                                dataGridView2.Rows.Add(id, itemId, itemName, itemPriceId, itemPrice, amount, totalCost);
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
        private void button3_Click(object sender, EventArgs e)
        {
            using (var amountForm = new ServicesAmountForm())
            {
                var result = amountForm.ShowDialog();
                if (result != DialogResult.OK) return;
                var item = amountForm.Item;
                dataGridView2.Rows.Add(null, item.ItemId, item.ItemName, item.ItemPriceId, item.ItemPrice, item.Amount, item.TotalCost);
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
            _deletedInventorySalesIds.Add(deletedTransaction);
            dataGridView1.Rows.RemoveAt(e.RowIndex);
            UpdateTotalLabel();
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (e.ColumnIndex != dataGridView2.Columns["DeleteColumn1"].Index || e.RowIndex < 0) return;
            var row = dataGridView2.Rows[e.RowIndex];
            if (row.IsNewRow) return;
            if (row.Cells["id1"]?.Value != null)
            {
                if (!long.TryParse(row.Cells["id1"].Value.ToString(), out var recordId)) return;
                _deletedServiceSalesIds.Add(recordId);
            }
            dataGridView2.Rows.RemoveAt(e.RowIndex);
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
                    long savedSalesId = 0;
                    const string insertQuery = "INSERT INTO sales (customer_id, notes, service_total, inventory_total, other_cost, tax, discount, grand_total, created_by, created_datetime, modified_by, modified_datetime) " +
                                              "VALUES (@customer_id, @notes, @service_total, @inventory_total, @other_cost, @tax, @discount, @grand_total, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        UpdateTotalLabel();
                        if (comboBox1.SelectedItem is ItemDisplay selectedItem)
                        {
                            var currentDateTime = DateTime.Now;
                            var description = richTextBox1.Text.Trim();
                            var grandTotal = decimal.TryParse(customTextbox4.Text.Replace(",", "").Replace("PHP ", ""), out var parsedGrandTotal) ? parsedGrandTotal : 0;
                            var inventoryTotal = decimal.TryParse(customTextbox1.Text, out var parsedServiceTotal) ? parsedServiceTotal : 0;
                            var serviceTotal = decimal.TryParse(customTextbox2.Text, out var parsedInventoryTotal) ? parsedInventoryTotal : 0;
                            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
                            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
                            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
                            command.Parameters.AddWithValue("@customer_id", selectedItem.Id);
                            command.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                            command.Parameters.AddWithValue("@service_total", serviceTotal);
                            command.Parameters.AddWithValue("@inventory_total", inventoryTotal);
                            command.Parameters.AddWithValue("@other_cost", otherCost);
                            command.Parameters.AddWithValue("@tax", tax);
                            command.Parameters.AddWithValue("@discount", discount);
                            command.Parameters.AddWithValue("@grand_total", grandTotal);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                            savedSalesId = command.LastInsertedId;
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show(@"Invalid customer item selected.",Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                        const string insertItemQuery = "INSERT INTO sales_inventory (sales_id, inventory_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                       "VALUES (@sales_id, @inventory_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemQuery, connection))
                        {
                            var currentDateTime = DateTime.Now;
                            command.Parameters.AddWithValue("@sales_id", savedSalesId);
                            command.Parameters.AddWithValue("@inventory_price_id", itemPriceId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@total_cost", totalCost);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }


                        const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, created_by, created_datetime, modified_by, modified_datetime) " +
                                                       "VALUES (@reference_id, @inventory_id, @amount, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                        {
                            var currentDateTime = DateTime.Now;
                            command.Parameters.AddWithValue("@reference_id", savedSalesId);
                            command.Parameters.AddWithValue("@inventory_id", itemId);
                            command.Parameters.AddWithValue("@amount", amount * -1);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }
                    }

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        long itemPriceId = 0;
                        if (row.Cells["item_price_id1"].Value != null && long.TryParse(row.Cells["item_price_id1"].Value.ToString(), out var parsedItemPriceId))
                        {
                            itemPriceId = parsedItemPriceId;
                        }

                        var amount = 0;
                        if (row.Cells["amount1"].Value != null)
                            int.TryParse(row.Cells["amount1"].Value.ToString(), out amount);

                        decimal totalCost = 0;
                        if (row.Cells["total_cost1"].Value != null)
                            decimal.TryParse(row.Cells["total_cost1"].Value.ToString(), out totalCost);

                        const string insertItemQuery = "INSERT INTO sales_service (sales_id, service_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                       "VALUES (@sales_id, @service_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemQuery, connection))
                        {
                            var currentDateTime = DateTime.Now;
                            command.Parameters.AddWithValue("@sales_id", savedSalesId);
                            command.Parameters.AddWithValue("@service_price_id", itemPriceId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@total_cost", totalCost);
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
                    const string updateQuery = "UPDATE sales SET " +
                                               "customer_id = @customer_id, " +
                                               "notes = @notes, " +
                                               "service_total = @service_total, " +
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
                            var serviceTotal = decimal.TryParse(customTextbox2.Text, out var parsedInventoryTotal) ? parsedInventoryTotal : 0;
                            var otherCost = decimal.TryParse(customTextbox3.Text, out var parsedOtherCost) ? parsedOtherCost : 0;
                            var tax = decimal.TryParse(customTextbox6.Text, out var parsedTax) ? parsedTax : 0;
                            var discount = decimal.TryParse(customTextbox5.Text, out var parsedDiscount) ? parsedDiscount : 0;
                            command.Parameters.AddWithValue("@customer_id", selectedItem.Id);
                            command.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                            command.Parameters.AddWithValue("@service_total", serviceTotal);
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
                            MessageBox.Show(@"Invalid customer item selected.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            const string insertItemQuery = "INSERT INTO sales_inventory (sales_id, inventory_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                           "VALUES (@sales_id, @inventory_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                            using (var command = new MySqlCommand(insertItemQuery, connection))
                            {
                                var currentDateTime = DateTime.Now;
                                command.Parameters.AddWithValue("@sales_id", _formId);
                                command.Parameters.AddWithValue("@inventory_price_id", itemPriceId);
                                command.Parameters.AddWithValue("@amount", amount);
                                command.Parameters.AddWithValue("@total_cost", totalCost);
                                command.Parameters.AddWithValue("@user_id", _userId);
                                command.Parameters.AddWithValue("@current_date", currentDateTime);
                                command.ExecuteNonQuery();
                            }

                            const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, created_by, created_datetime, modified_by, modified_datetime) " +
                                                                      "VALUES (@reference_id, @inventory_id, @amount, @user_id, @current_date, @user_id, @current_date);";
                            using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                            {
                                var currentDateTime = DateTime.Now;
                                command.Parameters.AddWithValue("@reference_id", _formId);
                                command.Parameters.AddWithValue("@inventory_id", itemId);
                                command.Parameters.AddWithValue("@amount", amount * -1);
                                command.Parameters.AddWithValue("@user_id", _userId);
                                command.Parameters.AddWithValue("@current_date", currentDateTime);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        long itemPriceId = 0;
                        if (row.Cells["item_price_id1"].Value != null && long.TryParse(row.Cells["item_price_id1"].Value.ToString(), out var parsedItemPriceId))
                        {
                            itemPriceId = parsedItemPriceId;
                        }

                        var amount = 0;
                        if (row.Cells["amount1"].Value != null)
                            int.TryParse(row.Cells["amount1"].Value.ToString(), out amount);

                        decimal totalCost = 0;
                        if (row.Cells["total_cost1"].Value != null)
                            decimal.TryParse(row.Cells["total_cost1"].Value.ToString(), out totalCost);

                        if (row.Cells["id1"].Value != null && long.TryParse(row.Cells["id1"].Value.ToString(), out _))
                        {
                            
                        }
                        else
                        {
                            const string insertItemQuery = "INSERT INTO sales_service (sales_id, service_price_id, amount, total_cost, created_by, created_datetime, modified_by, modified_datetime) " +
                                                           "VALUES (@sales_id, @service_price_id, @amount, @total_cost, @user_id, @current_date, @user_id, @current_date);";
                            using (var command = new MySqlCommand(insertItemQuery, connection))
                            {
                                var currentDateTime = DateTime.Now;
                                command.Parameters.AddWithValue("@sales_id", _formId);
                                command.Parameters.AddWithValue("@service_price_id", itemPriceId);
                                command.Parameters.AddWithValue("@amount", amount);
                                command.Parameters.AddWithValue("@total_cost", totalCost);
                                command.Parameters.AddWithValue("@user_id", _userId);
                                command.Parameters.AddWithValue("@current_date", currentDateTime);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    foreach (var item in _deletedInventorySalesIds)
                    {
                        var currentDateTime = DateTime.Now;
                        const string deleteQuery = "UPDATE sales_inventory SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        using (var command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", item.Id);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }

                        const string insertItemTransactionQuery = "INSERT INTO inventory_transactions (reference_id, inventory_id, amount, created_by, created_datetime, modified_by, modified_datetime) " +
                                                                  "VALUES (@reference_id, @inventory_id, @amount, @user_id, @current_date, @user_id, @current_date);";
                        using (var command = new MySqlCommand(insertItemTransactionQuery, connection))
                        {
                            command.Parameters.AddWithValue("@reference_id", _formId);
                            command.Parameters.AddWithValue("@inventory_id", item.ItemId);
                            command.Parameters.AddWithValue("@amount", item.Amount);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }
                    }
                    foreach (var item in _deletedServiceSalesIds)
                    {
                        var currentDateTime = DateTime.Now;
                        const string deleteQuery = "UPDATE sales_service SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        using (var command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", item);
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

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "item_price1")
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


            if (dataGridView2.Columns[e.ColumnIndex].Name == "amount1")
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


            if (dataGridView2.Columns[e.ColumnIndex].Name != "total_cost1") return;
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