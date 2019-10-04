using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace WebCrawler
{
    class Program
    {
        static IWebDriver _driver;

        static string LoginUrl = "https://statementdog.com/users/sign_in";
        static string HealthCheckUrl = "https://statementdog.com/analysis/tpe/6153/stock-health-check";

        static void Main(string[] args)
        {
            Initial();

            Complete();
        }

        static void Initial()
        {
            ChromeOptions chromeBrowserOptions = new ChromeOptions();
            //需要阻擋跳出視窗時，可將下面註解移除
            chromeBrowserOptions.AddArgument("--disable-popup-blocking");
            //要節省流量，不載入圖片的時候，可將下面註解移除
            //chromeBrowserOptions.AddExtension(@"ChromeDriver\Block-image_v1.1.crx"); // 載入阻擋圖片外掛程式
            _driver = new ChromeDriver(@"..\..\..\..\BrowserPath", chromeBrowserOptions);
            //可避免網頁被cache住，一直查到舊資料(需不斷重複查詢同一網頁時就會用到)
            _driver.Manage().Cookies.DeleteAllCookies();
            _driver.Navigate().GoToUrl(LoginUrl);
            var emailElement = _driver.FindElement(By.Id("user_email"));
            var passwordElement = _driver.FindElement(By.Id("user_password"));
            var submitElement = _driver.FindElement(By.ClassName("submit-btn"));

            emailElement.SendKeys("lunyi.lester@gmail.com");
            passwordElement.SendKeys("1q2w3e4r");
            submitElement.Click();
        }

        static void Complete()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}
