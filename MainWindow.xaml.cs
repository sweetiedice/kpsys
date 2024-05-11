using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


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
            FillComboBoxes();
        }
        double sumX = 0;
        double sumY1 = 0;


        double sumMultX_Y1 = 0;


        double sumSqX = 0;





        int count = 0;

        double sumPX_Y1 = 0;
        readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tonik\\source\\repos\\kpsys\\kpsys_database.mdf;Integrated Security=True;";
        private DataTable dataTable;
        private void LoadData()
        {
            dataTable = new DataTable();
            string query = "SELECT [Values].ID, [Values].X AS [369 нм], [Values].Y1 AS [463 нм], [Values].Y2 AS [530 нм], [Values].Y3 AS [572 нм], [Values].Y4 AS [627 нм], Cities.CityName COLLATE Cyrillic_General_CI_AS AS [Город], Months.Month COLLATE Cyrillic_General_CI_AS AS [Месяц] " +
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

            // Создаем объект подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Открываем соединение
                connection.Open();

                // Создаем объект команды
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    // Выполняем команду и получаем считыватель данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {


                        // Проверяем, есть ли данные
                        if (reader.HasRows)
                        {
                            // Обрабатываем каждую строку данных
                            while (reader.Read())
                            {

                                int xValue = reader.GetInt32(1);
                                int y1Value = reader.GetInt32(2);
                                sumMultX_Y1 += xValue * y1Value;
                                sumSqX += Math.Pow(xValue, 2);
                                sumX += xValue;
                                sumY1 += y1Value;
                                count++;
                            }
                        }
                        reader.Close();
                    }

                    double avgX = count > 0 ? Math.Round(sumX / count, 1) : 0;
                    double avgY1 = count > 0 ? Math.Round(sumY1 / count, 1) : 0;

                    double BetaX_Y1 = Math.Round((count * sumMultX_Y1 - sumX * sumY1) / (count * sumSqX - Math.Pow(sumX, 2)), 2);      
                    double AlphaX_Y1 = Math.Round(((sumY1 - (BetaX_Y1 * sumX)) / count), 5);

                    // Выполняем команду и получаем считыватель данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем значения X и Y из столбцов
                            int xValue = reader.GetInt32(1);
                            int y1Value = reader.GetInt32(2);

                            sumPX_Y1 += xValue * BetaX_Y1 + AlphaX_Y1;
                        }
                        reader.Close();
                    }
                    double PavgX_Y1 = Math.Round(sumPX_Y1 / count, 3);

                    string Rating =
                    PavgX_Y1 >= 90 ? "Отличная" :
                    PavgX_Y1 >= 70 ? "Хорошая" :
                    PavgX_Y1 >= 50 ? "Умеренная" :
                    PavgX_Y1 >= 30 ? "Плохая" :
                                     "Очень плохая";
                    ResultLabel.Content = $"{Rating}";

                    switch (Rating)
                    {
                        case "Отличная":
                            ResultLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                            break;
                        case "Хорошая":
                            ResultLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                            break;
                        case "Умеренная":
                            ResultLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 165, 0));
                            break;
                        case "Плохая":
                            ResultLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                            break;
                        case "Очень плохая":
                            ResultLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 139, 0, 0));
                            break;
                        default:
                            ResultLabel.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                    }
                }
            }
        }
      

        private void ButtonGetResults_Click(object sender, RoutedEventArgs e)
        {
            Window ResultWindow = new ResultWindow();
            ResultWindow.Show();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        public class City
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class Month
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        private void FillComboBoxes()
        {
            try
            {
                // Города
                List<City> cities = GetCitiesFromDatabase();
                comboBoxCity.ItemsSource = cities;
                comboBoxCity.DisplayMemberPath = "Name"; // Отображаемое текстовое значение
                comboBoxCity.SelectedValuePath = "ID"; // Значение ID

                // Месяцы
                List<Month> months = GetMonthsFromDatabase();
                comboBoxMonth.ItemsSource = months;
                comboBoxMonth.DisplayMemberPath = "Name"; // Отображаемое текстовое значение
                comboBoxMonth.SelectedValuePath = "ID"; // Значение ID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных для ComboBox'ов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<City> GetCitiesFromDatabase()
        {
            List<City> cities = new List<City>();

            string query = "SELECT * FROM Cities"; // Запрос для получения данных о городах

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string name = reader["CityName"].ToString();

                        City city = new City { ID = id, Name = name };
                        cities.Add(city);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных о городах: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return cities;
        }

        private List<Month> GetMonthsFromDatabase()
        {
            List<Month> months = new List<Month>();

            string query = "SELECT * FROM [Months]"; // Запрос для получения данных о месяцах

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string name = reader["Month"].ToString();

                        Month month = new Month { ID = id, Name = name };
                        months.Add(month);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных о месяцах: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return months;
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
                command.Parameters.AddWithValue("@City", comboBoxCity.SelectedValue);
                command.Parameters.AddWithValue("@Month", comboBoxMonth.SelectedValue);

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
            {
                try
                {
                    if (MessageBox.Show("Удалить выбранную запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DataRowView row = (DataRowView)dataGrid.SelectedItem;
                        int id = (int)row["ID"];
                        DeleteRecord(id);
                    }
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
