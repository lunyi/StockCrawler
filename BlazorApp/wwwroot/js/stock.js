window.onload = function () {
    new Promise(function (resolve, reject) {
        setTimeout(function () {
            getDateList();
            getStocksByType(0);
            getChosenStockTypes();
            getBestStockTypes();

            var stockId = getUrlParameter('stockId');
            var url = getUrlParameter('url');

            if (stockId !== null && url !== null) {
                currentStockId = stockId;
                onUrlChangeAsync(url);
            }
        }, 1000);
    });
};

document.onkeydown = function () {
    console.log("event.keyCode=" + event.keyCode);
    var keyLeft = 37;
    var keyRight = 39;

    var dateIndex = $("#selectDateList").prop("selectedIndex");
    if (event.keyCode === keyLeft) {
        if (dateIndex >= $('#selectDateList').children('option').length) {
            dateIndex = $('#selectDateList').children('option').length - 1;
        } else if (dateIndex < 0) {
            dateIndex = 0;
        }
        else {
            dateIndex = dateIndex + 1;
        }
    } else if (event.keyCode === keyRight) {

        if (dateIndex < 0) {
            dateIndex = 0;
        } else if (dateIndex >= $('#selectDateList').children('option').length) {
            dateIndex = $('#selectDateList').children('option').length - 1;
        } else {
            dateIndex = dateIndex - 1;
        }
    }

    if (event.keyCode === keyLeft || event.keyCode === keyRight) {
        var newDate = $("#selectDateList option").eq(dateIndex).val();
        console.log("newDate=" + newDate);
        $("#selectDateList").val(newDate);
        onGetStocksByDate(0);
    }

    var keyCtrl = 17;
    var keyShift = 16;
    var keyEnter = 13;
    var keyDelete = 46;

    if (event.keyCode === keyCtrl) {
        onUrlChangeAsync(9);
    } else if (event.keyCode === keyShift) {
        onUrlChangeAsync(2);
    //} else if (event.keyCode === keyEnter) {
    //    onUrlChangeAsync(3);
    } else if (event.keyCode === 220) {
        onUrlChangeAsync(1);
    } else if (event.keyCode === keyDelete) {
        onUrlChangeAsync(2);
    }
};

//start to Auto Browser Stocks
var autoStatus = false;
var currentIndex = 0;
var timeoutId = null;
var second = 4000;

function autoStockStart() {
    autoStatus = !autoStatus;
    if (autoStatus) {
        currentIndex = $("#stockList").prop("selectedIndex");
        var parsed = parseInt($("#number").val());
        second = parsed * 1000;

        $("#autoPlay").val("停止瀏覽股票");
        timeoutId = window.setInterval(() => autoSetUrl(), second);
    } else {
        $("#autoPlay").val("自動瀏覽股票");
        clearInterval(timeoutId);
    }
}

function autoSetUrl() {
    currentStockId = $('#stockList option').eq(currentIndex).val();
    $("#stockList").val(currentStockId);
    $("#StockPage").attr("src", urls[currentUrlIndex].replace('{0}', currentStockId));

    currentIndex++;
}

function goTo104Page() {
    var url = "https://www.104.com.tw/jobs/search/?keyword=" + getName();
    window.open(url, '_blank').focus();
}

function goToWikiPedia() {
    var url = "https://www.moneydj.com/KMDJ/search/list.aspx?_QueryType_=WK&_Query_=" + getName();
    $("#StockPage").attr("src", url);
}

function getName() {
    var text = $("#stockList :selected").text();
    var startIndex = text.indexOf('-') + 2;
    var endIndex = text.indexOf('(') - 1;
    var name = text.substring(startIndex, endIndex);
    return name;
}
//End Auto Browser Stocks
//http://5850web.moneydj.com/z/zc/zca/zca_1101.djhtm
//https://concords.moneydj.com/z/zc/zca/zca_1101.djhtm
//http://jsjustweb.jihsun.com.tw/z/zc/zcx/zcx_5521.djhtm
//https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_1101.djhtm
//https://fubon-ebrokerdj.fbs.com.tw/z/zk/zk4/zkparse_560_4.djhtm 集保庫存選股

