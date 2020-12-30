using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using do_an_framework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace do_an_framework.Controllers
{
    public class StoreController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public StoreController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        // Lấy thông tin sản phẩm được thêm vào giỏ hàng
        public ProductModel getProductById(int id)
        {
            ProductModel product = new ProductModel();
            string sql = "SELECT * FROM san_pham WHERE ma_san_pham = @masp";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("masp", id);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    product.product_id = reader.GetInt32(0);
                    product.product_name = reader.GetString(1);
                    product.product_image = reader.GetString("anh_san_pham");
                    product.product_price = reader.GetInt32("gia");
                }
            }
            reader.Close();
            return product;
        }
        // Thêm một đơn hàng
        public long InsertOrder(OrderModel order)
        {
            string sql = "INSERT INTO don_hang(ten_khach_hang," +
                                                "dien_thoai_khach_hang," +
                                                "dia_chi_giao_hang," +
                                                "thoi_gian_giao_hang," +
                                                "ghi_chu_khach_hang," +
                                                "tong_tien," +
                                                "tong_so_luong," +
                                                "lich_su," +
                                                "tinh_trang) "+
                        "VALUES(@name,@tel,@address,@delivery_time, @note,@total,@amount,@lichsu,1)";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            string lichsu = order.delivery_time.ToString() + ": ĐƠN HÀNG ĐƯỢC TẠO \n";
            DateTime created_time = new DateTime();
            command.CommandText = sql;
            command.Parameters.AddWithValue("name", order.customer_name);
            command.Parameters.AddWithValue("tel", order.customer_phone);
            command.Parameters.AddWithValue("address", order.address);
            command.Parameters.AddWithValue("@delivery_time", order.delivery_time);
            command.Parameters.AddWithValue("note", order.customer_note);
            command.Parameters.AddWithValue("total", order.total);
            command.Parameters.AddWithValue("amount", order.count);
            command.Parameters.AddWithValue("lichsu", lichsu);
            var reader = command.ExecuteNonQuery();
            if(reader > 0)
            {
                return command.LastInsertedId;
            }
            else
            {
                return -1;
            } 
        }

        // Thêm chi tiết đơn hàng
        public bool InsertDetailOrder(OrderDetailModel detail)
        {
            string sql = "INSERT INTO chi_tiet_don_hang(ma_don_hang," +
                                                        "ma_san_pham," +
                                                        "so_luong_ban," +
                                                        "don_gia," +
                                                        "thanh_tien)" +
                          "VALUES(@orderid,@productid,@amount,@price,@total)";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("orderid", detail.order_id);
            command.Parameters.AddWithValue("productid", detail.product_id);
            command.Parameters.AddWithValue("amount", detail.count);
            command.Parameters.AddWithValue("price", detail.price);
            command.Parameters.AddWithValue("total", detail.total);

            var reader = command.ExecuteNonQuery();
            if(reader > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Lấy thông tin đơn hàng
        public OrderModel getOrder(long id)
        {
            OrderModel order = new OrderModel();
            string sql = "SELECT * FROM don_hang WHERE ma_don_hang = @id";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("id", id);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    order.order_id = reader.GetInt32("ma_don_hang");
                    order.customer_name = reader.GetString("ten_khach_hang");
                    order.customer_phone = reader.GetString("dien_thoai_khach_hang");
                    order.address = reader.GetString("dia_chi_giao_hang");
                    order.total = reader.GetInt32("tong_tien");
                    order.count = reader.GetInt32("tong_so_luong");
                }
            }
            else
            {
                order = null;
            }
            reader.Close();
            return order;
        }

        // Lấy các chi tiết hóa đơn của đơn hàng
        public List<OrderDetailModel> getListDetailOrder(long id)
        {
            List<OrderDetailModel> listDetail = new List<OrderDetailModel>();
            string sql = "SELECT chi_tiet_don_hang.*, san_pham.ten_san_pham FROM chi_tiet_don_hang INNER JOIN san_pham " +
                            "ON san_pham.ma_san_pham = chi_tiet_don_hang.ma_san_pham" +
                            " WHERE ma_don_hang = @id";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("id", id);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    OrderDetailModel detail = new OrderDetailModel();
                    detail.order_id = reader.GetInt32("ma_don_hang");
                    detail.product_id = reader.GetInt32("ma_san_pham");
                    detail.product_name = reader.GetString("ten_san_pham");
                    detail.price = reader.GetInt32("don_gia");
                    detail.count = reader.GetInt32("so_luong_ban");
                    detail.total = reader.GetInt32("thanh_tien");
                    listDetail.Add(detail);
                }
            }
            else
            {
                listDetail = null;
            }
            reader.Close();
            return listDetail;
        }

        [HttpGet]
        public IActionResult Cart()
        {
            return View();
        }


        // CART GET
        [HttpGet]
        public IActionResult AddProductCart(int id)
        {
            bool isExist = false;
            if (HttpContext.Session.GetString("cart") != null)
            {
                string obj = HttpContext.Session.GetString("cart");
                var cart = JsonConvert.DeserializeObject<List<CartModel>>(obj);
                foreach (var order in cart)
                {
                    if (order.product_id == id)
                    {
                        order.quantity = order.quantity + 1;
                        isExist = true;
                        break;
                    }
                }
                if (isExist == false)
                {
                    ProductModel product = getProductById(id);
                    CartModel product_order = new CartModel();
                    product_order.product_id = product.product_id;
                    product_order.product_name = product.product_name;
                    product_order.product_price = product.product_price;
                    product_order.product_img = product.product_image;
                    product_order.quantity = 1;
                    cart.Add(product_order);
                }
                obj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cart", obj);
            }
            else
            {
                ProductModel product = getProductById(id);
                CartModel product_order = new CartModel();
                product_order.product_id = product.product_id;
                product_order.product_name = product.product_name;
                product_order.product_price = product.product_price;
                product_order.product_img = product.product_image;
                product_order.quantity = 1;
                List<CartModel> cart = new List<CartModel>();
                cart.Add(product_order);
                var obj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cart", obj);
            }
            if (Request.Headers["Referer"].Equals("") == false)
            {
                return Redirect(Request.Headers["Referer"].ToString()); // Chuyển hướng lại previos page thực hiện request
            }
            else
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

        }
        [HttpPost]
        public IActionResult Cart(IFormCollection collection)
        {
            // Thông tin lấy từ form chi tiết giỏ hàng
            var categories = collection["product_id_list[]"];
            var quantities = collection["product_quantity_list[]"];
            // Thông tin lấy từ form sản phẩm
            var category = collection["product_id"];
            var quantity = collection["product_quantity"];
            bool isExist = false;
            // Kiểm tra có sản phẩm trong giỏ hàng hay không?
            if (HttpContext.Session.GetString("cart") != null)
            {
                string obj = HttpContext.Session.GetString("cart"); // Lấy giá trị của session có key = cart
                var cart = JsonConvert.DeserializeObject<List<CartModel>>(obj); // Chuyển đổi giá trị từ string sang object đã được JSONConvert
                for (int i = 0; i < cart.Count; i++)
                {
                    //Kiểm tra thông tin lấy từ form nào
                    if(quantity.ToString().Equals(""))
                    {
                        // Nếu có sản phẩm trong giỏ hàng thì cập nhật lại số lượng
                        if (cart[i].product_id == Int32.Parse(categories[i]))
                        {
                            cart[i].quantity = Int32.Parse(quantities[i]);
                            isExist = true;
                        }
                    }
                    else
                    {
                        // Nếu có sản phẩm trong giỏ hàng thì phải thêm số lượng 
                        // Vì lấy từ form sản phẩm
                        if (cart[i].product_id == Int32.Parse(category))
                        {
                            cart[i].quantity += Int32.Parse(quantity);
                            isExist = true;
                        }
                    }
                }
                // Nếu sản phẩm không có trong giỏ hàng thì lưu giá trị các sản phẩm được thêm
                if (isExist == false)
                {
                    ProductModel product = new ProductModel();
                    CartModel product_order = new CartModel();
                    product = getProductById(Int32.Parse(category));
                    product_order.quantity = Int32.Parse(quantity);
                    product_order.product_id = product.product_id;
                    product_order.product_name = product.product_name;
                    product_order.product_price = product.product_price;
                    product_order.product_img = product.product_image;
                    cart.Add(product_order); 
                }
                obj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cart", obj); // Nén list --> string để đẩy lên session
            }
            // Giỏ hàng trống
            else
            {
                ProductModel product = new ProductModel();
                product = getProductById(Int32.Parse(category));
                CartModel product_order = new CartModel();
                product_order.product_id = product.product_id;
                product_order.product_name = product.product_name;
                product_order.product_price = product.product_price;
                product_order.product_img = product.product_image;
                product_order.quantity = Int32.Parse(quantity);
                List<CartModel> cart = new List<CartModel>(); // Tạo một list sản phẩm để đẩy lên session
                cart.Add(product_order);
                var obj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cart", obj);
            }
            return View();
        }

        public IActionResult Order()
        {
            return View();
        }


        // INSERT ORDER
        [HttpPost]
        public IActionResult Order(IFormCollection collection)
        {
            OrderModel order = new OrderModel();
            long id = 0;
            order.customer_name = collection["customer_name"];
            order.address = collection["customer_address"];
            order.customer_phone = collection["customer_phone"].ToString();
            order.delivery_time = Convert.ToDateTime(collection["customer_time_delivery"]);
            order.customer_note = collection["customer_note"];
            order.total = 0;
            order.count = 0;
            if(HttpContext.Session.GetString("cart") != null)
            {
                string obj = HttpContext.Session.GetString("cart"); // Lấy giá trị của session có key = cart
                var cart = JsonConvert.DeserializeObject<List<CartModel>>(obj); // Chuyển đổi giá trị từ string sang object đã được JSONConvert
                List<OrderDetailModel> listproduct = new List<OrderDetailModel>();
                foreach(var product in cart)
                {
                    order.total += product.product_price * product.quantity;
                    order.count += product.quantity;
                }
                id = InsertOrder(order);
                foreach(var product in cart)
                {
                    OrderDetailModel detail = new OrderDetailModel();
                    detail.order_id= (int)id;
                    detail.product_id = product.product_id;
                    detail.count = product.quantity;
                    detail.price = product.product_price;
                    detail.total = product.product_price * detail.count;
                    bool check = InsertDetailOrder(detail);
                }
                HttpContext.Session.Remove("cart");
            }

            return RedirectToAction("OrderComplete", "Store", new { @id = id });
        }
        public IActionResult OrderComplete(int id)
        {
            OrderModel order = getOrder(id);
            List<OrderDetailModel> detail = getListDetailOrder(id);
            OrderDetailModelView orderview = new OrderDetailModelView();
            orderview.order_id = id;
            orderview.customer_name = order.customer_name;
            orderview.customer_tel = order.customer_phone;
            orderview.customer_address = order.address;
            orderview.total = order.total;
            orderview.total_amount = order.count;
            orderview.orderdetail = detail;
            return View(orderview);
        }

        public IActionResult CancleOrder()
        {
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Index","Home");
        }
    }
}
