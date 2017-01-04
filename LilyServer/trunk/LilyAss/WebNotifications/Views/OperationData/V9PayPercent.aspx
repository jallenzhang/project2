<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    付费率
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>付费率</h2>
<div>
     <p>
     <% using (Html.BeginForm(new { Action = "V9PayPercentAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V9PayPercentAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV9PayPercentAction")%>"' />
        <p><label>选择查询日期:&nbsp;</label><%=Html.TextBox("tbName")%></p>
     <% } %>
     </p> 
     <p>至查询日的导入(至少有一次登陆)人数=&nbsp;<%=Html.DisplayText("tbLoginNum")%></p>
     <p>至查询日期为止的付费人数=&nbsp;<%=Html.DisplayText("tbPayNum")%></p>
     <p>至查询日期的付费率=&nbsp;<%=Html.DisplayText("tbPayPercent")%></p>
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
