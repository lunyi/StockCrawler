$(function () {
    var datetime = getUrlParameter('datetime');
    var stockid = getUrlParameter('stockId');
    var type = getUrlParameter('type');
    var url = "http://" + window.location.hostname + "/photo/" + datetime + "/" + type + "/"+ stockid + ".png"
    console.log(url);
    $("#imgPhoto").attr("src", url)
});


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