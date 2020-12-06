(function () {

    var stockId = getUrlParameter('stockId');
    var datetime = getUrlParameter('datetime');
    var chkDate = getUrlParameter('chkDate');

    var url = "http://" + window.location.hostname + ":8081/stock/" + stockId + '?datetime=' + datetime + '&chkDate=' + chkDate;

    if (window.location.hostname === "localhost") {
        url = "https://localhost:44367/stock/" + stockId + '?datetime=' + datetime + '&chkDate=' + chkDate;
    }
    
    //var kUrl = "https://so.cnyes.com/JavascriptGraphic/chartstudy.aspx?country=tw&market=tw&code=" + stockId + "&divwidth=550&divheight=330";


    $.get(url, function (data) {

        $("#stockName").text(data.stock.stockId + " "　+ data.stock.name);
        $("#income").text(data.stock.營收比重);
        $("#address").text(data.stock.address);
        $("#asset").text(data.stock.股本);

        $("#roe").text(data.stock.roe);
        $("#roa").text(data.stock.roa);

        $("#cValue").text(data.stock.每股淨值);
        $("#YValue").text(data.stock.每股盈餘);
        $("#price").text(data.stock.股價);
        $("#website").attr("href", data.stock.website);
        //$("#kChart").attr("src", kUrl);
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

        if (weekChips[i].主力買賣超 > 0) {
            weekChips[i].mmColor = "red";
        }
        else if (weekChips[i].主力買賣超 < 0) {
            weekChips[i].mmColor = "green";
        }

        if (weekChips[i].外資買賣超 > 0) {
            weekChips[i].ffColor = "red";
        }
        else if (weekChips[i].外資買賣超 < 0) {
            weekChips[i].ffColor = "green";
        }

        if (weekChips[i].投信買賣超 > 0) {
            weekChips[i].iiColor = "red";
        }
        else if (weekChips[i].投信買賣超 < 0) {
            weekChips[i].iiColor = "green";
        }

        if (weekChips[i].融資買賣超 > 0) {
            weekChips[i].zzColor = "red";
        }
        else if (weekChips[i].融資買賣超 < 0) {
            weekChips[i].zzColor = "green";
        }

        if (weekChips[i].董監買賣超 > 0) {
            weekChips[i].ddColor = "red";
        }
        else if (weekChips[i].董監買賣超 < 0) {
            weekChips[i].ddColor = "green";
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

        if (months[i].董監持股增減 > 0) {
            months[i].iColor = "red";
        }
        else if (months[i].董監持股增減 < 0) {
            months[i].iColor = "green";
        }

        if (months[i].percent > 0) {
            months[i].pColor = "red";
        }
        else if (months[i].percent < 0) {
            months[i].pColor = "green";
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
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股) + "↑";
            prices[i].dColor = "red";
        }
        else if (prices[i].董監持股 < prices[i + 1].董監持股) {
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股) + "↓";
            prices[i].dColor = "green";
        }
        else {
            prices[i].董監持股 = (prices[i].董監持股 - prices[i + 1].董監持股);
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

        if (prices[i].主力買賣比例 > 1) {
            prices[i].chipPColor = "red";
        }
        else if (prices[i].主力買賣比例 < 1) {
            prices[i].chipPColor = "green";
        }

        if (prices[i].主力買賣比例 > 1.5 || prices[i].主力買賣比例 < 0.5) {
            prices[i].cfontPWeight = "bold";
        }
        else {
            prices[i].cfontPWeight = "normal";
        }

        if (prices[i].籌碼集中度 > 25 || prices[i].籌碼集中度 < -25) {
            prices[i].cfontWeight = "bold";
        }
        else {
            prices[i].cfontWeight = "normal";
        }


        if (prices[i].五日籌碼集中度 > 0) {
            prices[i].chip5Color = "red";
        }
        else if (prices[i].五日籌碼集中度 < 0) {
            prices[i].chip5Color = "green";
        }

        if (prices[i].五日籌碼集中度 > 15 || prices[i].五日籌碼集中度 < -15) {
            prices[i].c5fontWeight = "bold";
        }
        else {
            prices[i].c5fontWeight = "normal";
        }


        if (prices[i].十日籌碼集中度 > 0) {
            prices[i].chip10Color = "red";
        }
        else if (prices[i].十日籌碼集中度 < 0) {
            prices[i].chip10Color = "green";
        }

        if (prices[i].十日籌碼集中度 > 12 || prices[i].十日籌碼集中度 < -12) {
            prices[i].c10fontWeight = "bold";
        }
        else {
            prices[i].c10fontWeight = "normal";
        }


        if (prices[i].二十日籌碼集中度 > 0) {
            prices[i].chip20Color = "red";
        }
        else if (prices[i].二十日籌碼集中度 < 0) {
            prices[i].chip20Color = "green";
        }

        if (prices[i].二十日籌碼集中度 > 10 || prices[i].二十日籌碼集中度 < -10) {
            prices[i].c20fontWeight = "bold";
        }
        else {
            prices[i].c20fontWeight = "normal";
        }

        if (prices[i].十日主力買賣比例 > 1) {
            prices[i].chipP10Color = "red";
        }
        else if (prices[i].十日主力買賣比例 < 1) {
            prices[i].chipP10Color = "green";
        }

        if (prices[i].十日主力買賣比例 > 1.3 || prices[i].十日主力買賣比例 < 0.7) {
            prices[i].c10fontPWeight = "bold";
        }
        else {
            prices[i].c10fontPWeight = "normal";
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
            prices[i].投信買賣超 = prices[i].投信買賣超 + "↑";
            prices[i].tColor = "red";
        }
        else if (prices[i].投信買賣超 < 0) {
            prices[i].投信買賣超 = prices[i].投信買賣超 + "↓";
            prices[i].tColor = "green";
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

        //prices[i].漲跌 = prices[i].漲跌 + " (" + prices[i].漲跌百分比 + "%)";

        if (prices[i].融資買賣超 > 0) {
            prices[i].融資買賣超 = prices[i].融資買賣超 + "↑ (" + prices[i].融資使用率 + "%)";
            prices[i].lColor = "red";
        }
        else if (prices[i].融資買賣超 < 0) {
            prices[i].融資買賣超 = prices[i].融資買賣超 + "↓ (" + prices[i].融資使用率 + "%)";
            prices[i].lColor = "green";
        } else {
            prices[i].融資買賣超 = prices[i].融資買賣超 + " (" + prices[i].融資使用率 + "%)";
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

        if (prices[i].mA5.indexOf("↗") > -1) {
            prices[i].ma5Color = "red";
        } else if (prices[i].mA5.indexOf("↘") > -1) {
            prices[i].ma5Color = "green";
            prices[i].mA5 = prices[i].mA5.replace("↘", "↓")
        }

        if (prices[i].mA10 !== null) {
            if (prices[i].mA10.indexOf("↗") > -1) {
                prices[i].ma10Color = "red";
            } else if (prices[i].mA10.indexOf("↘") > -1) {
                prices[i].ma10Color = "green";
                prices[i].mA10 = prices[i].mA10.replace("↘", "↓")
            }
        }

        if (prices[i].mA20 !== null) {
            if (prices[i].mA20.indexOf("↗") > -1) {
                prices[i].ma20Color = "red";
            } else if (prices[i].mA20.indexOf("↘") > -1) {
                prices[i].ma20Color = "green";
                prices[i].mA20 = prices[i].mA20.replace("↘", "↓")
            }
        }

        if (prices[i].k9 !== null) {
            if (prices[i].k9.indexOf("↗") > -1) {
                prices[i].k9Color = "red";
            } else if (prices[i].k9.indexOf("↘") > -1) {
                prices[i].k9Color = "green";
                prices[i].k9 = prices[i].k9.replace("↘", "↓")
            }
        }

        if (prices[i].d9 !== null) {
            if (prices[i].d9.indexOf("↗") > -1) {
                prices[i].d9Color = "red";
            } else if (prices[i].d9.indexOf("↘") > -1) {
                prices[i].d9Color = "green";
                prices[i].d9 = prices[i].d9.replace("↘", "↓")
            }
        }

        if (prices[i].macd !== null) {
            if (prices[i].macd.indexOf("↗") > -1) {
                prices[i].macdColor = "red";
            } else if (prices[i].macd.indexOf("↘") > -1) {
                prices[i].macdColor = "green";
                prices[i].macd = prices[i].macd.replace("↘", "↓");
            }
        }

        if (prices[i].dif !== null) {
            if (prices[i].dif.indexOf("↗") > -1) {
                prices[i].difColor = "red";
            } else if (prices[i].dif.indexOf("↘") > -1) {
                prices[i].difColor = "green";
                prices[i].dif = prices[i].dif.replace("↘", "↓");
            }
        }

        if (prices[i].osc !== null) {
            if (prices[i].osc.indexOf("↗") > -1) {
                prices[i].oscColor = "red";
            } else if (prices[i].osc.indexOf("↘") > -1) {
                prices[i].oscColor = "green";
                prices[i].osc = prices[i].osc.replace("↘", "↓");
            }
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
}