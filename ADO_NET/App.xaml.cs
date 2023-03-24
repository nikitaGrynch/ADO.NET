using ADO_NET.Service;
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
        public static readonly string ConnectionString = @"Server=eu-central.connect.psdb.cloud;Database=ado_net;user=1yf05zn5pm3w42a22kbb;password=pscale_pw_aDqNymHidJf49RlwZ6B4MvmAtFXeHAHbKQyzHDwUmDh;SslMode=VerifyFull;";
        // "Database=freedb_ADOnet;Datasource=sql.freedb.tech;User=freedb_daniiladmin;Password=k6rraNKpr@ZyJrP"
        internal static readonly ILogger Logger = new FileLogger();
        
    }
}
