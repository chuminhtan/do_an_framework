using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace do_an_framework.Models
{
    public class OrderModel
    {
        public int order_id { get; set; }
        public string customer_name { get; set; }
        public int customer_phone { get; set; }
        public string address { get; set; }
        public DateTime delivery_time { get; set; }
        public string customer_note { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public int status { get; set; }
        public string user_note { get; set; }
        public DateTime order_time { get; set; }
        public string history { get; set; }
        public int user_id { get; set; }
    }
}
