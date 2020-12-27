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
            //IndexProductModel indexModel = new IndexProductModel();
            //indexModel.productList = await this.GetProductList();
            //indexModel.categoryList = await this.GetCategoryList();

            return View(await this.GetProductList());
        }


        /**
         * Get Product List
         */
        public async Task<List<ProductModel>> GetProductList ()
        {
            List<ProductModel> productList = new List<ProductModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select ma_san_pham, ten_san_pham, mo_ta_san_pham, gia, tinh_trang, phan_loai, anh_san_pham, san_pham.ma_danh_muc, ten_danh_muc, san_pham.thoi_gian_tao from san_pham, danh_muc WHERE san_pham.ma_danh_muc=danh_muc.ma_danh_muc";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    var tam = new ProductModel()
                    {
                        product_id = reader.GetFieldValue<int>(0),
                        product_name = reader.GetFieldValue<string>(1),
                        product_description = reader.GetFieldValue<string>(2),
                        product_price = reader.GetFieldValue<int>(3),
                        product_state = reader.GetFieldValue<int>(4),
                        product_kind = reader.GetFieldValue<int>(5),
                        product_image = reader.GetFieldValue<string>(6),
                        category_id = reader.GetFieldValue<int>(7),
                        category_name = reader.GetFieldValue<string>(8),
                        product_created_at = reader.GetFieldValue<DateTime>(9)
                    };
                    productList.Add(tam);
                }
            return productList;
        }

        /**
 * Get Category List
 */
        public async Task<List<CategoryModel>> GetCategoryList()
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT ma_danh_muc, ten_danh_muc from danh_muc";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    var category = new CategoryModel()
                    {
                        id = reader.GetFieldValue<int>(0),
                        name = reader.GetFieldValue<string>(1),

                    };
                    categoryList.Add(category);
                }
            return categoryList;
        }


        /**
         * View Create
         */
        [HttpGet]
        public async Task<IActionResult> CreateView()
        {
            return View(await this.GetCategoryList());
        }


        //Insert product
        public async Task<IActionResult> Insert (List<IFormFile> product_image)
        {
            string productName = Request.Form["product_name"];
            string productDes = Request.Form["product_description"];
            int categoryId = Convert.ToInt32(Request.Form["product_category"]);

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
            cmd.Parameters.AddWithValue("@category_id", categoryId);

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


        /**
         * View Info
         */
        [HttpGet]
        public async Task<IActionResult> Info (int id) 
    {
            int productId = id;
            InfoProductModel infoModel = new InfoProductModel();


            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"Select ma_san_pham, ten_san_pham, mo_ta_san_pham, gia, tinh_trang, phan_loai, anh_san_pham, ma_danh_muc from san_pham where ma_san_pham = " + productId;

            var product = new ProductModel();

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    {
                        product.product_id = reader.GetFieldValue<int>(0);
                        product.product_name = reader.GetFieldValue<string>(1);
                        product.product_description = reader.GetFieldValue<string>(2);
                        product.product_price = reader.GetFieldValue<int>(3);
                        product.product_state = reader.GetFieldValue<int>(4);
                        product.product_kind = reader.GetFieldValue<int>(5);
                        product.product_image = reader.GetFieldValue<string>(6);
                        product.category_id = reader.GetFieldValue<int>(7);
                    };
                }

            infoModel.product = product;
            infoModel.categoryList = await this.GetCategoryList();

            return View(infoModel);
        }


        /**
         * Edit Product
         */
        [HttpPost]
        public async Task<IActionResult> Edit(List<IFormFile> product_image)
        {
            int productId = Convert.ToInt32(Request.Form["product_id"]);
            string productName = Request.Form["product_name"];
            int productPrice = Convert.ToInt32(Request.Form["product_price"]);
            int productStatus = Convert.ToInt32(Request.Form["product_status"]);
            int productType = Convert.ToInt32(Request.Form["product_type"]);
            int categoryId = Convert.ToInt32(Request.Form["product_category"]);
            string productDescription = Request.Form["product_description"];

            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE san_pham SET ten_san_pham=@product_name, mo_ta_san_pham=@product_description, gia=@product_price,tinh_trang=@product_status, phan_loai=@product_type, ma_danh_muc=@category_id WHERE ma_san_pham=@product_id ";

            cmd.Parameters.AddWithValue("@product_id", productId);
            cmd.Parameters.AddWithValue("@product_name", productName);
            cmd.Parameters.AddWithValue("@product_description", productDescription);
            cmd.Parameters.AddWithValue("@product_price", productPrice);
            cmd.Parameters.AddWithValue("@product_status", productStatus);
            cmd.Parameters.AddWithValue("@product_type", productType);
            cmd.Parameters.AddWithValue("@category_id", categoryId);

            var recs = cmd.ExecuteNonQuery();


            // Update ảnh tải lên nếu có
            foreach (var formFile in product_image)
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
                    string path = Path.Combine(wwwrootPath + "/images/product/" + newFileName);

                    // Lưu file vào thư mục theo đường dẫn đã chỉ định
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }

                var cmdUpdateImage = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;
                cmdUpdateImage.CommandText = @"UPDATE san_pham SET anh_san_pham=@product_image WHERE ma_san_pham=@product_id";

                cmdUpdateImage.Parameters.AddWithValue("@product_id", productId);
                cmdUpdateImage.Parameters.AddWithValue("@product_image", newFileName);

                var resultUpdateImage = cmdUpdateImage.ExecuteNonQuery();
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


        /**
         * Delete Product
         */
        public IActionResult Delete(int id)
        {
            int productId = id;

            // Lấy file ảnh của product từ db
            string product_image = "";

            var cmdGetProduct = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmdGetProduct.CommandText = @"SELECT anh_san_pham FROM san_pham WHERE ma_san_pham=@product_id";
            cmdGetProduct.Parameters.AddWithValue("@product_id", productId);

            using (var reader = cmdGetProduct.ExecuteReader())
            {
                while (reader.Read())
                {
                    product_image = reader["anh_san_pham"].ToString();
                }
            }


            // Xóa product trong db
            var cmd = this.MySqlDatabase.Connection.CreateCommand() as MySqlCommand;

            cmd.CommandText = @"DELETE FROM san_pham WHERE ma_san_pham=@product_id";
            cmd.Parameters.AddWithValue("@product_id", productId);

            var recs = cmd.ExecuteNonQuery();


            // Xóa ảnh trong thư mục wwwroot/images/user
            string imageToBeDeleted = Path.Combine(_hostEnvironment.WebRootPath + "/images/product/" + product_image);

            if ((System.IO.File.Exists(imageToBeDeleted)))
            {
                System.IO.File.Delete(imageToBeDeleted);

            }

            // Kiểm tra kết quả insert db . =1 thành công <1 là thất bại
            if (recs == 1)
            {
                HttpContext.Session.SetString("result", "success");
                HttpContext.Session.SetString("message", "Đã xóa sản phẩm");
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

