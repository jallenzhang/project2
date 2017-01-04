var alsoenlarge = true;
$(function () {
    if (isScalePossible()) {
        // Run scale function on start
        hight();

        // run scale function on browser resize
        $(window).resize(hight);
    }
});
function hight() {
    sitew = $('#ibackimage').width();
    siteh = $('#ibackimage').height();
    $('#logoHight').css({
        "height": siteh + "px"
    });
}
function isScalePossible() {
    can = 'MozTransform' in document.body.style;
    if (!can) can = 'webkitTransform' in document.body.style;
    if (!can) can = 'msTransform' in document.body.style;
    if (!can) can = 'OTransform' in document.body.style;
    if (!can) can = 'transform' in document.body.style;
    if (!can) can = 'Transform' in document.body.style;
    return can;
}