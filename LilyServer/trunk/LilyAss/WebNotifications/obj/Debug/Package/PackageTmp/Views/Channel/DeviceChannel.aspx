<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.DeviceChannelModel>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
<h2> 渠道--安装量统计查询</h2>
<% using (Html.BeginForm(new { Action = "DeciveChannelSearchByDay" }))
     { %>        
        <p><label>时间(2011/7/8):&nbsp;</label><%=Html.TextBox("tbFilterTime")%>
        </p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("DeciveChannelSearchByDay")%>"' />               
<% } %>
    <div class="roundedBorders right" id="content">
     <div>
        
        <table>
            <tr>
                <th>渠道</th>
                <th>总量</th>
                <% foreach (var m in ViewData.Model.DateNameList) { %> 
                    <th><div><%= m%></div></th> 
                <% } %>
            </tr>
           <% foreach (var m in ViewData.Model.DChannelObjList) { %>
            <tr>
                <td><div><%= m.ChannelName %></div></td>
                <td><div><%= m.DeviceNumberList[0] %></div></td>
                <td><div><%= m.DeviceNumberList[1]%></div></td>
                <td><div><%= m.DeviceNumberList[2]%></div></td>
                <td><div><%= m.DeviceNumberList[3]%></div></td>
                <td><div><%= m.DeviceNumberList[4]%></div></td>
                <td><div><%= m.DeviceNumberList[5]%></div></td>
                <td><div><%= m.DeviceNumberList[6]%></div></td>
                <td><div><%= m.DeviceNumberList[7]%></div></td>
            </tr>
            <% } %>
        </table>
        <%=Html.AjaxPager(Model.DChannelObjList, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
       </div>
    </div>
</asp:Content>