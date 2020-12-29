using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using do_an_framework.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace do_an_framework.Controllers
{
    public class ShopController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public ShopController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
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

        // Lấy danh sách các sản phẩm
        public List<ProductModel> getListProducts()
        {
            List<ProductModel> products = new List<ProductModel>();

            string sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "GROUP BY san_pham.ma_danh_muc";

            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new ProductModel();
                    product.product_id= reader.GetInt32(0);
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

        // Lấy danh sách sản phẩm nhóm theo danh mục sản phẩm
        public List<ProductModel> getListProductGroupByCategory(int id)
        {
            List<ProductModel> products = new List<ProductModel>();
            string sql = "SELECT danh_muc.ma_danh_muc, danh_muc.ten_danh_muc, san_pham.ma_san_pham, " +
                                "san_pham.ten_san_pham, san_pham.anh_san_pham, san_pham.gia " +
                         "FROM san_pham INNER JOIN danh_muc " +
                         "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                         "WHERE danh_muc.ma_danh_muc = @madm " +
                         "GROUP BY danh_muc.ma_danh_muc";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("madm", id);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new ProductModel();
                    product.category_id = reader.GetInt32(0);
                    product.category_name = reader.GetString(1);
                    product.product_id = reader.GetInt32(2);
                    product.product_name = reader.GetString(3);
                    product.product_image = reader.GetString(4);
                    product.product_price = reader.GetInt32(5);
                    products.Add(product);
                }
                reader.Close();
            } else
            {
                reader.Close();
                products = null;
            }
            return products;
        }
        // Lấy thông tin sản phẩm bằng id
        public ProductModel getProductById(int id)
        {
            ProductModel product = new ProductModel();
            var sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "WHERE ma_san_pham = @id";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("id", id);

            var reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while (reader.Read())
                {
                    product.product_id = reader.GetInt32(0);
                    product.product_name = reader.GetString(1);
                    product.product_description = reader.GetString(2);
                    product.product_price = reader.GetInt32(3);
                    product.product_state = reader.GetInt32(4);
                    product.product_kind = reader.GetInt32(5);
                    product.product_image = reader.GetString(6);
                    product.category_name = reader.GetString(9);
                }
            }
            else
            {
                product = null;
            }
            reader.Close();
            return product;
        }
        public IActionResult Category(int id)
        {

            List<ProductModel> products = new List<ProductModel>();
            products = getListProductGroupByCategory(id);
            var category_name = products[0].category_name;
            ViewData["category_name"] = category_name;
            return View(products);
        }

        public IActionResult List()
        {
            var categories = getListCategories();
            var obj = JsonConvert.SerializeObject(categories);
            HttpContext.Session.SetString("category", obj);
            List<ProductModel> products = new List<ProductModel>();
            products = getListProducts();
            obj = JsonConvert.SerializeObject(products);
            HttpContext.Session.SetString("products", obj);
            return View(products);
        }

        public IActionResult Product(int id)
        {
            ProductModel product = new ProductModel();
            product = getProductById(id);
            return View(product);
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult ShopDetail()
        {
            return View();
        }
    }
}
