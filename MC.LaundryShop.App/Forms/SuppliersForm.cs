using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class SuppliersForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _formId;
        private readonly long? _userId;

        public SuppliersForm(long? formId, long? userId)
        {
            InitializeComponent();
            _formId = formId;
            _userId = userId;   
            if (_formId.HasValue && userId.HasValue)
            {
                label3.Text = @"Update your supplier record here.";
                submit_button.Text = @"Update";
                LoadData();
            }
            else
            {
                label3.Text = @"Save your supplier record here.";
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
                    const string query = "SELECT name, contact_person_first_name, contact_person_middle_name, contact_person_last_name, contact_person_suffix, email, contact_number, is_active FROM suppliers WHERE id = @id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return;
                            customTextbox7.Text = reader.GetString(reader.GetOrdinal("name"));
                            customTextbox1.Text = reader.GetString(reader.GetOrdinal("contact_person_first_name"));
                            customTextbox5.Text = reader.IsDBNull(reader.GetOrdinal("contact_person_middle_name"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("contact_person_middle_name"));
                            customTextbox2.Text = reader.GetString(reader.GetOrdinal("contact_person_last_name"));
                            customTextbox6.Text = reader.IsDBNull(reader.GetOrdinal("contact_person_suffix"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("contact_person_suffix"));
                            customTextbox3.Text = reader.IsDBNull(reader.GetOrdinal("email"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("email"));
                            customTextbox4.Text = reader.IsDBNull(reader.GetOrdinal("contact_number"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("contact_number"));
                            checkBox1.Checked = reader.GetBoolean(reader.GetOrdinal("is_active"));
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

                    var name = customTextbox7.Text.Trim();

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM suppliers WHERE name = @name AND is_deleted = FALSE;";
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

                    const string query = "INSERT INTO suppliers (name, contact_person_first_name, contact_person_middle_name, contact_person_last_name, contact_person_suffix, contact_person_full_name, email, contact_number, is_active, created_by, created_datetime, modified_by, modified_datetime) " +
                                         "VALUES (@name, @first_name, @middle_name, @last_name, @suffix, @full_name, @email, @contact_number, @is_active, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var firstName = customTextbox1.Text.Trim();
                        var middleName = customTextbox5.Text.Trim();
                        var lastName = customTextbox2.Text.Trim();
                        var suffix = customTextbox6.Text.Trim();
                        var email = customTextbox3.Text.Trim();
                        var contactNumber = customTextbox4.Text.Trim();
                        var middleInitial = string.Empty;
                        if (!string.IsNullOrEmpty(middleName))
                        {
                            middleInitial = middleName.Substring(0, 1).ToUpper() + ".";
                        }
                        var fullName = firstName;
                        if (!string.IsNullOrEmpty(middleInitial))
                        {
                            fullName += " " + middleInitial;
                        }
                        fullName += " " + lastName;
                        if (!string.IsNullOrEmpty(suffix))
                        {
                            fullName += " " + suffix;
                        }
                        fullName = fullName.Trim();
                        var currentDateTime = DateTime.Now;
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@first_name", firstName);
                        command.Parameters.AddWithValue("@middle_name", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                        command.Parameters.AddWithValue("@last_name", lastName);
                        command.Parameters.AddWithValue("@suffix", string.IsNullOrEmpty(suffix) ? (object)DBNull.Value : suffix);
                        command.Parameters.AddWithValue("@full_name", fullName);
                        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                        command.Parameters.AddWithValue("@contact_number", string.IsNullOrEmpty(contactNumber) ? (object)DBNull.Value : contactNumber);
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

                    var name = customTextbox7.Text.Trim();

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM suppliers WHERE name = @name AND is_deleted = FALSE AND Id != @id;";
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

                    const string query = "UPDATE suppliers SET " +
                                         "name = @name, " +
                                         "contact_person_first_name = @first_name, " +
                                         "contact_person_middle_name = @middle_name, " +
                                         "contact_person_last_name = @last_name, " +
                                         "contact_person_suffix = @suffix, " +
                                         "contact_person_full_name = @full_name, " +
                                         "email = @email, " +
                                         "contact_number = @contact_number, " +
                                         "is_active = @is_active, " +
                                         "modified_by = @modified_by, " +
                                         "modified_datetime = @modified_datetime " +
                                         "WHERE id = @id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        var firstName = customTextbox1.Text.Trim();
                        var middleName = customTextbox5.Text.Trim();
                        var lastName = customTextbox2.Text.Trim();
                        var suffix = customTextbox6.Text.Trim();
                        var email = customTextbox3.Text.Trim();
                        var contactNumber = customTextbox4.Text.Trim();
                        var middleInitial = string.Empty;
                        if (!string.IsNullOrEmpty(middleName))
                        {
                            middleInitial = middleName.Substring(0, 1).ToUpper() + ".";
                        }
                        var fullName = firstName;
                        if (!string.IsNullOrEmpty(middleInitial))
                        {
                            fullName += " " + middleInitial;
                        }
                        fullName += " " + lastName;
                        if (!string.IsNullOrEmpty(suffix))
                        {
                            fullName += " " + suffix;
                        }
                        fullName = fullName.Trim();

                        var currentDateTime = DateTime.Now;
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@first_name", firstName);
                        command.Parameters.AddWithValue("@middle_name", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName);
                        command.Parameters.AddWithValue("@last_name", lastName);
                        command.Parameters.AddWithValue("@suffix", string.IsNullOrEmpty(suffix) ? (object)DBNull.Value : suffix);
                        command.Parameters.AddWithValue("@full_name", fullName);
                        command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                        command.Parameters.AddWithValue("@contact_number", string.IsNullOrEmpty(contactNumber) ? (object)DBNull.Value : contactNumber);
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
            if (string.IsNullOrWhiteSpace(customTextbox1.Text) || string.IsNullOrWhiteSpace(customTextbox2.Text) || string.IsNullOrWhiteSpace(customTextbox7.Text))
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
    }
}