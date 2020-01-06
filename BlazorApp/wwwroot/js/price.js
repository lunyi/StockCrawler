(function () {

    var stockId = getUrlParameter('stockId');

    var url = "http://" + window.location.hostname + ":8081/stock/" + stockId;

    if (window.location.hostname === "localhost") {
        url = "https://localhost:44367/stock/" + stockId;
    }
    
    var kUrl = "https://so.cnyes.com/JavascriptGraphic/chartstudy.aspx?country=tw&market=tw&code=" + stockId + "&divwidth=550&divheight=330";


    $.get(url, function (data) {

        $("#stockName").text(data.stock.stockId + " "　+ data.stock.name);
        $("#income").text(data.stock.營收比重);
        $("#address").text(data.stock.address);
        $("#asset").text(data.stock.股本);
        $("#cValue").text(data.stock.每股淨值);
        $("#YValue").text(data.stock.每股盈餘);
        $("#website").attr("href", data.stock.website);
        $("#kChart").attr("src", kUrl);
        //console.log(renewPrices(data.monthData));

        var vue = new Vue({
            el: '#textExample',
            data: {
                Stock: data.stock,
                Prices: renewPrices(data.prices)
            }  
        });

        var vue2 = new Vue({
            el: '#month',
            data: {
                Months: renewMonth(data.monthData)
            }
        });

        var vueThousand = new Vue({
            el: '#weekChip',
            data: {
                WeekChips: renewWeekChip(data.weeklyChip)
            }
        });
    });
})();

