using System;
using System.Collections.Generic;
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

namespace ADO_NET
{
    /// <summary>
    /// Interaction logic for CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        public Entity.Department EditedDepartment { get; private set; }
        public CrudDepartmentWindow(Entity.Department department)
        {
            InitializeComponent();
            this.EditedDepartment = department;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EditedDepartment != null)
            {
                IdText.Text = EditedDepartment.Id.ToString();
                NameText.Text = EditedDepartment.Name;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.EditedDepartment.Name = NameText.Text;
            this.DialogResult = true; // результат ShowDialog() = truw и закрывается окно
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.EditedDepartment = null;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
