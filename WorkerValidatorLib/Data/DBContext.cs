using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerValidatorLib.Data
{
    public class DBContext
    {
        public readonly List<OrderModel> Orders  = new  List<OrderModel>();
        public void Add(OrderModel order)
        {
            Orders.Add(order);
        }
        public DBContext()
        {
            
        }
    }
}
