(function () {

    var url = "http://220.133.185.1:8081/stock/" + getUrlParameter('stockId');
    $.get(url, function (data) {

        $("#income").text(data.stock.營收比重);
        $("#address").text(data.stock.address);
        $("#asset").text(data.stock.股本);
        $("#website").attr("href", data.stock.website);

        var vue = new Vue({
            el: '#textExample',
            data: {
                Stock: data.stock,
                Prices: renewPrices(data.prices)
            }  
        });
    });
})();

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

function tableCreate(el, data) {
    var tbl = document.createElement("table");
    tbl.style.width = "100%";

    for (var i = 0; i < data.length; ++i) {
        var tr = tbl.insertRow();
        for (var j = 0; j < data[i].length; ++j) {
            var td = tr.insertCell();
            td.appendChild(document.createTextNode(data[i][j].toString()));
        }
    }
    el.appendChild(tbl);
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