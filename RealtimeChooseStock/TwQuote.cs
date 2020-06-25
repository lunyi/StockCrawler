using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataService.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SKCOMLib;

namespace RealtimeChooseStock
{
    public class TwQuote
    {
        private bool m_bfirst = true;
        private int m_nCode;
        private string StockName;
        private StockDbContext DbContext;
        public delegate void MyMessageHandler(string strType, int nCode, string strMessage);
        public event MyMessageHandler GetMessage;
        private ConcurrentBag<MinuteKLine> MinuteKines = new ConcurrentBag<MinuteKLine>();
        private SKQuoteLib m_SKQuoteLib { get; set; }
        private DataTable dtStocks;
        private DataTable dtBest5Ask;
        private DataTable dtBest5Bid;
        private static object locker = new object();

        public TwQuote(StockDbContext dbContext)
        {
            dtStocks = CreateStocksDataTable();
            dtBest5Ask = CreateBest5AskTable();
            dtBest5Bid = CreateBest5AskTable();
            m_SKQuoteLib = new SKQuoteLibClass();
            DbContext = dbContext;
            Connect();
        }

        void SendReturnMessage(string strType, int nCode, string strMessage)
        {
            GetMessage?.Invoke(strType, nCode, strMessage);
        }

        private void Connect()
        {
            if (m_bfirst == true)
            {
                m_SKQuoteLib.OnConnection += m_SKQuoteLib_OnConnection;
                m_SKQuoteLib.OnNotifyQuote += m_SKQuoteLib_OnNotifyQuote;
                m_SKQuoteLib.OnNotifyBest5 += m_SKQuoteLib_OnNotifyBest5;
                m_SKQuoteLib.OnNotifyKLineData += m_SKQuoteLib_OnNotifyKLineData;
                //m_SKQuoteLib.OnNotifyServerTime += m_SKQuoteLib_OnNotifyServerTime;
                m_SKQuoteLib.OnNotifyMarketTot += m_SKQuoteLib_OnNotifyMarketTot;
                m_SKQuoteLib.OnNotifyMarketBuySell += m_SKQuoteLib_OnNotifyMarketBuySell;
                m_bfirst = false;
            }
            m_nCode = m_SKQuoteLib.SKQuoteLib_EnterMonitor();
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_EnterMonitor");
        }

        public void GetPricesByStockId(string stockId, string stockName)
        {
            lock (locker)
            {
                StockName = stockName;
                m_nCode = m_SKQuoteLib.SKQuoteLib_RequestKLine(stockId, KLineType.PerMinute, OutputFormat.New);
            }
        }

        //public void GetPricesByStocks(Stocks[] stocks)
        //{
        //    Parallel.ForEach(stocks, currentStock =>
        //    {
        //        StockName = currentStock.Name;
        //        Console.WriteLine($"{DateTime.UtcNow} {currentStock.StockId} {StockName}");
        //        m_nCode = m_SKQuoteLib.SKQuoteLib_RequestKLine(currentStock.StockId, KLineType.PerMinute, OutputFormat.New);
        //    });
        //}

        void m_SKQuoteLib_OnNotifyKLineData(string bstrStockNo, string bstrData)
        {
            var data = bstrData.Split(new[] { ',' });
            var datetime = Convert.ToDateTime(data[0]);

            if (datetime <= DateTime.Today)
                return;

            var k = new MinuteKLine
            {
                Datetime = datetime,
                StockId = bstrStockNo,
                Name = StockName,
                Open = Convert.ToDecimal(data[1]),
                High = Convert.ToDecimal(data[2]),
                Low = Convert.ToDecimal(data[3]),
                Close = Convert.ToDecimal(data[4]),
                Volume = Convert.ToInt32(data[5]),
            };
            MinuteKines.Add(k);
        }

        public void SaveMinuteKLines()
        {
            DbContext.BulkInsert(MinuteKines.ToArray());
        }
        void m_SKQuoteLib_OnConnection(int nKind, int nCode)
        {
            if (nKind == 3001)
            {
                if (nCode == 0)
                {
                    Console.WriteLine("Connecting....");
                }
            }
            else if (nKind == 3002)
            {
                Console.WriteLine("Connection failed!!");
            }
            else if (nKind == 3003)
            {
                Console.WriteLine("Connection OK!!");
            }
            else if (nKind == 3021)//網路斷線
            {
                Console.WriteLine("Disconnection !!");
            }
        }

        private void TickStop(object sender, EventArgs e)
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestTicks(50, "2330");

            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_CancelRequestTicks");
        }

        void m_SKQuoteLib_OnNotifyQuote(short sMarketNo, short sStockIdx)
        {
            var pSKStock = new SKSTOCK();

            m_SKQuoteLib.SKQuoteLib_GetStockByIndex(sMarketNo, sStockIdx, ref pSKStock);

            OnUpDateDataRow(pSKStock);
        }

