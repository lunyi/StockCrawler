window.onload = function () {
    new Promise(function (resolve, reject) {
        setTimeout(function () {
            getDateList();
            getStocksByType(0);
            getChosenStockTypes();
        }, 100);
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
        currentIndex = document.getElementById("stockList").selectedIndex;
        var parsed = parseInt(document.getElementById("number").value);
        second = parsed * 1000;

        $("#autoPlay").val("停止瀏覽股票");
        timeoutId = window.setInterval(() => autoSetUrl(), second);
    } else {
        $("#autoPlay").val("自動瀏覽股票");
        clearInterval(timeoutId);
    }
}

function autoSetUrl() {
    var list = document.getElementById("stockList");
    if (currentIndex < 0) {
        currentIndex = 0;
    }
    currentStockId = list.options[currentIndex].value;
    currentUrl = urls[currentUrlIndex];
    var url = currentUrl.replace('{0}', currentStockId);

    document.getElementById("StockPage").src = url;
    list.value = list.options[currentIndex].value;
    currentIndex++;
}

function goTo104Page() {
    var url = "https://www.104.com.tw/jobs/search/?keyword=" + getName();
    window.open(url, '_blank').focus();
}

function goToWikiPedia() {
    var url = "https://www.moneydj.com/KMDJ/search/list.aspx?_QueryType_=WK&_Query_=" + getName();
    document.getElementById("StockPage").src = url;
}

function getName() {
    var text = $("#stockList").text();
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
//

var urls = {
    //1: 'https://concords.moneydj.com/Z/ZC/ZCX/ZCX_{0}.djhtm',
    //2: 'https://concords.moneydj.com/z/zc/zco/zca_{0}.djhtm',
    //3: 'https://concords.moneydj.com/z/zc/zca/zco_{0}.djhtm',
    //4: 'https://concords.moneydj.com/z/zc/zcl/zcl_{0}.djhtm',
    1: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm',
    2: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zca_{0}.djhtm',
    3: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zca/zco_{0}.djhtm',
    4: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zcl/zcl_{0}.djhtm',
    5: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=d',
    6: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=l',
    7: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=mf',
    8: 'https://www.cmoney.tw/finance/stockmainkline.aspx?s={0}',
    9: 'https://www.cnyes.com/twstock/Margin/{0}.htm',
    10: 'https://statementdog.com/analysis/tpe/{0}/stock-health-check',
    11: 'https://www.fugle.tw/ai/{0}?p=2460385721&perfect=true',
    12: 'http://www.fortunengine.com.tw/evaluator.aspx?menu=on&scode={0}'
};

var currentUrlIndex = 1;
var currentStockId = "1101";
var currentUrl = "https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm";

function onStockChangeAsync(obj) {
    currentStockId = obj.value;
    goToUrl();
}

function onUrlChangeAsync(index) {
    currentUrlIndex = index;
    goToUrl();
}

function goToUrl() {
    currentUrl = urls[currentUrlIndex];
    var url = currentUrl.replace('{0}', currentStockId);

    if (currentUrlIndex >= 10) {
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
                currentStockId = stockId;
                goToUrl();
                find = true;
            }
        }

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

function onGetStocksByDate() {
    $("#selectStockType").val(0);
    var date = $("#selectDateList");
    var type = $("#selectRankType");

    if (type.val() !== 0) {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksDateAsync', date.val(), parseInt(type.val()))
            .then(data => {
                setStocks(data);
            });
    }
}

function onClear() {
    $("#txtChosenStockType").val("");
}

//$(document).on('change', '#txtChosenStockType', function () {
//    var s = $("#chosenStockType");
//    var optionslist = $("#chosenStockType")[0].options;
//    var value = $(this).val();
//    for (var x = 0; x < optionslist.length; x++) {
//        if (optionslist[x].value === value) {
//            setChosenStockTypes(value);
//            break;
//        }
//    }
//});

function setChosenStockTypes(chosenType) {
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByTypeAsync', chosenType)
        .then(data => {
            setStocks(data);
        });
}

function setStocks(data) {
    $("#stockList option").remove();
    for (var i = 0; i < data.length; i++) {
        var desc = data[i].description === null ? "" : data[i].description
        var text = data[i].stockId + " - " + data[i].name + " (" + data[i].industry + " " + desc + " )";
        $("#stockList").append($("<option></option>").attr("value", data[i].stockId).text(text));
    }
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