using System.Data;
using Microsoft.Data.SqlClient;

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
