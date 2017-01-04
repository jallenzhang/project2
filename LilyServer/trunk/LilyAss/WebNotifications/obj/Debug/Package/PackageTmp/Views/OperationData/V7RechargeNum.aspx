<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    充值用户数量
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>充值用户数量</h2>
<div>
     <% using (Html.BeginForm(new { Action = "V7RechargeNumAction" }))
     { %>
        <p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V7RechargeNumAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV7RechargeNumAction")%>"' />
        </p> 
        <p><label>开始时间(e; 2012/08/12):</label><%=Html.TextBox("tbStartDate")%> --- <label>结束时间(e; 2012/09/08):</label><%=Html.TextBox("tbEndDate")%></p>
     <% } %>     
     <% if(Model != null) { %>
         <% foreach(var item in Model)
         { %>
            <p><label>时间: <%= item.Name%></label>
            <label>人数: <%= item.Count2%></label>
            </p>                
         <% } %>
     <% } %>
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
