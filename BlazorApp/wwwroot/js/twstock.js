(function () {
    var url = "https://localhost:44367/stock/";

    $.get(url, function (data) {
        var vue = new Vue({
            el: '#textExample',
            data: {
                Stock: data.stock,
                Prices: renewPrices(data)
            }  
        });
    });
})();

function renewPrices(data) {
    var m = moment();
    for (var i = 0; i < data.length-1; i++) {

        data[i].datetime = moment(data[i].datetime).format("YYYY-MM-DD");

        if (data[i].收盤價 > data[i + 1].收盤價) {
            data[i].收盤價 = data[i].收盤價 + "↑";
            data[i].updownColor = "red";
        }
        else if (data[i].收盤價 < data[i + 1].收盤價) {
            data[i].收盤價 = data[i].收盤價 + "↓";
            data[i].updownColor = "green";
        }

        if (data[i].成交量 > (data[i + 1].成交量 * 2.5)) {
            data[i].zfontWeight = "bold";
        }
        else {
            data[i].zfontWeight = "normal";
        }

        if (data[i].外資買賣超 > 0) {
            data[i].外資買賣超 = data[i].外資買賣超 + "↑";
            data[i].fColor = "red";
        }
        else if (data[i].外資買賣超 < 0) {
            data[i].外資買賣超 = data[i].外資買賣超 + "↓";
            data[i].fColor = "green";
        }
        else {
            data[i].外資買賣超 = data[i].外資買賣超 ;
        }

        if (data[i].投信買賣超 > 0) {
            data[i].投信買賣超 = data[i].投信買賣超 + "↑";
            data[i].tColor = "red";
        }
        else if (data[i].投信買賣超 < 0) {
            data[i].投信買賣超 = data[i].投信買賣超 + "↓";
            data[i].tColor = "green";
        }
        else {
            data[i].投信買賣超 = data[i].投信買賣超;
        }

        if (data[i].自營商買賣超 > 0) {
            data[i].zColor = "red";
        }
        else if (data[i].自營商買賣超 < 0) {
            data[i].zColor = "green";
        }

        if (data[i].漲跌百分比 > 0) {
            data[i].pColor = "red";
        }
        else if (data[i].漲跌百分比 < 0) {
            data[i].pColor = "green";
        }

        if (data[i].漲跌百分比 > 1 || data[i].漲跌百分比 < -1) {
            data[i].fontWeight = "bold";
        }
        else {
            data[i].fontWeight = "normal";
        }

        if (data[i].融資增加 > 0) {
            data[i].融資增加 = data[i].融資增加 + "↑";
            data[i].lColor = "red";
        }
        else if (data[i].融資增加 < 0) {
            data[i].融資增加 = data[i].融資增加 + "↓";
            data[i].lColor = "green";
        } else {
            data[i].融資增加 = data[i].融資增加 ;
        }

        if (data[i].融券增加 === undefined) {
            data[i].融券增加 = null;
        }
        else if (data[i].融券增加 > 0) {
            data[i].融券增加 = data[i].融券增加 + "↑";
            data[i].chColor = "red";``
        }
        else if (data[i].融券增加 < 0) {
            data[i].融券增加 = data[i].融券增加 + "↓";
            data[i].chColor = "green";
        } else {
            data[i].融券增加 = data[i].融券增加;
        }
    }
    return data;
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