using MC.LaundryShop.App.Helper;
using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Forms
{
    public partial class EmployeesForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _formId;
        private readonly long? _userId;

        public EmployeesForm(long? formId, long? userId)
        {
            InitializeComponent();
            _formId = formId;
            _userId = userId;   
            if (_formId.HasValue && userId.HasValue)
            {
                label3.Text = @"Update your employee record here.";
                submit_button.Text = @"Update";
                LoadData();
            }
            else
            {
                label3.Text = @"Save your employee record here.";
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
                    const string query = "SELECT user_id, position, password_hash, image, first_name, middle_name, last_name, suffix, email, contact_number, is_active FROM employees WHERE id = @id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _formId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return;
                            customTextbox7.Text = reader.GetString(reader.GetOrdinal("user_id"));
                            customTextbox8.Text = reader.GetString(reader.GetOrdinal("position"));
                            customTextbox9.Text = StaticFunctions.ComputeSha256Hash(reader.GetString(reader.GetOrdinal("password_hash")));

                            customTextbox1.Text = reader.GetString(reader.GetOrdinal("first_name"));
                            customTextbox5.Text = reader.IsDBNull(reader.GetOrdinal("middle_name"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("middle_name"));
                            customTextbox2.Text = reader.GetString(reader.GetOrdinal("last_name"));
                            customTextbox6.Text = reader.IsDBNull(reader.GetOrdinal("suffix"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("suffix"));
                            customTextbox3.Text = reader.IsDBNull(reader.GetOrdinal("email"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("email"));
                            customTextbox4.Text = reader.IsDBNull(reader.GetOrdinal("contact_number"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("contact_number"));
                            checkBox1.Checked = reader.GetBoolean(reader.GetOrdinal("is_active"));

                            var imageColumnIndex = reader.GetOrdinal("image");
                            if (reader.IsDBNull(imageColumnIndex)) return;
                            var imageData = (byte[])reader.GetValue(imageColumnIndex);
                            if (imageData == null || imageData.Length <= 0) return;
                            using (var ms = new MemoryStream(imageData))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
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
        private bool SaveData()
        {
            var result = false;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var userId = customTextbox7.Text.Trim();
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

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM employees WHERE (full_name = @full_name OR user_id = @user_id) AND is_deleted = FALSE;";
                    using (var checkCommand = new MySqlCommand(checkDuplicateQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@full_name", fullName);
                        checkCommand.Parameters.AddWithValue("@user_id", userId);
                        var existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            MessageBox.Show(Resources.Messages_Success_Duplicate, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    const string insertQuery = "INSERT INTO employees (user_id, position, password_hash, image, first_name, middle_name, last_name, suffix, full_name, email, contact_number, is_active, created_by, created_datetime, modified_by, modified_datetime) " +
                                              "VALUES (@user_personal_id, @position, @password, @image, @first_name, @middle_name, @last_name, @suffix, @full_name, @email, @contact_number, @is_active, @user_id, @current_date, @user_id, @current_date);";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        var position = customTextbox8.Text.Trim();
                        var password = StaticFunctions.ComputeSha256Hash(customTextbox9.Text.Trim());
                        byte[] imageData = null;
                        if (pictureBox1.Image != null)
                        {
                            using (var ms = new MemoryStream())
                            {
                                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                                imageData = ms.ToArray();
                            }
                        }
                        command.Parameters.AddWithValue("@user_personal_id", userId);
                        command.Parameters.AddWithValue("@position", position);
                        command.Parameters.AddWithValue("@password", password);
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
                        if (imageData != null)
                        {
                            command.Parameters.Add("@image", MySqlDbType.LongBlob, imageData.Length).Value = imageData;
                        }
                        else
                        {
                            command.Parameters.Add("@image", MySqlDbType.LongBlob).Value = DBNull.Value;
                        }
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

                    var userId = customTextbox7.Text.Trim();
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

                    // --- Duplicate Check ---
                    const string checkDuplicateQuery = "SELECT COUNT(*) FROM customers WHERE (full_name = @full_name OR user_id = @user_id) AND is_deleted = FALSE AND Id != @id ;";
                    using (var checkCommand = new MySqlCommand(checkDuplicateQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@full_name", fullName);
                        checkCommand.Parameters.AddWithValue("@id", _formId);
                        checkCommand.Parameters.AddWithValue("@user_id", userId);
                        var existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            MessageBox.Show(Resources.Messages_Success_Duplicate, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    const string query = "UPDATE employees SET " +
                                         "user_id = @user_personal_id, " +
                                         "position = @position, " +
                                         "password_hash = @password, " +
                                         "first_name = @first_name, " +
                                         "first_name = @first_name, " +
                                         "middle_name = @middle_name, " +
                                         "last_name = @last_name, " +
                                         "suffix = @suffix, " +
                                         "full_name = @full_name, " +
                                         "email = @email, " +
                                         "image = @image, " +
                                         "contact_number = @contact_number, " +
                                         "is_active = @is_active, " +
                                         "modified_by = @modified_by, " +
                                         "modified_datetime = @modified_datetime " +
                                         "WHERE id = @id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        var position = customTextbox8.Text.Trim();
                        var password = StaticFunctions.ComputeSha256Hash(customTextbox9.Text.Trim());
                        byte[] imageData = null;
                        if (pictureBox1.Image != null)
                        {
                            using (var ms = new MemoryStream())
                            {
                                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                                imageData = ms.ToArray();
                            }
                        }

                        command.Parameters.AddWithValue("@user_personal_id", userId);
                        command.Parameters.AddWithValue("@position", position);
                        command.Parameters.AddWithValue("@password", password);
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
                        if (imageData != null)
                        {
                            command.Parameters.Add("@image", MySqlDbType.LongBlob, imageData.Length).Value = imageData;
                        }
                        else
                        {
                            command.Parameters.Add("@image", MySqlDbType.LongBlob).Value = DBNull.Value;
                        }
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
            if (string.IsNullOrWhiteSpace(customTextbox1.Text) || string.IsNullOrWhiteSpace(customTextbox2.Text) || string.IsNullOrWhiteSpace(customTextbox8.Text) || string.IsNullOrWhiteSpace(customTextbox7.Text) || string.IsNullOrWhiteSpace(customTextbox9.Text))
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

        private void button3_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files (*.*)|*.*";
            openFileDialog.Title = @"Select an Image File";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                var imagePath = openFileDialog.FileName;
                pictureBox1.Image = Image.FromFile(imagePath);
                button2.Visible = true;
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
            button2.Visible = false;
        }
    }
}