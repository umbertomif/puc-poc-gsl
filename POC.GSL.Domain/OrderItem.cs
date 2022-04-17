using POC.GSL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.GSL.Domain
{
    public class OrderItem
    {
        public string MerchandiseId { get; set; }
        public string DepositId { get; set; }
        public Double UnitaryValue { get; set; }
        public int Quantity { get; set; }
    }
}
