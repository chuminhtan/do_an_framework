using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using do_an_framework.Models;
using MySql.Data.MySqlClient;
namespace do_an_framework.Controllers
{
    public class CategoriesController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public CategoriesController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }

        // Lấy danh sách tất cả các loại sản phẩm
        public List<CategoryModel> GetCategories()
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
                    var createdDate = reader.GetDateTime(3);
                    CategoryModel newCategory = new CategoryModel(id, name, description);

                    categories.Add(newCategory);
                }
            }
            else
            {
                categories = null;
            }
            return categories;
        }

        // Tìm kiếm thông tin loại sản phẩm
        public CategoryModel FindAndGetCategory(int id)
        {
            CategoryModel category = new CategoryModel();
            string sql = "SELECT * FROM danh_muc WHERE ma_danh_muc = @madm";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("madm", id);

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    category.id = reader.GetInt32(0);
                    category.name = reader.GetString(1);
                    category.description = reader.GetString(2);
                }
            }
            return category;
        }
        // Cập nhật thông tin loại sản phẩm
        public bool UpdateCategory(CategoryModel category)
        {
            string sql = "UPDATE danh_muc SET ten_danh_muc = @ten, mo_ta = @des WHERE ma_danh_muc = @madm";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("madm", category.id);
            command.Parameters.AddWithValue("ten", category.name);
            command.Parameters.AddWithValue("des", category.description);

            var count = command.ExecuteNonQuery();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Thêm một loại sản phẩm mới
        public bool InsertCategory(string Name, string mota)
        {
            string sql = "INSERT INTO danh_muc(ten_danh_muc, mo_ta) VALUES(@name, @des)";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("name", Name);
            command.Parameters.AddWithValue("des", mota);

            var count = command.ExecuteNonQuery();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Xóa một loại sản phẩm
        public bool DeleteCategories(int id)
        {
            string sql = "DELETE FROM danh_muc WHERE ma_danh_muc = @ma";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);

            command.CommandText = sql;
            command.Parameters.AddWithValue("ma", id);

            var count = command.ExecuteNonQuery();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: CategoriesController/List
        public ActionResult List()
        {
            CategoryModel category = new CategoryModel();
            List<CategoryModel> categories = new List<CategoryModel>();
            categories = GetCategories();
            return View(categories);
        }

        // GET: CategoriesController/Info/5
        public ActionResult Info(int id)
        {
            CategoryModel category = new CategoryModel();
            category = FindAndGetCategory(id);
            if (category == null)
            {
                ViewData["categories"] = false;
                return View();
            }
            else
            {
                return View(category);
            }

        }

        // GET: CategoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Name = collection["name"];
                    var Description = collection["description"];
                    if (InsertCategory(Name, Description))
                    {
                        HttpContext.Session.SetString("result", "success");
                        HttpContext.Session.SetString("message", "Tạo mới một danh mục thành công");
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        HttpContext.Session.SetString("result", "fail");
                        HttpContext.Session.SetString("result", "Tạo mới một danh mục thất bại");
                        return RedirectToAction(nameof(List));
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return View(e.ToString());
            }
        }

        // POST: CategoriesController/Info/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(int id, IFormCollection collection)
        {
            try
            {
                CategoryModel category = new CategoryModel();
                if (ModelState.IsValid)
                {
                    Int32.TryParse(collection["id"], out id);
                    category.id = id;
                    category.name = collection["name"];
                    category.description = collection["description"];
                    if (UpdateCategory(category))
                    {
                        HttpContext.Session.SetString("result", "success");
                        HttpContext.Session.SetString("message", "Cập nhật danh mục sản phẩm thành công");
                        return RedirectToAction(nameof(List));
                    }
                    else
                    {
                        HttpContext.Session.SetString("result", "fail");
                        HttpContext.Session.SetString("message", "Cập nhật danh mục sản phẩm thất bại");
                        return RedirectToAction(nameof(List));
                    }
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                if (DeleteCategories(id))
                {
                    HttpContext.Session.SetString("result", "success");
                    HttpContext.Session.SetString("message", "Xóa danh mục thành công");
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    HttpContext.Session.SetString("result", "fail");
                    HttpContext.Session.SetString("message", "Xóa danh mục thất bại");
                    return RedirectToAction(nameof(List));
                }

            }
            catch
            {
                return View();
            }
        }
    }
}
