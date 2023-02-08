using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using MySql.Data.MySqlClient;

namespace ADO_NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // об'єкт-підключення, основа ADO
        private MySqlConnection _connection;
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            /*
            string host = "sql7.freemysqlhosting.net"; // Имя хоста
            string database = "sql7595012"; // Имя базы данных
            string user = "sql7595012"; // Имя пользователя
            string password = "qRQ5I3dUJb"; // Пароль пользователя
            */

            string conStr = App.ConnectionString;
            _connection = new();
            _connection.ConnectionString = conStr;
            // !! створення об'єкта не відкиває підключення
             }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try  // відкриваємо підключення
            {
                _connection.Open();
                // відображаємо статус підключення на вікні
                StatusConnection.Content = "Connected";
                StatusConnection.Foreground = Brushes.Green;
            }
            catch (MySqlException ex)
            {
                // відображаємо статус підключення на вікні
                StatusConnection.Content = "Disconnected";
                StatusConnection.Foreground = Brushes.Red;

                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowMonitor();
            ShowDepartmentsView();
            ShowProductsView();
            ShowManagersView();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        #region Запити без повернення результатів
        private void CreateDepartments_Click(object sender, RoutedEventArgs e)
        {
            // команда - ресурс для передачі SQL запиту до СУБД
            MySqlCommand cmd = new();
            // Обов'язкові параметри команди:
            cmd.Connection = _connection;  // відкрите підключення
            cmd.CommandText =              // та текст SQL запиту
                @"CREATE TABLE Departments(
                 Id          CHAR(36) NOT NULL PRIMARY KEY,
                 Name        VARCHAR(50) NOT NULL
             ) ENGINE = INNODB DEFAULT CHARSET = UTF8 ";
            /* MySql: CREATE TABLE Departments(
                 Id          CHAR(36) NOT NULL PRIMARY KEY,
                 Name        VARCHAR(50) NOT NULL
             )*/
            try
            {
                cmd.ExecuteNonQuery();  // NonQuery - без повернення рез-ту
                MessageBox.Show("Create OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Create error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();  // команда - unmanaged, потрібно вивільняти ресурс
        }


        private void CreateProducts_Click(object sender, RoutedEventArgs e)
        {
            String sql =
                @"CREATE TABLE Products (
	                Id			CHAR(36) NOT NULL PRIMARY KEY,
	                Name		VARCHAR(50) NOT NULL,
	                Price		FLOAT  NOT NULL
                    ) ENGINE = INNODB DEFAULT CHARSET = UTF8";
            using MySqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Create Products OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Create Products error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();
        }

        private void CreateManagers_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/CreateManagers.sql");
            using MySqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Create Managers OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Create Managers error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();
        }
        #endregion

        private void FillDepartment_Click(object sender, RoutedEventArgs e)
        {
            MySqlCommand cmd = new();
            cmd.Connection = _connection;
            cmd.CommandText =
                 @"INSERT INTO Departments 
                        ( Id, Name )
                  VALUES 
                    ( 'D3C376E4-BCE3-4D85-ABA4-E3CF49612C94',  N'IT отдел'            ), 
                    ( '131EF84B-F06E-494B-848F-BB4BC0604266',  N'Бухгалтерия'         ), 
                    ( '8DCC3969-1D93-47A9-8B79-A30C738DB9B4',  N'Служба безопасности' ), 
                    ( 'D2469412-0E4B-46F7-80EC-8C522364D099',  N'Отдел кадров'        ),
                    ( '1EF7268C-43A8-488C-B761-90982B31DF4E',  N'Канцелярия'          ), 
                    ( '415B36D9-2D82-4A92-A313-48312F8E18C6',  N'Отдел продаж'        ), 
                    ( '624B3BB5-0F2C-42B6-A416-099AAB799546',  N'Юридическая служба'  )";
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fill OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Fill error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();
            ShowMonitor();
        }

        private void FillProducts_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/FillProducts.sql");
            using MySqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fill Products OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Fill Products error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();
            ShowMonitor();
        }

        private void FillManagers_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/FillManagers.sql");
            using MySqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fill Managers OK");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Fill Managers error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();
            ShowMonitor();
        }

        #region Запросы с одним (скалярным) результатом

        private void ShowMonitorDepartments()
        {
            using MySqlCommand cmd = new("SELECT COUNT(1) FROM Departments", _connection);
            try
            {
                object res = cmd.ExecuteScalar(); // Выполнение запроса + возвращение
                // левого-верхнего результата из возврата табоицы
                // возвращает типизованные данные (число, ряд, дату-время, тд), но в форме объекта
                int cnt = Convert.ToInt32(res);
                StatusDepartments.Content = cnt.ToString();
            }
            catch(MySqlException ex) // ошибка запроса
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusDepartments.Content = "---";
            }
            catch(Exception ex) // остальные ошибки (приведение типов)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusDepartments.Content = "---";
            }

        }

        private void ShowMonitorProducts()
        {
            using MySqlCommand cmd = new("SELECT COUNT(1) FROM Products", _connection);
            try
            {
                object res = cmd.ExecuteScalar(); // Выполнение запроса + возвращение
                // левого-верхнего результата из возврата табоицы
                // возвращает типизованные данные (число, ряд, дату-время, тд), но в форме объекта
                int cnt = Convert.ToInt32(res);
                StatusProducts.Content = cnt.ToString();
            }
            catch (MySqlException ex) // ошибка запроса
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusProducts.Content = "---";
            }
            catch (Exception ex) // остальные ошибки (приведение типов)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusProducts.Content = "---";
            }

        }

        private void ShowMonitorManagers()
        {
            using MySqlCommand cmd = new("SELECT COUNT(1) FROM Managers", _connection);
            try
            {
                object res = cmd.ExecuteScalar(); // Выполнение запроса + возвращение
                // левого-верхнего результата из возврата табоицы
                // возвращает типизованные данные (число, ряд, дату-время, тд), но в форме объекта
                int cnt = Convert.ToInt32(res);
                StatusManagers.Content = cnt.ToString();
            }
            catch (MySqlException ex) // ошибка запроса
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusManagers.Content = "---";
            }
            catch (Exception ex) // остальные ошибки (приведение типов)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusManagers.Content = "---";
            }

        }

        private void ShowMonitor()
        {
            ShowMonitorDepartments();
            ShowMonitorProducts();
            ShowMonitorManagers();
        }

        #endregion


        #region Запросы с табличными результатами

        private void ShowDepartmentsView()
        {
            using MySqlCommand cmd = new("SELECT * FROM Departments", _connection);
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                String str = String.Empty;
                // передача данных по одному ряду
                while(reader.Read()) // считывает один ряд, если нет - false
                {
                    // ряд считывается в сам reader, данные из него можно достать
                    // а) через геттеры
                    // б) через индексаторы
                    String id = reader.GetString(0);
                    str += id.Substring(0, 4) + "..." + id.Substring(id.Length - 2)         // типизированный геттер (рекоммендовано)
                        + " "                                                               // 
                        + reader[1]                                                         // индексатор - object
                        + "\n";                                                             // отсчет от 0 по порядку полей в результате

                }
                ViewDepartments.Text = str;
                reader.Close(); // !! незакрытый reader блокирует другие комманды к БД
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }

        private void ShowProductsView()
        {
            // using MySqlCommand cmd = new("SELECT Id, Name, Price FROM Products", _connection);
            try
            {
                MySqlCommand cmd = new() { Connection= _connection };
                cmd.CommandText = new("SELECT Id, Name, Price FROM Products");
                MySqlDataReader reader = cmd.ExecuteReader();
                String str = String.Empty;
                while (reader.Read())
                {
                    String id = reader.GetGuid(0).ToString();
                    String name = reader.GetString(1);
                    double price = reader.GetDouble(2);
                    str += id.Substring(0, 4) + "..." + id.Substring(id.Length - 2) + " " + name + Math.Round(price, 2).ToString() + "\n";

                }
                ViewProducts.Text = str;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }

        private void ShowManagersView()
        {
            using MySqlCommand cmd = new("SELECT * FROM Managers JOIN Departments ON Id_main_dep = Departments.Id", _connection);
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                String str = String.Empty;
                while (reader.Read())
                {
                    String id = reader.GetGuid(0).ToString();
                    String lastName = reader.GetString(1);
                    String name = reader.GetString(2);
                    String secName = reader.GetString(3);
                    String department = reader.GetString(8);
                    str += id.Substring(0, 4) + "..." + id.Substring(id.Length - 2) + " " + lastName + " " + name.Substring(0, 1) + ". " + secName.Substring(0, 1) + ". " + department + "\n";

                }
                ViewManagers.Text = str;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }

        #endregion



    }
}
