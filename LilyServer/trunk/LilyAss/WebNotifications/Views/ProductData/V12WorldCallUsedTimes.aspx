<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    12 世界喊话功能使用次数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>12 世界喊话功能使用次数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V12WorldCallUsedTimesAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V12WorldCallUsedTimesAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV12WorldCallUsedTimesAction")%>"' />
     <% } %>
     </p>
     <% if(Model != null) { %>
         <% foreach(var item in Model)
         { %>
            <p><label>世界喊话时间: <%= item.Name%></label>
            <label>,&nbsp;次数: <%= item.Count%></label>
            <label>人数: <%= item.Count2%></label>
            </p>                
         <% } %>
     <% } %>
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>