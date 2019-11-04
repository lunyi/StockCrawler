(function () {

    var url = "http://localhost:8081/stock/" + getUrlParameter('stockId');
    $.get(url, function (data) {

        var vue = new Vue({
            el: '#textExample',
            data: {
                Stock: data.stock,
                Prices: data.prices
            }  
        });
    });

})();

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