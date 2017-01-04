<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    推广
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Promotion/PromotionMenu.ascx"); %> 
  <div class="roundedBorders" id="content">
    <h2>增加合作商</h2>
    <% using (Html.BeginForm(new { Action="AddParterAction" }))
       { %>
         <p><label>合作商名称：</label><%=Html.TextBox("tbName")%></p>
         <p><label>备注：</label><%=Html.TextBox("tbNote")%></p>
         <p><input type="submit" value="添加" /></p>
     <% } %>
     </div>
</asp:Content>