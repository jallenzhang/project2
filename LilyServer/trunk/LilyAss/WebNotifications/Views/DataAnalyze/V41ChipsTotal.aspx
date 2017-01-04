<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    筹码数量量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>筹码数量
</h2>
<p>
总量: <%=Html.DisplayText("totalChips") %>
</p>
<p>
用户平均持有量: <%=Html.DisplayText("averageChips")%>
</p>


</asp:Content>