        void m_SKQuoteLib_OnNotifyBest5(short sMarketNo, short sStockIdx, int nBestBid1, int nBestBidQty1, int nBestBid2, int nBestBidQty2, int nBestBid3, int nBestBidQty3, int nBestBid4, int nBestBidQty4, int nBestBid5, int nBestBidQty5, int nExtendBid, int nExtendBidQty, int nBestAsk1, int nBestAskQty1, int nBestAsk2, int nBestAskQty2, int nBestAsk3, int nBestAskQty3, int nBestAsk4, int nBestAskQty4, int nBestAsk5, int nBestAskQty5, int nExtendAsk, int nExtendAskQty, int nSimulate)
        {
            var pSKStock = new SKSTOCK();
            double dDigitNum = 0.000;
            string strStockNoTick = "2330";
            var nCode = m_SKQuoteLib.SKQuoteLib_GetStockByNo(strStockNoTick, ref pSKStock);
            //[-1022-a-]
            if (nCode == 0)
                dDigitNum = (Math.Pow(10, pSKStock.sDecimal));
            else
                dDigitNum = 100.00;//default value

            if (dtBest5Ask.Rows.Count == 0 && dtBest5Bid.Rows.Count == 0)
            {
                DataRow myDataRow;

                myDataRow = dtBest5Ask.NewRow();
                myDataRow["m_nAskQty"] = nBestAskQty1;
                myDataRow["m_nAsk"] = nBestAsk1 / dDigitNum;///100.00;
                dtBest5Ask.Rows.Add(myDataRow);

                myDataRow = dtBest5Ask.NewRow();
                myDataRow["m_nAskQty"] = nBestAskQty2;
                myDataRow["m_nAsk"] = nBestAsk2 / dDigitNum;//100.00;
                dtBest5Ask.Rows.Add(myDataRow);

                myDataRow = dtBest5Ask.NewRow();
                myDataRow["m_nAskQty"] = nBestAskQty3;
                myDataRow["m_nAsk"] = nBestAsk3 / dDigitNum;//100.00;
                dtBest5Ask.Rows.Add(myDataRow);

                myDataRow = dtBest5Ask.NewRow();
                myDataRow["m_nAskQty"] = nBestAskQty4;
                myDataRow["m_nAsk"] = nBestAsk4 / dDigitNum;// 100.00;
                dtBest5Ask.Rows.Add(myDataRow);

                myDataRow = dtBest5Ask.NewRow();
                myDataRow["m_nAskQty"] = nBestAskQty5;
                myDataRow["m_nAsk"] = nBestAsk5 / dDigitNum;// 100.00;
                dtBest5Ask.Rows.Add(myDataRow);



                myDataRow = dtBest5Bid.NewRow();
                myDataRow["m_nAskQty"] = nBestBidQty1;
                myDataRow["m_nAsk"] = nBestBid1 / dDigitNum;
                dtBest5Bid.Rows.Add(myDataRow);

                myDataRow = dtBest5Bid.NewRow();
                myDataRow["m_nAskQty"] = nBestBidQty2;
                myDataRow["m_nAsk"] = nBestBid2 / dDigitNum;
                dtBest5Bid.Rows.Add(myDataRow);

                myDataRow = dtBest5Bid.NewRow();
                myDataRow["m_nAskQty"] = nBestBidQty3;
                myDataRow["m_nAsk"] = nBestBid3 / dDigitNum;
                dtBest5Bid.Rows.Add(myDataRow);

                myDataRow = dtBest5Bid.NewRow();
                myDataRow["m_nAskQty"] = nBestBidQty4;
                myDataRow["m_nAsk"] = nBestBid4 / dDigitNum;
                dtBest5Bid.Rows.Add(myDataRow);

                myDataRow = dtBest5Bid.NewRow();
                myDataRow["m_nAskQty"] = nBestBidQty5;
                myDataRow["m_nAsk"] = nBestBid5 / dDigitNum;
                dtBest5Bid.Rows.Add(myDataRow);
            }
            else
            {
                dtBest5Ask.Rows[0]["m_nAskQty"] = nBestAskQty1;
                dtBest5Ask.Rows[0]["m_nAsk"] = nBestAsk1 / dDigitNum;

                dtBest5Ask.Rows[1]["m_nAskQty"] = nBestAskQty2;
                dtBest5Ask.Rows[1]["m_nAsk"] = nBestAsk2 / dDigitNum;

                dtBest5Ask.Rows[2]["m_nAskQty"] = nBestAskQty3;
                dtBest5Ask.Rows[2]["m_nAsk"] = nBestAsk3 / dDigitNum;

                dtBest5Ask.Rows[3]["m_nAskQty"] = nBestAskQty4;
                dtBest5Ask.Rows[3]["m_nAsk"] = nBestAsk4 / dDigitNum;

                dtBest5Ask.Rows[4]["m_nAskQty"] = nBestAskQty5;
                dtBest5Ask.Rows[4]["m_nAsk"] = nBestAsk5 / dDigitNum;


                dtBest5Bid.Rows[0]["m_nAskQty"] = nBestBidQty1;
                dtBest5Bid.Rows[0]["m_nAsk"] = nBestBid1 / dDigitNum;

                dtBest5Bid.Rows[1]["m_nAskQty"] = nBestBidQty2;
                dtBest5Bid.Rows[1]["m_nAsk"] = nBestBid2 / dDigitNum;

                dtBest5Bid.Rows[2]["m_nAskQty"] = nBestBidQty3;
                dtBest5Bid.Rows[2]["m_nAsk"] = nBestBid3 / dDigitNum;

                dtBest5Bid.Rows[3]["m_nAskQty"] = nBestBidQty4;
                dtBest5Bid.Rows[3]["m_nAsk"] = nBestBid4 / dDigitNum;

                dtBest5Bid.Rows[4]["m_nAskQty"] = nBestBidQty5;
                dtBest5Bid.Rows[4]["m_nAsk"] = nBestBid5 / dDigitNum;
            }
        }

