using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using do_an_framework.Models;

namespace do_an_framework.Models
{
    public class IndexProductModel
    {
        public List<ProductModel> productList { get; set; }
        public List<CategoryModel> categoryList { get; set; }
    }
}
