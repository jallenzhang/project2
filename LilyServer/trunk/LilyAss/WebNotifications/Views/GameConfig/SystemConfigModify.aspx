<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.configSystem>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    系统配置修改
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/GameConfig/GameConfigList.ascx"); %> 
  <div class="roundedBorders" id="content">
    <div class="systemmodify">     
    <h2>系统配置修改</h2>
     <p>
     <% using (Html.BeginForm(new { Action = "SystemConfigModify", Model.id }))
     { %>
     <%: Html.ValidationSummary() %>
     </p>
        <label>描述:&nbsp;</label><%=Html.TextAreaFor(m => Model.description)%><br/>
        <label>value:&nbsp;</label><%=Html.TextBoxFor(m => Model.value)%><br/>
        <label>valuestr:&nbsp;</label><%=Html.TextAreaFor(m => Model.valuestr)%><br/>
        <p>
        <input type="submit" value="修改" onclick='this.form.action="<%=Url.Action("SystemConfigModify", new{ Model.id })%>"'/>    
        <%--<input type="submit" value="删除" onclick='this.form.action="<%=Url.Action("SystemDeleteModifyAction", new{ Model.id })%>"'/>     --%>
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("SystemConfigCancleAction")%>"'/>         
        </p>
     <% } %>
    </div>
 </div>
</asp:Content>
