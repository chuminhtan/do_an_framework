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
        // số sản phẩm trên 1 trang
        const int PAGINATION = 10;
        private MySqlDatabase MySqlDatabase { get; set; }
        public ShopController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        
        // Lấy danh sách các sản phẩm
        public List<ProductModel> getListProducts()
        {
            List<ProductModel> products = new List<ProductModel>();

            string sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc";

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
                         "WHERE danh_muc.ma_danh_muc = @madm ";
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
        // Lấy thông tin sản phẩm bằng key word
        public List<ProductModel> getListProductByName(string keyword)
        {
            List<ProductModel> products = new List<ProductModel>();
            var sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "WHERE ten_san_pham LIKE '%" + keyword + "%'";
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
        // Lấy danh sách sản phẩm theo yêu cầu phân trang dữ liệu 
        public List<ProductModel> getListProductByPagination(int page)
        {
            var start = 0;
            var finish = 0;
            if(page > 1)
            {
                start = (page - 1) * PAGINATION;
                finish = page * PAGINATION;
            } else
            {
                start = 0;
                finish = PAGINATION - 1;
            }
            List<ProductModel> products = new List<ProductModel>();
            var sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "WHERE ma_san_pham LIMIT " + start + ", " + finish ;
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
        // Danh sách sản phẩm theo loại và phân trang theo yêu cầu
        public List<ProductModel> getListProductByPaginationAndCategory(int page, int id)
        {
            var start = 0;
            var finish = 0;
            if (page > 1)
            {
                start = (page - 1) * PAGINATION;
                finish = page * PAGINATION;
            }
            else
            {
                start = 0;
                finish = PAGINATION - 1;
            }
            List<ProductModel> products = new List<ProductModel>();
            var sql = "SELECT san_pham.*, danh_muc.ten_danh_muc " +
                "FROM san_pham INNER JOIN danh_muc " +
                "ON san_pham.ma_danh_muc = danh_muc.ma_danh_muc " +
                "WHERE danh_muc.ma_danh_muc = @category ORDER BY ma_san_pham LIMIT @start, @finish";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("category", id);
            command.Parameters.AddWithValue("start", start);
            command.Parameters.AddWithValue("finish", finish);

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

        public IActionResult Category(int id, int page = 1)
        {
            List<ProductModel> products = new List<ProductModel>();
            products = getListProductGroupByCategory(id);
            var category_name = products[0].category_name;
            ViewData["category_name"] = category_name;
            ViewData["category_id"] = id;
            if(products.Count > PAGINATION)
            {
                ViewData["pages"] = products.Count/PAGINATION + 1;
            }
            else
            {
                ViewData["pages"] = 1;
            }
            products = getListProductByPaginationAndCategory(page, id);
            return View(products);
        }

        public IActionResult List(int page = 1)
        {
            List<ProductModel> products = new List<ProductModel>();
            products = getListProducts();
            if (products.Count > PAGINATION)
            {
                ViewData["pages"] = products.Count / PAGINATION + 1;
            }
            else
            {
                ViewData["pages"] = 1;
            }
            products = getListProductByPagination(page);
            return View(products);
        }

        public IActionResult Product(int id)
        {
            ProductModel product = new ProductModel();
            product = getProductById(id);
            ViewBag.products = getListProducts();
            return View(product);
        }

        public IActionResult Search(string product_name, int page)
        {
            List<ProductModel> products = new List<ProductModel>();
            products = getListProductByName(product_name);
            ViewData["key_word"] = product_name;
            if (products.Count > PAGINATION)
            {
                ViewData["pages"] = products.Count / PAGINATION + 1;
            }
            else
            {
                ViewData["pages"] = 1;
            }
            products = getListProductByPagination(page);
            return View(products);
        }
    }
}
