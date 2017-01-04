<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.user>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    软件注册量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>
<h2>软件注册量为：<%: ViewBag.Message %>
</h2>
<table>
<tr>
    <th>id</th>
    <th>nickname</th>
    <th>username</th>
    <th>mail</th>
</tr>
<% foreach (var m in Model)
   { %>
   <tr>
       <td><%= m.id %></td>
       <td><%= m.userid %></td>
       <%--<td><%= m.nickname ?? string.Empty %></td>
       <td><%= m.username ?? string.Empty%></td>
       <td><%= m.mail ?? string.Empty %></td>--%>
   </tr>
<% } %>
</table>
<%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
<% using (Html.BeginForm(new { Action = "DownloadV23SoftRegisterNum" }))
   { %>
         <p><input type="submit" value="下载" /></p>
<% } %>
</asp:Content>