<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    7 玩家购买筹码花费
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>7 玩家购买筹码花费
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V7CostOfPalyerBuyChips" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V7CostOfPalyerBuyChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV7CostOfPalyerBuyChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>今日购买筹码的人数：</label><%=Html.DisplayText("tbUserNum")%></p>  
     <p><label>今日购买的筹码总量：</label><%=Html.DisplayText("tbTotalChips")%></p>    
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>