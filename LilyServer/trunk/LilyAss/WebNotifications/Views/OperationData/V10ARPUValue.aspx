<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ARPU
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>ARPU</h2>
<div>
     <p>
     <% using (Html.BeginForm(new { Action = "V10ARPUValueAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V10ARPUValueAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV10ARPUValueAction")%>"' />
        <p><label>选择查询日期(2012/09/08):&nbsp;</label><%=Html.TextBox("tbName")%></p>
     <% } %>
     </p> 
     <p>海外和港台计算方式</p>
     <ul>
        <li><p>至查询日的导入人数=&nbsp;<%=Html.DisplayText("tbPayNumF")%></p></li>
        <li><p>至查询日的付费总量(RMB)=&nbsp;<%=Html.DisplayText("tbPayPercent")%></p></li>
        <li><p>ARPU值=&nbsp;<%=Html.DisplayText("tbPayPercentF")%></p></li>
     </ul>
     <hr />
     <p>大陆ARPU值计算方式</p>
     <ul>
        <li><p>至查询日的付费人数=&nbsp;<%=Html.DisplayText("tbPayNumI")%></p></li>
        <li><p>至查询日的付费总量(RMB)=&nbsp;<%=Html.DisplayText("tbPayPercent")%></p></li>
        <li><p>ARPU值=&nbsp;<%=Html.DisplayText("tbPayPercentI")%></p></li>
     </ul>
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
