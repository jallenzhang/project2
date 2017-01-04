<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%: Styles.Render("~/Content/customCss")%>
<%: Scripts.Render("~/bundles/customJs")%>
<div class="bImg" id="sidebar">
    <div class="bInner">
        <span class="bTR"></span><span class="bBL"></span>
        <ul id="side-nav">
            <%Html.RenderPartial("~/Views/MarketData/MarketList.ascx"); %> 
            <%Html.RenderPartial("~/Views/ProductData/ProductList.ascx"); %> 
            <%Html.RenderPartial("~/Views/OperationData/OperationList.ascx"); %> 
            <%Html.RenderPartial("~/Views/BalancePayAccount/BalancePayAccountList.ascx"); %> 
            <%Html.RenderPartial("~/Views/Payment/PaymentList.ascx"); %> 
            <%Html.RenderPartial("~/Views/UserSearch/UserSearchList.ascx"); %> 
        </ul>
    </div>
</div>
