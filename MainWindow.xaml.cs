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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
                    MessageBox.Show("Ошибка при отображении данных: " + ex.Message);
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


            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (dataGrid.SelectedItem != null)
            {
                try
                {
                    DataRowView row = (DataRowView)dataGrid.SelectedItem;
                    int id = (int)row["ID"];
                    DeleteRecord(id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message);
                }
            }
        }

        private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        {
            Window ResultWindow = new ResultWindow();
            ResultWindow.Show();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Window AddDataWindow = new AddDataWindow();
            AddDataWindow.Show();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}