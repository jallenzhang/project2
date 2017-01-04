<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    11 牌局默认语句使用次数统计
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>11 牌局默认语句使用次数统计
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V11LanguageUsedTimesAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V11LanguageUsedTimesAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV11LanguageUsedTimesAction")%>"' />
     <% } %>
     </p>
     这里会动态显示
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>