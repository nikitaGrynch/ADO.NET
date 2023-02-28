using ADO_NET.Entity;
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
    internal class DepartmentApi     // DAO - Data Access Object
    {
        private readonly MySqlConnection _connection;
        private readonly ILogger _logger;
        public DepartmentApi(MySqlConnection connection)
        {
            _connection = connection;
            _logger = App.Logger;
        }

        public List<Department> GetAll()
        {
            var departments = new List<Department>();
            try
            {
                using MySqlCommand command = new("SELECT D.* FROM Departments D", _connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new(reader));
                }
                reader.Close();
            }
            catch (Exception ex)    // Logging - правильный способ обработки исключений
            { 
                _logger.Log(ex.Message, "SEVERE", this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
            }
            return departments;
        }
    }
}
