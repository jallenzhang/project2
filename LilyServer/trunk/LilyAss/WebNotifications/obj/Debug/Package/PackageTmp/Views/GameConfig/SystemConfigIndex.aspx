<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.configSystem>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    系统配置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.RenderPartial("~/Views/GameConfig/GameConfigList.ascx"); %> 
    <div class="roundedBorders right" id="content">
        <div>     
    <h2>系统配置</h2>
    <%=ViewData["Message"]%>
     <a href='/GameConfig/SystemConfigAdd'>新增</a>
     <table>
    <tr>
        <th>描述</th>
        <th>value</th>
        <th>valuestr</th>
        <th>更新时间</th>
        <th>更新者</th>
        <th>操作</th>
    </tr>
    <% foreach (var m in Model)
        { %>
        <tr>
            <td><%= m.description %></td>
            <td><%= m.value %></td>
            <td><%= m.valuestr %></td>
            <td><%= m.updatetime %></td>
            <td><%= m.updateby %></td>
            <td><a href='/GameConfig/SystemConfigModify/<%= m.id %>'>修改</a></td>
        </tr>
    <% } %>
    </table>
    <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
    </div>
    </div>
</asp:Content>