var urls = {
    //1: 'https://concords.moneydj.com/Z/ZC/ZCX/ZCX_{0}.djhtm',
    //2: 'https://concords.moneydj.com/z/zc/zco/zca_{0}.djhtm',
    //3: 'https://concords.moneydj.com/z/zc/zca/zco_{0}.djhtm',
    //4: 'https://concords.moneydj.com/z/zc/zcl/zcl_{0}.djhtm',
    1: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm',
    2: 'Prices?stockId={0}&datetime={1}&chkDate={2}',
    0: 'TwStock',
    3: 'StockPhoto?stockId={0}&datetime={1}',
    //3: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zca_{0}.djhtm',
    4: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zca/zco_{0}_5.djhtm',
    5: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zcl/zcl_{0}.djhtm',
    6: 'https://www.wantgoo.com/stock/{0}/technical-chart',
    20: 'StockPhoto?stockId={0}&datetime={1}&type=daily',
    21: 'StockPhoto?stockId={0}&datetime={1}&type=weekly',
    22: 'StockPhoto?stockId={0}&datetime={1}&type=five-minutes',
    23: 'StockPhoto?stockId={0}&datetime={1}&type=quarter-hour',
    24: 'StockPhoto?stockId={0}&datetime={1}&type=half-hour',
    25: 'StockPhoto?stockId={0}&datetime={1}&type=hour',
    7: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=l',
    8: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=mf',
    9: 'https://www.cmoney.tw/finance/technicalanalysis.aspx?s={0}',
    10: 'https://www.cmoney.tw/finance/f00033.aspx?s={0}',
    11: 'https://www.cmoney.tw/finance/f00040.aspx?s={0}&o=3',
    12: 'https://www.cmoney.tw/finance/f00038.aspx?s={0}',
    13: 'https://www.cmoney.tw/finance/f00043.aspx?s={0}',
    14: 'https://histock.tw/stock/financial.aspx?no={0}',
    15: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCW/ZCWG/ZCWG_{0}_1.djhtm',
    16: 'https://www.moneydj.com/KMDJ/search/list.aspx?_Query_={0}&_QueryType_=NW',
    17: 'https://goodinfo.tw/StockInfo/StockDividendPolicy.asp?STOCK_ID={0}',
    30: 'https://statementdog.com/analysis/tpe/{0}/stock-health-check',
    31: 'http://www.fortunengine.com.tw/evaluator.aspx?menu=on&scode={0}',
    32: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm',
    33: 'http://' + window.location.hostname + ':8081/stock/minuteKLines',
    34: 'https://www.cmoney.tw/finance/f00025.aspx?s={0}',
};
//個股新聞 https://www.moneydj.com/KMDJ/search/list.aspx?_Query_=6449&_QueryType_=NW
//https://www.moneydj.com/KMDJ/Common/ListNewArticles.aspx?svc=NW&a=X0200000
//https://tw.stock.yahoo.com/q/h?s=6449 

var currentUrlIndex = 1;
var currentStockId = "1101";
var currentSelectedDate = "";
var currentUrl = "https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm";
var urlIndexNewTab = 30;

function onStockChangeAsync(obj) {
    currentStockId = obj.value;
    currentIndex = obj.selectedIndex;
    goToUrl();
}

var b = null;
var bhid = null;

function onPeriodChangeAsync(obj) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetBrokersAsync', currentStockId, currentSelectedDate, parseInt(obj.value))
        .then(data => {
            $("#brokerList option").remove();
            for (var i = 0; i < data.length; i++) {
                $("#brokerList").append($("<option></option>").attr("value", data[i].b + "=" +  data[i].bhid).text(data[i].name + "(" + data[i].val + ")"));
            }
        });
}

function getBroker() {
    if (currentStockId === "") {
        alert("請選擇股票!!");
        return;
    }

    var date = $("#selectDateList");
    var selectedDay = moment(date.val()).format('YYYY-M-D');
    var bhid = $("#brokerList").val();
    var tmpUrl = "https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco0/zco0.djhtm?A={0}&BHID={1}&b={2}&C=1&D={3}&E={4}&ver=V3";
    tmpUrl = tmpUrl.replace("{0}", currentStockId)
        .replace("{1}", bhid.split('=')[1])
        .replace("{2}", bhid.split('=')[0])
        .replace("{3}", "2020-1-1" )
        .replace("{4}", selectedDay);
    console.log(tmpUrl);
    $("#StockPage").attr("src", tmpUrl);
}

