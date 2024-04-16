using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kpsys
{
    /// <summary>
    /// Логика взаимодействия для addDataWindow.xaml
    /// </summary>
    public partial class AddDataWindow : Window
    {
        public AddDataWindow()
        {
            InitializeComponent();
            LoadData();
        }
        readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tonik\\source\\repos\\kpsys\\kpsys_database.mdf;Integrated Security=True;";
        private DataTable dataTable;
        private void LoadData()
        {
            dataTable = new DataTable();
            string query = "SELECT * FROM [Values]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отображении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DeleteRecord(int id)
        {
            string query = "DELETE FROM [Values] WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show("Выбранная запись удалена", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
            private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO [Values] (X, Y1, Y2, Y3, Y4, City, Month) VALUES (@X, @Y1, @Y2, @Y3, @Y4, @City, @Month)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@X", textBoxX.Text);
                command.Parameters.AddWithValue("@Y1", textBoxY1.Text);
                command.Parameters.AddWithValue("@Y2", textBoxY2.Text);
                command.Parameters.AddWithValue("@Y3", textBoxY3.Text);
                command.Parameters.AddWithValue("@Y4", textBoxY4.Text);
                command.Parameters.AddWithValue("@City", textBoxCity.Text);
                command.Parameters.AddWithValue("@Month", textBoxMonth.Text);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (dataGrid.SelectedItem != null)
            { try
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItem;
                    int id = (int)row["ID"];
                    DeleteRecord(id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                deleteButton.IsEnabled = false;
            }
            else
            {
                deleteButton.IsEnabled = true;
            }
        }
    }
}
