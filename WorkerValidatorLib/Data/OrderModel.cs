using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerValidatorLib.Data
{
    public class OrderModel
    {
        public int OrderId { get; set; } = 0;
        public string Customer { get; set; } = "";
        public double Amount { get; set; } = 0;
        public int Checked { get; set; } = 0;
    }
}
