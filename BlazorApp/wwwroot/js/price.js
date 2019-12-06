(function () {

    //var url = "http://220.133.185.1:8081/stock/" + getUrlParameter('stockId');
    var url = "https://localhost:44368/stock/" + getUrlParameter('stockId');

    $.get(url, function (data) {

        $("#income").text(data.stock.營收比重);
        $("#address").text(data.stock.address);
        $("#asset").text(data.stock.股本);
        $("#cValue").text(data.stock.每股淨值);
        $("#website").attr("href", data.stock.website);

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
            el: '#thousand',
            data: {
                Thousands: renewThousand(data.thousand)
            }
        });
    });
})();


function renewThousand(thousands) {

    for (var i = 0; i < thousands.length - 1; i++) {
        if (thousands[i].p100 > thousands[i+1].p100) {
            thousands[i].p100 = thousands[i].p100 + "↑";
            thousands[i].sColor = "red";
        }
        else {
            thousands[i].p100 = thousands[i].p100 + "↓";
            thousands[i].sColor = "green";
        }
        if (thousands[i].p1000 > thousands[i + 1].p1000) {
            thousands[i].p1000 = thousands[i].p1000 + "↑";
            thousands[i].bColor = "red";
        }
        else {
            thousands[i].p1000 = thousands[i].p1000 + "↓";
            thousands[i].bColor = "green";
        }
    }
    return thousands;
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
            prices[i].fColor = "red";
        }
        else if (prices[i].外資買賣超 < 0) {
            prices[i].fColor = "green";
        }

        if (prices[i].投信買賣超 > 0) {
            prices[i].tColor = "red";
        }
        else if (prices[i].投信買賣超 < 0) {
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

        if (prices[i].融資買賣超 > 0) {
            prices[i].lColor = "red";
        }
        else if (prices[i].融資買賣超 < 0) {
            prices[i].lColor = "green";
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