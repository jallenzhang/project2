<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    德州扑克数据
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>
        德州扑克数据分析:</h3>
    <ol class="round">
        <li class="one"><a href="/ProductData/Index">产品数据</a> </li>
        <li class="two"><a href="/DataAnalyze/2OperationsData/Index">运营数据</a> </li>
        <li class="three"><a href="/DataAnalyze/3MarketData/Index">市场数据</a> </li>
        <li class="four"><a href="/DataAnalyze/4RevenueData/Index">营收数据</a> </li>
        <li class="five"><a href="/DataAnalyze/5BossData/Index">BOSS数据</a> </li>
    </ol>
</asp:Content>
