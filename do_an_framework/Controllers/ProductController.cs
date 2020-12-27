using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

using do_an_framework.Models;

namespace do_an_framework.Controllers
{
    public class ProductController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public ProductController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        

        // Index
        public async Task<IActionResult> Index()
        {
            var productlist = new List<ProductModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select*from danh_muc";
            
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    var tam = new ProductModel()
                    {
                        product_id = reader.GetFieldValue<int>(0),
                        product_name = reader.GetFieldValue<string>(1),
                        product_discription = reader.GetFieldValue<string>(2),
                        prodoct_price = reader.GetFieldValue<int>(3),
                        product_state = reader.GetFieldValue<int>(4),
                        product_kind = reader.GetFieldValue<int>(5),
                        product_image = reader.GetFieldValue<string>(6),
                        category_id = reader.GetFieldValue<int>(7),
                        product_created_at = reader.GetFieldValue<DateTime>(8)
                    };
                    productlist.Add(tam);
                }
            return View(productlist);
        }


        // Creat View
        public IActionResult CreateView()
        {
            return View();
        }
    }
}