function renewWeekChip(weekChips) {

    for (var i = 0; i < weekChips.length - 1; i++) {
        var close = (100 * (weekChips[i].close - weekChips[i + 1].close) / weekChips[i + 1].close).toFixed(2);

        if (weekChips[i].close > weekChips[i + 1].close) {
            weekChips[i].close = weekChips[i].close + "↑ (" + close + "%)";
            weekChips[i].updownColor = "red";
        }
        else if (weekChips[i].close < weekChips[i + 1].close) {
            weekChips[i].close = weekChips[i].close + "↓ (" + close + "%)";
            weekChips[i].updownColor = "green";
        }

        if (weekChips[i].pUnder100 > weekChips[i + 1].pUnder100) {
            weekChips[i].pUnder100 = weekChips[i].pUnder100 + "↑ (" + weekChips[i].sUnder100 + ")";
            weekChips[i].sColor = "red";
        }
        else if (weekChips[i].pUnder100 < weekChips[i + 1].pUnder100) {
            weekChips[i].pUnder100 = weekChips[i].pUnder100 + "↓ (" + weekChips[i].sUnder100 + ")";
            weekChips[i].sColor = "green";
        }
        if (weekChips[i].pOver1000 > weekChips[i + 1].pOver1000) {
            weekChips[i].pOver1000 = weekChips[i].pOver1000 + "↑ (" + weekChips[i].sOver1000 + ")";
            weekChips[i].bColor = "red";
        }
        else if (weekChips[i].pOver1000 < weekChips[i + 1].pOver1000){
            weekChips[i].pOver1000 = weekChips[i].pOver1000 + "↓ (" + weekChips[i].sOver1000 + ")";
            weekChips[i].bColor = "green";
        }

        if (weekChips[i].pOver400 > weekChips[i + 1].pOver400) {
            weekChips[i].pOver400 = weekChips[i].pOver400 + "↑ (" + weekChips[i].sOver400 + ")";
            weekChips[i].p400UpColor = "red";
        }
        else if (weekChips[i].pOver400 < weekChips[i + 1].pOver400) {
            weekChips[i].pOver400 = weekChips[i].pOver400 + "↓ (" + weekChips[i].sOver400 + ")";
            weekChips[i].p400UpColor = "green";
        }

        if (weekChips[i].pUnder400 > weekChips[i + 1].pUnder400) {
            weekChips[i].pUnder400 = weekChips[i].pUnder400 + "↑ (" + weekChips[i].sUnder400 + ")";
            weekChips[i].p400DownColor = "red";
        }
        else if (weekChips[i].pUnder400 < weekChips[i + 1].pUnder400) {
            weekChips[i].pUnder400 = weekChips[i].pUnder400 + "↓ (" + weekChips[i].sUnder400 + ")";
            weekChips[i].p400DownColor = "green";
        }

        if (weekChips[i].董監持股 > weekChips[i + 1].董監持股) {
            weekChips[i].董監持股 = weekChips[i].董監持股 + "↑";
            weekChips[i].doColor = "red";
        }
        else if (weekChips[i].董監持股 < weekChips[i + 1].董監持股) {
            weekChips[i].董監持股 = weekChips[i].董監持股 + "↓";
            weekChips[i].doColor = "green";
        }

        if (weekChips[i].融資買賣超 > 0) {
            weekChips[i].融資買賣超 = weekChips[i].融資買賣超 + "↑";
            weekChips[i].zoColor = "red";
        }
        else if (weekChips[i].融資買賣超 < 0) {
            weekChips[i].融資買賣超 = weekChips[i].融資買賣超 + "↓";
            weekChips[i].zoColor = "green";
        }

        if (weekChips[i].外資買賣超 > 0) {
            weekChips[i].外資買賣超 = weekChips[i].外資買賣超 + "↑";
            weekChips[i].wiColor = "red";
        }
        else if (weekChips[i].外資買賣超 < 0) {
            weekChips[i].外資買賣超 = weekChips[i].外資買賣超 + "↓";
            weekChips[i].wiColor = "green";
        }

        if (weekChips[i].投信買賣超 > 0) {
            weekChips[i].投信買賣超 = weekChips[i].投信買賣超 + "↑";
            weekChips[i].toColor = "red";
        }
        else if (weekChips[i].投信買賣超 < 0) {
            weekChips[i].投信買賣超 = weekChips[i].投信買賣超 + "↓";
            weekChips[i].toColor = "green";
        }

        if (weekChips[i].自營商買賣超 > 0) {
            weekChips[i].自營商買賣超 = weekChips[i].自營商買賣超 + "↑";
            weekChips[i].ziColor = "red";
        }
        else if (weekChips[i].自營商買賣超 < 0) {
            weekChips[i].自營商買賣超 = weekChips[i].自營商買賣超 + "↓";
            weekChips[i].ziColor = "green";
        }

        if (weekChips[i].主力買賣超 > 0) {
            weekChips[i].主力買賣超 = weekChips[i].主力買賣超 + "↑";
            weekChips[i].zuColor = "red";
        }
        else if (weekChips[i].主力買賣超 < 0) {
            weekChips[i].主力買賣超 = weekChips[i].主力買賣超 + "↓";
            weekChips[i].zuColor = "green";
        }
    }
    return weekChips;
}

