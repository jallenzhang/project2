<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>康熙德州扑克</title>
    <%: Styles.Render("~/Content/customCss")%> 
    <%: Scripts.Render("~/bundles/jquery")%> 
    <%: Scripts.Render("~/bundles/customJs")%>
</head>
<body>
    <div class="backimage" id="ibackimage">        
        <img id="logoHight" src="/Content/images/hight.png" style="position:absolute;width:100%;height:100%" alt=""/>
        <div class="logo">
            <!--<img src="image/logo.png" alt=""/>-->
            <div id="scalecontainer">
                <div id="frontpage_hype_container" style="position:relative;overflow:hidden;width:700px;height:700px;">
                    <%: Scripts.Render("~/bundles/homeJs")%>		           
	            </div>
            </div>
            <br/>                        
            <input id="btnAppStore" type="button" style="background-image:url('/Content/images/appstore.png')" onclick="window.open('<%=Html.DisplayText("tbAppStoreUrl")%>')" />
            <input id="btnAndroid" type="button" style="background-image:url('/Content/images/android.png')" onclick="window.open('<%=Html.DisplayText("tbAndroidUrl")%>')" />
        </div>
    </div>    
    <div class="BtnChoose">  
        <ul>
            <li class="lifirst">
                <input class="btnIcon1" type="button" style="background-image:url('/Content/images/toufetechlogo.png')" onclick="window.open('http://www.baidu.com')" />                
            </li>
            <li class="liother">
                <input class="btnIcon2" type="button" style="background-image:url('/Content/images/untiylogo.png')"  onclick="window.open('http://www.baidu.com')" />
            </li>
            <li class="liother">
                <input class="btnIcon3" type="button" style="background-image:url('/Content/images/photonlogo.png')" onclick="window.open('http://www.baidu.com')" />
             </li>
        </ul>      
        
    </div>
    <div class="foot">
        Copyright ©2010-2011 Toufe Tech. All rights reserved. 浙ICP备10204782号
    </div>
</body>
</html>
