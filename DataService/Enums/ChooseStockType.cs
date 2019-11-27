﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataService.Enums
{
    public enum ChooseStockType
    {
        一日漲幅排行榜 = 1,
        五日漲幅排行榜 = 2,
        外資投信同步買超排行榜 = 3,
        投信連續買超排行榜 = 4,
        外資買超排行榜 = 5,
        投信買超排行榜 = 6,
        自營買超排行榜 = 7,
        融資買超排行榜 = 8,
        融券賣超排行榜 = 9,

        外資連續買超排行榜 = 13,
        外資連續賣超排行榜 = -13,

        買方籌碼集中排行榜 = 14,
        賣方籌碼集中排行榜 = -14,
        集保庫存排行榜 = 15,
        董監買賣超排行榜 = 16,
        連續十二月單月年增率成長 = 17,
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
