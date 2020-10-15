﻿namespace DataService.Enums
{
    public enum ChooseStockType
    {
        一日漲幅排行榜 = 1,
        五日漲幅排行榜 = 2,
        十日漲幅排行榜 = 10,
        二十日漲幅排行 = 23,
        四十日漲幅排行 = 30,
        六十日漲幅排行 = 31,
        外資投信同步買超排行榜 = 3,
        投信連續買超排行榜 = 4,
        主力連續買超排行榜 = 22,
        主力連續賣超排行榜 = -22,
        融資連續買超排行榜 = 32,
        融資連續賣超排行榜 = -32,
        ROE大於15且股價小於50 = 33,
        Get淨值比小於2AndROE大於10 = 34,
        投信突然加入買方 = 35,
        每周投信買散戶賣 = 36,
        連續上漲天數 = 37,
        投信突然進前20名 = 38,
        外資突然進前20名 = 39,
        漲停板 = 40,
        投量比加主力買超 = 41,
        當沖比例 = 42,
        當沖總損益 = 43,
        當沖均損益 = 44,
        上漲破五日均 = 47,
        上漲破月線 = 50,   
        MACD和KD同時轉上 = 51,
        盤整突破 = 52,
        主力外資融資買進 = 53,
        MA5和MA10上彎 = 54,
        融資突然買進 = 55,
        連續三個月董資持股增加 = 56,

        均線上揚第1天 = 57,
        均線上揚第2天 = 58,
        均線上揚第3天 = 59,
        均線上揚第4天 = 60,
        均線上揚第5天 = 61,
        均線上揚第6天 = 62,
        均線上揚第7天 = 63,
        均線上揚第12天 = 64,

        連續兩天漲停板 = 48,
        多頭排列 = 45,
        三天漲百分之二十 = 46,
        三天漲百分之二十且投信買超 = 49,
        外資買超排行榜 = 5,
        投信買超排行榜 = 6,
        自營買超排行榜 = 7,
        融資買超排行榜 = 8,
        融券賣超排行榜 = 9,

        外資連續買超排行榜 = 13,
        外資連續賣超排行榜 = -13,

        買方籌碼集中排行榜 = 14,
        五日買方籌碼集中度排行榜 = 24,
        十日買方籌碼集中度排行榜 = 25,
        二十日買方籌碼集中度排行榜 = 26,
        六十日買方籌碼集中度排行榜 = 27,

        每周成交量增長排行榜 = 29,
        賣方籌碼集中排行榜 = -14,
        五日賣方籌碼集中度排行榜 = -24,
        十日賣方籌碼集中度排行榜 = -25,
        二十日賣方籌碼集中度排行榜 = -26,
        六十日賣方籌碼集中度排行榜 = -27,

        董監買賣超排行榜 = 16,
        當週大戶比例增加 = 20,
        連續兩週大戶增散戶減 = 15,
        連續大戶增加 = 19,

        連續十二月單月年增率成長 = 17,
        近月營收累積年增率成長 = 28,
        半年線附近 = 18,

        外資主力同步買超排行榜 = 21,
        CMoney選股 = 11,
        財報狗選股 = 12,

        一日跌幅排行榜 = -1,
        五日跌幅排行榜 = -2,
        外資投信同步賣超排行榜 = -3,
        投信連續賣超排行榜 = -4,
        外資賣超排行榜 = -5,
        投信賣超排行榜 = -6,
        自營賣超排行榜 = -7,
        融資賣超排行榜 = -8,
        融券買超排行榜 = -9,
    }
}