function renewMonth(months) {
    for (var i = 0; i < months.length; i++) {
        if (months[i].單月年增率 > 0) {
            months[i].單月年增率 = months[i].單月年增率 + "↑";
            months[i].updownColor = "red";
        }
        else {
            months[i].單月年增率 = months[i].單月年增率 + "↓";
            months[i].updownColor = "green";
        }

        if (months[i].累積年增率 > 0) {
            months[i].累積年增率 = months[i].累積年增率 + "↑";
            months[i].cColor = "red";
        }
        else {
            months[i].累積年增率 = months[i].累積年增率 + "↓";
            months[i].cColor = "green";
        }
        months[i].datetime = formatDate(months[i].datetime);
    }
    return months;
}
function renewPrices(prices) {
    for (var i = 0; i < prices.length-2; i++) {
        if (prices[i].close > prices[i + 1].close) {
            prices[i].close = prices[i].close + "↑";
            prices[i].updownColor = "red";
        }
        else if (prices[i].close < prices[i + 1].close) {
            prices[i].close = prices[i].close + "↓";
            prices[i].updownColor = "green";
        }

        if (prices[i].董監持股 > prices[i + 1].董監持股) {
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股) + "↑ (" + prices[i].董監持股比例 + "%)";
            prices[i].dColor = "red";
        }
        else if (prices[i].董監持股 < prices[i + 1].董監持股) {
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股) + "↓ (" + prices[i].董監持股比例 + "%)";
            prices[i].dColor = "green";
        }
        else {
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股) + " (" + prices[i].董監持股比例 + "%)";
        }

        if (prices[i].成交量 > (prices[i + 1].成交量 * 2.5)) {
            prices[i].zfontWeight = "bold";
        }
        else {
            prices[i].zfontWeight = "normal";
        }

        if (prices[i].籌碼集中度 > 0) {
            prices[i].chipColor = "red";
        }
        else if (prices[i].籌碼集中度 < 0) {
            prices[i].chipColor = "green";
        }

        if (prices[i].籌碼集中度 > 25 || prices[i].籌碼集中度 < -25) {
            prices[i].cfontWeight = "bold";
        }
        else {
            prices[i].cfontWeight = "normal";
        }

        if (prices[i].外資買賣超 > 0) {
            prices[i].外資買賣超 = prices[i].外資買賣超 + "↑ (" + prices[i].外資持股比例 + "%)";
            prices[i].fColor = "red";
        }
        else if (prices[i].外資買賣超 < 0) {
            prices[i].外資買賣超 = prices[i].外資買賣超 + "↓ (" + prices[i].外資持股比例 + "%)";
            prices[i].fColor = "green";
        }
        else {
            prices[i].外資買賣超 = prices[i].外資買賣超 + " (" + prices[i].外資持股比例 + "%)";
        }

        if (prices[i].投信買賣超 > 0) {
            prices[i].投信買賣超 = prices[i].投信買賣超 + "↑ (" + prices[i].投信持股比例 + "%)";
            prices[i].tColor = "red";
        }
        else if (prices[i].投信買賣超 < 0) {
            prices[i].投信買賣超 = prices[i].投信買賣超 + "↓ (" + prices[i].投信持股比例 + "%)";
            prices[i].tColor = "green";
        }
        else {
            prices[i].投信買賣超 = prices[i].投信買賣超 + " (" + prices[i].投信持股比例 + "%)";
        }

        if (prices[i].自營商買賣超 > 0) {
            prices[i].zColor = "red";
        }
        else if (prices[i].自營商買賣超 < 0) {
            prices[i].zColor = "green";
        }

        if (prices[i].主力買賣超 > 0) {
            prices[i].mColor = "red";
        }
        else if (prices[i].主力買賣超 < 0) {
            prices[i].mColor = "green";
        }

        if (prices[i].漲跌百分比 > 0) {
            prices[i].pColor = "red";
        }
        else if (prices[i].漲跌百分比 < 0) {
            prices[i].pColor = "green";
        }

        if (prices[i].漲跌百分比 > 3 || prices[i].漲跌百分比 < -3) {
            prices[i].fontWeight = "bold";
        }
        else {
            prices[i].fontWeight = "normal";
        }

        prices[i].漲跌 = prices[i].漲跌 + " (" + prices[i].漲跌百分比 + "%)";

        if (prices[i].融資買賣超 > 0) {
            prices[i].融資買賣超 = prices[i].融資買賣超 + "↑ (" + prices[i].融資使用率 + ")";
            prices[i].lColor = "red";
        }
        else if (prices[i].融資買賣超 < 0) {
            prices[i].融資買賣超 = prices[i].融資買賣超 + "↓ (" + prices[i].融資使用率 + ")";
            prices[i].lColor = "green";
        } else {
            prices[i].融資買賣超 = prices[i].融資買賣超 + " (" + prices[i].融資使用率 + ")";
        }

        if (prices[i].融券買賣超 === undefined) {
            prices[i].融券買賣超 = null;
        }
        else if (prices[i].融券買賣超 > 0) {
            prices[i].融券買賣超 = prices[i].融券買賣超 + "↑ (" + prices[i].融券餘額 + ")";
            prices[i].chColor = "red";
        }
        else if (prices[i].融券買賣超 < 0) {
            prices[i].融券買賣超 = prices[i].融券買賣超 + "↓ (" + prices[i].融券餘額 + ")";
            prices[i].chColor = "green";
        } else {
            prices[i].融券買賣超 = prices[i].融券買賣超 + " (" + prices[i].融券餘額 + ")";
        }
    }
    return prices;
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;
    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};