function getBrokerBuy() {
    if (currentStockId === "") {
        alert("請選擇股票!!"); onStockChangeAsync
        return;
    }

    var date = $("#selectDateList");
    var selectedDay = moment(date.val()).format('YYYY-M-D');
    var bhid = $("#brokerList").val();
    var tmpUrl = "https://fubon-ebrokerdj.fbs.com.tw/z/zg/zgb/zgb0.djhtm?a={0}&b={1}&c=E&e={2}&f={3}";
    tmpUrl = tmpUrl.replace("{0}", bhid.split('=')[1])
        .replace("{1}", bhid.split('=')[0])
        .replace("{2}", "2020-1-1")
        .replace("{3}", selectedDay);
    console.log(tmpUrl);
    $("#StockPage").attr("src", tmpUrl);
}

function onUrlChangeAsync(index) {
    if (index === undefined)
        return;
    currentStockId = currentStockId || "1101";
    currentSelectedDate = currentSelectedDate || moment($("#selectDateList option").eq(0).val()).format('YYYY-MM-DD');
    var url = urls[index]
        .replace('{0}', currentStockId)
        .replace('{1}', currentSelectedDate)
        .replace('{2}', $("#chkDate").prop('checked'));

    if (index >= urlIndexNewTab) {
        window.open(url, '_blank').focus();
    }
    else {
        currentUrlIndex = index;
        console.log(url);
        $("#StockPage").attr("src", url);
    }
}

function goToUrl() {
    currentUrl = urls[currentUrlIndex];
    var url = currentUrl.replace('{0}', currentStockId);
    currentSelectedDate = currentSelectedDate || moment($("#selectDateList option").eq(0).val()).format('YYYY-MM-DD');
    url = url.replace('{1}', currentSelectedDate).replace('{2}', $("#chkDate").prop('checked'));

    if (currentUrlIndex >= urlIndexNewTab) {
        window.open(url, '_blank').focus();
    }
    else {
        console.log(url);
        $("#StockPage").attr("src", url);
        $('#stockList').val(currentStockId);
    }
}
//start to Auto Browser Stocks
function onFindStockId(event) {
    if (window.event.key === "Enter") {
        var stockId = event.value;
        var stockList = document.getElementById("stockList");
        var find = false;
        for (var i = 0; i < stockList.options.length; i++) {
            if (stockList.options[i].value === stockId) {
                stockList.value = stockId;

                find = true;
                event.value = "";
                break;
            }
        }
        currentStockId = stockId;
        goToUrl();

        if (find === false) {
            event.placeholder = stockId + " not found!!";
            event.value = "";
            window.setTimeout(() => {
                event.placeholder = "輸入編號";
            }, 3000);
        }
    }
}

function onSetCurrentIndex() {
    currentUrlIndex = 1;
}

function goToStockMaster() {
    var chooseStockUrl = $("#chooseStockList").val();
    if (chooseStockUrl !== "0") {
        window.open(chooseStockUrl, '_blank').focus();
    }
}

function onSelectTypeChange() {
    
    if ($("#selectStockType :selected").val()=== "0") {
        getStocksByType(0);
        return;
    }
    var text = $("#selectStockType :selected").text();
    getStocksByBestStockType(text);
}

function getStocksByBestStockType(selectStockType) {
    currentSelectedDate = currentSelectedDate || moment($("#selectDateList option").eq(0).val()).format('YYYY-MM-DD');
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByTypeAsync', selectStockType, currentSelectedDate)
        .then(data => {
            setStocks(data);
        });
}

function getDateList() {
    DotNet.invokeMethodAsync('BlazorApp', 'GetDateListAsync')
        .then(data => {
            for (var i = 0; i < data.length; i++) {
                $("#selectDateList").append($("<option></option>").attr("value", data[i]).text(data[i]));
            }
        });
}

function getStocksByType(stockType) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetBestStocksAsync', stockType)
        .then(data => {
            setStocks(data);
        });
}

