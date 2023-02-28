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
    /// Interaction logic for CrudManagerWindow.xaml
    /// </summary>
    public partial class CrudManagerWindow : Window
    {
        public Entity.Manager? EditedManager;
        public CrudManagerWindow(Entity.Manager? EditedManager)
        {
            InitializeComponent();
            this.EditedManager = EditedManager;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Owner;
            if(EditedManager is null)
            {
                EditedManager = new();  // id создается в конструкторе
                DeleteButton.IsEnabled= false;
            }
            else
            {
                SurnameText.Text = EditedManager.Surname;
                NameText.Text = EditedManager.Name;
                SecnameText.Text = EditedManager.Secname;
                if (Owner is OrmWindow owner)
                {
                    MainDepCombobox.SelectedItem = owner.Departments.FirstOrDefault(dep => dep.Id == EditedManager.IdMainDep);
                    SecDepCombobox.SelectedItem = owner.Departments.FirstOrDefault(dep => dep.Id == EditedManager.IdSecDep);
                    ChiefCombobox.SelectedItem = owner.Managers.FirstOrDefault(man => man.Id == EditedManager.IdChief);
                }
                else
                {
                    MessageBox.Show("Owner is not OrmWindow");
                }
            }
            IdText.Text = EditedManager.Id.ToString();

            if(EditedManager.IdSecDep == null) { RemoveSecDepButton.Visibility = Visibility.Hidden; };
            if(EditedManager.IdChief == null) { RemoveChiefButton.Visibility = Visibility.Hidden; };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.EditedManager is null)
            {
                return;
            }
            if (NameText.Text == String.Empty)
            {
                MessageBox.Show("Введите имя сотрудника");
                NameText.Focus();
                return;
            }
            if(SurnameText.Text == String.Empty)
            {
                MessageBox.Show("Введите фамилию сотрудника");
                SurnameText.Focus();
                return;
            }
            if(SecnameText.Text == String.Empty)
            {
                MessageBox.Show("Введите отчество сотрудника");
                SecnameText.Focus();
                return;
            }

            EditedManager.Name= NameText.Text;
            EditedManager.Surname= SurnameText.Text;
            EditedManager.Secname= SecnameText.Text;

            EditedManager.IdMainDep = (MainDepCombobox.SelectedItem as Entity.Department).Id;

            if (SecDepCombobox.SelectedItem == null)
            {
                EditedManager.IdSecDep = null;
            }
            else
            {
                EditedManager.IdSecDep = (SecDepCombobox.SelectedItem as Entity.Department).Id;
            }
            if (ChiefCombobox.SelectedItem == null)
            {
                EditedManager.IdChief = null;
            }
            else
            {
                EditedManager.IdChief = (ChiefCombobox.SelectedItem as Entity.Manager).Id;
            }
            this.DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.EditedManager= null;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RemoveSecDepButton_Click(object sender, RoutedEventArgs e)
        {
            EditedManager.IdSecDep = null;
            SecDepCombobox.SelectedItem = null;
            RemoveSecDepButton.Visibility = Visibility.Hidden;
        }

        private void RemoveChiefButton_Click(object sender, RoutedEventArgs e)
        {
            EditedManager.IdChief = null;
            ChiefCombobox.SelectedItem = null;
            RemoveChiefButton.Visibility = Visibility.Hidden;
        }


        private void SecDepCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveSecDepButton.Visibility = Visibility.Visible;
        }

        private void ChiefCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveChiefButton.Visibility = Visibility.Visible;
        }
    }
}
