using DataService.Models;
using DataService.Services;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace WebCrawler
{
    class WebCrawler
    {

        static async Task Main(string[] args)
        {

            var sql = @$"

  DECLARE @ColumnGroup NVARCHAR(MAX), @PivotSQL NVARCHAR(MAX) 

  SELECT @ColumnGroup = COALESCE(@ColumnGroup + ',' ,'' ) + QUOTENAME([NAme])
  FROM dbo.[Stocks] where [Status] = 1


  SELECT @PivotSQL = N'
  select * from 
  (select  [Datetime], [Name], [外資買賣超]
  from [Prices]) t 
  pivot  (
	MAX([外資買賣超]) 
	for	[Name] in (' +@ColumnGroup+ ')
  ) p order by [Datetime] desc'


  EXEC sp_executesql  @PivotSQL;";

            var dt = DsTable.GetData(sql);

            foreach (DataColumn column in dt.Columns)
            {
                Console.WriteLine(column.ColumnName);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    var s = dt.Rows[i][column];
                }

                foreach (var item in dt.Rows[i].ItemArray)
                {
                    var s = item;
                }
            }


            //var ss = new MonthDataParser();
            //ss.RunAsync


            //var s = new UpdateStockListParser();
            //await s.RunAsync();


            //await ss.ParserMarginAsync();
            //var ss =  s.ParseTrust("2330", "2017-01-01", "2019-10-29");

            Console.ReadLine();
        }
    }
}
