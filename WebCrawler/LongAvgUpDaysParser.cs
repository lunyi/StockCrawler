using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class LongAvgUpDaysParser : BaseParser
    {
        public async Task RunAsync()
        {
            var context = new StockDbContext();
            var dates = context.Prices.Where(p => p.StockId == "2330" && p.Datetime >= new DateTime(2019,10,14))
                .OrderBy(p => p.Datetime)
                .Select(p => p.Datetime.ToString("yyyy-MM-dd"))
                .ToList();

            for (int i = 0; i < dates.Count; i++)
            {
                context.Database.ExecuteSqlRaw($"exec [usp_Update_LongAvgUpDays] '{dates[i]}'");
                Console.WriteLine($" {dates[i]}");
            }
        }
    }
}