        void m_SKQuoteLib_OnNotifyServerTime(short sHour, short sMinute, short sSecond, int nTotal)
        {
            Console.WriteLine(sHour.ToString("D2") + ":" + sMinute.ToString("D2") + ":" + sSecond.ToString("D2"));
            //QueryStocks();
        }

        public void QueryStocks()
        {
            short sPage = 0;
            dtStocks.Clear();
            string[] Stocks = { "2330", "2317" };

            foreach (var s in Stocks)
            {
                var pSKStock = new SKSTOCK();
                var nCode = m_SKQuoteLib.SKQuoteLib_GetStockByNo(s.Trim(), ref pSKStock);

                if (nCode == 0)
                {
                    OnUpDateDataRow(pSKStock);
                }

                Console.WriteLine();
            }

            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestStocks(ref sPage, "2330, 2317");

            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_RequestStocks");
        }


        void m_SKQuoteLib_OnNotifyMarketTot(short sMarketNo, short sPrt, int nTime, int nTotv, int nTots, int nTotc)
        {
            double dTotv = nTotv / 100.0;

            if (sMarketNo == 0)
            {
                Console.WriteLine(dTotv.ToString() + "(億)");
                Console.WriteLine(nTots.ToString() + "(張)");
                Console.WriteLine(nTotc.ToString() + "(筆)");
            }
            else
            {
                Console.WriteLine(dTotv.ToString() + "(億)");
                Console.WriteLine(nTots.ToString() + "(張)");
                Console.WriteLine(nTotc.ToString() + "(筆)");
            }
        }

        void m_SKQuoteLib_OnNotifyMarketBuySell(short sMarketNo, short sPrt, int nTime, int nBc, int nSc, int nBs, int nSs)
        {
            if (sMarketNo == 0)
            {
                Console.WriteLine(nBc.ToString() + "(筆)");
                Console.WriteLine(nBs.ToString() + "(張)");
                Console.WriteLine(nSc.ToString() + "(筆)");
                Console.WriteLine(nSs.ToString() + "(張)");
            }
            else
            {
                Console.WriteLine(nBc.ToString() + "(筆)");
                Console.WriteLine(nBs.ToString() + "(張)");
                Console.WriteLine(nSc.ToString() + "(筆)");
                Console.WriteLine(nSs.ToString() + "(張)");
            }
        }

        private DataTable CreateStocksDataTable()
        {
            DataTable myDataTable = new DataTable();

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int16");
            myDataColumn.ColumnName = "m_sStockidx";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int16");
            myDataColumn.ColumnName = "m_sDecimal";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int16");
            myDataColumn.ColumnName = "m_sTypeNo";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "m_cMarketNo";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "m_caStockNo";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "m_caName";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nOpen";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nHigh";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nLow";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nClose";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nTickQty";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nRef";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nBid";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nBc";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nAsk";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nAc";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nTBc";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nTAc";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nFutureOI";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nTQty";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nYQty";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nUp";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nDown";
            myDataTable.Columns.Add(myDataColumn);

            myDataTable.PrimaryKey = new DataColumn[] { myDataTable.Columns["m_caStockNo"] };

            return myDataTable;
        }

