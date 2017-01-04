<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    4 活动赠送的筹码量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>4 活动赠送的筹码量
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V4ActiveSendChipsAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V4ActiveSendChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV4ActiveSendChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>选择活动：</label><%=Html.DropDownList("tbActiveList")%></p>  
     <p><label>活动持续时间：</label><%=Html.TextBox("tbStartDate")%> -- <%=Html.TextBox("tbEndDate")%></p>    
     <p><label>活动参与人数：</label><%=Html.TextBox("tbAttentNum")%></p>
     <p><label>活动赠送的筹码数量：</label><%=Html.TextBox("tbActiveSendChips")%></p>
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>