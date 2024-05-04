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
            string query = "SELECT [Values].X AS [369 нм], [Values].Y1 AS [463 нм], [Values].Y2 AS [530 нм], [Values].Y3 AS [572 нм], [Values].Y4 AS [627 нм], Cities.CityName COLLATE Cyrillic_General_CI_AS AS [Город], Months.Month COLLATE Cyrillic_General_CI_AS AS [Месяц] " +
                "FROM [Values] " +
                "LEFT JOIN Cities ON [Values].City = Cities.Id " +
                "LEFT JOIN Months ON [Values].Month = Months.Id; ";
            

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
      

        private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        {
            Window ResultWindow = new ResultWindow();
            ResultWindow.Show();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Window AddDataWindow = new AddDataWindow();
            AddDataWindow.Show();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}