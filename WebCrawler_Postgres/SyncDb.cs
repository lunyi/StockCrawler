using Microsoft.Data.SqlClient;
using PostgresData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class SyncDb
    {

        public async Task UpdateDataAsync()
        {
            var ds = GetData();

            var context = new stockContext();

            for (int i = 0; i < ds.Rows.Count; i++)
            {
                var stock = context.Stocks.FirstOrDefault(p => p.StockId == Convert.ToString(ds.Rows[i]["StockId"]));
                stock.Name = Convert.ToString(ds.Rows[i]["Name"]);

                Console.WriteLine($"{stock.StockId} {stock.Name}");

                stock.MarketCategory = Convert.ToString(ds.Rows[i]["MarketCategory"]);
                stock.Industry = Convert.ToString(ds.Rows[i]["Industry"]);
                stock.Address = Convert.ToString(ds.Rows[i]["Address"]);
                stock.Website = Convert.ToString(ds.Rows[i]["Website"]);
                stock.營收比重 = Convert.ToString(ds.Rows[i]["營收比重"]);
                stock.Description = Convert.ToString(ds.Rows[i]["Description"]);
                await context.SaveChangesAsync();
            }
          
        }


        private DataTable GetData()
        {
            SqlConnection conn = new SqlConnection("Server=220.133.185.1;Database=StockDb;User ID=stock;Password=stock;");
            SqlDataAdapter da = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 300;
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM [Thousand] order by StockId";
            da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);//把資料庫資料填入到DataSet
            return ds.Tables[0];
        }
    }
}
