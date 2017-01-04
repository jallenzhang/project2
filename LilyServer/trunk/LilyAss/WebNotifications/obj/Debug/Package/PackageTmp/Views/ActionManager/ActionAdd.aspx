<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.ActionPermission>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Action 添加
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="botmodify">     
    <h2>Action 添加</h2>
    <%: Html.ValidationSummary() %>
     <% using (Html.BeginForm(new { Action = "ActionAdd" }))
     { %>
        <label>ActionName:&nbsp;</label><%=Html.DropDownList("dpActions")%><br/>
        <label>Permission:&nbsp;</label><%=Html.DropDownList("dpRoles")%><br/>
        <p>
        <input type="submit" value="新增" onclick='this.form.action="<%=Url.Action("ActionAdd")%>"' />
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("ActionAddCancleAction")%>"' />
        </p>
     <% } %>
    </div>
</asp:Content>
