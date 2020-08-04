﻿using System;
using System.Collections.Generic;

namespace DataService.Models
{
    public partial class Prices
    {
        public Guid Id { get; set; }
        public string StockId { get; set; }
        public string Name { get; set; }
        public DateTime Datetime { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal 漲跌 { get; set; }
        public decimal 漲跌百分比 { get; set; }
        public int 成交量 { get; set; }
        public int 成交金額 { get; set; }
        public decimal 本益比 { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? 融資買進 { get; set; }
        public int? 融資賣出 { get; set; }
        public int? 融資現償 { get; set; }
        public int? 融資餘額 { get; set; }
        public decimal? 融資使用率 { get; set; }
        public int? 融券買進 { get; set; }
        public int? 融券賣出 { get; set; }
        public int? 融券餘額 { get; set; }
        public int? 資券相抵 { get; set; }
        public int? 外資持股 { get; set; }
        public decimal? 外資持股比例 { get; set; }
        public int? 外資買賣超 { get; set; }
        public int? 投信買賣超 { get; set; }
        public int? 自營商買賣超 { get; set; }
        public decimal? 當沖張數 { get; set; }
        public decimal? 當沖比例 { get; set; }
        public decimal? 當沖總損益 { get; set; }
        public decimal? 當沖均損益 { get; set; }
        public decimal? MA3 { get; set; }
        public decimal? MA5 { get; set; }
        public decimal? MA10 { get; set; }
        public decimal? MA20 { get; set; }
        public decimal? MA60 { get; set; }
        public decimal? MA120 { get; set; }
        public decimal? MA240 { get; set; }
        public decimal? VMA3 { get; set; }
        public decimal? VMA5 { get; set; }
        public decimal? VMA10 { get; set; }
        public decimal? VMA20 { get; set; }
        public decimal? VMA60 { get; set; }
        public decimal? VMA120 { get; set; }
        public decimal? VMA240 { get; set; }
        public decimal? 主力買超張數 { get; set; }
        public decimal? 主力賣超張數 { get; set; }
        public decimal? 五日主力買超張數 { get; set; }
        public decimal? 五日主力賣超張數 { get; set; }
        public decimal? 十日主力買超張數 { get; set; }
        public decimal? 十日主力賣超張數 { get; set; }
        public decimal? 二十日主力買超張數 { get; set; }
        public decimal? 二十日主力賣超張數 { get; set; }
        public decimal? 四十日主力買超張數 { get; set; }
        public decimal? 四十日主力賣超張數 { get; set; }
        public decimal? 六十日主力買超張數 { get; set; }
        public decimal? 六十日主力賣超張數 { get; set; }
        public int? 投信持股 { get; set; }
        public decimal? 投信持股比例 { get; set; }
        public int? 自營商持股 { get; set; }
        public int? 董監持股 { get; set; }
        public decimal? 董監持股比例 { get; set; }
        public decimal DIF { get; set; }
        public decimal MACD { get; set; }
        public decimal OSC { get; set; }
        public decimal RSV { get; set; }
        public decimal K { get; set; }
        public decimal D { get; set; }
    }
}
