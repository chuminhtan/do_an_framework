using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_framework.Models
{
    public class OrderDetailModel
    {
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int count { get; set; }
        public int price { get; set; }
        public int total { get; set; }
    }
}
