using ADO_NET.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ADO_NET.View
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {
        private EfContext efContext;
        private ICollectionView DepartmentsListView;    // интерфейс для лоступа к DepartmensList с возможностью изменения
        public EfWindow()
        {
            InitializeComponent();
            efContext = new();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            efContext.Departments.Load();
            DepartmentsList.ItemsSource = efContext.Departments.Local.ToObservableCollection();
            // после связывания коллекции и представления: получаем интерфейс ICollectionView
            DepartmentsListView = CollectionViewSource.GetDefaultView(DepartmentsList.ItemsSource);
            // реализуем фильтр: через этот интерфейс
            DepartmentsListView.Filter = // Predicate<Object>
                obj => (obj as Department)?.DeleteDt == null;   // TODO: Replace with HideDeletedDepartmentsFilter
            UpdateMonitor();
        }

        private void UpdateMonitor()
        {
            MonitorTextBlock.Text = "\nDepartments: " + efContext.Departments.Count();
            MonitorTextBlock.Text += "\nProducts: " + efContext.Managers.Count();
            MonitorTextBlock.Text += "\nManagers: " + efContext.Products.Count();
            MonitorTextBlock.Text += "\nSales: " + efContext.Sales.Count();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (sender is ListViewItem item)
            {
                if (item.Content is EFCore.Department department)
                {

                    CrudDepartmentWindow dialog = new(new Entity.Department() { Id = department.Id, Name = department.Name});
                    if (dialog.ShowDialog() == true)  // подтвержденное дейсвтие
                    {
                        if (dialog.EditedDepartment is null)  // действие удаления
                        {
                            efContext.Departments.Remove(department);
                            department = new() { Id = department.Id, Name = department.Name, DeleteDt = DateTime.Now };
                            MessageBox.Show("Remove: " + department.Name);
                        }
                        else  // дейсвтие сохранения
                        {
                            efContext.Departments.Remove(department);
                            department = new() { Id = department.Id, Name = dialog.EditedDepartment.Name};
                            MessageBox.Show("Update: " + department.Name);
                        }
                        efContext.Departments.Add(department);
                        efContext.SaveChanges();
                    }
                    else  // окно закрыто или нажата кнопка "Cancel"
                    {
                        MessageBox.Show("Действие отменено");
                    }
                }
            }
            
        }

        private void CreateDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            if(dialog.ShowDialog() == true)
            {
                efContext.Departments.Add(
                    new Department()
                    {
                        Id = dialog.EditedDepartment.Id,
                        Name = dialog.EditedDepartment.Name,
                    });
                efContext.SaveChanges();
            }
        }

        private bool HideDeletedDepartmentsFilter(object item)
        {
            if(item is Department department)
            {
                return department.DeleteDt is null;
            }
            return false;
        }

        private void ShowDeletedDepartmentsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = null;
            ((GridView)DepartmentsList.View).Columns[2].Width = Double.NaN; // автоширина
        }

        private void ShowDeletedDepartmentsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DepartmentsListView.Filter = HideDeletedDepartmentsFilter;
            ((GridView)DepartmentsList.View).Columns[2].Width = 0;
        }
    }
}
