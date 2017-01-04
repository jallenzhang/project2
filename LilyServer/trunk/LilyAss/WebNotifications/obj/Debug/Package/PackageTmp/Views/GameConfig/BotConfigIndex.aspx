<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.configRobotStrategy>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    机器人配置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.RenderPartial("~/Views/GameConfig/GameConfigList.ascx"); %> 
    <div class="roundedBorders right" id="content">
        <div>     
    <h2>机器人配置</h2>
    <%=ViewData["Message"]%>
     <%--<% using (Html.BeginForm("BotConfigImportAction", "GameConfig", FormMethod.Post, new { enctype = "multipart/form-data" }))
     { %>
         <p>
        <input type="file" name="uploadBotConfig"/>
        <input type="submit" value="导入配置"' />
        </p>
     <% } %><br/>--%>
    <%-- <a href='/GameConfig/BotConfigAdd'>新增</a>--%>
     <table>
    <tr>
        <th>strategyid</th>
        <th>strategy</th>
        <th>回合</th>
        <th>盖牌率</th>
        <th>叫牌率</th>
        <th>延迟min</th>
        <th>延迟max</th>
        <th>rasiea</th>
        <th>rasieb</th>
        <th>操作</th>
    </tr>
    <% foreach (var m in Model)
        { %>
        <tr>
            <td><%= m.strategyid %></td>
            <td><%= m.strategy ?? string.Empty%></td>
            <td><%= m.typeround %></td>
            <td><%= m.foldratio %></td>
            <td><%= m.callratio %></td>
            <td><%= m.delaymin %></td>
            <td><%= m.delaymax %></td>
            <td><%= m.rasiea %></td>
            <td><%= m.rasieb %></td>
            <td><a href='/GameConfig/BotConfigModify/<%= m.id %>'>修改</a></td>
        </tr>
    <% } %>
    </table>
    <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
     <% using (Html.BeginForm(new { Action = "BotConfigExportAction" }))
     { %>
     <p>
    <input type="submit" value="导出配置" />
    </p>
     <% } %>
    </div>
    </div>
</asp:Content>
