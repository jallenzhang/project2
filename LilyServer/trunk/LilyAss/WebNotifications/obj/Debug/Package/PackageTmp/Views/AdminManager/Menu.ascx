<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%: Styles.Render("~/Content/customCss")%>
<%: Scripts.Render("~/bundles/customJs")%>
<div class="bImg" id="sidebar">
    <div class="bInner">
        <span class="bTR"></span><span class="bBL"></span>
        <ul id="side-nav">
            <li class="active"><a class="button" title="Posts" href="#"><strong><span class="title">
                Admin 管理</span> <span class="expand expanded"></span></strong></a>
                <ul style="display: block;">
                    <li><%: Html.ActionLink("用户权限管理", "AddUserToRoles", "ActionManager")%></li>
                    <li><%: Html.ActionLink("Action管理", "Index", "ActionManager")%></li>
                    <li><%: Html.ActionLink("渠道用户管理", "Index", "ChannelUserManager")%></li>
                </ul>
            </li>
        </ul>
    </div>
</div>
