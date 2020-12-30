using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using do_an_framework.Models;
using BCrypt;
using Newtonsoft.Json;

namespace do_an_framework.Controllers
{
    public class AdminController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public AdminController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        // Kiểm tra thông tin tài khoản có trong cơ sở dữ liệu hay không
        public bool CheckLogin(string email, string pass)
        {
            var sql = "SELECT mat_khau FROM nguoi_dung WHERE email = @email";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("email", email);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                string password;
                bool verified = true;
                while (reader.Read())
                {
                    password = reader.GetString(0);
                    verified = BCrypt.Net.BCrypt.Verify(pass, password);
                }
                if (verified == true)
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    return false;
                }
            } else
            {
                return false;
            }
        }
        // Lấy thông tin đăng nhập User
        public UserModel getUser(string email)
        {
            UserModel user = new UserModel();
            var sql = "SELECT * FROM nguoi_dung WHERE email = @email";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("email", email);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    user.user_id = reader.GetInt32(0);
                    user.user_name = reader["ten_nguoi_dung"].ToString();
                    user.user_name = reader["email"].ToString();
                    user.user_type = reader.GetInt32(3);
                    user.user_image = reader.GetString(6);
                }
                return user;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var email = collection["user_email"];
                    var password = collection["user_password"];
                    if (CheckLogin(email, password))
                    {
                        UserModel user = getUser(email);
                        var obj = JsonConvert.SerializeObject(user);
                        HttpContext.Session.SetString("user", obj);
                        HttpContext.Session.SetInt32("user_id", user.user_id);
                        HttpContext.Session.SetString("result", "success");
                        HttpContext.Session.SetString("message", "Đăng nhập thành công");
                        return RedirectToAction("Index","Dashboard");
                    }
                    else
                    {
                        HttpContext.Session.SetString("result", "fail");
                        HttpContext.Session.SetString("message", "Mật khẩu/Email không hợp lệ");
                        return RedirectToAction(nameof(Login));
                    }
                }
                else
                {
                    return View();
                }
            }
            catch(Exception e)
            {
                return View(e.ToString());
            }
            
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
