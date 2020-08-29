using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebAutoCrawler
{
    public abstract class BaseCrawler2 : IDisposable
    {
        private static IWebDriver _driver;
        public BaseCrawler2()
        {
            ChromeOptions chromeBrowserOptions = new ChromeOptions();
            //需要阻擋跳出視窗時，可將下面註解移除
            chromeBrowserOptions.AddArgument("--disable-popup-blocking");
            //要節省流量，不載入圖片的時候，可將下面註解移除
            //chromeBrowserOptions.AddExtension(@"ChromeDriver\Block-image_v1.1.crx"); // 載入阻擋圖片外掛程式
            _driver = new ChromeDriver(@"D:\Code\StockCrawlerNew\BrowserPath", chromeBrowserOptions);
            //可避免網頁被cache住，一直查到舊資料(需不斷重複查詢同一網頁時就會用到)
            _driver.Manage().Cookies.DeleteAllCookies();
        }

        protected static void GoToUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        protected static IWebElement FindElement(By by)
        {
            return _driver.FindElement(by);
        }

        protected static ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _driver.FindElements(by);
        }

        public void Dispose()
        {
            _driver.Close();
            _driver.Dispose();
        }
    }
}
