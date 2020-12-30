using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using do_an_framework.Models;
using do_an_framework.Controllers;
using MySql.Data.MySqlClient;

namespace do_an_framework.Controllers
{
    public class OrderController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public OrderController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        

        public List<OrderModel> GetOrderList()
        {
            List<OrderModel> orderList = new List<OrderModel>();
            var sql = "Select ma_don_hang, ten_khach_hang, dia_chi_giao_hang, thoi_gian_giao_hang, tong_tien, tinh_trang, thoi_gian_tao from don_hang";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    OrderModel temp = new OrderModel();
                    temp.order_id = reader.GetInt32(0);
                    temp.customer_name = reader.GetString(1);
                    temp.address = reader.GetString(2);
                    temp.delivery_time = reader.GetDateTime(3);
                    temp.total = reader.GetInt32(4);
                    temp.status = reader.GetInt32(5);
                    temp.order_time = reader.GetDateTime(6);

                    orderList.Add(temp);
                }
            }
            else
            {
                orderList = null;
            }
            return orderList;
        }

        public ActionResult Index()
        {
            List<OrderModel> orderList = new List<OrderModel>();
            orderList = GetOrderList();
            return View(orderList);
        }

        public IActionResult Info(int id)
        {

            OrderModel order = new OrderModel();
            order = GetInfoOrder(id);

            if (order == null)
            {
                HttpContext.Session.SetString("result", "fail");
                HttpContext.Session.SetString("message", "Không tồn tại hóa đơn");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Order = order;
                ViewBag.Details = GetListDetail(id);
                ViewBag.Products = GetListProductForOrder();
                ViewBag.Categories = GetListCategoryForOrder();
                return View();
            }
        }

        public OrderModel GetInfoOrder(int id)
        {
            OrderModel order = new OrderModel();
            var sql = "Select * from don_hang where ma_don_hang = @id";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("id", id);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    order.order_id = Convert.ToInt32(reader["ma_don_hang"]);
                    order.customer_name = reader["ten_khach_hang"].ToString();
                    order.customer_phone = reader["dien_thoai_khach_hang"].ToString();
                    order.address = reader["dia_chi_giao_hang"].ToString();
                    order.delivery_time = Convert.ToDateTime(reader["thoi_gian_giao_hang"]);
                    order.customer_note = reader["ghi_chu_khach_hang"].ToString();
                    order.total = Convert.ToInt32(reader["tong_tien"]);
                    order.count = Convert.ToInt32(reader["tong_so_luong"]);                
                    order.status = Convert.ToInt32(reader["tinh_trang"]);
                    order.user_note = reader["ghi_chu_nhan_vien"].ToString();
                    order.order_time = Convert.ToDateTime(reader["thoi_gian_tao"]);
                    order.history = reader["ghi_chu_nhan_vien"].ToString();

                }
            } else
            {
                order = null;
            }
               
            reader.Close();
            return order;
        }

        public List<OrderDetailModel> GetListDetail(int id)
        {
            int orderId = id;
           
            List<OrderDetailModel> detailList = new List<OrderDetailModel>();

            var sql = "Select chi_tiet_don_hang.*, ten_san_pham, anh_san_pham from chi_tiet_don_hang join san_pham on chi_tiet_don_hang.ma_san_pham = san_pham.ma_san_pham where ma_don_hang = @order_id";

            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("@order_id", orderId);

            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    OrderDetailModel temp = new OrderDetailModel();
                    temp.order_id = reader.GetInt32(0);
                    temp.product_id = reader.GetInt32(1);
                    temp.count = reader.GetInt32(2);
                    temp.price = reader.GetInt32(3);
                    temp.total = reader.GetInt32(4);
                    temp.product_name = reader.GetString(5);
                    temp.product_image = reader.GetString(6);

                    detailList.Add(temp);
                }
            }else
            {
                detailList = null;
            }

            reader.Close();

            return detailList;
        }

        public List<ProductModel> GetListProductForOrder()
        {
            List<ProductModel> productList = new List<ProductModel>();
            var sql = "Select ma_san_pham, ten_san_pham, anh_san_pham, gia, ma_danh_muc from san_pham";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
  
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel temp = new ProductModel();
                    temp.product_id = reader.GetInt32(0);
                    temp.product_name = reader.GetString(1);
                    temp.product_image = reader.GetString(2);
                    temp.product_price = reader.GetInt32(3);

                    productList.Add(temp);
                }
            }else
            {
                return null;
            }

            reader.Close();
            return productList;
        }

        public List<CategoryModel> GetListCategoryForOrder()
        {
            /*
                // Lấy đơn hàng
                var order = new OrderModel();
                var cmd1 = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd1.CommandText = @"SELECT * from don_hang where ma_don_hang = @order_id";
                cmd1.Parameters.AddWithValue("@order_id", id);

                using (var reader = await cmd1.ExecuteReaderAsync())
                    if (await reader.ReadAsync())
                    {
                        var x = new OrderModel()
                        {
                            order_id = reader.GetFieldValue<int>(0),
                            customer_name = reader.GetFieldValue<string>(1),
                            customer_phone = reader.GetFieldValue<int>(2),
                            address = reader.GetFieldValue<string>(3),
                            delivery_time = reader.GetFieldValue<DateTime>(4),
                            customer_note = reader.GetFieldValue<string>(5),
                            total = reader.GetFieldValue<int>(6),
                            count = reader.GetFieldValue<int>(7),
                            status = reader.GetFieldValue<int>(8),
                            user_note = reader.GetFieldValue<string>(9),
                            order_time = reader.GetFieldValue<DateTime>(10),
                            history = reader.GetFieldValue<string>(11),
                            user_id = reader.GetFieldValue<int>(12)
                        };
                        order = x;
                    }

                // Nếu không tồn tại đơn hàng quay lại trang danh sách đơn hàng
                if (order == null)
                    return View(await this.OrderList());

                // Lấy chi tiết đơn hàng
                var detailList = new List<OrderDetailModel>();
                var cmd2 = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd2.CommandText = @"SELECT * from chi_tiet_don_hang where ma_don_hang = @order_id";
                cmd2.Parameters.AddWithValue("@order_id", id);

                using (var reader = await cmd2.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        var y = new OrderDetailModel()
                        {
                            order_id = reader.GetFieldValue<int>(0),
                            product_id = reader.GetFieldValue<int>(1),
                            count = reader.GetFieldValue<int>(2),
                            price = reader.GetFieldValue<int>(3),
                            total = reader.GetFieldValue<int>(4)
                        };
                        detailList.Add(y);
                    }

                // Lấy danh sách sản phẩm kèm danh mục
                var productList = new List<ProductModel>();
                var cmd3 = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmd3.CommandText = @"SELECT ma_san_pham, anh_san_pham, ten_san_pham, don_gia, ten_danh_muc from san_pham join danh_muc on san_pham.ma_danh_muc = danh_muc.ma_danh_muc";

                using (var reader = await cmd3.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        var z = new ProductModel()
                        {
                            product_id = reader.GetFieldValue<int>(0),
                            product_image = reader.GetFieldValue<string>(1),
                            product_name = reader.GetFieldValue<string>(2),
                            product_price = reader.GetFieldValue<int>(3),
                            category_name = reader.GetFieldValue<string>(4)
                        };
                        productList.Add(z);
                    }

                if (order.status == 0)
                    return View();
                else if (order.status == 1)
                    return View();
                else if (order.status == 2)
                    return View();
                else if (order.status == 3)
                    return View();
                else
                    return View(await this.OrderList());
*/
            List<CategoryModel> categoriesList = new List<CategoryModel>();
            var sql = "Select ma_danh_muc, ten_danh_muc from danh_muc";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CategoryModel temp = new CategoryModel();
                    temp.id = reader.GetInt32(0);
                    temp.name = reader.GetString(1);

                    categoriesList.Add(temp);
                }
            }
            else
                return null;
            return categoriesList;
        }
    }
}
