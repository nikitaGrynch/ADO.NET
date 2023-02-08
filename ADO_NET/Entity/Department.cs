using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.Entity
{
    public class Department                 // Entity - класс, который отражает таблицу БД
    {                                       // данная сущность - для таблицы Department
        public Guid Id { get; set; }        // Отображение поля Id UNIQUEIDENTIFIER
        public String Name { get; set; }    // Отображение поля Name VARCHAR
    }
}
