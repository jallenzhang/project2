<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.configRobotStrategy>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    机器人配置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/GameConfig/GameConfigList.ascx"); %> 
  <div class="roundedBorders" id="content">
    <div class="botmodify">     
    <h2>机器人配置</h2>
     <p>
     <% using (Html.BeginForm(new { Action = "BotConfigModify", Model.id, Model.strategyid, Model.strategy, Model.typeround }))
     { %>
     <%: Html.ValidationSummary() %>
     </p>
        <label>strategyid:&nbsp;</label><%=Html.DisplayTextFor(m => Model.strategyid)%><br/>
        <label>strategy:&nbsp;</label><%=Html.DisplayTextFor(m => Model.strategy)%><br/>
        <label>回合:&nbsp;</label><%=Html.DisplayTextFor(m => Model.typeround)%><br/>
        <label>盖牌率:&nbsp;</label><%=Html.TextBoxFor(m => Model.foldratio)%><br/>
        <label>叫牌率:&nbsp;</label><%=Html.TextBoxFor(m => Model.callratio)%><br/>
        <label>延迟min(<10):&nbsp;</label><%=Html.TextBoxFor(m => Model.delaymin)%><br/>
        <label>延迟max(<10):&nbsp;</label><%=Html.TextBoxFor(m => Model.delaymax)%><br/>
        <label>rasiea:&nbsp;</label><%=Html.TextBoxFor(m => Model.rasiea)%><br/>
        <label>rasieb:&nbsp;</label><%=Html.TextBoxFor(m => Model.rasieb)%><br/>
        <p>
        <input type="submit" value="修改" onclick='this.form.action="<%=Url.Action("BotConfigModify", new{ Model.id, Model.strategyid, Model.strategy, Model.typeround })%>"'/>    
        <%--<input type="submit" value="删除" onclick='this.form.action="<%=Url.Action("BotDeleteModifyAction", new{ Model.id, Model.strategyid, Model.strategy, Model.typeround })%>"'/>    --%> 
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("BotConfigCancleAction")%>"'/>         
        </p>
     <% } %>
    </div>
</div>
</asp:Content>
