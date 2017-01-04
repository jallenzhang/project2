<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.Role>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    添加权限
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>添加权限</h2>
<%: Styles.Render("~/Content/customCss")%>
   


<div class="roundedBorders right" id="content">
<% using (Html.BeginForm(new { Action="CreateRoles" }))
       { %>
       <p><label>权限名称：</label><%=Html.TextBox("tbName")%></p>
       <p><input type="submit" value="添加" /></p>   
    <table>
        <tr>
            <th>权限名称</th>
            <th>删除</th>
        </tr>
        <% foreach (var m in Model)
            { %>
            <tr>
                <td><%= m.RoleName %></td>                      
                <td><%: Html.ActionLink("删除", "DeleteRolesAction", new { m.RoleName })%></td>
            </tr>
        <% } %>
    </table>
    <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
<% } %>
</div>

</asp:Content>
