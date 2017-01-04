<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.ChannelUser.UserInChannelExt>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    添加用户到渠道
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Styles.Render("~/Content/customCss")%>

<h2>添加用户到渠道</h2>
<div class="roundedBorders right" id="Div1">
<% using (Html.BeginForm(new { Action="AddUserToChannelsAction" }))
       { %> 
         <div class="roundedBorders" id="content">
             <table>
                <tr>
                    <th>多选框</th>
                    <th>用户名</th>
                    <th>渠道</th>
                    <th>删除</th>
                </tr>
                <% foreach (var m in Model)
                    { %>
                    <tr>
                        <td><input type="checkbox" name="selectedNames" value="<%=m.UserId%>" /></td>
                        <td><%= m.UserName %></td>
                        <td><%= m.ChannelName %></td>                        
                        <td><%: Html.ActionLink("删除", "DeleteUserInChannelAction", new { m.UserId })%></td>
                    </tr>
                <% } %>
            </table>
            <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
        </div>
         <p><label>渠道列表：</label><%=Html.DropDownList("tbChannelName")%></p>
         <p><input type="submit" value="添加" /></p>
<% } %>
</div>
</asp:Content>