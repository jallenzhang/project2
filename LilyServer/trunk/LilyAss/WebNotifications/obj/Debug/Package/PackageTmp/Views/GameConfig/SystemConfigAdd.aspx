<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.configSystem>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    系统配置增加
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/GameConfig/GameConfigList.ascx"); %> 
  <div class="roundedBorders" id="content">
    <div class="botmodify">     
    <h2>系统配置增加</h2>
    <%: Html.ValidationSummary() %>
     <% using (Html.BeginForm(new { Action = "SystemConfigAdd" }))
     { %>
        <label>描述:&nbsp;</label><%=Html.TextAreaFor(m => Model.description)%><br/>
        <label>value:&nbsp;</label><%=Html.TextBoxFor(m => Model.value)%><br/>
        <label>valuestr:&nbsp;</label><%=Html.TextAreaFor(m => Model.valuestr)%><br/>
        <p>
        <input type="submit" value="新增" onclick='this.form.action="<%=Url.Action("SystemConfigAdd")%>"' />
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("SystemConfigCancleAction")%>"' />
        </p>
     <% } %>
    </div>
</div>
</asp:Content>
