using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kpsys
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tonik\\source\\repos\\kpsys\\kpsys_database.mdf;Integrated Security=True;";
        public AuthWindow()
        {
            InitializeComponent();
        }



        private bool AuthenticateUser(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        private void AuthenticateUserButton(object sender, RoutedEventArgs e)
        {
            {
                string username = login.Text;
                string password = passbox.Password;

                if (AuthenticateUser(username, password))
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Неверный логин или пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }

            }
        }

        private void PasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (eye_button.Visibility == Visibility.Visible)
            {
                eye_button.Visibility = Visibility.Collapsed;
                hide_button.Visibility = Visibility.Visible;
            }
            else
            {
                eye_button.Visibility = Visibility.Visible;
                hide_button.Visibility = Visibility.Collapsed;
            }

            // Если в данный момент отображается TextBox, меняем его обратно на PasswordBox.
            if (passbox.Visibility == Visibility.Visible)
            {
                passbox.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Visible;
                // Устанавливаем пароль из TextBox в PasswordBox
                passwordTextBox.Text = passbox.Password;
                // Устанавливаем фокус на TextBox и устанавливаем курсор в конец текста
                passwordTextBox.Focus();
                passwordTextBox.CaretIndex = passwordTextBox.Text.Length;
            }
            else
            {
                // Если в данный момент отображается PasswordBox, меняем его на TextBox.
                passwordTextBox.Visibility = Visibility.Collapsed;
                passbox.Visibility = Visibility.Visible;
                // Устанавливаем текст из PasswordBox в TextBox
                passbox.Password = passwordTextBox.Text;

            }



        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}

