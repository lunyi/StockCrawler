using OpenQA.Selenium;
using System.Threading.Tasks;

namespace WebAutoCrawler
{
    public class StatementDogCrawler : BaseCrawler
    {
        string LoginUrl = "https://statementdog.com/users/sign_in";
        string HealthCheckUrl = "https://statementdog.com/analysis/tpe/{0}/stock-health-check";
        string Username = "lunyi.lester@gmail.com";
        string Password = "1q2w3e4r";

        public StatementDogCrawler() : base()
        {
            GoToUrl(LoginUrl);
            var emailElement = FindElement(By.Id("user_email"));
            var passwordElement = FindElement(By.Id("user_password"));
            var submitElement = FindElement(By.ClassName("submit-btn"));

            emailElement.SendKeys(Username);
            passwordElement.SendKeys(Password);
            submitElement.Click();
        }
        public override async Task ExecuteAsync()
        {
            //var context = new StockDbContext();
            //var stocks = context.Stocks
            //    .Where(p => p.Status == 1)
            //    .OrderBy(p => p.StockId)
            //    .ToList();

            //foreach (var stock in stocks)
            //{
            //    try
            //    {
            //        var checks = Parser(string.Format(HealthCheckUrl, stock.StockId));
            //        foreach (var check in checks)
            //        {
            //            foreach (var checkItem in check.Value)
            //            {
            //                var item = new AnaStatementDog
            //                {
            //                    Id = Guid.NewGuid(),
            //                    StockId = stock.StockId,
            //                    Name = stock.Name,
            //                    Type = check.Key,
            //                    Pass = checkItem.Result,
            //                    Description = checkItem.Name,
            //                    CreatedOn = DateTime.Now,
            //                };
            //                context.AnaStatementDogs.Add(item);
            //            }
            //        }
            //        await context.SaveChangesAsync();
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine($"{stock.StockId} {stock.Name} Parser Failed !");
            //    }
            //}
        }

        //private Dictionary<string, CheckModel[]> Parser(string url)
        //{
        //    GoToUrl(url);

        //    var checks = FindElements(By.ClassName("stock-health-check-module"));
        //    var list = new Dictionary<string, CheckModel[]>();

        //    for (int i = 0; i < 4; i++)
        //    {
        //        var check = checks[i];
        //        var titleItem = check.FindElement(By.ClassName("stock-health-check-module-title"));
        //        var item = check.FindElement(By.ClassName("stock-health-check-module-modal-active-btn"));
        //        Thread.Sleep(500);
        //        item.Click();
        //        var checkItems = check.FindElements(By.ClassName("stock-health-check-item"));
        //        var checkModels = new List<CheckModel>();

        //        foreach (var checkItem in checkItems)
        //        {
        //            var result = checkItem.FindElement(By.ClassName("stock-health-check-item-result"));
        //            var name = checkItem.FindElement(By.ClassName("stock-health-check-item-name"));

        //            var model = new CheckModel
        //            {
        //                Name = name.Text,
        //                Result = result.Text == "通過" ? true : false
        //            };
        //            checkModels.Add(model);
        //        }

        //        list.Add(titleItem.Text, checkModels.ToArray());
        //        var closeBtn = check.FindElement(By.ClassName("modal-btn"));
        //        closeBtn.Click();
        //    }
        //    return list;
        //}
    }
}
