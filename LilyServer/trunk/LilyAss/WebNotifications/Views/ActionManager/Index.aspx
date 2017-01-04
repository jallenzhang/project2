<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.ActionPermission>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Action 权限管理
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>
<h2>Action 权限管理</h2>
 <% using (Html.BeginForm("ActionConfigImportAction", "ActionManager", FormMethod.Post, new { enctype = "multipart/form-data" }))
    { %>
        <p>
    <input type="file" name="uploadActionConfig"/>
    <input type="submit" value="导入配置"' />
    </p>
    <% } %>
     <% using (Html.BeginForm(new { Action = "ActionConfigExportAction" }))
     { %>
    <input type="submit" value="导出配置" />
     <% } %>
<% using (Html.BeginForm(new { Action="AddActionToRolesAction" }))
       { %>
         <p><label>RoleName：</label><%=Html.DropDownList("tbRolename")%><input type="submit" value="添加" /><%: Html.ActionLink("增添权限", "AddRoles", "ActionManager")%></p>
         <p></p>
         <div class="right" id="content">
         <a href='/ActionManager/ActionAdd'>新增</a>
             <table>
                <tr>
                    <th>--</th>
                    <%--<th>Controller Name</th>--%>
                    <%--<th>Action Name</th>--%>
                    <th>Action 描述</th>
                    <th>当前权限</th>
                    <th>操作</th>
                </tr>
                <% foreach (var m in Model)
                    { %>
                    <tr>
                        <td><input type="checkbox" name="selectedNames" value="<%=m.id%>" /></td>
                        <%--<td><%= m.controllerName %></td>--%>
                        <%--<td><%= m.actionName %></td>--%>
                        <td><%= m.description %></td>
                        <td><%= m.permission %></td>
                        <td><%: Html.ActionLink("删除", "ActionNameDeleteAction", new { m.id })%></td>
                    </tr>
                <% } %>
            </table>
            <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
        </div>
<% } %>

</asp:Content>