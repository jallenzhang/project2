<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%: Styles.Render("~/Content/customCss")%>
<%: Scripts.Render("~/bundles/customJs")%>
<div class="bImg" id="sidebar">
    <div class="bInner">
        <span class="bTR"></span><span class="bBL"></span>
        <ul id="side-nav">
            <li class="active"><a class="button" title="Posts" href="#"><strong><span class="title">
                游戏配置</span> <span class="expand expanded"></span></strong></a>
                <ul style="display: block;">
                    <li><a href="/GameConfig/BotConfigIndex">机器人配置</a> </li>
                    <li><a href="/GameConfig/SystemConfigIndex">系统配置</a> </li>
                </ul>
            </li>
        </ul>
    </div>
</div>
