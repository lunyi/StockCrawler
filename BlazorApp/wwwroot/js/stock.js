window.onload = function () {
    new Promise(function (resolve, reject) {
        setTimeout(function () {
            getDateList();
            getStocksByType(0);
            getChosenStockTypes();
            getBestStockTypes();
        }, 1000);
    });
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
    2: 'Prices?stockId={0}',
    //3: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zca_{0}.djhtm',
    4: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zca/zco_{0}.djhtm',
    5: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zcl/zcl_{0}.djhtm',
    6: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=d',
    3: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=r',
    7: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=l',
    8: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=mf',
    9: 'https://www.cmoney.tw/finance/stockmainkline.aspx?s={0}',
    10: 'https://www.cmoney.tw/finance/f00033.aspx?s={0}',
    11: 'https://www.cmoney.tw/finance/f00040.aspx?s={0}&o=3',
    12: 'https://www.cmoney.tw/finance/f00038.aspx?s={0}',
    13: 'https://www.cmoney.tw/finance/f00043.aspx?s={0}',
    14: 'https://histock.tw/stock/financial.aspx?no={0}',
    15: 'https://www.cnyes.com/twstock/Margin/{0}.htm',
    16: 'https://www.moneydj.com/KMDJ/search/list.aspx?_Query_={0}&_QueryType_=NW',
    17: 'https://statementdog.com/analysis/tpe/{0}/stock-health-check',
    18: 'https://www.fugle.tw/ai/{0}?p=2460385721&perfect=true',
    19: 'http://www.fortunengine.com.tw/evaluator.aspx?menu=on&scode={0}',
    20: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm',
};
//個股新聞 https://www.moneydj.com/KMDJ/search/list.aspx?_Query_=6449&_QueryType_=NW
//https://www.moneydj.com/KMDJ/Common/ListNewArticles.aspx?svc=NW&a=X0200000
//https://tw.stock.yahoo.com/q/h?s=6449 


var currentUrlIndex = 1;
var currentStockId = "1101";
var currentUrl = "https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm";
var urlIndexNewTab = 17;

function onStockChangeAsync(obj) {
    currentStockId = obj.value;
    currentIndex = obj.selectedIndex;
    goToUrl();
}

function onUrlChangeAsync(index) {
    var url = urls[index].replace('{0}', currentStockId);

    if (index >= urlIndexNewTab) {
        window.open(url, '_blank').focus();
    }
    else {
        currentUrlIndex = index;
        $("#StockPage").attr("src", url);
    }
}

function goToUrl() {
    currentUrl = urls[currentUrlIndex];
    var url = currentUrl.replace('{0}', currentStockId);

    if (currentUrlIndex >= urlIndexNewTab) {
        window.open(url, '_blank').focus();
    }
    else {
        $("#StockPage").attr("src",url);
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
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByTypeAsync', selectStockType)
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

    if (val !== 0) {
        currentType = val;
    } else {
        if (currentUrlIndex === 4) {

            var _url = "https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco.djhtm?a=" + currentStockId + "&e=" + date.val() + "&f=" + date.val();
            console.log(_url);
            $("#StockPage").attr("src", _url);
            return;
        }
    } 


    console.log("val=" + val + ", currentType=" + currentType);

    if (currentType === 1 && bestType.val() !== 0) {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByBestStockTypeAsync', bestType.val(), date.val())
            .then(data => {
                setStocks(data);
            });
    } else if (currentType=== 2 && rankType.val() !== 0) {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksDateAsync', date.val(), parseInt(rankType.val()))
            .then(data => {
                setStocks(data);
            });
    }
}

function onClear() {
    $("#txtChosenStockType").val("");
}

function setChosenStockTypes(chosenType) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByTypeAsync', chosenType)
        .then(data => {
            setStocks(data);
        });
}

function setStocks(data) {
    $("#stockList option").remove();
    for (var i = 0; i < data.length; i++) {
        var desc = data[i].description === null ? "" : data[i].description;
        var text = data[i].stockId + " - " + data[i].name + " (" + data[i].industry + " " + desc + " )";
        $("#stockList").append($("<option></option>").attr("value", data[i].stockId).text(text));
    }

    if (data.length > 0) {
        $("#stockList").val(data[currentIndex].stockId);
        currentStockId = data[currentIndex].stockId;
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