using System;
using System.Data;
using SKCOMLib;

namespace RealtimeChooseStock
{
    class Program
    {
        static readonly SKCenterLib PskCenter = new SKCenterLib();
        static SKReplyLib m_pSKReply;
        //static SKReply skReply1;
        static void Main(string[] args)
        {
            m_pSKReply = new SKReplyLib();
            m_pSKReply.OnReplyMessage += OnAnnouncement;
            PskCenter.SKCenterLib_SetAuthority(1);
            Login();
            Console.ReadLine();
        }

        private static void Login()
        {
            var responseCode = PskCenter.SKCenterLib_Login("M121591178", "1q2w3e");
            if (responseCode == 0)
            {
                Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "登入成功");
                var quote = new TwQuote();
                quote.QueryStocks();
            }
            else
            {
                Console.WriteLine($"Response code : {responseCode}");
            }
        }

        static void OnAnnouncement(string strUserID, string bstrMessage, out short nConfirmCode)
        {
            Console.WriteLine(strUserID + "_" + bstrMessage);
            nConfirmCode = -1;

        }
    }
}