        private void OnUpDateDataRow(SKSTOCK pStock)
        {
            DataRow myDataRow = dtStocks.NewRow();

            myDataRow["m_sStockidx"] = pStock.sStockIdx;
            myDataRow["m_sDecimal"] = pStock.sDecimal;
            myDataRow["m_sTypeNo"] = pStock.sTypeNo;
            myDataRow["m_cMarketNo"] = pStock.bstrMarketNo;
            myDataRow["m_caStockNo"] = pStock.bstrStockNo;
            myDataRow["m_caName"] = pStock.bstrStockName;
            myDataRow["m_nOpen"] = pStock.nOpen / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nHigh"] = pStock.nHigh / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nLow"] = pStock.nLow / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nClose"] = pStock.nClose / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nTickQty"] = pStock.nTickQty;
            myDataRow["m_nRef"] = pStock.nRef / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nBid"] = pStock.nBid / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nBc"] = pStock.nBc;
            myDataRow["m_nAsk"] = pStock.nAsk / (Math.Pow(10, pStock.sDecimal));

            myDataRow["m_nAc"] = pStock.nAc;
            myDataRow["m_nTBc"] = pStock.nTBc;
            myDataRow["m_nTAc"] = pStock.nTAc;
            myDataRow["m_nFutureOI"] = pStock.nFutureOI;
            myDataRow["m_nTQty"] = pStock.nTQty;
            myDataRow["m_nYQty"] = pStock.nYQty;
            myDataRow["m_nUp"] = pStock.nUp / (Math.Pow(10, pStock.sDecimal));
            myDataRow["m_nDown"] = pStock.nDown / (Math.Pow(10, pStock.sDecimal));

            Console.WriteLine($"{pStock.sStockIdx} {pStock.bstrStockNo} {pStock.bstrStockName} {pStock.nYQty} {pStock.nUp / (Math.Pow(10, pStock.sDecimal))} {pStock.nDown / (Math.Pow(10, pStock.sDecimal))} {pStock.nTickQty}");
            //Console.WriteLine(pStock.bstrStockNo);
            //Console.WriteLine(pStock.bstrStockName);
            //Console.WriteLine(pStock.nOpen / (Math.Pow(10, pStock.sDecimal)));
            //Console.WriteLine(pStock.nHigh / (Math.Pow(10, pStock.sDecimal)));
            //Console.WriteLine(pStock.nLow / (Math.Pow(10, pStock.sDecimal)));
            //Console.WriteLine(pStock.nClose / (Math.Pow(10, pStock.sDecimal)));

            //Console.WriteLine(pStock.nYQty);
            //Console.WriteLine(pStock.nUp / (Math.Pow(10, pStock.sDecimal)));
            //Console.WriteLine(pStock.nDown / (Math.Pow(10, pStock.sDecimal)));

            //Console.WriteLine(pStock.nTickQty);
            //Console.WriteLine(pStock.sStockIdx);
            //Console.WriteLine(pStock.sStockIdx);
            //Console.WriteLine(pStock.sStockIdx);
            //Console.WriteLine(pStock.sStockIdx);
            //Console.WriteLine(pStock.sStockIdx);


            //dtStocks.Rows.Add(myDataRow);
        }

        private DataTable CreateBest5AskTable()
        {
            DataTable myDataTable = new DataTable();

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "m_nAskQty";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Double");
            myDataColumn.ColumnName = "m_nAsk";
            myDataTable.Columns.Add(myDataColumn);

            return myDataTable;

        }

        private void update_Click(object sender, EventArgs e)
        {
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_MarketTrading");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_GetMarketBuySellUpDown();
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_RequestMarketBuySellUpDown");
        }

        private void RequestFutureTradeInfo()
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestFutureTradeInfo(0, "TX00");
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_RequestFutureTradeInfo");
        }

        private void CancelFutureTradeInfo(object sender, EventArgs e)
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestFutureTradeInfo(50, "TX00");
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_CancelRequestFutureTradeInfo");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_LeaveMonitor();
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_LeaveMonitor");
        }

        private void btnTicks_Click(object sender, EventArgs e)
        {
            short sPage = 0;
            dtBest5Ask.Clear();
            dtBest5Bid.Clear();

            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestTicks(ref sPage, "2330");
            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_RequestTicks");
        }

        private void GetServerTime()
        {
            m_nCode = m_SKQuoteLib.SKQuoteLib_RequestServerTime();

            SendReturnMessage("Quote", m_nCode, "SKQuoteLib_RequestServerTime");
        }
    }


    public class KLineType
    {
        public const short PerMinute = 0;
        public const short Per5Minutes = 1;
        public const short Per30Minutes = 2;
        public const short PerDay = 3;
        public const short PerWholeDay = 4;
        public const short PerWeek = 5;
        public const short PerMonth = 6;
    }

    public class OutputFormat
    {
        public const short Old = 0;
        public const short New = 1;
    }
}