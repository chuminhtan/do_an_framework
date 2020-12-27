using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using do_an_framework.Models;

using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using BCrypt;

namespace do_an_framework.Controllers
{
    public class UserController : Controller
    {
        // Host Environment
        private readonly IWebHostEnvironment _hostEnvironment;

        // Khai báo biến Mysql
        private MySqlDatabase MySqlDatabase { get; set; }

        // Gán giá trị cho Mysql
        public UserController(MySqlDatabase mySqlDb, IWebHostEnvironment hostEnvironment)
        {
            this.MySqlDatabase = mySqlDb;
            this._hostEnvironment = hostEnvironment;
        }
        
        
        /**
         Get User List
         */
        public async Task<List<UserModel>> UserList()
        {
            var userList = new List<UserModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM nguoi_dung";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    var t = new UserModel()
                    {
                        user_id= reader.GetFieldValue<int>(0),
                        user_name = reader.GetFieldValue<string>(1),
                        user_phone = reader.GetFieldValue<string>(2),
                        user_type = reader.GetFieldValue<int>(3),
                        user_email = reader.GetFieldValue<string>(4),
                        user_password = reader.GetFieldValue<string>(5),
                        user_image = reader.GetFieldValue<string>(6),
                        user_created_at = reader.GetFieldValue<DateTime>(8),
                    };

                    userList.Add(t);
                }
            
            return userList;
        }

        public async Task<IActionResult> index()
        {

            return View(await this.UserList());
        }

        // Create View
        public IActionResult Create()
        {
            return View();
        }

        // Insert User
        public async Task<IActionResult> Insert(List<IFormFile> user_image)
        {
            long size = user_image.Sum(f => f.Length);
            string newFileName = "";
            foreach (var formFile in user_image)
            {
                if (formFile.Length > 0)
                {
                    // Lấy đường dẫn đến wwwrootPath
                    string wwwrootPath = _hostEnvironment.WebRootPath;

                    // Lấy tên file không lấy phần mở rộng
                    string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);

                    // Lấy phần mở rộng không lấy tên
                    string extension = Path.GetExtension(formFile.FileName);

                    // Phải nối file + random thời gian để tạo fileName mới
                    newFileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    // Tạo đường dẫn tới thư mục lưu file = Root 
                    string path = Path.Combine(wwwrootPath + "/images/user/" + newFileName);

                    // Lưu file vào thư mục theo đường dẫn đã chỉ định
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }


            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

             cmd.CommandText = @"INSERT INTO nguoi_dung(ten_nguoi_dung, dien_thoai, loai, email, mat_khau, anh_nguoi_dung) VALUES (@user_name, @user_phone, @user_permission, @user_email, @user_password, @user_image);";

            cmd.Parameters.AddWithValue("@user_name", Request.Form["user_name"]);
                      cmd.Parameters.AddWithValue("@user_phone", Request.Form["user_phone"]);
                      cmd.Parameters.AddWithValue("@user_permission", Request.Form["user_"]);
                      cmd.Parameters.AddWithValue("@user_email", Request.Form["user_email"]);
                      cmd.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(Request.Form["user_password"]));
                      cmd.Parameters.AddWithValue("@user_image", newFileName);

                      var recs = cmd.ExecuteNonQuery();

            if (recs == 1)
            {
                ViewData["result"] = "success";
                ViewData["message"] = "Đã tạo tài khoản";
            }
            else
            {
                ViewData["result"] = "fail";
                ViewData["message"] = "Lỗi";
            }
            return Redirect("/admin/user/index");
        }
    }
}
