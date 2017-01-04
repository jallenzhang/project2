<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    游戏人数数据
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>
<h2>游戏人数数据为：<%: ViewBag.Message %></h2>
<table>
<tr>
    <th>id</th>
    <th>nickname</th>
    <th>username</th>
    <th>mail</th>
</tr>
<% foreach (var m in ViewData.Model)
   { %>
   <tr>
       <td><%= m.id %></td>
       <td><%= m.nickname == null ? string.Empty : m.nickname.ToString() %></td>
       <td><%= m.username == null ? string.Empty : m.username.ToString()%></td>
       <td><%= m.mail == null ? string.Empty : m.mail.ToString()%></td>
   </tr>
<% } %>
</table>
</asp:Content>