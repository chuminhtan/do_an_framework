using do_an_framework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace do_an_framework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MySqlDatabase MySqlDatabase { get; set; }

        public HomeController(ILogger<HomeController> logger, MySqlDatabase mySqlDb)
        {
            _logger = logger;
            MySqlDatabase = mySqlDb;
        }
        // Lấy danh sách các danh mục
        public List<CategoryModel> getListCategories()
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            string sql = "SELECT * FROM danh_muc";

            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var description = reader.GetString(2);
                    CategoryModel category = new CategoryModel(id, name, description);
                    categories.Add(category);
                }
            }
            else
            {
                categories = null;
            }
            reader.Close();
            return categories;
        }

        // Lấy danh sách sản phẩm nổi bật
        public List<ProductModel> getListHightLightProduct()
        {
            List<ProductModel> products = new List<ProductModel>();
            var sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "WHERE phan_loai = 1";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new ProductModel();
                    product.product_id = reader.GetInt32(0);
                    product.product_name = reader.GetString(1);
                    product.product_description = reader.GetString(2);
                    product.product_price = reader.GetInt32(3);
                    product.product_state = reader.GetInt32(4);
                    product.product_kind = reader.GetInt32(5);
                    product.product_image = reader.GetString(6);
                    product.category_name = reader.GetString(9);
                    products.Add(product);
                }
            }
            else
            {
                products = null;
            }
            reader.Close();
            return products;
        }
        public IActionResult Index()
        {
            var categories = getListCategories();
            var obj = JsonConvert.SerializeObject(categories);
            HttpContext.Session.SetString("category", obj);
            List<ProductModel> products = new List<ProductModel>();
            products = getListHightLightProduct();
            return View(products);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
