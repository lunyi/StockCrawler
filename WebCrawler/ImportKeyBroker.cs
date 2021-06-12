using DataService.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class ImportKeyBroker
    {
        public ImportKeyBroker()
        { 
        }

        public async Task RunAsync()
        {
            var context = new StockDbContext();
            var s = new StreamReader(@"C:\Users\lunyi\Downloads\券商分點.csv");
            var r = await s.ReadToEndAsync();
            var ss = r.Split("\r\n");
            int count = 1;
            for (int i = 1; i < ss.Length; i++)
            {
                var res = ss[i].Split(",");
                if (res[2] != "")
                { 
                    
                    var keys = FindKey(context, res[2]);
                    if (keys.Length == 3)
                    {
                        var k = new KeyBroker
                        {
                            Id = Guid.NewGuid(),
                            StockId = res[0],
                            Name = res[1],
                            BrokerName = keys[0],
                            BHID = keys[1],
                            b = keys[2]
                        };
                        await context.KeyBrokers.AddAsync(k);
                        Console.WriteLine($"{count}.{string.Join(",", res)}");
                        count++;
                    }

                }
            }
            await context.SaveChangesAsync();
            Console.ReadLine();
        }

        private string[] FindKey(StockDbContext context, string keyOnTheFile)
        {
            var brokers = context.Brokers.ToList();
            for (int i = 0; i < brokers.Count; i++)
            {
                var ori = brokers[i].BrokerName.Replace("-", "");

                if (ori == keyOnTheFile)
                {
                    return new[] { $"{brokers[i].BrokerName}", brokers[i].BHID, brokers[i].b };
                }
                else if (ori == keyOnTheFile + "證券")
                {
                    return new[] { $"{brokers[i].BrokerName}", brokers[i].BHID, brokers[i].b };
                }
            }

            return new string[0];
        }
    }
}
