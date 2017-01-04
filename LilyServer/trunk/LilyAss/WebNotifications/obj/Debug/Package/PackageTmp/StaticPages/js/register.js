var str_input_pwd = "请输入密码";
var str_input_pwd_again = "请再次输入密码";
var str_input_register_mail = "请输入您的邮箱";
var str_error_mail_format = "邮箱格式不对, 请重新输入";
var str_error_input_pwd = "请输入密码!";
var str_error_input_pwd_again = "两次输入的密码不一致!";
var str_error_pwd_tips = "6位到12位的英文、数字!";
var str_label_mail_tips = "6位到30位的邮箱帐号";
var str_label_pwd_tips = "6位到12位的英文、数字";
var str_label_pwd_again_tips = "重复上面的密码";

var error_icon_html = "<img src='image/LIError.png' />";
var right_icon_html = "<img src='image/LIHook.png' />";


/* for pwd input focus */
function onBlur() {
    var pwdvalue = $("#loginPwd").val();
    if (pwdvalue != "" && pwdvalue != str_input_pwd) {
        if (checkPwd(pwdvalue)) {
            $("#icon2 img").remove();
            $("#icon2").append(right_icon_html);
            $("#ltips2").html(str_label_pwd_tips);
            setlabelFont("ltips2", '#C0C0C0');
        }
        else {
            $("#icon2 img").remove();
            $("#icon2").append(error_icon_html);
            setLabelValue("ltips2", str_error_pwd_tips);
            setlabelFont("ltips2", 'red');
        }
    } else {
        $("#icon2 img").remove();
        $("#ltips2").html(str_label_pwd_tips);
        setlabelFont("ltips2", '#C0C0C0');
    }
    zoomInOut();
}
function checkPwd(str) {
    var mylen = 0;
    for (i = 0; i < str.length; i++) {
        var ch = str[i];
        if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')) {
            mylen++;
        }
    }
    if (mylen == str.length && mylen >= 6 && mylen <= 12) {
        return true;
    }
    return false;
}

function onBlur2() {
    var pwdvalue = $("#loginPwd").val();
    var pwdvalue2 = $("#again_loginPwd").val();
    if (pwdvalue2 != "" && pwdvalue2 != str_input_pwd_again) {
        if (pwdvalue2 != pwdvalue) {
            $("#icon3 img").remove();
            $("#icon3").append(error_icon_html);
            setlabelFont("ltips3", "red");
            setLabelValue("ltips3", str_error_input_pwd_again);
        }
        else {
            $("#icon3 img").remove();
            $("#icon3").append(right_icon_html);
            setlabelFont("ltips3", "#C0C0C0");
            setLabelValue("ltips3", str_label_pwd_again_tips);
        }
    } else {
        $("#icon3 img").remove();
        $("#ltips3").html(str_label_pwd_again_tips);
        setlabelFont("ltips3", '#C0C0C0');
    }
    zoomInOut();
}

/* for mail input */
function blurMail() {
    var mailvalue = $("#email").val();
    if (mailvalue != "" && mailvalue != str_input_register_mail) {
        if (checkMail(mailvalue)) {
            $("#icon1 img").remove();
            $("#icon1").append(right_icon_html);
            setLabelValue("ltips1", str_input_register_mail);
            setlabelFont("ltips1", '#C0C0C0');
        }
        else {
            $("#icon1 img").remove();
            $("#icon1").append(error_icon_html);
            setLabelValue("ltips1", str_error_mail_format);
            setlabelFont("ltips1", 'red');
        }
    }
    else {
        $("#icon1 img").remove();
        setLabelValue("ltips1", str_label_mail_tips);
        setlabelFont("ltips1", '#C0C0C0');
    }
    zoomInOut();
}
function checkMail(mailvalue) {
    if (mailvalue.length >= 6 && mailvalue.length <= 30 && validateMail(mailvalue)) {
        return true;
    }
    return false;
}

/* login action */
function doregister() {
    // check all
    if (!checkAllInput()) return;

    // to do
    if (navigator.userAgent.match(/android/i)) {
        window.WebBinding.Submit(document.getElementById('email').value + ',' + document.getElementById('loginPwd').value);
    }

    if (navigator.userAgent.match(/ipad|iphone|ipod/i)) {
        window.location = '/Submit／' + document.getElementById('email').value + ',' + document.getElementById('loginPwd').value;
    }
}
function checkAllInput() {
    var mailValue = $("#email").val();
    var pwdValue = $("#loginPwd").val();
    var pwdValue2 = $("#again_loginPwd").val();
    if (mailValue == "" || mailValue == str_input_register_mail
    || pwdValue == "" || pwdValue == str_input_pwd
    || pwdValue2 == "" || pwdValue2 == str_input_pwd_again) {
        return false;
    }
    var isMailRight = checkMail(mailValue);
    var isPwdRight = checkPwd(pwdValue);
    var isPwd2Right = (pwdValue == pwdValue2 ? true : false);
    if (isMailRight && isPwdRight && isPwd2Right) {
        return true;
    }
    return false;
}

/* button effect */
function btnReg_DownEffect() {
    setImgSrc("btnreg", "image/Btn_reg_press.png");
}
function btnReg_UpEffect() {
    setImgSrc("btnreg", "image/Btn_reg.png");
}

/* for mobile */
$(function () {
    $('#abtnreg').bind('touchstart', function () {
        btnReg_DownEffect();
    }).bind('touchend', function () {
        btnReg_UpEffect();
    });

    $('#btnClosePage').bind('touchstart', function () {
        btnClose_DownEffect();
    }).bind('touchend', function () {
        btnClose_UpEffect();
    });
});