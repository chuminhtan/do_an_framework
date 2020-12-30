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
    public class DashboardController : Controller
    {
        private MySqlDatabase MySqlDatabase { get; set; }
        public DashboardController(MySqlDatabase mySqlDb)
        {
            this.MySqlDatabase = mySqlDb;
        }
        public IActionResult Index()
        {
            ViewBag.incomeyear = GetIncomeYear();
            ViewBag.incomemonth = GetIncomeMonth();
            ViewBag.countproduct = GetCountProduct();
            ViewBag.pendingorder = GetNumPendingOrder();
            ViewBag.incomepermonth = GetIncomePerMonth();
            int t = GetNumOrderYear();
            ViewBag.percentorder = GetOrderPerStatus(t);
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
            reader.Close();
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
            reader.Close();
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
            reader.Close();
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
            reader.Close();
            return number;
        }

        public List<int> GetIncomePerMonth()
        {
            List<int> income = new List<int>();
            var year = DateTime.Now.Year;

            //var sql = "Select MONTH(thoi_gian_tao), sum(tong_tien) from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year group by MONTH(thoi_gian_tao) order by MONTH(thoi_gian_tao)";
            
            for (int i = 1; i <= 12; i++)
            {  
                var sql = "Select MONTH(thoi_gian_tao), sum(tong_tien) from don_hang where tinh_trang = 3 and YEAR(thoi_gian_tao) = @year and MONTH(thoi_gian_tao)=@month group by MONTH(thoi_gian_tao) order by MONTH(thoi_gian_tao)";
                var command = new MySqlCommand(sql, MySqlDatabase.Connection);
                command.CommandText = sql;
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@month", i);

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        income.Add(reader.GetInt32(1));
                    }
                } else
                {
                    income.Add(0);
                }

                reader.Close();
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
            reader.Close();

            return number;
        }

        public int[] GetOrderPerStatus(int numorder)
        {
            int year = DateTime.Now.Year;
            int[] percentList = new int[4];
            for (int i=0; i<=3; i++)
            {
                var sql = "Select count(ma_don_hang) as sl from don_hang where YEAR(thoi_gian_tao) = @year and tinh_trang = @status";
                var command = new MySqlCommand(sql, MySqlDatabase.Connection);
                command.CommandText = sql;
                command.Parameters.AddWithValue("year", year);
                command.Parameters.AddWithValue("status", i);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int t = Convert.ToInt32(reader["sl"]);
                        percentList[i] = ((t * 100) / numorder);
                    }
                }
                reader.Close();
            }
            return percentList;
        }

    }
}
