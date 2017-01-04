<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.DeviceTypeObj>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
<h2> 渠道--Android机型查询</h2>
<% using (Html.BeginForm(new { Action = "DeviceTypeAndroidSearch" }))
     { %>        
        <p><label>时间(2011/7/8):&nbsp;</label><%=Html.TextBox("tbFilterTime")%>
        </p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("DeviceTypeAndroidSearch")%>"' />               
<% } %>
    <div class="roundedBorders right" id="content">
        <div>
            <h3> Android机型</h3>
            <div class="scroll">       
                <table>
                    <tr>
                        <th>机型</th>
                        <th>数量</th>
                    </tr>
                   <% if(ViewData.Model != null) { %>
                       <% foreach (var m in ViewData.Model) { %>
                        <tr>
                            <td><div><%= m.Name%></div></td>
                            <td><div><%= m.Amount%></div></td>
                        </tr>
                        <% } %>
                    <% } %>
                </table>
            </div>
            <%=Html.AjaxPager(Model, new PagerOptions() { CssClass = "mvcPager", PageIndexParameterName = "page", ShowMorePagerItems = false, AlwaysShowFirstLastPageNumber = true, ShowPageIndexBox = true, PageIndexBoxWrapperFormatString = "页{0}" }, new AjaxOptions() { UpdateTargetId = "dvOrders" })%>
       </div>
    </div>
</asp:Content>