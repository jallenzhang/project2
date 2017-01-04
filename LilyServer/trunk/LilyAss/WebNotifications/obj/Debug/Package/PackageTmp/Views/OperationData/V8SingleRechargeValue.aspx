<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    单个用户充值金额
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>单个用户充值金额</h2>
<div>
     <p>
     <% using (Html.BeginForm(new { Action = "V8SingleRechargeValueAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V8SingleRechargeValueAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV8SingleRechargeValueAction")%>"' />
        <p><label>用户帐户或昵称:&nbsp;<%=Html.TextBox("tbName")%></label></p>
     <% } %>
     </p> 
     <% if(Model != null) { %>
         <% foreach(var item in Model)
         { %>
            <label>充值时间: <%= item.time.ToString()%></label>
            <label>充值金钱: <%= item.money%></label>
            </p>                
         <% } %>
     <% } %>
   </div>   
   <a href="/DataManager/Index">返回</a> </div>   
</asp:Content>
