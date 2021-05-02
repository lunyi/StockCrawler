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
            //await new WangooParser().RunAsync(1,1);
            //await DailyNotifyAsync();

            //await new CFullyDeliverySharesParser().RunAsync();
            //var ss = new HiStockParser();
            //await ss.RunSingleAsync();
            //await ss.ParserMarginAsync();

            //await new InsertBrokesParser()
            //    .RunAsync(args[0], args[1]);

            //await new DailySupportParser().RunAsync();

            //Console.ReadLine();

            //await RunAsync<BrokerParser>();

            //var context = new StockDbContext();
            //var s1 = new CnyParser();
            ////await s1.ExecuteLastAsync(context, "5210", "寶碩");
            //await s1.RunAsync(int.Parse(args[0]), int.Parse(args[1]));

            //var n = new DailyRecordNotifier();
            //await n.RunAsync();


            //await RunAsync<DailyNotifier>();

            //InitailLineNotifyBot();
            //var s1 = new StockBrokerParser();
            //await s1.RunAsync();

            //var s2 = new CopyImage();
            //s2.Run();D:\Code\StockCrawlerNew\WebCrawler\BaseParser.cs

            //s1.Test_usp_Update_MA_And_VMA();
            //s1.ParserLastDay("8913", "恩得利");

            //await new DailyNotifier().RunAsync();

            // Console.ReadLine();
            //await ss.ParserMarginAsync();
            //var ss =  s.ParseTrust("2330", "2017-01-01", "2019-10-29");

            await RunAsync<WeekNotifier>();

            //await RunAsync<RealtimeStockParser>();
            //Console.ReadLine();
            //await DailyNotifyAsync();

            //await new MusicList().RunAsync();
        }

        static async Task RunExe()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = @"/K D:\test\abc /username bobj /psw kjsjdf port/ 22";
            p.Start();
        }

        private static ServiceCollection _serviceCollection;

        private static void InitailLineNotifyBot<T>() where T : BaseParser
        {
            _serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            _serviceCollection.AddTransient<RealtimeChooseParser>();
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

        private static async Task DailyNotifyAsync()
        {
            InitailLineNotifyBot<DailyNotifier>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            // 3. 執行主服務
            await serviceProvider.GetRequiredService<DailyNotifier>().RunAsync();
            //await serviceProvider.GetRequiredService<RealtimeParser>().RunAsync();
        }
    }
}
