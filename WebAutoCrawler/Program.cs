using WebAutoCrawler;

namespace WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var s = Stopwatch.StartNew();
            s.Start();

            var h = new FutuneEngineCrawler();
            await h.ExecuteAsync();
            ~h;

            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
