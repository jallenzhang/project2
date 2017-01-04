<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.Channel>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道修改
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
  <div class="roundedBorders" id="content">
    <div class="systemmodify">     
    <h2>渠道修改</h2>
     <p>
     <% using (Html.BeginForm(new { Action = "ChannelModify", Model.channelId }))
     { %>
     <%: Html.ValidationSummary() %>
     </p>
        <label>渠道Id:&nbsp;</label><label><%= Model.channelId %></label><br/>
        <label>渠道名称:&nbsp;</label><%=Html.TextAreaFor(m => Model.channelName)%><br/>        
        <label>分成:&nbsp;</label><%=Html.TextBoxFor(m => Model.proportion)%><br/> 
        <p>
        <input type="submit" value="修改" onclick='this.form.action="<%=Url.Action("ChannelModify", new{ Model.channelId })%>"'/>            
        <input type="submit" value="删除" onclick='this.form.action="<%=Url.Action("ChannelDelete", new{ Model.channelId })%>"'/>            
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("ChannelModifyCancleAction")%>"'/>         
        </p>
     <% } %>
    </div>
 </div>
</asp:Content>
