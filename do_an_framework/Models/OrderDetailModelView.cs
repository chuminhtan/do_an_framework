using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_framework.Models
{
    public class OrderDetailModelView
    {
        public int order_id { get; set; }
        public string customer_name { get; set; }
        public string customer_tel { get; set; }
        public string customer_address { get; set; }
        public List<OrderDetailModel> orderdetail { get; set; }
        public int total { get; set; }
        public int total_amount { get; set; }
    }
}
