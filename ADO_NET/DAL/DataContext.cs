using ADO_NET.Service;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.DAL
{
    class DataContext
    {
        internal DepartmentApi Departments;
        private MySqlConnection _connection;
        public DataContext()
        {
            _connection = new(App.ConnectionString);
            try
            {
                _connection.Open();
            }
            catch(Exception ex)
            {
                App.Logger.Log(ex.Message, "SEVERE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
                throw new Exception("Data context init error. See logs for details");
            }
            Departments = new DepartmentApi(_connection);
        }
    }
}
