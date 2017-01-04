var str_error_mail_format = "邮箱格式有误";
var str_input_register_mail = "请输入您注册时的邮箱";
var str_input_pwd = "请输入密码";

/* input mail check */
function blurMail() {
    var mailvalue = getResetTheInputValue("#email");
    if (mailvalue != "" && mailvalue != str_input_register_mail) {
        if (!validateMail(mailvalue)) {
            $("#ltips1").html(str_error_mail_format);
            return;
        }
    }

    $("#ltips1").html("");
}

/* button effect */
function btnReg_DownEffect() {
    setImgSrc("btnlogin", "image/Btn_launch_press.png");
}
function btnReg_UpEffect() {
    setImgSrc("btnlogin", "image/Btn_launch.png");
}

/* login action */
function dologin() {    
    // check all
    if (!checkAllInput()) return;
    var mailValue = getResetTheInputValue("#email");
    var pwd = $("#loginPwd").val();

    showloading();

    // to do
    if (isAndroidOrIOS()) {
        window.WebBinding.Submit(mailValue + ',' + pwd);
    }

    if (!isAndroidOrIOS()) {
        window.location = '/Submit/' + mailValue + ',' + pwd;
    }
}
function checkAllInput() {
    var mailvalue = getResetTheInputValue("#email");
    var pwdvalue = $("#loginPwd").val();
    if (mailvalue == "" || mailvalue == str_input_register_mail || pwdvalue == "" || pwdvalue == str_input_pwd) {
        return false;
    }
    return validateMail(mailvalue);
}

/* control the mobile layout */
$(document).bind("mobileinit", function () {
});
$(function () {
    $('#abtnlogin').bind('touchstart', function () {
        btnReg_DownEffect();
    }).bind('touchend', function () {
        btnReg_UpEffect();
    });

    if (isAndroidOrIOS()) {
        $('#btnClosePage').bind('touchstart', function () {
            btnClose_DownEffect();
        }).bind('touchend', function () {
            btnClose_UpEffect();
        });
    }
});

/* show and close loading  */
function showloading() {
    $.mobile.loadingMessageTextVisible = true;
    $.mobile.showPageLoadingMsg('a', "登录中...");
    mask();
}
function hideloading() {
    $.mobile.hidePageLoadingMsg();
    unmask();
}
function mask() {
    var myheight = $(document.body).height();
    var mywidth = $(document.body).width();
    $("#mask").attr("class", "maskShow");
    $("#mask").css("height", myheight + "px").css("width", mywidth + "px");
}
function unmask() {
    $("#mask").attr("className", "maskUnset");
    $("#mask").css("height", "0px").css("width", "0px");
}