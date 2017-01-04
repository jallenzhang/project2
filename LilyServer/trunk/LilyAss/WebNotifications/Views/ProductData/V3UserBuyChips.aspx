<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    3 用户购买的筹码数量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>3 用户购买的筹码数量
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V3UserBuyChipsAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V3UserBuyChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV3UserBuyChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>今日购买筹码人数：</label><%=Html.DisplayText("tbBuyChipsUserNum")%></p>    
     <p><label>今日用户购买的筹码数量：</label><%=Html.DisplayText("tbBuyChipsNum")%></p>
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>