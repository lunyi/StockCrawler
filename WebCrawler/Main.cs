using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LineBotLibrary;
using Microsoft.Extensions.DependencyInjection;
using WebCrawler;
using LineBotLibrary.Models;

namespace WebCrawler
{
    class WebCrawler
    {
        [Obsolete]
        static async Task Main(string[] args)
        {
            //await RunAsync();


            //var ss = new HiStockParser();
            //await ss.RunSingleAsync();
            //await ss.ParserMarginAsync();

            //var s = new UpdateStockListParser();
            //await s.RunAsync();

            //var s1 = new MoneyDjParser();
            //await s1.RunAsync();

            var s1 = new CnyParser();
            await s1.RunAsync();
            //s1.ParserLastDay("1333", "恩得利");



            //await ss.ParserMarginAsync();
            //var ss =  s.ParseTrust("2330", "2017-01-01", "2019-10-29");

            //Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            var serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            serviceCollection.AddTransient<RealtimeParser>();
            //serviceCollection.AddTransient<IService, ChtService>();

            serviceCollection.AddLineNotifyBot(new LineNotifyBotSetting
            {
                ClientID = "BCHYbMmFT9Tgz4ckkSNPsX",
                ClientSecret = "SIhnxiIzgcu9UQBHseTm2N6XsZs6nuDyGKmVkHdJL9x",
                AuthorizeApi = "https://notify-bot.line.me/oauth/authorize",
                TokenApi = "https://notify-bot.line.me/oauth/token",
                NotifyApi = "https://notify-api.line.me/api/notify",
                StatusApi = "https://notify-api.line.me/api/status",
                RevokeApi = "https://notify-api.line.me/api/revoke"
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            // 3. 執行主服務
           await serviceProvider.GetRequiredService<RealtimeParser>().RunAsync();
        }
    }
}
