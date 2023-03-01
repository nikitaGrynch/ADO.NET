using ADO_NET.DAL;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.Entity
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public String Secname { get; set; }
        public Guid IdMainDep { get; set; }  // NOT NULL
        public Guid? IdSecDep { get; set; }  // NULL
        public Guid? IdChief { get; set; }   // NULL
        public DateTime? FireDt { get; set; } // NULL

        public Manager()
        {
            Id = Guid.NewGuid();
            Name = null!;
            Surname = null!;
            Secname = null!;
            FireDt= null!;
        }

        public Manager(MySqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Name = reader.GetString("Name");
            Surname = reader.GetString("Surname");
            Secname = reader.GetString("Secname");
            IdMainDep = reader.GetGuid("Id_main_dep");
            IdSecDep = reader.IsDBNull("Id_sec_dep") ? null : reader.GetGuid("Id_sec_dep");
            IdChief = reader.IsDBNull("Id_chief") ? null : reader.GetGuid("Id_chief");
            FireDt = reader.IsDBNull("FireDt") ? null : reader.GetDateTime("FireDt");
        }

        ///////////////////// NAVIGATION PROPERTIES /////////////////////

        internal DataContext? _dataContext { get; set; } // зависимость - ссылка на контекст данных
        public Department? MainDep  // навигационное свойство
        {
            get
            {
                return _dataContext?
                    .Departments
                    .GetById(this.IdMainDep);
            }
        }

        public Department? SecDep
        {
            get
            {
                return _dataContext?
                    .Departments
                    .GetById(this.IdSecDep);
            }
        }

        public Manager? Chief
        {
            get
            {
                return _dataContext?
                    .Managers
                    .GetAll()
                    .Find(m => m.Id == this.IdChief);
            }
        }

    }
}
