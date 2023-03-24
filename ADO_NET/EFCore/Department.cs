using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.EFCore
{
    public class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime? DeleteDt { get; set; }

        ///////////////////////////// NAVIGATION PROPERTIES /////////////////////////////
        public List<Manager> MainWorkers { get; set; }
        public List<Manager> SecWorkers { get; set; }
    }


}
