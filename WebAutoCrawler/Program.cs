using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebAutoCrawler;

namespace WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var h = new CMoneyCrawler();
            await h.ExecuteAsync();

            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
