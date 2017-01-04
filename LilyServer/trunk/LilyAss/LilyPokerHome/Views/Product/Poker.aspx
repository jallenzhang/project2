<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" 
   "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
<head>
    <title>康熙德州扑克</title>
    <meta property="qc:admins" content="15165622766475656375" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <script type='text/javascript' src='http://resource.app.qq.com:8080/api/scripts/sjqqapi.js'charset='utf-8'></script>
    <link rel='stylesheet' type='text/css' href='http://resource.app.qq.com:8080/api/styles/sjqqapi.css' />
    <%: Styles.Render("~/Content/customCss")%> 
    <%: Scripts.Render("~/bundles/jquery")%> 
    <%: Scripts.Render("~/bundles/homeJs")%>	
    <%: Scripts.Render("~/bundles/customJs")%>    
</head>
<body>
    <div class="backimage" id="ibackimage">        
        <img id="logoHight" src="/Content/images/hight.png" style="position:absolute;width:100%;height:100%" alt=""/>
        <div class="logo">
            <div id="frontpage_hype_container" style="position:relative;overflow:hidden;width:700px;height:700px;margin:0 auto">                    	           
	        </div>
            <br/>         
            <a id="alinkapp" href='<%=Html.DisplayText("tbAppStoreUrl")%>' target="_blank" style="cursor:pointer;"><img id="btnAppStore" src="/Content/images/appstore.png" alt=""/></a>
            <a id="alinkandorid" href='<%=Html.DisplayText("tbAndroidUrl")%>' target="_blank" style="cursor:pointer;"><img id="btnAndroid" src="/Content/images/android.png" alt=""/></a>
            <div id="onkey">
                <a  style="cursor:pointer" onclick='qqapp_dl_apk(this);' ex_url='<%=Html.DisplayText("tbAndroidUrl")%>' appname='康熙德州扑克' asistanturlid='' title='使用腾讯手机管家(PC版)一键安装到手机'>
                    <img id="onkeyimg" src="/Content/images/onekeyinstall.png" alt='使用腾讯手机管家(PC版)一键安装到手机'/>
                </a>
            </div>
         </div>       
    </div>    
    <div class="BtnChoose">  
        <ul>
            <li class="lifirst">
                <img class="btnIcon1" src="/Content/images/toufetechlogo.png" alt=""/>                
            </li>
            <li class="liother">
                <img class="btnIcon2" src="/Content/images/untiylogo.png" alt=""/>                
            </li>
            <li class="liother">
                <img class="btnIcon3" src="/Content/images/photonlogo.png" alt=""/>
             </li>
        </ul>        
    </div>
    <div class="foot">
        Copyright ©2010-2011 Toufe Tech. All rights reserved. 浙ICP备10204782号
    </div>
</body>
</html>
