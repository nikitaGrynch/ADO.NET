using ADO_NET.EFCore;
using ADO_NET.Entity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X500;
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
        private static readonly Random random = new();
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
                obj => (obj as EFCore.Department)?.DeleteDt == null;   // TODO: Replace with HideDeletedDepartmentsFilter
            UpdateMonitor();
            UpdateDailyStatistics();
        }

        private void UpdateMonitor()
        {
            MonitorTextBlock.Text = "\nDepartments: " + efContext.Departments.Count();
            MonitorTextBlock.Text += "\nProducts: " + efContext.Managers.Count();
            MonitorTextBlock.Text += "\nManagers: " + efContext.Products.Count();
            MonitorTextBlock.Text += "\nSales: " + efContext.Sales.Count();
        }

        private void UpdateDailyStatistics()
        {
            /*
            // Общее кол-во продаж (чеков) за сегодня
            SalesChecks.Content = "0";
            // Общее кол-во проданных товаров (сумма)
            SalesCnt.Content = "0";
            // Фактическое время старта продаж
            StartMoment.Content = "00:00:00";
            // Время последней продажи
            FinishMoment.Content = "00:00:00";
            // Максимальное кол-во товаров в одном чеке
            MaxCheckCnt.Content = "0";
            // Срденее кол-во проданых товаров на один чек
            AvgCheckCnt.Content = "0.0";
            // Возвраты - удаленные чеки
            DeletedCheckCnt.Content = "0";
            */

            DateTime date = new DateTime(2023, 3, 10);
            var dailySales = efContext.Sales.Where(sale => sale.SaleDt.Date == date.Date).ToList();
            SalesChecks.Content = dailySales.Count().ToString();

            int totalProducts = 0;
            foreach (var sale in dailySales)
            {
                totalProducts += sale.Cnt;
            }
            SalesCnt.Content = totalProducts.ToString();

            var minTime = dailySales.MinBy(sale => sale.SaleDt.TimeOfDay);
            StartMoment.Content = minTime?.SaleDt.TimeOfDay.ToString() ?? "------";

            var maxTime = dailySales.MaxBy(sale => sale.SaleDt.TimeOfDay);
            FinishMoment.Content = maxTime?.SaleDt.TimeOfDay.ToString() ?? "------";

            int? maxCnt = dailySales.MaxBy(sale => sale.Cnt)?.Cnt ?? null;
            MaxCheckCnt.Content = maxCnt?.ToString() ?? "------";

            var avgCnt = dailySales.Average(sale => sale.Cnt);
            AvgCheckCnt.Content = Math.Round(avgCnt, 2);

            var deletedCnt = dailySales.Where(sale => sale.DeleteDt is not null).Count();
            DeletedCheckCnt.Content = deletedCnt.ToString();

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
                    new EFCore.Department()
                    {
                        Id = dialog.EditedDepartment.Id,
                        Name = dialog.EditedDepartment.Name,
                    });
                efContext.SaveChanges();
            }
        }

        private bool HideDeletedDepartmentsFilter(object item)
        {
            if(item is EFCore.Department department)
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

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            int manCnt = efContext.Managers.Count();
            int proCnt = efContext.Products.Count();
            double maxPrice = efContext.Products.Max(p => p.Price);
            for (int i = 0; i < 100; i++)
            {
                // случайный менеджер
                int randIndex = random.Next(manCnt);
                EFCore.Manager manager = efContext.Managers.Skip(randIndex).First();
                // MessageBox.Show(manager.Surname + " " + manager.Name);

                // случайный товар
                randIndex = random.Next(proCnt);
                EFCore.Product product = efContext.Products.Skip(randIndex).First();

                // случайное время продажи - в пределах двух дней
                DateTime moment = DateTime
                    .Today                          // сегодня 00:00
                    .AddHours(8).                   // начало продаж 08:00
                    AddSeconds(random.Next(43200)). // случайное время - интервал 12 часов       
                    AddDays(random.Next(2));        // случайный день - смещение дня (на 0 / -1)

                // случайное кол-во товаров - зависит от цены товара
                int cntLimit = (int)(100 * (1 - product.Price / maxPrice) + 2);
                int cnt = random.Next(cntLimit);

                // случайный DeleteDt - с вероятностью 1/50, значени - не меньше чем время продажи
                DateTime? deleteDt = random.Next(0, 2) > 0 ? moment.AddHours(random.Next(1, 48)) : null;

                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Cnt = cnt,
                    SaleDt = moment,
                    DeleteDt = deleteDt
                });
                efContext.SaveChanges();
                UpdateMonitor();
            }
        }
    }
}
