<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    6 牌桌税收
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>6 牌桌税收
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V6TableTaxAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V6TableTaxAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV6TableTaxAction")%>"' />
     <% } %>
     </p>
     <p><label>今日对局次数：</label><%=Html.DisplayText("tbRoundTimes")%></p>  
     <p><label>今日牌桌回收的筹码数：</label><%=Html.DisplayText("tbRecvTableChips")%></p>    
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>