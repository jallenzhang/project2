<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<WebNotifications.Models.DeviceUserObj>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    用户查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
    <div class="roundedBorders right" id="content">
        <div>     
        <h2>用户查询</h2>     
        <% using (Html.BeginForm(new { Action = "UserSearchList" }))
             { %>        
                <p><label>时间(2011/7/8):&nbsp;</label><%=Html.TextBox("tbFilterTime")%>
                </p>
                <p>
                <label>渠道列表:&nbsp;</label><%=Html.DropDownList("dpChannels")%><br/>
                </p>
                <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("UserSearchList")%>"' />               
        <% } %>   
         <table>
        <tr>
            <th>时间</th>
            <th>总用户</th>                
            <th>注册用户</th>    
            <th>社区用户</th>    
            <th>游客</th>    
            <th>游客转注册</th>    
        </tr>    
        <% foreach (var m in Model)
            { %>
            <tr>
                <td><%= m.DateTime.ToShortDateString() %></td>
                <td><%= m.TotalAmount %></td>
                <td><%= m.RegisterAmount %></td>
                <td><%= m.CommunityAmount %></td>
                <td><%= m.GuestAmount %></td>
                <td><%= m.GuestToRegister %></td>                
            </tr>
        <% } %>
        </table>        
        </div>
    </div>
</asp:Content>
