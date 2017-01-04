<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.user>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    输赢平率数据
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>
<h2>输赢平率数据</h2>
<table>
<tr>
    <th>id</th>
    <th>nickname</th>
    <th>username</th>    
    <th>mail</th>
    <th>totalgame</th>
    <th>wins</th>
    <th>win percent</th>
</tr>
<% foreach (var m in ViewData.Model)
   { %>
   <tr>
       <td><%= m.id %></td>
       <td><%= m.nickname ?? string.Empty %></td>
       <td><%= m.username ?? string.Empty %></td>
       <td><%= m.mail ?? string.Empty %></td>
       <td><%= m.totalgame ?? 0 %></td>
       <td><%= m.wins ?? 0 %></td>
       <td><%
               double total = double.Parse((m.totalgame == null ? 0 : m.totalgame).ToString());
               double wins = double.Parse((m.wins == null ? 0 : m.wins).ToString());
               double res = 0;
               if (total != 0) res = wins * 100 / total;
               res = Math.Round(res, 2);
               Response.Write(res + "%");
            %></td>
   </tr>
<% } %>
</table>
<%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
<% using (Html.BeginForm(new { Action = "DownloadV34WinLossData" }))
   { %>
         <p><input type="submit" value="下载" /></p>
<% } %>
</asp:Content>