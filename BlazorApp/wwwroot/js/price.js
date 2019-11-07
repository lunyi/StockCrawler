(function () {

    var url = "http://localhost:8081/stock/" + getUrlParameter('stockId');
    $.get(url, function (data) {

        $("#income").text(data.stock.營收比重);
        $("#address").text(data.stock.address);
        $("#asset").text(data.stock.股本);
        $("#website").attr("href", data.stock.website);

        var vue = new Vue({
            el: '#textExample',
            data: {
                Stock: data.stock,
                Prices: data.prices//renewPrices(data.prices)
            }  
        });
    });

})();

function renewPrices(prices) {
    for (var i = 0; i < prices.length-2; i++) {
        if (prices[i].close > prices[i + 1].close) {
            prices[i].close = prices[i].close + "↑";
            prices[i].closeStyle = "color:red";
        }
        else {
            prices[i].close = prices[i].close + "↓";
            prices[i].closeStyle = "color:green";
            //prices[i].close = "<span style='color:green'>" + prices[i].close + "↓ </span>";
        }
    }
    return prices;
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