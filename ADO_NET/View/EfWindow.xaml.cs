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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


            efContext.Managers.Load();
            ManagersList.ItemsSource = efContext.Managers.Local.ToObservableCollection();

            efContext.Sales.Load();
            SalesList.ItemsSource = efContext.Sales.Local.ToObservableCollection();

            efContext.Products.Load();
            ProductList.ItemsSource = efContext.Products.Local.ToObservableCollection();

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
            /*
            var query = efContext.Sales
                .Where(s => s.SaleDt.Date == DateTime.Today)
                .GroupBy(s => s.ProductId)
                .ToList()
                .Join(                          // Объединение с предыдущим (не с Sales, а с результатом)
                    efContext.Products,         // inner - с чем объединяем
                    grp => grp.Key,             // outer - outerKey - grp.Key == s.ProductId (ключ группировки)
                    p => p.Id,                  // innerKey - Products.Id (с чем совпадает outerKey)
                    (grp, p) => new             // resultSelector - что делать с парой (grp, p) в
                    {                           // которой совпадают grp.Key и p.Id :
                        Name = p.Name,          // создается объект анонимного типа с полями Name и Cnt
                        Cnt = grp.Count()
                    }
                 );
            */

            var products = efContext.Products
                .GroupJoin(efContext.Sales
                    .Where(
                        s => s.SaleDt.Date == DateTime.Today),
                        p => p.Id,
                        s => s.ProductId,
                        (p, sales) => new { Name = p.Name, checksCnt = sales.Count(), Price = p.Price, productsCnt = sales.Sum(p => p.Cnt)}
                    );
            foreach (var item in products)
            {
                LogBlock.Text += $"{item.Name} --- {item.checksCnt} шт. (чеков) --- {item.productsCnt} шт. (товаров) --- {Math.Round((item.Price * item.productsCnt), 2)} грн.\n";
            }

            var managers = efContext.Managers
                .GroupJoin(efContext.Sales
                .Where(
                    s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, s) => new {m.Name, m.Surname, m.Secname, Cnt = s.Count()}
                    );

            var bestMan1 = managers.OrderByDescending(m => m.Cnt).First();

            BestManager1.Content = bestMan1.Surname + " " + bestMan1.Name[0] + ". " + bestMan1.Secname[0] + ". ---" + bestMan1.Cnt + " шт.";

            var managers2 = efContext.Managers
                .GroupJoin(efContext.Sales
                .Where(
                    s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, s) => new {m.Name, m.Surname, m.Secname, Cnt = s.Sum(s => s.Cnt)}
                );

            var bestManagers = managers2.OrderByDescending(m => m.Cnt).Take(3).ToList();

            BestManagers.Content = "1 - " + bestManagers[0].Surname + " " + bestManagers[0].Name[0] + ". " + bestManagers[0].Secname[0] + ". ---" + bestManagers[0].Cnt + " шт." + "\n" +
                                    "2 - " + bestManagers[1].Surname + " " + bestManagers[1].Name[0] + ". " + bestManagers[1].Secname[0] + ". ---" + bestManagers[1].Cnt + " шт." + "\n" +
                                    "3 - " + bestManagers[2].Surname + " " + bestManagers[2].Name[0] + ". " + bestManagers[2].Secname[0] + ". ---" + bestManagers[2].Cnt + " шт." + "\n";


            var bestManagerGrn = efContext.Managers
                .GroupJoin(efContext.Sales
                .Where(
                    s => s.SaleDt.Date == DateTime.Today),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, s) => new {
                        Manager = m,
                        Grn = s.Join(efContext.Products, s => s.ProductId, p => p.Id, (s, p) => s.Cnt * p.Price).Sum()
                    }
                )
                .OrderByDescending(m => m.Grn)
                .First();

            var bestGrn = efContext.Managers
                .GroupJoin(efContext.Sales
                .Where(
                    s => s.SaleDt.Date == DateTime.Today)
                    .Join(efContext.Products, s => s.ProductId, p => p.Id, (s, p) => new {s.ManagerId, Summ = s.Cnt * p.Price}),
                    m => m.Id,
                    s => s.ManagerId,
                    (m, s) => new {
                        Manager = m,
                        Grn = s.Sum(s => s.Summ)
                    }
                )
                .OrderByDescending(m => m.Grn)
                .First();

            BestManagerGrn.Content = bestGrn.Manager.Surname + " " + bestGrn.Manager.Name[0] + ". " + bestGrn.Manager.Secname[0] + " ---- " + Math.Round(bestGrn.Grn, 2) + " грн.";

            var bestProduct1 = products.OrderByDescending(p => p.checksCnt).First();
            BestProduct1.Content = $"{bestProduct1.Name} = {bestProduct1.checksCnt} шт.";

            var bestProduct2 = products.OrderByDescending(p => p.checksCnt).First();
            BestProduct2.Content = $"{bestProduct2.Name} = {bestProduct2.productsCnt} шт.";

            var bestProduct3 = products.OrderByDescending(p => p.productsCnt * p.Price).First();
            BestProduct3.Content = $"{bestProduct3.Name} = {Math.Round((bestProduct3.productsCnt * bestProduct3.Price), 2)} грн.";
            DateTime date = new DateTime(2023, 3, 10);
            var dailySales = efContext.Sales.Where(sale => sale.SaleDt.Date == DateTime.Today).ToList();
            SalesChecks.Content = dailySales.Count().ToString();


            if(dailySales.Count() == 0)
            {
                return;
            }

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


            var departmentsStat = efContext.Managers.GroupJoin(efContext.Sales
                            .Where(
                                s => s.SaleDt.Date == DateTime.Today),
                                m => m.Id,
                                s => s.ManagerId,
                                (m, s) => new
                                {
                                    DepId = m.IdMainDep,
                                    Grn = s.Join(efContext.Products, s => s.ProductId, p => p.Id, (s, p) => s.Cnt * p.Price).Sum(),
                                    ProductsCnt = s.Sum(s => s.Cnt),
                                    ChecksCnt = s.Count()
                                })
                            .Join(efContext.Departments,
                                m => m.DepId,
                                d => d.Id,
                                (m, d) => new
                                {
                                    DepId = d.Id,
                                    DepName = d.Name,
                                    Grn = m.Grn,
                                    ProductsCnt = m.ProductsCnt,
                                    ChecksCnt = m.ChecksCnt
                                }
                            )
                            .GroupBy(d => d.DepName)
                            .Select(n => new
                            {
                                DepName = n.Key,
                                GrnSum = Math.Round(n.Sum(n => n.Grn),2),
                                ChecksCnt = n.Sum(n => n.ChecksCnt),
                                ProductsCnt = n.Sum(n => n.ProductsCnt)
                            })
                            .OrderByDescending(d => d.GrnSum);

            DepartmentsStatList.ItemsSource = departmentsStat.ToList();

        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (sender is ListViewItem item)
            {
                if (item.Content is EFCore.Department department)
                {

                    CrudDepartmentWindow dialog = new(new Entity.Department() { Id = department.Id, Name = department.Name });
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
                            department = new() { Id = department.Id, Name = dialog.EditedDepartment.Name };
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
            if (dialog.ShowDialog() == true)
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
            if (item is EFCore.Department department)
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
            }

            efContext.SaveChanges();
            UpdateMonitor();
            UpdateDailyStatistics();
        }
    }
}
