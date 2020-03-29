using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using LineBotApi.Models;
using LineBotLibrary;
using LineBotLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LineBotApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly LineNotifyBotApi _lineNotifyBotApi = null;

        public HomeController(LineNotifyBotApi lineNotifyBotApi)
        {
            _lineNotifyBotApi = lineNotifyBotApi;
        }

        public async Task<IActionResult> Index(string code)
        {
            string callBackUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Action("CallBack"));

            if (!string.IsNullOrWhiteSpace(code))
            {
                var apiResult = await _lineNotifyBotApi.Token(new TokenRequestDTO
                {
                    Code = code,
                    RedirectUri = callBackUrl
                });

                InsertToken(apiResult.Access_Token);
                ViewBag.AccessToken = apiResult.Access_Token;
            }

            ViewBag.ClientAuthorizeUrl = _lineNotifyBotApi.GenerateAuthorizeUrl(callBackUrl, "1234");

            return View();
        }
        private void InsertToken(string token)
        {
            var con = new SqlConnection(@"Data Source=220.133.185.1;Initial Catalog=StockDb;User ID=stock;Password=stock;");
            try
            {
                var sql = $"delete from [Token];Insert into [Token] ([LineToken]) values ('{token}')";
                SqlCommand cmdinsert = new SqlCommand(sql, con);
                con.Open();
                cmdinsert.CommandType = CommandType.Text;
                cmdinsert.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public IActionResult CallBack(AuthorizeCallbackDTO callbackDTO)
        {
            // 可能是第三方攻擊
            if (callbackDTO.State != "1234")
            {
                return new UnauthorizedResult();
            }

            return RedirectToAction("Index", new { callbackDTO.Code });
        }

        public async Task<IActionResult> Notify(string accessToken, string message)
        {
            var apiResult = await _lineNotifyBotApi.Notify(new NotifyRequestDTO
            {
                AccessToken = accessToken,
                Message = accessToken + " " + message
            });

            ViewBag.ResultJson = JsonConvert.SerializeObject(apiResult, Formatting.Indented);

            return PartialView("Notify", accessToken);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
