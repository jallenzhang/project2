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

function getResetTheInputValue(id){
	var mailvalue = $(id).val();
	mailvalue = $.trim(mailvalue);
	$(id).val(mailvalue);
	return mailvalue;
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
    return true;
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

function forgetPwd(){
	// android call
    if (isAndroidOrIOS()) {
        // window.WebBinding.Submit("", "ForgetPwd");
		window.location.href="file:///android_asset/ForgetPwd_android.html";
    }
	else{ // ios call
		window.location = '/OpenFgetPwdPage/';
		// window.location.href="file:////var/mobile/Applications/796D2563-433B-4C32-8DC0-073D2DA5C8F7/lilypoker.app/Data/Raw/ForgetPwd_ios.html";
	}	
}