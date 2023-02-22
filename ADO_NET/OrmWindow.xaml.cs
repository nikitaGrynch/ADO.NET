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
        public ObservableCollection<Entity.Product> Products { get; set; }
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public ObservableCollection<Entity.Sale> Sales { get; set; }
        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Products = new();
            Managers = new();
            Sales = new();
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
                cmd.CommandText = "SELECT P.* FROM Products P WHERE P.DeleteDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new(reader));
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

                #region Load Sales
                cmd.CommandText = "SELECT S.* FROM Sales S WHERE S.DeleteDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sales.Add(new(reader));
                }
                #endregion
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


        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Product product)
                {
                    // MessageBox.Show(department.Name);
                    CrudProductWindow dialog = new(product);
                    if (dialog.ShowDialog() == true)  // подтвержденное действие
                    {
                        if (dialog.EditedProduct == null)  // Удаление
                        {
                            String sql = "UPDATE Products P SET DeleteDt = CURRENT_TIMESTAMP WHERE P.Id = @id";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", product.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Удаление: " + product.Name);
                                this.Products.Remove(product);
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Delete error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();
                            
                        }
                        else  // Сохранение
                        {
                            int index = this.Products.IndexOf(product);
                            this.Products.Remove(product);
                            this.Products.Insert(index, product);
                            MessageBox.Show("Обновление: " + product.Name);
                        }
                    }
                    else  // окно закрыто или нажата кнопка Cancel
                    {
                        MessageBox.Show("Действие отменено");
                    }
                }
            }
        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    CrudManagerWindow dialog = new(manager) { Owner = this };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.DialogResult == true)
                        {

                        }
                    }
                }
            }
        }
        private void SalesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Sale sale)
                {
                    CrudSaleWindow dialog = new(sale) { Owner = this};
                    if (dialog.ShowDialog() == true)  // подтвержденное действие
                    {
                        if (dialog.Sale == null)  // Удаление
                        {
                            String sql = "UPDATE Sales S SET DeleteDt = CURRENT_TIMESTAMP WHERE S.Id = @id";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", sale.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Delete OK");
                                this.Sales.Remove(sale);
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Delete error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();

                        }
                        else  // Сохранение
                        {
                            String sql = "UPDATE Sales S SET ProductId = @productId, ManagerId = @managerId, Cnt = @cnt WHERE S.Id = @id;";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", dialog.Sale.Id);
                            cmd.Parameters.AddWithValue("@productId", dialog.Sale.ProductId);
                            cmd.Parameters.AddWithValue("@managerId", dialog.Sale.ManagerId);
                            cmd.Parameters.AddWithValue("@cnt", dialog.Sale.Cnt);
                            try
                            {
                                int index = this.Sales.IndexOf(sale);
                                this.Sales.Remove(sale);
                                this.Sales.Insert(index, sale);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Update OK");
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Update error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();
                        }
                    }
                    else  // окно закрыто или нажата кнопка Cancel
                    {
                        MessageBox.Show("Действие отменено");
                    }
                }
            }
        }

        private void CreateDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            dialog.ShowDialog();
        }

        private void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {
            CrudProductWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.EditedProduct is not null)
                {
                    String sql = "INSERT INTO Products (Id, Name, Price) VALUES(@id, @name, @price)";
                    using MySqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.EditedProduct.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.EditedProduct.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.EditedProduct.Price);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Create OK");
                        Products.Add(dialog.EditedProduct);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Create error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);
                    }
                    cmd.Dispose();
                }
            }
        }

        private void CreateSaleButton_Click(object sender, RoutedEventArgs e)
        {
            CrudSaleWindow dialog = new(null) { Owner = this };
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Sale is not null)
                {
                    String sql = "INSERT INTO Sales (Id, ProductId, ManagerId, Cnt, SaleDt) VALUES(@id, @productId, @managerId, @cnt, @saleDt)";
                    using MySqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Sale.Id);
                    cmd.Parameters.AddWithValue("@productId", dialog.Sale.ProductId);
                    cmd.Parameters.AddWithValue("@managerId", dialog.Sale.ManagerId);
                    cmd.Parameters.AddWithValue("@cnt", dialog.Sale.Cnt);
                    cmd.Parameters.AddWithValue("@saleDt", dialog.Sale.SaleDt);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Create OK");
                        Sales.Add(dialog.Sale);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Create error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Stop);
                    }
                    cmd.Dispose();
                }
            }
        }
    }
}
