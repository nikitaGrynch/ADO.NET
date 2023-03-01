using ADO_NET.DAL;
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

namespace ADO_NET.View
{
    /// <summary>
    /// Interaction logic for DalWindow.xaml
    /// </summary>
    public partial class DalWindow : Window
    {
        public ObservableCollection<Entity.Department> DepartmentsList { get; set; }
        public ObservableCollection<Entity.Manager> ManagersList { get; set; }
        private readonly DataContext _context;
        public DalWindow()
        {
            InitializeComponent();
            _context = new();
            DepartmentsList = new(_context.Departments.GetAll());
            ManagersList = new(_context.Managers.GetAll());
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_context.Departments.GetAll().Count.ToString());
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    CrudDepartmentWindow dialog = new(department);
                    if (dialog.ShowDialog() == true)  // подтвержденное дейсвтие
                    {
                        if (dialog.EditedDepartment is null)  // действие удаления
                        {
                            DepartmentsList.Remove(department);
                            MessageBox.Show("Remove: " + department.Name);
                        }
                        else  // дейсвтие сохранения
                        {
                            int index = DepartmentsList.IndexOf(department);
                            DepartmentsList.Remove(department);
                            DepartmentsList.Insert(index, department);
                            MessageBox.Show("Update: " + department.Name);
                        }
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
            dialog.ShowDialog();
        }

        private void CreateManagerButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ManagerItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                
            }
        }
    }
}
