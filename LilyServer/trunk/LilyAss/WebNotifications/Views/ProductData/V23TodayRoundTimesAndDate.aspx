<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    V23TodayRoundTimesAndDate
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>今日对局数和对局时间</h2>
 <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V23TodayRoundTimesAndDateAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V23TodayRoundTimesAndDateAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV23TodayRoundTimesAndDateAction")%>"' />
     </p>
     <p><label>选择查询日期= </label><%= Html.TextBox("tbDate") %></p>
     <p><label>对局数</label></p>    
     <p><label>玩家创建牌局(次)=</label><%=Html.DisplayText("tbPlayerCreates")%></p>    
     <p><label>系统牌局(次)=</label><%=Html.DisplayText("tbSystemCreates")%></p>    
     <p><label>平均对局时间</label></p>    
     <p><label>玩家创建牌局(s)：</label><%=Html.DisplayText("tbPlayerTime")%></p>    
     <p><label>系统牌局(s)：</label><%=Html.DisplayText("tbSystemTime")%></p>    
     <% } %>
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>