using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ADO_NET.Entity;
using MySql.Data.MySqlClient;

namespace ADO_NET
{
    /// <summary>
    /// Interaction logic for OrmWindow.xaml
    /// </summary>
    public partial class OrmWindow : Window
    {
        private MySqlConnection _connection;
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Products> Products { get; set; }
        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Products = new();
            DataContext = this;  // место поиска {Binding Departments}
            _connection = new(App.ConnectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                MySqlCommand cmd = new() { Connection = _connection };

                #region Load Departments
                cmd.CommandText = "SELECT Id, Name FROM Departments D";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(
                    new Entity.Department
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                    });
                    #endregion
                }
                reader.Close();

                #region Load Products
                cmd.CommandText = "SELECT Id, Name, Price FROM Products P";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(
                    new Entity.Products
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Price = Math.Round(reader.GetDouble(2), 2)
                    });
                    #endregion
                }
                reader.Close();

                cmd.Dispose();



            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Window will be closed", MessageBoxButton.OK, MessageBoxImage.Stop);
                this.Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)
            {
                if(item.Content is Entity.Department department)
                {
                    MessageBox.Show(department.Name);
                }
            }
        }

        private void ListViewItem_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Products product)
                {
                    MessageBox.Show(product.Name);
                }
            }
        }
    }
}
