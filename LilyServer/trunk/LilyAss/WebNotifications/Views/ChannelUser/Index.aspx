<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<WebNotifications.Models.ChannelUser.DataReport>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.RenderPartial("~/Views/ChannelUser/Menu.ascx"); %>   
<div class="roundedBorders right" id="content">
     <div>
     <h2>数据汇总--<%=Html.DisplayText("tbChannelName") %></h2>
         <% using (Html.BeginForm(new { Action = "DataSearchByDay" }))
                { %>        
                <p><label>时间(2011/7/8):&nbsp;</label><%=Html.TextBox("tbFilterTime")%>
                <input type="submit" value="查询" />    
                </p>                       
        <% } %>
        <table>
            <tr>
                <th>时间</th>
                <th>当日激活</th>
                <th>当日启动</th>
                <th>当日付费人数</th>
                <th>当日收入</th>
            </tr>
            <% if (ViewData.Model != null) {%>
               <% foreach (var m in ViewData.Model) { %>
                <tr>
                    <td><div><%= m.MyDateTime %></div></td>
                    <td><div><%= m.ActiveNum %></div></td>
                    <td><div><%= m.StartNum %></div></td>
                    <td><div><%= m.PayNum %></div></td>
                    <td><div><%= m.IncomeNum.ToString("0.00") %></div></td>
                </tr>
                <% } %>
            <% } %>
        </table>             
        <% using (Html.BeginForm(new { Action = "Index" })) { %>
           <input type="submit" value="上一页" onclick='this.form.action="<%=Url.Action("Index", "ChannelUser", new { page = -1 })%>"'  />
           <input type="submit" value="下一页" onclick='this.form.action="<%=Url.Action("Index", "ChannelUser", new { page = 1})%>"' />        
         <% } %>
         <% using (Html.BeginForm(new { Action = "DataReportExportAction" }))
         { %>
         <p>
        <input type="submit" value="导出excel" />
        </p>
         <% } %>
      </div>
    </div>
</asp:Content>
