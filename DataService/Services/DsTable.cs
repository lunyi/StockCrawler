using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public class DsTable
    {
        public static DataTable GetData(string sql)
        {
            DataTable table = new DataTable();
            string connString = @"Server=220.133.185.1;Database=StockDb;User ID=stock;Password=stock;";
            using (var da = new SqlDataAdapter(sql, connString))
            {
                da.Fill(table);
            }
            return table;
        }
    }
}
