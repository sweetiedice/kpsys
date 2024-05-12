using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kpsys
{
    public static class DatabaseHelper
    {
        public static string GetConnectionString()
        {
            // Находим путь к директории приложения
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Формируем путь к файлу базы данных
            string databaseFilePath = Path.Combine(appDirectory, "Resourses", "Data", "kpsys_database.mdf");

            // Проверяем существует ли файл базы данных
            if (!File.Exists(databaseFilePath))
            {
                throw new FileNotFoundException("Файл базы данных kpsys_database.mdf не найден.");
            }

            // Создаем и возвращаем строку подключения
            return $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={databaseFilePath};Integrated Security=True";
        }
    }
}
