//start to Auto Browser Stocks
var autoStatus = false;
var currentIndex = 0;
var timeoutId = null;
var second = 4000;
var currentUrlIndex = 1;
var currentStockId = "1101";
var currentUrl = "https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm";

window.onload = function () {
    (function () {
        if (window.DotNet) {
            console.log("window.DotNet is set");
            if (window.DotNet.invokeMethodAsync) {
                console.log("window.DotNet.invokeMethodAsync is set");
                start();
            }
        }
    })();
};

function start() {
    Blazor.start({})
        .then(() => initial());
}

function initial() {
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByIndexAsync', 0)
        .then(data => {
            setStockList(data);
        });
}
function autoStockStart() {
    autoStatus = !autoStatus;
    var btnAutoPlay = document.getElementById("autoPlay");
    if (autoStatus) {
        currentIndex = document.getElementById("stockList").selectedIndex;
        var parsed = parseInt(document.getElementById("number").value);
        second = parsed * 1000;

        btnAutoPlay.value = "停止瀏覽股票";
        timeoutId = window.setInterval(() => autoSetUrl(), second);
    } else {
        btnAutoPlay.value = "自動瀏覽股票";
        clearInterval(timeoutId);
    }
}

function autoSetUrl() {
    var list = document.getElementById("stockList");
    //var url =  "http://5850web.moneydj.com/z/zc/zcx/zcxNew_" + list.options[currentIndex].value + ".djhtm"
    //var url = "https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_" + list.options[currentIndex].value + ".djhtm";
    //var url = "https://www.cmoney.tw/follow/channel/stock-" + list.options[currentIndex].value + "?chart=d";
    //var url = "https://concords.moneydj.com/Z/ZC/ZCX/ZCX_" +list.options[currentIndex].value+ ".djhtm";

    if (currentIndex < 0) {
        currentIndex = 0;
    }
    currentStockId = list.options[currentIndex].value;
    currentUrl = urls[currentUrlIndex];
    let url = currentUrl.replace('{0}', currentStockId);

    document.getElementById("StockPage").src = url;
    list.value = list.options[currentIndex].value;
    currentIndex++;
}

function goTo104Page() {
    let url = "https://www.104.com.tw/jobs/search/?keyword=" + getName();
    window.open(url, '_blank').focus();
}

function goToWikiPedia() {
    let url = "https://www.moneydj.com/KMDJ/search/list.aspx?_QueryType_=WK&_Query_=" + getName();
    document.getElementById("StockPage").src = url;
}

function getName() {
    let list = document.getElementById("stockList");
    let s = list.options[list.selectedIndex];
    let startIndex = s.innerText.indexOf('-') + 2;
    let endIndex = s.innerText.indexOf('(') - 1;
    let name = s.innerText.substring(startIndex, endIndex);
    return name;
}
//End Auto Browser Stocks

var urls = {
    1: 'https://fubon-ebrokerdj.fbs.com.tw/Z/ZC/ZCX/ZCX_{0}.djhtm',
    2: 'https://fubon-ebrokerdj.fbs.com.tw/z/zc/zco/zco_{0}.djhtm',
    3: 'https://www.cnyes.com/twstock/Margin/{0}.htm',
    4: 'https://www.cmoney.tw/follow/channel/stock-{0}?chart=d',
    5: 'https://www.fugle.tw/ai/{0}?p=2460385721&perfect=true',
    6: 'https://statementdog.com/analysis/tpe/{0}/stock-health-check',
    7: 'http://www.fortunengine.com.tw/evaluator.aspx?menu=on&scode={0}'
};



function onBestStocksChange() {
    var bestStock = document.getElementById("BestStockList");
    DotNet.invokeMethodAsync('BlazorApp', 'GetStocksByIndexAsync', bestStock.selectedIndex)
        .then(data => {
            setStockList(data);
        });
}
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

    if (currentUrlIndex >= 5) {
        window.open(url, '_blank').focus();
    }
    else {
        document.getElementById("StockPage").src = url;
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
                event.placeholder = "輸入編號找股票";
            }, 3000);
        }
    }
}

function selectStock() {
    DotNet.invokeMethodAsync('BlazorApp', 'SetBestStockAsync', currentStockId, "頁面選股", "test")
        .then(data => {
            console.log(data);
        });
}

function onSetCurrentIndex() {
    currentUrlIndex = 1;
}

function goToStockMaster() {
    var list = document.getElementById("chooseStockList");
    if (list.selectedIndex > 0) {
        window.open(list.value, '_blank').focus();
    }
}

function onGetStocksByDate() {
    var date = document.getElementById("selectDateList");
    var type = document.getElementById("selectDateType");

    if (type.selectedIndex > 0) {
        DotNet.invokeMethodAsync('BlazorApp', 'GetStocksAndDateAsync', date.value, type.selectedIndex)
            .then(data => {
                setStockList(data);
            });
    }
}

function setStockList(data) {
    var select = document.getElementById("stockList");
    for (i = select.options.length - 1; i >= 0; i--) {
        select.remove(i);
    }

    for (var i = 0; i < data.length; i++) {
        var option = document.createElement("option");
        option.value = data[i].stockId;
        option.text = data[i].stockId + " - " + data[i].name + " (" + data[i].industry + ")";
        select.add(option);
    }
}