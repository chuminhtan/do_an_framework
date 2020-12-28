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
    public class AdminController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public AdminController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        public IActionResult Index()
        {
            ViewData["incomeyear"] = GetIncomeYear();
            ViewData["incomemonth"] = GetIncomeMonth();
            ViewData["countproduct"] = GetCountProduct();
            ViewData["pendingorder"] = GetNumPendingOrder();
            ViewBag["incomepermonth"] = GetIncomePerMonth();
            ViewBag["percentorder"] = Percent(GetOrderPerStatus(), GetNumOrderYear());
            return View();
        }

        public int GetIncomeYear()
        {
            var year = DateTime.Now.Year;
            var incomeYear = 0;
            var sql = "Select sum(tong_tien) as doanhthu from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("year", year);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    incomeYear = reader.GetInt32(0);
                }
            }
            return incomeYear;
        }

        public int GetIncomeMonth()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var incomeMonth = 0;
            var sql = "Select sum(tong_tien) from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year and MONTH(thoi_gian_tao) = @month";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("year", year);
            command.Parameters.AddWithValue("month", month);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    incomeMonth = reader.GetInt32(0);
                }
            }
            return incomeMonth;
        }

        public int GetCountProduct()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var number = 0;
            var sql = "Select sum(tong_so_luong) from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year and MONTH(thoi_gian_tao) = @month";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("year", year);
            command.Parameters.AddWithValue("month", month);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    number = reader.GetInt32(0);
                }
            }
            return number;
        }

        public int GetNumPendingOrder()
        {
            var number = 0;
            var sql = "Select count(ma_don_hang) from don_hang where tinh_trang = 1";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    number = reader.GetInt32(0);
                }
            }
            return number;
        }

        public List<int> GetIncomePerMonth()
        {
            List<int> income = new List<int>();
            var year = DateTime.Now.Year;

            for (var i = 1; i <= 12; i++)
            {
                var sql = "Select sum(tong_tien) from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year and MONTH(thoi_gian_tao) = @month";
                var command = new MySqlCommand(sql, MySqlDatabase.Connection);
                command.CommandText = sql;
                command.Parameters.AddWithValue("year", year);
                command.Parameters.AddWithValue("month", i);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        income.Add(reader.GetInt32(0));
                    }
                }
            }

            return income;
        }

        public int GetNumOrderYear()
        {
            int number = 0;
            int year = DateTime.Now.Year;
            var sql = "Select count(ma_don_hang) from don_hang where YEAR(thoi_gian_tao) = @year";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("year", year);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    number = reader.GetInt32(0);
                }
            }
            return number;
        }

        public List<int> GetOrderPerStatus()
        {
            List<int> number = new List<int>();
            int year = DateTime.Now.Year;
            var sql = "Select tinh_trang, count(ma_don_hang) from don_hang where YEAR(thoi_gian_tao) = @year group by tinh_trang order by tinh_trang";
            var command = new MySqlCommand(sql, MySqlDatabase.Connection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("year", year);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    number.Add(reader.GetInt32(0));
                }
            }

            return number;
        }

        public List<int> Percent(List<int> number, int total_order)
        {
            List<int> result = new List<int>();
            foreach (var i in number)
            {
                result.Add(i / total_order * 100);
            }
            return result;
        }
    }
}
