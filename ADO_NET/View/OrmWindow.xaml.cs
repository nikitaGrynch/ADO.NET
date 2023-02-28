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
                cmd.CommandText = "SELECT D.* FROM Departments D";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Departments.Add(
                    new Entity.Department(reader));
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
                cmd.CommandText = "SELECT M.* FROM Managers M WHERE M.FireDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(new(reader));
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
                        if (dialog.EditedManager == null)  // Удаление
                        {
                            String sql = "UPDATE Managers M SET FireDt = CURRENT_TIMESTAMP WHERE M.Id = @id";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", manager.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Удаление: " + manager.Surname);
                                this.Managers.Remove(manager);
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show(
                                    ex.Message,
                                    "Fire error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Stop);
                            }
                            cmd.Dispose();

                        }
                        else  // Сохранение
                        {
                            String sql = "UPDATE Managers M SET Name = @name, Surname = @surname, Secname = @secname, Id_main_dep = @idMainDep, Id_sec_dep = @idSecDep, Id_chief = @idChief WHERE M.Id = @id;";
                            using MySqlCommand cmd = new(sql, _connection);
                            cmd.Parameters.AddWithValue("@id", dialog.EditedManager.Id);
                            cmd.Parameters.AddWithValue("@name", dialog.EditedManager.Name);
                            cmd.Parameters.AddWithValue("@surname", dialog.EditedManager.Surname);
                            cmd.Parameters.AddWithValue("@secname", dialog.EditedManager.Secname);
                            cmd.Parameters.AddWithValue("@idMainDep", dialog.EditedManager.IdMainDep);
                            cmd.Parameters.AddWithValue("@idSecDep", dialog.EditedManager.IdSecDep);
                            cmd.Parameters.AddWithValue("@idChief", dialog.EditedManager.IdChief);
                            try
                            {
                                int index = this.Managers.IndexOf(manager);
                                this.Managers.Remove(manager);
                                this.Managers.Insert(index, manager);
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
