using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET.EFCore
{
    public class ManagerProduct
    {
        public int ManagerId { get; set; }
        public Manager Manager { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
