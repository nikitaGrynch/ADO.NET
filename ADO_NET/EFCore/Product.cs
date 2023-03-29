using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.EFCore
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }

        ///////////////////////////// NAVIGATION PROPERTIES ////////////////////////////

        public List<Sale> Sales { get; set; }
        public ICollection<Manager> Managers { get; set; }
    }
}
