<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.Custom.PaymentObject>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    付费用户信息
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
  <h2>付费用户信息</h2>
    <% using (Html.BeginForm(new { Action = "PaymentInfoDownload" }))
     { %>
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("PaymentInfoDownload")%>"' /> 
        <p><label>开始时间:&nbsp;</label><%=Html.TextBox("tbStartTime")%>
        <label>结束时间:&nbsp;</label><%=Html.TextBox("tbEndTime")%>
        </p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("PaymentInfoSearch")%>"' />               
     <% } %>
    <div class="roundedBorders right" id="content">
        <div class="scroll">       
            <table>
            <tr>
                <th>user-id</th>
                <th>时间</th>
                <th>设备</th>   
                <th>购买渠道</th>
                <th>购买金额</th>
                <th>购买物品</th>        
                <th>物品名称</th>        
                <th>保存的字符串</th>
                <th>DeviceToken</th>
            </tr>
            <% foreach (var m in Model)
                { %>
                <tr>
                    <td><%= m.userid %></td>
                    <td><%= m.time %></td>
                    <td><%= m.devicetype %></td>
                    <td><%= m.buyway %></td>
                    <td><%= m.buyMoney %></td>
                    <td><%= m.buyItem %></td>
                    <td><%= m.ItemName %></td>
                    <td><%= m.note %></td>
                    <td><%= m.deviceToken %></td>            
                </tr>
            <% } %>
            </table>
        </div>
        <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
   </div>      
</asp:Content>
