using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WebAutoCrawler;

namespace WebCrawler
{
    class Program
    {
        [Obsolete]
        static async Task Main(string[] args)
        {
            //var p = new ThousandDataCrawlerV2();
            //await p.ExecuteLastAsync();

            //var p = new DirectorSupervisorCrawler();
            //await p.ExecuteAsync();

            //var c = new MonthDataCrawler();
            //await c.ExecuteAsync();

            //var c = new TwDataCrawler();
            //await c.ExecuteLatestAsync();

            //var c = new UpdateBrokeListParser();
            //await c.ExecuteAsync(args[0]);

            var c = new SeasonDataCrawler();
            await c.ExecuteAsync();
            //Console.ReadLine();

            //if (args.Length != 1)
            //{
            //    Console.WriteLine("沒有設定參數");
            //    return;
            //}

            //await new DailyTraderCrawler().ExecuteAsync(args[0]);
        }

        private static ServiceCollection _serviceCollection;

        private static void InitailLineNotifyBot<T>() where T : BaseCrawler
        {
            _serviceCollection = new ServiceCollection();
            // 2. 註冊服務
            //_serviceCollection.AddTransient<RealtimeParser>();
            //_serviceCollection.AddTransient<DailyNotifier>();
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
        private static async Task RunAsync<T>() where T : BaseCrawler
        {
            InitailLineNotifyBot<T>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            // 3. 執行主服務
            await serviceProvider.GetRequiredService<T>().ExecuteAsync();
        }
    }
}
