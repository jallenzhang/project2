<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.Custom.PartnerExtern>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    推广
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Promotion/PromotionMenu.ascx"); %> 
  <div class="right" id="content">
    <h2> 合作商列表</h2>
    <table>
        <tr>
            <th>ID</th>
            <th>合作商名称</th>
            <th>推广的用户数量</th>
            <th>备注</th>
            <th>推广链接</th>
            <th>生成链接</th>
        </tr>
        <% foreach (var m in ViewData.Model) { %>
        <tr>
            <td><div><%= m.Partner_P.partnerid%></div></td>
            <td><div><%= m.Partner_P.name == null ? string.Empty : m.Partner_P.name.ToString()%></div></td>
            <td><div><%= m.Partner_P.count == null ? 0 : m.Partner_P.count %></div></td>
            <td><div><%= Html.Encode(m.Partner_P.note == null ? string.Empty : m.Partner_P.note.ToString()) %></div></td>
            <td><div><%= Html.Encode(m.Partner_P.pageurl == null ? string.Empty : m.Partner_P.pageurl.ToString()) %></div></td>
            <td><div><%= Html.Encode(m.Redirect_url) %></td>
        </tr>
        <% } %>
    </table>
     <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
    </div>
</asp:Content>
