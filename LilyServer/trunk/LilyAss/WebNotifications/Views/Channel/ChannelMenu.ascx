<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%: Styles.Render("~/Content/customCss")%>
<%: Scripts.Render("~/bundles/customJs")%>
<div class="bImg" id="sidebar">
    <div class="bInner">
        <span class="bTR"></span><span class="bBL"></span>
        <ul id="side-nav">
            <li class="active"><a class="button" title="Posts" href="#"><strong><span class="title">
                渠道页面</span> <span class="expand expanded"></span></strong></a>
                <ul style="display: block;">
                    <li><a href="/Channel/ChannelIndex">渠道列表</a> </li>
                    <li><a href="/Channel/DeviceChannel">渠道-设备安装量统计</a> </li>
                    <li><a href="/Channel/DeviceTypeIOS">渠道-机型IOS</a> </li>
                    <li><a href="/Channel/DeviceTypeAndroid">渠道-机型Android</a> </li>
                    <li><a href="/Channel/DeviceSystemIOS">渠道-IOS系统</a> </li>
                    <li><a href="/Channel/DeviceSystemAndroid">渠道-Andorid系统</a> </li>
                    <li><a href="/Channel/UserIndex">渠道-用户</a> </li>                    
                    <li><a href="/Channel/DeviceIncome">渠道-收入</a> </li>
                </ul>
            </li>
        </ul>
    </div>
</div>
