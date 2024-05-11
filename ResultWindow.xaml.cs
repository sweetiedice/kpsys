using OxyPlot.Series;
using OxyPlot;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Windows.Controls;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Diagnostics;

namespace kpsys
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
            CalculationData();
        }

        readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\tonik\\source\\repos\\kpsys\\kpsys_database.mdf;Integrated Security=True;";

        // Запрос для извлечения данных из базы данных
        readonly string query = "SELECT * FROM [Values]";
        double sumX = 0;
        double sumY1 = 0;
        double sumY2 = 0;
        double sumY3 = 0;
        double sumY4 = 0;

        double sumMultX_Y1 = 0;
        double sumMultX_Y2 = 0;
        double sumMultX_Y3 = 0;
        double sumMultX_Y4 = 0;

        double sumMultY1_Y2 = 0;
        double sumMultY1_Y3 = 0;
        double sumMultY1_Y4 = 0;

        double sumMultY2_Y3 = 0;
        double sumMultY2_Y4 = 0;

        double sumMultY3_Y4 = 0;

        double sumSqX = 0;
        double sumSqY1 = 0;
        double sumSqY2 = 0;
        double sumSqY3 = 0;

        double sumSqdiffXavg_X = 0;

        double sumSqdiffY1avg_Y1 = 0;
        double sumSqdiffY2avg_Y2 = 0;
        double sumSqdiffY3avg_Y3 = 0;
        double sumSqdiffY4avg_Y4 = 0;

        double sumdiffX_Xavg_Y1 = 0;
        double sumdiffX_Xavg_Y2 = 0;
        double sumdiffX_Xavg_Y3 = 0;
        double sumdiffX_Xavg_Y4 = 0;

        double sumdiffY1_Y1avg_Y2 = 0;
        double sumdiffY1_Y1avg_Y3 = 0;
        double sumdiffY1_Y1avg_Y4 = 0;

        double sumdiffY2_Y2avg_Y3 = 0;
        double sumdiffY2_Y2avg_Y4 = 0;

        double sumdiffY3_Y3avgY4 = 0;

        double sumdiffX_Xavg = 0;
        double sumdiffY1_Y1avg = 0;

        double sumdiffY2_Y2avg = 0;

        double sumdiffY3_Y3avg = 0;
        double sumdiffY4_Y4avg = 0;

        int count = 0;

        double sumPX_Y1 = 0;
        double sumPX_Y2 = 0;
        double sumPX_Y3 = 0;
        double sumPX_Y4 = 0;

        double sumPY1_Y2 = 0;
        double sumPY1_Y3 = 0;
        double sumPY1_Y4 = 0;

        double sumPY2_Y3 = 0;
        double sumPY2_Y4 = 0;

        double sumPY3_Y4 = 0;

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалоговое окно сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Результаты"
            };

            // Открываем диалоговое окно и проверяем, был ли выбран файл для сохранения
            if (saveFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                string filePath = saveFileDialog.FileName;

                // Создаем документ PDF
                Document document = new Document();

                try
                {
                    // Создаем объект PdfWriter для записи в PDF-файл
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                    // Открываем документ для записи
                    document.Open();

                    // Список имен элементов, которые нужно добавлять в PDF
                    List<string> includedElementNames = new List<string> { "Grid1", "Grid2", "Grid3", "Grid4", "Grid5", "plotView", "functionPlotView" };
                    
                    foreach (var element in FindVisualChildren<FrameworkElement>(this))
                    {
                        // Проверяем, входит ли имя элемента в список включенных элементов
                        if (includedElementNames.Contains(element.Name))
                        {
                            // Создаем новую страницу перед каждым элементом
                            document.NewPage();

                            // Создаем рисунок элемента
                            using (MemoryStream stream = new MemoryStream())
                            {
                                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                                renderTargetBitmap.Render(element);
                                var encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                                encoder.Save(stream);
                                byte[] imageBytes = stream.ToArray();
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imageBytes);

                                // Добавляем изображение на страницу PDF
                                document.Add(img);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при экспорте в PDF: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Закрываем документ
                    document.Close();
                }

                
                MessageBox.Show("Экспорт в PDF успешно выполнен!", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start(filePath);
            }

        }

        // Метод для поиска всех дочерних элементов указанного типа в указанном элементе
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void CalculationData()
        {

            // Создаем модель графика

            var plotModel = new PlotModel
            {
                Title = "Точки"
            };

            // Создаем серию точек
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 5, Title = "Точка", MarkerFill = OxyColor.FromRgb(60, 100, 255) };


            var functionPlotModel = new PlotModel
            {
                Title = "y = Beta x + Alpha"
            };

            // Создаем серию для отображения функции y = beta * x + alpha
            var functionLineSeries = new LineSeries
            {
                StrokeThickness = 1,
                Color = OxyColors.Black
            };


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
                                int y2Value = reader.GetInt32(3);
                                int y3Value = reader.GetInt32(4);
                                int y4Value = reader.GetInt32(5);

                                scatterSeries.Points.Add(new ScatterPoint(xValue, y1Value));
                                scatterSeries.Points.Add(new ScatterPoint(y1Value, y2Value));
                                scatterSeries.Points.Add(new ScatterPoint(y2Value, y3Value));
                                scatterSeries.Points.Add(new ScatterPoint(y3Value, y4Value));

                                sumMultX_Y1 += xValue * y1Value;
                                sumMultX_Y2 += xValue * y2Value;
                                sumMultX_Y3 += xValue * y3Value;
                                sumMultX_Y4 += xValue * y4Value;

                                sumMultY1_Y2 += y1Value * y2Value;
                                sumMultY1_Y3 += y1Value * y3Value;
                                sumMultY1_Y4 += y1Value * y4Value;

                                sumMultY2_Y3 += y2Value * y3Value;
                                sumMultY2_Y4 += y2Value * y4Value;

                                sumMultY3_Y4 += y3Value * y4Value;

                                sumSqX += Math.Pow(xValue, 2);
                                sumSqY1 += Math.Pow(y1Value, 2);
                                sumSqY2 += Math.Pow(y2Value, 2);
                                sumSqY3 += Math.Pow(y3Value, 2);

                                sumX += xValue;
                                sumY1 += y1Value;
                                sumY2 += y2Value;
                                sumY3 += y3Value;
                                sumY4 += y4Value;
                                count++;

                                // Добавляем точку в серию


                            }
                        }
                        reader.Close();
                    }
                    // Добавляем серию к модели
                    plotModel.Series.Add(scatterSeries);

                    // Подключаем модель к контексту данных окна
                    plotView.Model = plotModel;

                    double avgX = count > 0 ? Math.Round(sumX / count, 1) : 0;
                    double avgY1 = count > 0 ? Math.Round(sumY1 / count, 1) : 0;
                    double avgY2 = count > 0 ? Math.Round(sumY2 / count, 1) : 0;
                    double avgY3 = count > 0 ? Math.Round(sumY3 / count, 1) : 0;
                    double avgY4 = count > 0 ? Math.Round(sumY4 / count, 1) : 0;


                    // Выполняем команду и получаем читатель данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем значения X и Y из столбцов
                            int xValue = reader.GetInt32(1);
                            int y1Value = reader.GetInt32(2);
                            int y2Value = reader.GetInt32(3);
                            int y3Value = reader.GetInt32(4);
                            int y4Value = reader.GetInt32(5);

                            sumSqdiffXavg_X += Math.Pow(avgX - xValue, 2);
                            sumSqdiffY1avg_Y1 += Math.Pow(avgY1 - y1Value, 2);
                            sumSqdiffY2avg_Y2 += Math.Pow(avgY2 - y2Value, 2);
                            sumSqdiffY3avg_Y3 += Math.Pow(avgY3 - y3Value, 2);
                            sumSqdiffY4avg_Y4 += Math.Pow(avgY4 - y4Value, 2);



                            sumdiffX_Xavg_Y1 += ((xValue - avgX) * (y1Value - avgY1));

                            sumdiffX_Xavg_Y2 += (xValue - avgX) * (y2Value - avgY2);
                            sumdiffX_Xavg_Y3 += (xValue - avgX) * (y3Value - avgY3);
                            sumdiffX_Xavg_Y4 += (xValue - avgX) * (y4Value - avgY4);

                            sumdiffY1_Y1avg_Y2 += (y1Value - avgY1) * (y2Value - avgY2);
                            sumdiffY1_Y1avg_Y3 += (y1Value - avgY1) * (y3Value - avgY3);
                            sumdiffY1_Y1avg_Y4 += (y1Value - avgY1) * (y4Value - avgY4);

                            sumdiffY2_Y2avg_Y3 += (y2Value - avgY2) * (y3Value - avgY3);
                            sumdiffY2_Y2avg_Y4 += (y2Value - avgY2) * (y4Value - avgY4);

                            sumdiffY3_Y3avgY4 += (y3Value - avgY3) * (y4Value - avgY4);

                            sumdiffX_Xavg += Math.Pow((xValue - avgX), 2);


                            sumdiffY1_Y1avg += Math.Pow((y1Value - avgY1), 2);

                            sumdiffY2_Y2avg += Math.Pow((y2Value - avgY2), 2);
                            sumdiffY3_Y3avg += Math.Pow((y3Value - avgY3), 2);
                            sumdiffY4_Y4avg += Math.Pow((y4Value - avgY4), 2);
                        }
                        reader.Close();
                    }


                    double R_X_Y1 = Math.Round(sumdiffX_Xavg_Y1 / Math.Sqrt(sumdiffX_Xavg * sumdiffY1_Y1avg), 2);

                    double R_X_Y2 = Math.Round(sumdiffX_Xavg_Y2 / Math.Sqrt(sumdiffX_Xavg * sumdiffY2_Y2avg), 2);
                    double R_X_Y3 = Math.Round(sumdiffX_Xavg_Y3 / Math.Sqrt(sumdiffX_Xavg * sumdiffY3_Y3avg), 2);
                    double R_X_Y4 = Math.Round(sumdiffX_Xavg_Y4 / Math.Sqrt(sumdiffX_Xavg * sumdiffY4_Y4avg), 2);

                    double R_Y1_Y2 = Math.Round(sumdiffY1_Y1avg_Y2 / Math.Sqrt(sumdiffY1_Y1avg * sumdiffY2_Y2avg), 2);
                    double R_Y1_Y3 = Math.Round(sumdiffY1_Y1avg_Y3 / Math.Sqrt(sumdiffY1_Y1avg * sumdiffY3_Y3avg), 2);
                    double R_Y1_Y4 = Math.Round(sumdiffY1_Y1avg_Y4 / Math.Sqrt(sumdiffY1_Y1avg * sumdiffY4_Y4avg), 2);

                    double R_Y2_Y3 = Math.Round(sumdiffY2_Y2avg_Y3 / Math.Sqrt(sumdiffY2_Y2avg * sumdiffY3_Y3avg), 2);
                    double R_Y2_Y4 = Math.Round(sumdiffY2_Y2avg_Y4 / Math.Sqrt(sumdiffY2_Y2avg * sumdiffY4_Y4avg), 2);

                    double R_Y3_Y4 = Math.Round(sumdiffY3_Y3avgY4 / Math.Sqrt(sumdiffY3_Y3avg * sumdiffY4_Y4avg), 2);

                    double BetaX_Y1 = Math.Round((count * sumMultX_Y1 - sumX * sumY1) / (count * sumSqX - Math.Pow(sumX, 2)), 2);
                    double BetaX_Y2 = Math.Round((count * sumMultX_Y2 - sumX * sumY2) / (count * sumSqX - Math.Pow(sumX, 2)), 2);
                    double BetaX_Y3 = Math.Round((count * sumMultX_Y3 - sumX * sumY3) / (count * sumSqX - Math.Pow(sumX, 2)), 2);
                    double BetaX_Y4 = Math.Round((count * sumMultX_Y4 - sumX * sumY4) / (count * sumSqX - Math.Pow(sumX, 2)), 2);

                    double AlphaX_Y1 = Math.Round(((sumY1 - (BetaX_Y1 * sumX)) / count), 5);
                    double AlphaX_Y2 = Math.Round(((sumY1 - (BetaX_Y2 * sumX)) / count), 5);
                    double AlphaX_Y3 = Math.Round(((sumY1 - (BetaX_Y3 * sumX)) / count), 5);
                    double AlphaX_Y4 = Math.Round(((sumY1 - (BetaX_Y4 * sumX)) / count), 5);

                    double D_X = Math.Round((sumSqdiffXavg_X) / (count - 1), 2);
                    double D_Y1 = Math.Round((sumSqdiffY1avg_Y1) / (count - 1), 2);
                    double D_Y2 = Math.Round((sumSqdiffY2avg_Y2) / (count - 1), 2);
                    double D_Y3 = Math.Round((sumSqdiffY3avg_Y3) / (count - 1), 2);
                    double D_Y4 = Math.Round((sumSqdiffY4avg_Y4) / (count - 1), 2);

                    double V_X = Math.Round(D_X / avgX, 3);
                    double V_Y1 = Math.Round(D_Y1 / avgY1, 3);
                    double V_Y2 = Math.Round(D_Y2 / avgY2, 3);
                    double V_Y3 = Math.Round(D_Y3 / avgY3, 3);
                    double V_Y4 = Math.Round(D_Y4 / avgY4, 3);

                    double G_X = Math.Round(V_X / Math.Sqrt(count), 3);
                    double G_Y1 = Math.Round(V_Y1 / Math.Sqrt(count), 3);
                    double G_Y2 = Math.Round(V_Y2 / Math.Sqrt(count), 3);
                    double G_Y3 = Math.Round(V_Y3 / Math.Sqrt(count), 3);
                    double G_Y4 = Math.Round(V_Y4 / Math.Sqrt(count), 3);

                    double BetaY1_Y2 = Math.Round((count * sumMultY1_Y2 - sumY1 * sumY2) / (count * sumSqY1 - (Math.Pow(sumY1, 2))), 2);
                    double BetaY1_Y3 = Math.Round((count * sumMultY1_Y3 - sumY1 * sumY3) / (count * sumSqY1 - (Math.Pow(sumY1, 2))), 2);
                    double BetaY1_Y4 = Math.Round((count * sumMultY1_Y4 - sumY1 * sumY4) / (count * sumSqY1 - (Math.Pow(sumY1, 2))), 2);

                    double AlphaY1_Y2 = Math.Round((sumY2 - (BetaY1_Y2 * sumY1)) / count, 2);
                    double AlphaY1_Y3 = Math.Round((sumY2 - (BetaY1_Y3 * sumY1)) / count, 2);
                    double AlphaY1_Y4 = Math.Round((sumY2 - (BetaY1_Y4 * sumY1)) / count, 2);

                    double BetaY2_Y3 = Math.Round((count * sumMultY2_Y3 - sumY2 * sumY3) / (count * sumSqY2 - (Math.Pow(sumY2, 2))), 2);
                    double BetaY2_Y4 = Math.Round((count * sumMultY2_Y4 - sumY2 * sumY4) / (count * sumSqY2 - (Math.Pow(sumY2, 2))), 2);

                    double AlphaY2_Y3 = Math.Round((sumY3 - (BetaY2_Y3 * sumY2)) / count, 2);
                    double AlphaY2_Y4 = Math.Round((sumY3 - (BetaY2_Y4 * sumY2)) / count, 2);

                    double BetaY3_Y4 = Math.Round((count * sumMultY3_Y4 - sumY3 * sumY4) / (count * sumSqY3 - (Math.Pow(sumY3, 2))), 2);
                    double AlphaY3_Y4 = Math.Round((sumY4 - (BetaY3_Y4 * sumY3)) / count, 2);

                    // Выполняем команду и получаем считыватель данных
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем значения X и Y из столбцов
                            int xValue = reader.GetInt32(1);
                            int y1Value = reader.GetInt32(2);
                            int y2Value = reader.GetInt32(3);
                            int y3Value = reader.GetInt32(4);
                            int y4Value = reader.GetInt32(5);

                            sumPX_Y1 += xValue * BetaX_Y1 + AlphaX_Y1;
                            sumPX_Y2 += xValue * BetaX_Y2 + AlphaX_Y2;
                            sumPX_Y3 += xValue * BetaX_Y3 + AlphaX_Y3;
                            sumPX_Y4 += xValue * BetaX_Y4 + AlphaX_Y4;

                            sumPY1_Y2 = y1Value * BetaY1_Y2 + AlphaY1_Y2;
                            sumPY1_Y3 = y1Value * BetaY1_Y3 + AlphaY1_Y3;
                            sumPY1_Y4 = y1Value * BetaY1_Y4 + AlphaY1_Y4;

                            sumPY2_Y3 = y2Value * BetaY2_Y3 + AlphaY2_Y3;
                            sumPY2_Y4 = y2Value * BetaY2_Y4 + AlphaY2_Y4;

                            sumPY3_Y4 = y3Value * BetaY3_Y4 + AlphaY3_Y4;


                        }
                        reader.Close();
                    }

                    double PavgX_Y1 = Math.Round(sumPX_Y1 / count, 3);
                    double PavgX_Y2 = Math.Round(sumPX_Y2 / count, 3);
                    double PavgX_Y3 = Math.Round(sumPX_Y3 / count, 3);
                    double PavgX_Y4 = Math.Round(sumPX_Y4 / count, 3);

                    double PavgY1_Y2 = Math.Round(sumPY1_Y2 / count, 3);
                    double PavgY1_Y3 = Math.Round(sumPY1_Y3 / count, 3);
                    double PavgY1_Y4 = Math.Round(sumPY1_Y4 / count, 3);

                    double PavgY2_Y3 = Math.Round(sumPY2_Y3 / count, 3);
                    double PavgY2_Y4 = Math.Round(sumPY2_Y4 / count, 3);

                    double PavgY3_Y4 = Math.Round(sumPY3_Y4 / count, 3);

                    double minX = scatterSeries.Points.Min(p => p.X);
                    double maxX = scatterSeries.Points.Max(p => p.X);
                    double step = Math.Round((maxX - minX) / 20); // Настраиваем количество точек для плавного отображения

                    for (double x = minX; x <= maxX; x += step)
                    {
                        double y = Math.Round(BetaX_Y1 * x + AlphaX_Y1);
                        functionLineSeries.Points.Add(new DataPoint(x, y));
                    }

                    // Добавляем серию функции к модели графика функции
                    functionPlotModel.Series.Add(functionLineSeries);

                    // Подключаем модель графика функции к контексту данных второго PlotView
                    functionPlotView.Model = functionPlotModel;

                    Ax369x463.Text = $"{AlphaX_Y1}";
                    Ax369x530.Text = $"{AlphaX_Y2}";
                    Ax369x572.Text = $"{AlphaX_Y3}";
                    Ax369x627.Text = $"{AlphaX_Y4}";

                    Ax463x530.Text = $"{AlphaY1_Y2}";
                    Ax463x572.Text = $"{AlphaY1_Y3}";
                    Ax463x627.Text = $"{AlphaY1_Y4}";

                    Ax530x572.Text = $"{AlphaY2_Y3}";
                    Ax530x627.Text = $"{AlphaY2_Y4}";

                    Ax572x627.Text = $"{AlphaY3_Y4}";

                    Bx369x463.Text = $"{BetaX_Y1}";
                    Bx369x530.Text = $"{BetaX_Y2}";
                    Bx369x572.Text = $"{BetaX_Y3}";
                    Bx369x627.Text = $"{BetaX_Y4}";

                    Bx463x530.Text = $"{BetaY1_Y2}";
                    Bx463x572.Text = $"{BetaY1_Y3}";
                    Bx463x627.Text = $"{BetaY1_Y4}";

                    Bx530x572.Text = $"{BetaY2_Y3}";
                    Bx530x627.Text = $"{BetaY2_Y4}";

                    Bx572x627.Text = $"{BetaY3_Y4}";

                    Px369x463.Text = $"{PavgX_Y1}";
                    Px369x530.Text = $"{PavgX_Y2}";
                    Px369x572.Text = $"{PavgX_Y3}";
                    Px369x627.Text = $"{PavgX_Y4}";

                    Px463x530.Text = $"{PavgY1_Y2}";
                    Px463x572.Text = $"{PavgY1_Y3}";
                    Px463x627.Text = $"{PavgY1_Y4}";

                    Px530x572.Text = $"{PavgY2_Y3}";
                    Px530x627.Text = $"{PavgY2_Y4}";

                    Px572x627.Text = $"{PavgY3_Y4}";

                    x369x463.Text = $"{R_X_Y1}";
                    x369x530.Text = $"{R_X_Y2}";
                    x369x572.Text = $"{R_X_Y3}";
                    x369x627.Text = $"{R_X_Y4}";
                    x463x530.Text = $"{R_Y1_Y2}";
                    x463x572.Text = $"{R_Y1_Y3}";
                    x463x627.Text = $"{R_Y1_Y4}";
                    x530x572.Text = $"{R_Y2_Y3}";
                    x530x627.Text = $"{R_Y2_Y4}";
                    x572x627.Text = $"{R_Y3_Y4}";

                    s369_xavg.Text = $"{avgX}";
                    s369_D.Text = $"{D_X}";
                    s369_V.Text = $"{V_X}";
                    s369_G.Text = $"{G_X}";

                    s463_xavg.Text = $"{avgY1}";
                    s463_D.Text = $"{D_Y1}";
                    s463_V.Text = $"{V_Y1}";
                    s463_G.Text = $"{G_Y1}";

                    s530_xavg.Text = $"{avgY2}";
                    s530_D.Text = $"{D_Y2}";
                    s530_V.Text = $"{V_Y2}";
                    s530_G.Text = $"{G_Y2}";

                    s572_xavg.Text = $"{avgY3}";
                    s572_D.Text = $"{D_Y3}";
                    s572_V.Text = $"{V_Y3}";
                    s572_G.Text = $"{G_Y3}";

                    s627_xavg.Text = $"{avgY4}";
                    s627_D.Text = $"{D_Y4}";
                    s627_V.Text = $"{V_Y4}";
                    s627_G.Text = $"{G_Y4}";
                }
            }
        }
    }
}
