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
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Products = new();
            Managers = new();
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

                #region Load Managers
                cmd.CommandText = "SELECT M.Id, M.Surname, M.Name, M.Secname, M.Id_main_Dep, M.Id_sec_dep, M.Id_chief FROM Managers M";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(
                    new Entity.Manager
                    {
                        Id = reader.GetGuid(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Secname = reader.GetString(3),
                        IdMainDep = reader.GetGuid(4),
                        IdSecDep = reader.GetValue(5) == DBNull.Value ? null : reader.GetGuid(5),
                        IdChief = reader.IsDBNull(6) ? null : reader.GetGuid(6)
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
                    // MessageBox.Show(department.Name);
                    CrudDepartmentWindow dialog = new(department);
                    if (dialog.ShowDialog() == true)  // подтвержденное действие
                    {
                        if(dialog.EditedDepartment == null)  // Удаление
                        {
                            MessageBox.Show("Удаление: " + department.Name);
                            this.Departments.Remove(department);
                        }
                        else  // Сохранение
                        {
                            int index = this.Departments.IndexOf(department);
                            this.Departments.Remove(department);
                            this.Departments.Insert(index, department);
                            MessageBox.Show("Обновление: " + department.Name);
                        }
                    }
                    else  // окно закрыто или нажата кнопка Cancel
                    {
                        MessageBox.Show("Действие отменено");
                    }
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

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    MessageBox.Show(manager.Surname);
                }
            }
        }

        private void CreateDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            dialog.Show();
        }
    }
}
