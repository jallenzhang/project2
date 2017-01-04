<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.Channel>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
    <div class="roundedBorders right" id="content">
        <div>     
        <h2>渠道列表</h2>
        <%=ViewData["Message"]%>
         <a href='/Channel/ChannelAdd'>新增</a>     
         <table>
        <tr>
            <th>渠道Id</th>
            <th>渠道名称</th>                
            <th>分成</th>    
            <th>操作</th>    
        </tr>    
        <% foreach (var m in Model)
            { %>
            <tr>
                <td><%= m.channelId %></td>
                <td><%= m.channelName %></td>
                <td><%= m.proportion %></td>
                <td><a href='/Channel/ChannelModify/<%= m.channelId %>'>修改</a></td>
            </tr>
        <% } %>
        </table>
        <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
        </div>
    </div>
</asp:Content>
