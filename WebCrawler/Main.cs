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
using EFCore.BulkExtensions;

namespace WebCrawler
{
    class WebCrawler
    {
        [Obsolete]
        static async Task Main(string[] args)
        {
            //await DailyNotifyAsync();


            //var ss = new HiStockParser();
            //await ss.RunSingleAsync();
            //await ss.ParserMarginAsync();

            //await new CnyParser().RunAsync(int.Parse(args[0]), int.Parse(args[1]));


            //await RunAsync<BrokerParser>();

            //var context = new StockDbContext();
            //var s1 = new CnyParser();
            //await s1.ExecuteLastAsync(context, "5210", "寶碩");
            //await s1.RunAsync();


            //await RunAsync<DailyNotifier>();

            //InitailLineNotifyBot();
            //var s1 = new UpdateStockListParser();
            //await s1.RunAsync();
            //s1.Test_usp_Update_MA_And_VMA();
            //s1.ParserLastDay("8913", "恩得利");



            //await ss.ParserMarginAsync();
            //var ss =  s.ParseTrust("2330", "2017-01-01", "2019-10-29");

            await RunAsync<RealtimeParser>();

            Console.ReadLine();
        }

        private static ServiceCollection _serviceCollection;

        private static void InitailLineNotifyBot<T>() where T : BaseParser
        {
            _serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            _serviceCollection.AddTransient<RealtimeParser>();
            _serviceCollection.AddTransient<DailyNotifier>();
            _serviceCollection.AddTransient<T>();

            //serviceCollection.AddTransient<IService, ChtService>();

            _serviceCollection.AddLineNotifyBot(new LineNotifyBotSetting
            {
                ClientID = "BCHYbMmFT9Tgz4ckkSNPsX",
                ClientSecret = "SIhnxiIzgcu9UQBHseTm2N6XsZs6nuDyGKmVkHdJL9x",
                AuthorizeApi = "https://notify-bot.line.me/oauth/authorize",
                TokenApi = "https://notify-bot.line.me/oauth/token",
                NotifyApi = "https://notify-api.line.me/api/notify",
                StatusApi = "https://notify-api.line.me/api/status",
                RevokeApi = "https://notify-api.line.me/api/revoke"
            });
        }

        private static async Task RunAsync<T>() where T : BaseParser
        {
            InitailLineNotifyBot<T>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            // 3. 執行主服務
            await serviceProvider.GetRequiredService<T>().RunAsync();
        }

        private static async Task RunAsync<T>(int minute) where T : BaseParser
        {
            InitailLineNotifyBot<T>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            // 3. 執行主服務
            await serviceProvider.GetRequiredService<T>().RunAsync(minute);
        }

        //private static async Task DailyNotifyAsync()
        //{
        //    InitailLineNotifyBot();
        //    var serviceProvider = _serviceCollection.BuildServiceProvider();
        //    // 3. 執行主服務
        //    await serviceProvider.GetRequiredService<DailyNotifier>().RunAsync();
        //    await serviceProvider.GetRequiredService<RealtimeParser>().RunAsync();
        //}
    }
}
