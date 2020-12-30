using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_framework.Models
{
    public class CartModel
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int product_price { get; set; }
        public int quantity { get; set; }
        public string product_img { get; set; }
    }
}
