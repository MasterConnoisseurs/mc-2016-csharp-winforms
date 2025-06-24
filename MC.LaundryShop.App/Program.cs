using System;
using System.Windows.Forms;
using MC.LaundryShop.App.Class;
using MC.LaundryShop.App.Forms;

namespace MC.LaundryShop.App
{
    // Username : sa
    // Password : SecretPassword_00
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var splashScreen = new SplashScreen())
            {
                if (splashScreen.ShowDialog() != DialogResult.OK) return;
                while (true)
                {
                    UserDetails authenticatedUser;
                    using (var loginForm = new LoginForm())
                    {
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            authenticatedUser = loginForm.AuthenticatedUser;
                        }
                        else
                        {
                            return;
                        }
                    }

                    using (var mainForm = new MainForm(authenticatedUser))
                    {
                        var mainFormResult = mainForm.ShowDialog();

                        switch (mainFormResult)
                        {
                            case DialogResult.No:
                                break;
                            case DialogResult.OK:
                            case DialogResult.None:
                            case DialogResult.Cancel:
                            case DialogResult.Abort:
                            case DialogResult.Retry:
                            case DialogResult.Ignore:
                            case DialogResult.Yes:
                            default:
                                return;
                        }
                    }
                }
            }
        }
    }
}