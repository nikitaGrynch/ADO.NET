using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.Entity
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ManagerId { get; set; }
        public Int32 Cnt { get; set; }
        public DateTime SaleDt { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Sale()
        {
            Id = Guid.NewGuid();
            Cnt = 1;
            SaleDt = DateTime.Now;
        }
        public Sale(MySqlDataReader reader)
        {
            Id = reader.GetGuid(nameof(Id));
            ProductId = reader.GetGuid("ProductId");
            ManagerId = reader.GetGuid("ManagerId");
            Cnt = reader.GetInt32("Cnt");
            SaleDt = reader.GetDateTime("SaleDt");
            DeleteDt = reader.IsDBNull("DeleteDt") ? null : reader.GetDateTime("DeleteDt");
        }
    }
}
