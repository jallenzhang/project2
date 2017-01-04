<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    配置推广页面
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Promotion/PromotionMenu.ascx"); %> 
  <div class="roundedBorders" id="content">
  <h2>链接地址</h2>
  <%: ViewBag.Message.ToString() %>
 <% using (Html.BeginForm(new { Action = "GetUrlView" }))
       { %>
         <p><label>推广页面：</label><%=Html.TextBox("tbName")%></p>
         <p><label>合作商名称：</label><%=Html.DropDownList("dpPartersList")%></p>
         <p><input type="submit" value="生成地址" /></p>
 <% } %>
 </div>
</asp:Content>