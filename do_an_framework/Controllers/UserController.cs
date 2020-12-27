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
         * Function: Get User List
         * async
         */
        public async Task<List<UserModel>> UserList()
        {
            var userList = new List<UserModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM nguoi_dung";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    var user = new UserModel()
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

                    userList.Add(user);
                }
            
            return userList;
        }

        public async Task<IActionResult> index()
        {

            return View(await this.UserList());
        }


        /**
         * View Create
         */
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        /**
         * Insert User
         */
        public async Task<IActionResult> Insert(List<IFormFile> user_image)
        {
            // long size = user_image.Sum(f => f.Length);

            // Lấy file ảnh + đổi tên file
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


            // Lưu thông tin user vào db
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

             cmd.CommandText = @"INSERT INTO nguoi_dung(ten_nguoi_dung, dien_thoai, loai, email, mat_khau, anh_nguoi_dung) VALUES (@user_name, @user_phone, @user_permission, @user_email, @user_password, @user_image);";

            cmd.Parameters.AddWithValue("@user_name", Request.Form["user_name"]);
                      cmd.Parameters.AddWithValue("@user_phone", Request.Form["user_phone"]);
                      cmd.Parameters.AddWithValue("@user_permission", Request.Form["user_"]);
                      cmd.Parameters.AddWithValue("@user_email", Request.Form["user_email"]);
                      cmd.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(Request.Form["user_password"]));
                      cmd.Parameters.AddWithValue("@user_image", newFileName);

                      var recs = cmd.ExecuteNonQuery();


            // Kiểm tra kết quả insert db . =1 thành công <1 là thất bại
            if (recs == 1)
            {
                HttpContext.Session.SetString("result", "success");
                HttpContext.Session.SetString("message", "Đã tạo tài khoản");
            }
            else
            {
                HttpContext.Session.SetString("result", "fail");
                HttpContext.Session.SetString("message", "Lỗi");
            }

            // Chuyển hướng về hàm index
            return RedirectToAction("index");
        }


        /**
         * Delete User
         */
         public async Task<IActionResult> Delete (int id)
        {
            int userId = id;

            // Lấy file ảnh của user từ db
            string user_image = "";

            var cmdGetUser = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmdGetUser.CommandText = @"SELECT anh_nguoi_dung FROM nguoi_dung WHERE ma_nguoi_dung=@user_id";
            cmdGetUser.Parameters.AddWithValue("@user_id", userId);

            using (var reader = cmdGetUser.ExecuteReader())
            {
                while (reader.Read())
                {
                    user_image = reader["anh_nguoi_dung"].ToString();
                }
            }

            
            // Xóa người dùng trong db
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"DELETE FROM nguoi_dung WHERE ma_nguoi_dung=@user_id";
            cmd.Parameters.AddWithValue("@user_id", userId);

            var recs = cmd.ExecuteNonQuery();
            

            // Xóa ảnh trong thư mục wwwroot/images/user
                string imageToBeDeleted = Path.Combine(_hostEnvironment.WebRootPath + "/images/user/" + user_image);

                if ((System.IO.File.Exists(imageToBeDeleted)))
                {
                    System.IO.File.Delete(imageToBeDeleted);

                }

            // Kiểm tra kết quả insert db . =1 thành công <1 là thất bại
            if (recs == 1)
            {
                HttpContext.Session.SetString("result", "success");
                HttpContext.Session.SetString("message", "Đã tạo tài khoản");
            }
            else
            {
                HttpContext.Session.SetString("result", "fail");
                HttpContext.Session.SetString("message", "Lỗi");
            }

            // Chuyển hướng về hàm index
            return RedirectToAction("index");

        }


        /**
         * View User Info
         */
        [HttpGet]
        public IActionResult Info (int id)
        {
            int userId = id;
            UserModel user = new UserModel();
            // Lấy thông tin người dùng từ db
            var cmdGetUser = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmdGetUser.CommandText = @"SELECT * FROM nguoi_dung WHERE ma_nguoi_dung=@user_id";
            cmdGetUser.Parameters.AddWithValue("@user_id", userId);

            using (var reader = cmdGetUser.ExecuteReader())
            {
                while (reader.Read())
                {
                    user.user_id = Convert.ToInt32(reader["ma_nguoi_dung"]);
                    user.user_name = reader["ten_nguoi_dung"].ToString();
                    user.user_phone = reader["dien_thoai"].ToString();
                    user.user_type = Convert.ToInt32(reader["loai"]);
                    user.user_email = reader["email"].ToString();
                    user.user_image = reader["anh_nguoi_dung"].ToString();
                }
            }

            ViewBag.user = user;
            return View();
        }

        /**
         * Edit User
         */
        [HttpPost]
        public async Task<IActionResult> Edit (List<IFormFile> user_image)
        {
            int userId = Convert.ToInt32(Request.Form["user_id"]);
            string userPhone = Request.Form["user_phone"];
            string userName = Request.Form["user_name"];
            string userType = Request.Form["user_permission"];
            string isChangePassword = Request.Form["is_change_password"];

            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE nguoi_dung SET ten_nguoi_dung=@user_name, dien_thoai=@user_phone, loai=@user_type, thoi_gian_cap_nhat=@time_update WHERE ma_nguoi_dung=@user_id";

            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@user_name", userName);
            cmd.Parameters.AddWithValue("@user_phone", userPhone);
            cmd.Parameters.AddWithValue("@time_update", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("@user_type", userType);

            var recs = cmd.ExecuteNonQuery();


            // Update ảnh tải lên nếu có
            foreach (var formFile in user_image)
            {
                string newFileName = "";
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

                var cmdUpdateImage = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmdUpdateImage.CommandText = @"UPDATE nguoi_dung SET anh_nguoi_dung=@user_image WHERE ma_nguoi_dung=@user_id";

                cmdUpdateImage.Parameters.AddWithValue("@user_id", userId);
                cmdUpdateImage.Parameters.AddWithValue("@user_image", newFileName);

                var resultUpdateImage = cmdUpdateImage.ExecuteNonQuery();
            }

            // Nếu có dấu check đổi mật khẩu thì update mật khẩu
            if (isChangePassword == "one")
            {
                var cmdUpdatePassword = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmdUpdatePassword.CommandText = @"UPDATE nguoi_dung SET mat_khau=@user_password WHERE ma_nguoi_dung=@user_id";

                cmdUpdatePassword.Parameters.AddWithValue("@user_id", userId);
                cmdUpdatePassword.Parameters.AddWithValue("@user_password", BCrypt.Net.BCrypt.HashPassword(Request.Form["user_password"]));

                var resultUpdatePassword = cmdUpdatePassword.ExecuteNonQuery();
            }

            // Kiểm tra kết quả insert db . =1 thành công <1 là thất bại
            if (recs == 1)
            {
                HttpContext.Session.SetString("result", "success");
                HttpContext.Session.SetString("message", "Đã cập nhật");
            }
            else
            {
                HttpContext.Session.SetString("result", "fail");
                HttpContext.Session.SetString("message", "Lỗi");
            }

            // Chuyển hướng về hàm index
            return RedirectToAction("index");
        }

    }
}
