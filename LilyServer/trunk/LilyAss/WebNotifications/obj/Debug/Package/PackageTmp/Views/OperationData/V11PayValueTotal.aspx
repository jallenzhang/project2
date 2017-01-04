<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    付费总量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>付费总量</h2>
<div>
     <p>
     <% using (Html.BeginForm(new { Action = "V11PayValueTotalAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V11PayValueTotalAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV11PayValueTotalAction")%>"' />
     <% } %>
     </p> 
      <p><label>付费总量(RMB)：</label><%=Html.DisplayText("tbTotal")%></p>  
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
