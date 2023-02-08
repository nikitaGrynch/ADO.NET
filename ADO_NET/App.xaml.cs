using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ADO_NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string ConnectionString = @"Database=freedb_ADOnet;Datasource=sql.freedb.tech;User=freedb_daniiladmin;Password=k6rraNKpr@ZyJrP";
    }
}
