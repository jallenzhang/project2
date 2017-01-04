<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    2 每日奖励筹码数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>2 每日奖励筹码数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V2EveryChipsAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V2EveryChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV2EveryChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>今日登陆总人数：</label><%=Html.DisplayText("tbTodayLoginNum")%></p>    
     <p><label>今日登陆Guest总人数：</label><%=Html.DisplayText("tbTodayLoginGuestNum")%></p>
     <p><label>今日登陆注册用户数：</label><%=Html.DisplayText("tbTodayLoginRegistNum")%></p>
     <p><label>今日登陆付费用户数：</label><%=Html.DisplayText("tbTodayLoginPayNum")%></p>
     <p><label>Guest用户今日奖励总筹码数：</label><%=Html.DisplayText("tbTodayGuestChips")%></p>
     <p><label>注册用户今日奖励总筹码数：</label><%=Html.DisplayText("tbTodayRegistChips")%></p>
     <p><label>付费用户今日奖励筹码总数：</label><%=Html.DisplayText("tbTodayPayChips")%></p>
     <p><label>总计今日奖励筹码数：</label><%=Html.DisplayText("tbTodayTotalChips")%></p>
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>