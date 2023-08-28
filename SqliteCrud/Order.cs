using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteCrud
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderDate { get; set; }
        public int CustomerID { get; set; }
        public double TotalCost { get; set; }
        public double TotalTax { get; set; }
    }
}
