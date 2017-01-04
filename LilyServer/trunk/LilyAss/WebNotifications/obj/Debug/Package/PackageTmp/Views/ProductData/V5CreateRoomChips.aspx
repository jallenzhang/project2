<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    5 用户建房筹码数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>5 用户建房筹码数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V5CreateRoomChipsAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V5CreateRoomChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV5CreateRoomChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>今日建房次数：</label><%=Html.DisplayText("tbCreateRoomTimes")%></p>  
     <p><label>今日建房扣除的总费用：</label><%=Html.DisplayText("tbCreateRoomTotalMoney")%></p>    
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>