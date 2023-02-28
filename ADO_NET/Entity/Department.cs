using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.Entity
{
    public class Department                 // Entity - класс, который отражает таблицу БД
    {                                       // данная сущность - для таблицы Department
        public Department(DbDataReader reader)
        {
            Id = reader.GetGuid(0);
            Name = reader.GetString(1);
            DeleteDt = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
        }
        public Guid Id { get; set; }        // Отображение поля Id UNIQUEIDENTIFIER
        public String Name { get; set; }    // Отображение поля Name VARCHAR
        public DateTime? DeleteDt { get; set; }
    }
}
