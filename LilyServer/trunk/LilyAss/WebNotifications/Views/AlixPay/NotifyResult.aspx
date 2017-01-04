<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    测试支付宝通知服务端
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>测试支付宝通知服务端</h2>
<% using (Html.BeginForm(new { Action = "NotifyResult" }))
     { %>
        <input type="submit" value="测试" />
     </p>
     <p><label>数据：</label><%=Html.TextArea("data")%></p>    
 <% } %>
</asp:Content>