function getChosenStockTypes() {
    DotNet.invokeMethodAsync('BlazorApp', 'GetChosenStockTypesAsync')
        .then(data => {
            $("#chosenStockType option").remove();
            $("#selectStockType option").remove();
            $("#selectStockType").append($("<option></option>").attr("value", 0).text("----全部----"));

            for (var i = 0; i < data.length; i++) {
                $("#chosenStockType").append($("<option></option>").attr("value", data[i]).text(data[i]));
                $("#selectStockType").append($("<option></option>").attr("value",i+1).text(data[i]));
            }
        });
}

function getBestStockTypes() {
    DotNet.invokeMethodAsync('BlazorApp', 'GetBestStockTypeAsync')
        .then(data => {
            $("#selectBestType").append($("<option></option>").attr("value", 0).text("----全部----"));
            for (var i = 0; i < data.length; i++) {
                $("#selectBestType").append($("<option></option>").attr("value", data[i].value).text(data[i].value));
            }
        });
}

var currentType = 1;

function onGetStocksByDate(val) {
    $("#selectStockType").val(0);
    var date = $("#selectDateList");
    var bestType = $("#selectBestType");
    var rankType = $("#selectRankType");
    currentSelectedDate = date.val();

    if (val !== 0) {
        currentType = val;
    } else {
        console.log("currentUrlIndex=" + currentUrlIndex);

        if (currentUrlIndex === 4) {
            var selectedDay = moment(date.val()).format('YYYY-M-D');
            var _url = "https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco.djhtm?a=" + currentStockId + "&e=" + selectedDay + "&f=" + selectedDay;
            console.log(_url);
            $("#StockPage").attr("src", _url);
            return;
        }
        var photoIndex = [2, 3, 20, 21, 22, 23, 24, 25];

        if (photoIndex.indexOf(currentUrlIndex)>=0) {
            _url = urls[currentUrlIndex]
                .replace('{0}', currentStockId)
                .replace('{1}', currentSelectedDate)
                .replace('{2}', $("#chkDate").prop('checked'));

            console.log(_url);
            $("#StockPage").attr("src", _url);
            return;
        }
    } 

    if (currentType === 1 && bestType.val() !== "0") {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByBestStockTypeAsync', bestType.val(), date.val())
            .then(data => {
                setStocks(data);
            });
    } else if (currentType === 2 && rankType.val() !== "0") {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksDateAsync', date.val(), parseInt(rankType.val()))
            .then(data => {
                setStocks(data);
            });
    } else if (currentType === 3) {
        if ($("#selectAvgType").val() === "0")
            return;
        console.log("currentType3");

        $("#selectStockType").val(0);
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksDateAsync', date.val(), parseInt($("#selectAvgType").val()))
            .then(data => {
                setStocks(data);
            });
    }
}

function onClear() {
    $("#txtChosenStockType").val("");
}

//function setChosenStockTypes(chosenType) {
//    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByTypeAsync', chosenType)
//        .then(data => {
//            setStocks(data);
//        });
//}

function setStocks(data) {
    $("#stockList option").remove();
    for (var i = 0; i < data.length; i++) {
        var desc = data[i].description === null ? "" : data[i].description;
        var text = data[i].stockId + " - " + data[i].name + " (" + data[i].industry + " " + desc + " )";
        $("#stockList").append($("<option></option>").attr("value", data[i].stockId).text(text));
    }

    if (data.length > 0) {
        if (currentStockId === null) {
            $("#stockList").val(data[0].stockId);
            currentStockId = data[0].stockId;
        } else {
            $("#stockList").val(currentStockId);
        }
    }
    goToUrl();
}

function chooseStock() {
    if ($("#txtChosenStockType").val() === "") {
        alert("請選擇選股類型!!");
        return;
    }
    DotNet.invokeMethodAsync('BlazorApp', 'SetBestStockAsync', currentStockId, $("#txtChosenStockType").val())
        .then(data => {
            console.log(data);
        });
}

function removeStock() {
    if ($("#txtChosenStockType").val() === $("#selectStockType :selected").text()) {
        DotNet.invokeMethodAsync('BlazorApp', 'RemoveBestStockAsync', currentStockId, $("#txtChosenStockType").val())
            .then(data => {
                console.log(data);
                getStocksByBestStockType($("#txtChosenStockType").val());
            });    
    }
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