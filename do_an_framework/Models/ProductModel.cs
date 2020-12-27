using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_framework.Models
{
    public class ProductModel
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_discription { get; set; }
        public int prodoct_price { get; set; }
        public int product_state { get; set; }
        public int product_kind { get; set; }
        public string product_image { get; set; }
        public int category_id { get; set; }
        public DateTime product_created_at { get; set; }

        public string category_name { get; set; }
    }
}
