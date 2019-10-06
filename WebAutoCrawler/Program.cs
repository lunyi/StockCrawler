using DataService.Models;
using Messages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

            var h = new FutuneEngineCrawler(); 
            await h.ExecuteAsync();

            s.Stop();
            Console.WriteLine(s.Elapsed.TotalMinutes);
            Console.ReadLine();
        }
    }
}
