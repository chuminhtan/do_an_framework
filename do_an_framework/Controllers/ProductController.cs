using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

using do_an_framework.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using BCrypt;


namespace do_an_framework.Controllers
{
    public class ProductController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public ProductController(MySqlDatabase mySqlDb, IWebHostEnvironment hostEnvironment)
        {
            this.MySqlDatabase = mySqlDb;
            this._hostEnvironment = hostEnvironment;
        }
        // Host Environment
        private readonly IWebHostEnvironment _hostEnvironment;

        // Index get list product
        public async Task<IActionResult> Index()
        {
            var productlist = new List<ProductModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select ma_san_pham, ten_san_pham, mo_ta_san_pham, gia, tinh_trang, phan_loai, anh_san_pham, ma_danh_muc, thoi_gian_tao from san_pham";
            
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

        //Insert product
        public async Task<IActionResult> Insert (List<IFormFile> product_image)
        {

            string productName = Request.Form["product_name"];
            string productDes = Request.Form["product_description"];

            // long size = product_image.Sum(f => f.Length);
            string newFileName = "";

            foreach (var formFile in product_image)
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
                    string path = Path.Combine(wwwrootPath + "/images/product/" + newFileName);

                    // Lưu file vào thư mục theo đường dẫn đã chỉ định
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }

            }
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"INSERT INTO san_pham(ten_san_pham, mo_ta_san_pham, gia, tinh_trang, phan_loai, anh_san_pham, ma_danh_muc) VALUES (@product_name, @product_description, @product_price, @product_state, @product_kind, @product_image, @category_id);";
     
            cmd.Parameters.AddWithValue("@product_name", productName);
            cmd.Parameters.AddWithValue("@product_description", productDes);
            cmd.Parameters.AddWithValue("@product_price", Request.Form["product_price"]);
            cmd.Parameters.AddWithValue("@product_state", Request.Form["product_state"]);
            cmd.Parameters.AddWithValue("@product_kind", Request.Form["product_kind"]);
            cmd.Parameters.AddWithValue("@product_image", newFileName);
            cmd.Parameters.AddWithValue("@category_id", Request.Form["category_id"]);

            var recs = cmd.ExecuteNonQuery();

            if (recs == 1)
            {
                HttpContext.Session.SetString("result", "success");
                HttpContext.Session.SetString("message", "Đã tạo sản phẩm");
            }
            else
            {
                HttpContext.Session.SetString("result", "fail");
                HttpContext.Session.SetString("message", "Lỗi");
            }

            return RedirectToAction("index");
        }

        // Creat View
        public IActionResult CreateView()
        {
            return View();
        }


        // Info View
        [HttpGet]
        public async Task<IActionResult> Info (int id) {

            int productId = id;
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select ma_san_pham, ten_san_pham, mo_ta_san_pham, gia, tinh_trang, phan_loai, anh_san_pham, ma_danh_muc from san_pham where ma_san_pham = " + productId;
            var tam = new ProductModel();
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    {
                        tam.product_id = reader.GetFieldValue<int>(0);
                        tam.product_name = reader.GetFieldValue<string>(1);
                        tam.product_discription = reader.GetFieldValue<string>(2);
                        tam.prodoct_price = reader.GetFieldValue<int>(3);
                        tam.product_state = reader.GetFieldValue<int>(4);
                        tam.product_kind = reader.GetFieldValue<int>(5);
                        tam.product_image = reader.GetFieldValue<string>(6);
                        tam.category_id = reader.GetFieldValue<int>(7);
                    };
                }

            return View(tam);
        }
    }
}

