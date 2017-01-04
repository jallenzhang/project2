<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.CustomUser.MyUser>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    添加用户到角色
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>

<h2>添加用户到角色</h2>
<div class="roundedBorders right" id="Div1">
<% using (Html.BeginForm(new { Action="AddUserToRolesAction" }))
       { %>
         <%--<p><label>UserName：</label><%=Html.TextBox("tbUsername")%></p>--%>        
         <div class="roundedBorders" id="content">
             <table>
                <tr>
                    <th>多选框</th>
                    <th>用户名</th>
                    <th>当前权限</th>
                    <th>删除</th>
                </tr>
                <% foreach (var m in Model)
                    { %>
                    <tr>
                        <td><input type="checkbox" name="selectedNames" value="<%=m.UserName%>" /></td>
                        <td><%= m.UserName %></td>
                        <td><%= m.RoleName %></td>                        
                        <td><%: Html.ActionLink("删除", "DeleteUserAction", new { m.UserName })%></td>
                    </tr>
                <% } %>
            </table>
            <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
        </div>
         <p><label>RoleName：</label><%=Html.DropDownList("tbRolename")%></p>
         <p><input type="submit" value="添加" onclick='this.form.action="<%=Url.Action("AddUserToRolesAction")%>"' /></p>
<% } %>
</div>
</asp:Content>