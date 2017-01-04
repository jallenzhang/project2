function validateMail(mail) {
    return mail.match(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/);
}

function setImgSrc(id, url) {
    document.getElementById(id).src = url;
}

function setlabelFont(id, css) {
    document.getElementById(id).style.color = css;
}

function setLabelValue(id, text) {
    document.getElementById(id).innerHTML = text;
}

function checkSubmit(e, action) {
    if (e && e.keyCode == 13) {
        eval(action+"()");
    }
}

/* true:android; false:ios */
function isAndroidOrIOS() {
    if (navigator.userAgent.match(/android/i)) {
        return true;
    }
    if (navigator.userAgent.match(/ipad|iphone|ipod/i)) {
        return false;
    }
    /* this is only debug for PC */
    return false;
}

function btnClose_DownEffect() {
    setImgSrc("btnClosePage", "image/Btn_esc_press.png");
}

function btnClose_UpEffect() {
    setImgSrc("btnClosePage", "image/Btn_esc.png");
}

function doClosePage() {
    //ToDo
    if(navigator.userAgent.match(/android/i))
    {
        window.WebBinding.Close(); 
    }
}