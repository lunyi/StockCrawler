using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebAutoCrawler
{
    public class UpdateBrokeListParser : BaseCrawler2
    {
        string url = "https://fubon-ebrokerdj.fbs.com.tw/z/zg/zgb/zgb0.djhtm";
        public UpdateBrokeListParser() : base()
        {
            GoToUrl(url);
        }

        public async Task ExecuteAsync(string type)
        {
            var context = new StockDbContext();

            var count2 = 0;
            var node1 = new SelectElement(FindElement(By.Name("sel_Broker")));
            var count = node1.Options.Count;
            for (int i = 0; i < count; i++)
            {
                node1 = new SelectElement(FindElement(By.Name("sel_Broker")));
                var text = node1.Options[i].Text;
                var value = node1.Options[i].GetAttribute("value");
                node1.Options[i].Click();
                Thread.Sleep(1000);

                var node2 = new SelectElement(FindElement(By.Name("sel_BrokerBranch")));

                for (int j = 0; j < node2.Options.Count; j++)
                {
                    var text2 = node2.Options[j].Text;
                    var value2 = node2.Options[j].GetAttribute("value");              

                    if (text.Contains("(停") || text2.Contains("(停"))
                        continue;

                    count2++;
                    var broker = new Broker();
                    broker.Id = Guid.NewGuid();
                    broker.MainName = text;
                    broker.BHID = value;
                    broker.BrokerName = text2;
                    broker.b = value2;
                    context.Brokers.Add(broker);
                    context.SaveChanges();
                    Console.WriteLine($"{count2} {text} {value} :: {text2} {value2}");
                }
            }

            Console.ReadLine();
        }
    }
}
