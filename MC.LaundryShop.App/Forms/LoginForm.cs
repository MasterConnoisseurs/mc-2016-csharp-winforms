using System;
using System.Configuration;
using System.Windows.Forms;
using MC.LaundryShop.App.Class;
using MC.LaundryShop.App.Helper;
using MC.LaundryShop.App.Properties;
using MySql.Data.MySqlClient;

namespace MC.LaundryShop.App.Forms
{
    public partial class LoginForm : Form
    {
        
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        public UserDetails AuthenticatedUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            ActiveControl = login_button;
            login_button.Focus();
        }

        private void Login_Button_Click(object sender, EventArgs e)
        {
            
            user_id.Enabled = false;
            password.Enabled = false;
            login_button.Enabled = false;
            try
            {
                if (string.IsNullOrWhiteSpace(user_id.Text) || string.IsNullOrWhiteSpace(password.Text))
                {
                    MessageBox.Show(@"Please provide your username and password.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UserDetails currentUser = null;

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT id, first_name, middle_name, last_name, suffix, full_name, image, position FROM employees WHERE user_id = @user_id AND password_hash = @password_hash";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", user_id.Text);
                        command.Parameters.AddWithValue("@password_hash", StaticFunctions.ComputeSha256Hash(password.Text));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentUser = new UserDetails
                                {
                                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                    FullName = reader.GetString(reader.GetOrdinal("full_name")),
                                    Position = reader.GetString(reader.GetOrdinal("position"))
                                };
                                var middleNameOrdinal = reader.GetOrdinal("middle_name");
                                currentUser.MiddleName = reader.IsDBNull(middleNameOrdinal) ? null : reader.GetString(middleNameOrdinal);

                                var suffixOrdinal = reader.GetOrdinal("suffix");
                                currentUser.Suffix = reader.IsDBNull(suffixOrdinal) ? null : reader.GetString(suffixOrdinal);

                                var imageOrdinal = reader.GetOrdinal("image");
                                if (!reader.IsDBNull(imageOrdinal))
                                {
                                    currentUser.Image = (byte[])reader.GetValue(imageOrdinal);
                                }
                                else
                                {
                                    currentUser.Image = null;
                                }
                            }
                        }
                    }
                }

                if (currentUser != null)
                {
                    AuthenticatedUser = currentUser;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(@"Incorrect username or password.", Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show(Resources.Messages_Error_Fatal, Resources.Messages_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (DialogResult != DialogResult.OK)
                {
                    user_id.Enabled = true;
                    password.Enabled = true;
                    login_button.Enabled = true;
                }
            }
        }
    }
}