//var alsoenlarge = true;
$(function () {
    // Run scale function on start
    hight();
    // run scale function on browser resize
    $(window).resize(hight);

    changeElement();
});

function hight() {
    if ($('#ibackimage').length > 0) {
        sitew = $('#ibackimage').width();
        siteh = $('#ibackimage').height();
        $('#logoHight').css({
            "height": siteh + "px"
        });
    }
}
function changeElement() {
    if (!checkHtml5() || checkOpera()) {
        var idhtml5 = "frontpage_hype_container";
        var jsSrc1 = "/Content/Custom/FrontPage_Resources/frontpage_hype_generated_script.js";
        var jsSrc2 = getUrl() + "/Content/Custom/FrontPage_Resources/HYPE.js?hype_version=100";
        if ($("#" + idhtml5).length > 0) {
            if ($("script[src='" + jsSrc1 + "']").length > 0) {
                $("script[src='" + jsSrc1 + "']").remove();
            }
            if ($("script[src='" + jsSrc2 + "']").length > 0) {
                $("script[src='" + jsSrc2 + "']").remove();
            }
            $("#" + idhtml5).before("<img id='staticImg' src='/Content/images/logo.png' alt=''/>");
            $("#" + idhtml5).remove();            
        }
    }

    if (checkIE6()) {
        $("#logoHight").hide();
    }
}
function checkIE6() {
    var ie6 = ! -[1, ] && !window.XMLHttpRequest;
    return ie6;
}
function checkOpera() {
    return ($.browser.opera);
}
function checkHtml5() {
    return !!(document.createElement('video').canPlayType);
}
function getUrl() {
    if(window.location.host)
    var res = window.location.protocol + "//" + window.location.host;
    return res;
}
//function isScalePossible() {
//    can = 'MozTransform' in document.body.style;
//    if (!can) can = 'webkitTransform' in document.body.style;
//    if (!can) can = 'msTransform' in document.body.style;
//    if (!can) can = 'OTransform' in document.body.style;
//    if (!can) can = 'transform' in document.body.style;
//    if (!can) can = 'Transform' in document.body.style;
//    return can;
//}