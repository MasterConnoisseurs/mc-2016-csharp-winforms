using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;
using MC.LaundryShop.App.Helper;

namespace MC.LaundryShop.App.Forms
{
    public partial class ChangePasswordForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        private readonly long? _userId;

        public ChangePasswordForm(long? userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private bool UpdateData()
        {
            var result = false;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var password = customTextbox5.Text;

                    const string query = "UPDATE employees SET " +
                                         "password_hash = @password, " +
                                         "modified_by = @modified_by, " +
                                         "modified_datetime = @modified_datetime " +
                                         "WHERE id = @id;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        var currentDateTime = DateTime.Now;
                        command.Parameters.AddWithValue("@id", _userId);
                        command.Parameters.AddWithValue("@password", StaticFunctions.ComputeSha256Hash(password));
                        command.Parameters.AddWithValue("@modified_by", _userId);
                        command.Parameters.AddWithValue("@modified_datetime", currentDateTime);
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

        private void Submit_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(customTextbox1.Text) || string.IsNullOrWhiteSpace(customTextbox5.Text) || string.IsNullOrWhiteSpace(customTextbox2.Text))
            {
                MessageBox.Show(@"Please complete required details marked with red *.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (customTextbox5.Text != customTextbox2.Text)
            {
                MessageBox.Show(@"Invalid password confirmation.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT password_hash FROM employees WHERE id = @id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", _userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetString(reader.GetOrdinal("password_hash")) == StaticFunctions.ComputeSha256Hash(customTextbox1.Text)) continue;
                                MessageBox.Show(@"Invalid current Password.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!UpdateData())
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}