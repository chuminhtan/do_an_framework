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

        public ActionResult Info(int id)
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
                ViewData["Order"] = order;
                ViewBag["Details"] = GetListDetail(id);
                ViewBag["Products"] = GetListProductForOrder();
                ViewBag["Categories"] = GetListCategoryForOrder();
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
                    order.order_id = reader.GetInt32(0);
                    order.customer_name = reader.GetString(1);
                    order.customer_phone = reader.GetInt32(2);
                    order.address = reader.GetString(3);
                    order.delivery_time = reader.GetDateTime(4);
                    order.customer_note = reader.GetString(5);
                    order.total = reader.GetInt32(6);
                    order.count = reader.GetInt32(7);
                    order.status = reader.GetInt32(8);
                    order.user_note = reader.GetString(9);
                    order.order_time = reader.GetDateTime(10);
                    order.history = reader.GetString(11);
                    order.user_id = reader.GetInt32(12);
                }
            }
            else
                order = null;
            return order;
        }

        public List<OrderDetailModel> GetListDetail(int id)
        {
            List<OrderDetailModel> detailList = new List<OrderDetailModel>();
            var sql = "Select chi_tiet_hoa_don.*, ten_san_pham, anh_san_pham from chi_tiet_hoa_don join san_pham on chi_tiet_hoa_don.ma_san_pham = san_pham.ma_san_pham where ma_don_hang = @id";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("id", id);

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
            }
            else
                detailList = null;
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
                    temp.prodoct_price = reader.GetInt32(3);

                    productList.Add(temp);
                }
            }
            else
                return null;
            return productList;
        }

        public List<CategoryModel> GetListCategoryForOrder()
        {
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
