using MC.LaundryShop.App.Class;
using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class MainForm : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private int _pageNumber;
        private int _pageSize = 10;
        private int _totalRecords;
        private int _totalPages;
        private readonly long _userId;
        private enum Modules
        {
            Sales,
            Purchase,
            Inventory,
            Services,
            Customers,
            Suppliers,
            Employees
        }
        private Modules _selectedModule = Modules.Sales;
        private Dictionary<Modules, Label> _moduleLabels;
        private Font _moduleRegularFont;
        private Font _moduleBoldFont;

        public MainForm(UserDetails user)
        {
            InitializeComponent();
            user_name.Text = user.FullName;
            user_position.Text = user.Position;
            _userId = user.Id;
            if (user.Image == null || user.Image.Length <= 0) return;
            using (var ms = new MemoryStream(user.Image))
            {
                user_image.Image = Image.FromStream(ms);
            }
        }

        private void InitializeModuleLabels()
        {
            _moduleRegularFont = new Font("Poppins", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _moduleBoldFont = new Font("Poppins", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _moduleLabels = new Dictionary<Modules, Label>
            {
                { Modules.Sales, sales_button_label },
                { Modules.Purchase, purchase_button_label },
                { Modules.Inventory, inventory_button_label },
                { Modules.Services, services_button_label },
                { Modules.Customers, customers_button_label },
                { Modules.Suppliers, suppliers_button_label },
                { Modules.Employees, employees_button_label }
            };
        }
        private void SetModule(Modules id)
        {
            if (_selectedModule == id)
            {
                return;
            }

            if (_moduleLabels == null)
            {
                InitializeModuleLabels();
            }
            if (_moduleLabels != null && _moduleLabels.TryGetValue(_selectedModule, out var previousLabel))
            {
                previousLabel.Font = _moduleRegularFont;
                previousLabel.ForeColor = SystemColors.ControlText;
            }

            if (_moduleLabels != null && _moduleLabels.TryGetValue(id, out var currentLabel))
            {
                var moduleLabel = id.ToString();
                currentLabel.Font = _moduleBoldFont;
                currentLabel.ForeColor = Color.DodgerBlue;
                selected_module_label.Text = id.ToString();
                if (id == Modules.Employees)
                {
                    add_button.Visible = _userId == 1;
                }
                else
                {
                    add_button.Visible = true;
                }
                add_button.Text = $@"Add {moduleLabel}";
                label12.Text = @"Please find below the comprehensive list of the recent " + moduleLabel.ToLower() + @".";
                switch (id)
                {
                    case Modules.Sales:
                        search_criteria.PlaceholderText = "Search Customer";
                        break;
                    case Modules.Purchase:
                        search_criteria.PlaceholderText = "Search Supplier";
                        break;
                    case Modules.Inventory:
                    case Modules.Services:
                    case Modules.Customers:
                    case Modules.Suppliers: 
                    case Modules.Employees:
                    default:
                        search_criteria.PlaceholderText = "Search " + moduleLabel;
                        break;
                }
            }

            _selectedModule = id;
            LoadDataIntoDataGridView(true);
        }
        private void LoadDataIntoDataGridView(bool reset = false)
        {
            SetControlState(false);

            if (reset)
            {
                search_criteria.Text = string.Empty;
                displayed_rows.SelectedIndex = 0;
                _pageSize = Convert.ToInt32(displayed_rows.SelectedItem);
                _pageNumber = 0;
                _totalRecords = 0;
                _totalPages = 0;
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string countQuery;
                    switch (_selectedModule)
                    {
                        case Modules.Sales:
                            countQuery = "SELECT COUNT(s.id) " +
                                         "FROM sales AS s " +
                                         "LEFT JOIN customers AS c ON s.customer_id = c.id " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR c.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND s.is_deleted = FALSE;";
                            break;
                        case Modules.Purchase:
                            countQuery = "SELECT COUNT(p.id) " +
                                         "FROM purchase AS p " +
                                         "LEFT JOIN suppliers AS s ON p.supplier_id = s.id " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND p.is_deleted = FALSE;";
                            break;
                        case Modules.Inventory:
                            countQuery = "SELECT COUNT(i.id) " +
                                         "FROM inventory AS i " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR i.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND i.is_deleted = FALSE;";
                            break;
                        case Modules.Services:
                            countQuery = "SELECT COUNT(s.id) " +
                                         "FROM services AS s " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND s.is_deleted = FALSE;";
                            break;
                        case Modules.Customers:
                            countQuery = "SELECT COUNT(c.id) " +
                                         "FROM customers AS c " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR c.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND c.is_deleted = FALSE;";
                            break;
                        case Modules.Suppliers:
                            countQuery = "SELECT COUNT(s.id) " +
                                         "FROM suppliers AS s " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND s.is_deleted = FALSE;";
                           
                            break;
                        case Modules.Employees:
                            countQuery = "SELECT COUNT(e.id) " +
                                         "FROM employees AS e " +
                                         "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR e.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                         "AND e.is_deleted = FALSE AND e.user_id != 'sa';";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    using (var countCommand = new MySqlCommand(countQuery, connection))
                    {
                        countCommand.Parameters.AddWithValue("@SearchCriteria", search_criteria.Text.Trim());
                        _totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());
                    }
                    _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                    if (_totalPages == 0) _totalPages = 1;
                    if (_pageNumber >= _totalPages)
                    {
                        _pageNumber = _totalPages - 1;
                    }
                    if (_pageNumber < 0)
                    {
                        _pageNumber = 0;
                    }

                    string dataQuery;

                    switch (_selectedModule)
                    {
                        case Modules.Sales:
                            dataQuery = "SELECT " +
                                        "s.id, " +
                                        "s.customer_id, " +
                                        "c.full_name as customer_name, " +
                                        "s.notes, " +
                                        "s.service_total, " +
                                        "s.inventory_total, " +
                                        "s.other_cost, " +
                                        "s.tax, " +
                                        "s.discount, " +
                                        "s.grand_total, " +
                                        "s.created_by, " +
                                        "e.full_name AS modified_by_name, " +
                                        "s.created_datetime, " +
                                        "s.modified_by, " +
                                        "s.modified_datetime " +
                                        "FROM sales AS s " +
                                        "LEFT JOIN customers AS c ON s.customer_id = c.id " +
                                        "LEFT JOIN employees AS e ON s.modified_by = e.id " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR c.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND s.is_deleted = FALSE " +
                                        "ORDER BY s.id " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;
                        case Modules.Purchase:
                            dataQuery = "SELECT " +
                                        "p.id, " +
                                        "p.supplier_id, " +
                                        "s.name as supplier_name, " +
                                        "p.notes, " +
                                        "p.inventory_total, " +
                                        "p.other_cost, " +
                                        "p.tax, " +
                                        "p.discount, " +
                                        "p.grand_total, " +
                                        "p.created_by, " +
                                        "p.created_datetime, " +
                                        "p.modified_by, " +
                                        "p.modified_datetime " +
                                        "FROM purchase AS p " +
                                        "LEFT JOIN suppliers AS s ON p.supplier_id = s.id " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND p.is_deleted = FALSE " +
                                        "ORDER BY s.name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;
                        case Modules.Inventory:
                            dataQuery = "SELECT " +
                                        "i.id, " +
                                        "i.name, " +
                                        "i.description, " +
                                        "i.is_active, " +
                                        "i.created_by, " +
                                        "i.created_datetime, " +
                                        "i.modified_by, " +
                                        "i.modified_datetime " +
                                        "FROM inventory AS i " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR i.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND i.is_deleted = FALSE " +
                                        "ORDER BY i.name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;
                        case Modules.Services:
                            dataQuery = "SELECT " +
                                        "s.id, " +
                                        "s.name, " +
                                        "s.description, " +
                                        "s.is_active, " +
                                        "s.created_by, " +
                                        "s.created_datetime, " +
                                        "s.modified_by, " +
                                        "s.modified_datetime " +
                                        "FROM services AS s " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND s.is_deleted = FALSE " +
                                        "ORDER BY s.name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;
                        case Modules.Customers:
                            dataQuery = "SELECT " +
                                        "c.id, " +
                                        "c.first_name, " +
                                        "c.middle_name, " +
                                        "c.last_name, " +
                                        "c.suffix, " +
                                        "c.full_name, " +
                                        "c.email, " +
                                        "c.contact_number, " +
                                        "c.is_active, " +
                                        "c.created_by, " +
                                        "c.created_datetime, " +
                                        "c.modified_by, " +
                                        "c.modified_datetime " +
                                        "FROM customers AS c " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR c.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND c.is_deleted = FALSE " +
                                        "ORDER BY c.full_name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;
                        case Modules.Suppliers:
                            dataQuery = "SELECT " +
                                        "s.id, " +
                                        "s.name, " +
                                        "s.email, " +
                                        "s.contact_number, " +
                                        "s.contact_person_first_name, " +
                                        "s.contact_person_middle_name, " +
                                        "s.contact_person_last_name, " +
                                        "s.contact_person_suffix, " +
                                        "s.contact_person_full_name, " +
                                        "s.is_active, " +
                                        "s.created_by, " +
                                        "s.created_datetime, " +
                                        "s.modified_by, " +
                                        "s.modified_datetime " +
                                        "FROM suppliers AS s " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR s.name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND s.is_deleted = FALSE " +
                                        "ORDER BY s.name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";

                            break;
                        case Modules.Employees:
                            dataQuery = "SELECT " +
                                        "e.id, " +
                                        "e.user_id, " +
                                        "e.first_name, " +
                                        "e.middle_name, " +
                                        "e.last_name, " +
                                        "e.suffix, " +
                                        "e.full_name, " +
                                        "e.position, " +
                                        "e.email, " +
                                        "e.contact_number, " +
                                        "e.password_hash, " +
                                        "e.image, " +
                                        "e.is_active, " +
                                        "e.created_by, " +
                                        "e.created_datetime, " +
                                        "e.modified_by, " +
                                        "e.modified_datetime " +
                                        "FROM employees AS e " +
                                        "WHERE (@SearchCriteria IS NULL OR @SearchCriteria = '' OR e.full_name LIKE CONCAT('%', @SearchCriteria, '%')) " +
                                        "AND e.is_deleted = FALSE AND e.user_id != 'sa'" +
                                        "ORDER BY e.full_name " +
                                        "LIMIT @PageSize " +
                                        "OFFSET @PageOffset;";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    using (var adapter = new MySqlDataAdapter(dataQuery, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@PageSize", _pageSize);
                        adapter.SelectCommand.Parameters.AddWithValue("@PageOffset", _pageNumber * _pageSize);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchCriteria", search_criteria.Text.Trim());
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                        CustomizeDataGridViewColumns();
                        page_label.Text = $@"Page {_pageNumber + 1} of {_totalPages}";
                        records_label.Text = $@"{dataTable.Rows.Count} of {_totalRecords} records displayed.";
                        SetControlState(true);
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (_selectedModule == Modules.Sales) return;
            if (dataGridView1?.Columns["id"] != null)
            {
                dataGridView1.Columns["id"].Visible = false;
            }
        }
        private void SetControlState(bool enabled)
        {
            add_button.Enabled = enabled;
            dataGridView1.Enabled = enabled;
            displayed_rows.Enabled = enabled;
            search_button.Enabled = enabled;
            search_criteria.Enabled = enabled;


            if (enabled)
            {
                var currentPageOneIndexed = _pageNumber + 1;
                var noPaginationNeeded = (_totalPages <= 1);
                first_button.Enabled = !noPaginationNeeded && (currentPageOneIndexed > 1);
                previous_button.Enabled = !noPaginationNeeded && (currentPageOneIndexed > 1);
                next_button.Enabled = !noPaginationNeeded && (currentPageOneIndexed < _totalPages);
                last_button.Enabled = !noPaginationNeeded && (currentPageOneIndexed < _totalPages);
            }
            else
            {
                first_button.Enabled = false;
                next_button.Enabled = false;
                previous_button.Enabled = false;
                last_button.Enabled = false;
            }
        }
        private void CustomizeDataGridViewColumns()
        {
            dataGridView1.Columns.Clear();
            switch (_selectedModule)
            {
                case Modules.Sales:
                    CustomizeDataGridViewColumnsSales();
                    break;
                case Modules.Purchase:
                    CustomizeDataGridViewColumnsPurchase();
                    break;
                case Modules.Inventory:
                    CustomizeDataGridViewColumnsInventory();
                    break;
                case Modules.Services:
                    CustomizeDataGridViewColumnsServices();
                    break;
                case Modules.Customers:
                    CustomizeDataGridViewColumnsCustomers();
                    break;
                case Modules.Suppliers:
                    CustomizeDataGridViewColumnsSuppliers();
                    break;
                case Modules.Employees:
                    CustomizeDataGridViewColumnsEmployee();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_selectedModule == Modules.Employees)
            {
                if (_userId != 1) return;
            }

            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "EditColumn",
                HeaderText = @"Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 80
            });
            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "DeleteColumn",
                HeaderText = @"Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 90
            });
        }
        private void CustomizeDataGridViewColumnsSales()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                HeaderText = @"Id",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "customer_id",
                Name = "customer_id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "customer_name",
                Name = "customer_name",
                HeaderText = @"Customer Name",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "notes",
                Name = "notes",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "service_total",
                Name = "service_total",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "inventory_total",
                Name = "inventory_total",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "other_cost",
                Name = "other_cost",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "tax",
                Name = "tax",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "discount",
                Name = "discount",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "grand_total",
                Name = "grand_total",
                HeaderText = @"Grand Total",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by_name",
                Name = "modified_by_name",
                HeaderText = @"Modified By",
                Width = 120,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });
        }
        private void CustomizeDataGridViewColumnsPurchase()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "supplier_id",
                Name = "supplier_id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "supplier_name",
                Name = "supplier_name",
                HeaderText = @"Supplier Name",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "notes",
                Name = "notes",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "inventory_total",
                Name = "inventory_total",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "other_cost",
                Name = "other_cost",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "tax",
                Name = "tax",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "discount",
                Name = "discount",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "grand_total",
                Name = "grand_total",
                HeaderText = @"Grand Total",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by_name",
                Name = "modified_by_name",
                HeaderText = @"Modified By",
                Width = 120,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });
        }
        private void CustomizeDataGridViewColumnsEmployee()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "user_id",
                Name = "user_id",
                HeaderText = @"User Id",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "full_name",
                Name = "full_name",
                HeaderText = @"Employee Name",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "position",
                Name = "position",
                HeaderText = @"Position",
                Width = 150
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "email",
                Name = "email",
                HeaderText = @"Email Address",
                Width = 180
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_number",
                Name = "contact_number",
                HeaderText = @"Contact No.",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "is_active",
                Name = "is_active",
                HeaderText = @"Active",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "first_name",
                Name = "first_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "middle_name",
                Name = "middle_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "last_name",
                Name = "last_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "suffix",
                Name = "suffix",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "password_hash",
                Name = "password_hash",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "image",
                Name = "image",
                Visible = false
            });
        }
        private void CustomizeDataGridViewColumnsCustomers()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "full_name",
                Name = "full_name",
                HeaderText = @"Customer Name",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "email",
                Name = "email",
                HeaderText = @"Email Address",
                Width = 180
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_number",
                Name = "contact_number",
                HeaderText = @"Contact No.",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "is_active",
                Name = "is_active",
                HeaderText = @"Active",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "first_name",
                Name = "first_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "middle_name",
                Name = "middle_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "last_name",
                Name = "last_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "suffix",
                Name = "suffix",
                Visible = false
            });
        }
        private void CustomizeDataGridViewColumnsSuppliers()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "name",
                Name = "name",
                HeaderText = @"Supplier Name",
                Width = 200
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_person_full_name",
                Name = "contact_person_full_name",
                HeaderText = @"Contact Person",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "email",
                Name = "email",
                HeaderText = @"Email Address",
                Width = 180
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_number",
                Name = "contact_number",
                HeaderText = @"Contact No.",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "is_active",
                Name = "is_active",
                HeaderText = @"Active",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_person_first_name",
                Name = "contact_person_first_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_person_middle_name",
                Name = "contact_person_middle_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_person_last_name",
                Name = "contact_person_last_name",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "contact_person_suffix",
                Name = "contact_person_suffix",
                Visible = false
            });
        }
        private void CustomizeDataGridViewColumnsServices()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "name",
                Name = "name",
                HeaderText = @"Service Name",
                Width = 200
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "description",
                Name = "description",
                HeaderText = @"Description",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "is_active",
                Name = "is_active",
                HeaderText = @"Active",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });
        }
        private void CustomizeDataGridViewColumnsInventory()
        {
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "name",
                Name = "name",
                HeaderText = @"Item Name",
                Width = 200
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "description",
                Name = "description",
                HeaderText = @"Description",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "is_active",
                Name = "is_active",
                HeaderText = @"Active",
                Width = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_by",
                Name = "created_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "created_datetime",
                Name = "created_datetime",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_by",
                Name = "modified_by",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "modified_datetime",
                Name = "modified_datetime",
                HeaderText = @"Last Modified",
                Width = 120
            });
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView(true);
        }
        private void Search_button_Click(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }
        private void Add_button_Click(object sender, EventArgs e)
        {
            switch (_selectedModule)
            {
                case Modules.Sales:
                    using (var dialog = new SalesForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Purchase:
                    using (var dialog = new PurchaseForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Inventory:
                    using (var dialog = new InventoryForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Services:
                    using (var dialog = new ServicesForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Customers:
                    using (var dialog = new CustomerForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Suppliers:
                    using (var dialog = new SuppliersForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;
                case Modules.Employees:
                    using (var dialog = new EmployeesForm(null, _userId))
                    {
                        var result = dialog.ShowDialog();
                        if (result != DialogResult.OK) return;
                        MessageBox.Show(Resources.Messages_Success_Saved, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDataGridView();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var id = (long)dataGridView1.Rows[e.RowIndex].Cells["id"].Value;
            // ReSharper disable once PossibleNullReferenceException
            if (e.ColumnIndex == dataGridView1.Columns["EditColumn"].Index)
            {
                switch (_selectedModule)
                {
                    case Modules.Sales:
                        
                        using (var dialog = new SalesForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Purchase:
                        using (var dialog = new PurchaseForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Inventory:
                        using (var dialog = new InventoryForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Services:
                        using (var dialog = new ServicesForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Customers:
                        using (var dialog = new CustomerForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Suppliers:
                        using (var dialog = new SuppliersForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;
                    case Modules.Employees:
                        using (var dialog = new EmployeesForm(id, _userId))
                        {
                            var result = dialog.ShowDialog();
                            if (result != DialogResult.OK) return;
                            MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // ReSharper disable once PossibleNullReferenceException
            else if (e.ColumnIndex == dataGridView1.Columns["DeleteColumn"].Index)
            {
                var confirmResult = MessageBox.Show(
                    @"Are you sure you want to delete this item?",
                    @"Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult != DialogResult.Yes) return;
                string deleteQuery;
                switch (_selectedModule)
                {
                    case Modules.Sales:
                        deleteQuery = "UPDATE sales SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Purchase:
                        deleteQuery = "UPDATE purchase SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Inventory:
                        deleteQuery = "UPDATE inventory SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Services:
                        deleteQuery = "UPDATE services SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Customers:
                        deleteQuery = "UPDATE customers SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Suppliers:
                        deleteQuery = "UPDATE suppliers SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;
                    case Modules.Employees:
                        deleteQuery = "UPDATE employees SET is_deleted = TRUE, deleted_datetime = @current_date, deleted_by = @user_id  WHERE Id = @id;";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                try
                {
                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        connection.Open();
                        var currentDateTime = DateTime.Now;
                        using (var command = new MySqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@user_id", _userId);
                            command.Parameters.AddWithValue("@current_date", currentDateTime);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadDataIntoDataGridView();
                MessageBox.Show(Resources.Messages_Success_Deleted, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region Modules Events

        private void Sales_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Sales);
        }
        private void Purchase_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Purchase);
        }
        private void Inventory_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Inventory);
        }
        private void Services_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Services);
        }
        private void Customers_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Customers);
        }
        private void Suppliers_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Suppliers);
        }
        private void Employees_Button_Click(object sender, EventArgs e)
        {
            SetModule(Modules.Employees);
        }

        private void Sales_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Sales)
            {
                sales_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Purchase_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Purchase)
            {
                purchase_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Inventory_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Inventory)
            {
                inventory_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Services_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Services)
            {
                services_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Customers_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Customers)
            {
                customers_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Suppliers_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Suppliers)
            {
                suppliers_button_label.ForeColor = Color.DodgerBlue;
            }
        }
        private void Employees_Button_Hover(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Employees)
            {
                employees_button_label.ForeColor = Color.DodgerBlue;
            }
        }

        private void Sales_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Sales)
            {
                sales_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Purchase_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Purchase)
            {
                purchase_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Inventory_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Inventory)
            {
                inventory_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Services_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Services)
            {
                services_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Customers_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Customers)
            {
                customers_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Suppliers_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Suppliers)
            {
                suppliers_button_label.ForeColor = SystemColors.ControlText;
            }
        }
        private void Employees_Button_Hover_Leave(object sender, EventArgs e)
        {
            if (_selectedModule != Modules.Employees)
            {
                employees_button_label.ForeColor = SystemColors.ControlText;
            }
        }

        #endregion

        #region Pagination

        private void First_button_Click(object sender, EventArgs e)
        {
            _pageNumber = 0;
            LoadDataIntoDataGridView();
        }

        private void Previous_button_Click(object sender, EventArgs e)
        {
            _pageNumber -= 1;
            LoadDataIntoDataGridView();
        }

        private void Next_button_Click(object sender, EventArgs e)
        {
            _pageNumber += 1;
            LoadDataIntoDataGridView();
        }

        private void Last_button_Click(object sender, EventArgs e)
        {
            _pageNumber = _totalPages - 1;
            LoadDataIntoDataGridView();
        }

        private void Displayed_rows_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pageSize = Convert.ToInt32(displayed_rows.SelectedItem);
            LoadDataIntoDataGridView();
        }


        #endregion

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_selectedModule != Modules.Sales || dataGridView1.Columns[e.ColumnIndex].Name != "grand_total") return;
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

        private void PictureBox8_Click(object sender, EventArgs e)
        {
            var menuY = pictureBox8.Location.Y - 50;
            var menuX = pictureBox8.Location.X;
            contextMenuStrip1.Show(new Point(menuX, menuY));
        }
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var dialog = new ChangePasswordForm(_userId))
            {
                var result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;
                MessageBox.Show(Resources.Messages_Success_Updated, Resources.Messages_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                @"Are you sure you want to logout?",
                @"Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;
            DialogResult = DialogResult.No;
            Close();
        }
    }
}