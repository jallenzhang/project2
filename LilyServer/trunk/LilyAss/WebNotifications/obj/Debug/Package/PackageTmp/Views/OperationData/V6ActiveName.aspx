<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    活动名称
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>活动名称</h2>
<div>
     <p>
     <% using (Html.BeginForm(new { Action = "V6ActiveNameAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V6ActiveNameAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV6ActiveNameAction")%>"' />
     <% } %>
     </p> 
     活动数据
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
