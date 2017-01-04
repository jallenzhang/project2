<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WebNotifications.Models.Channel>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    渠道增加
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("~/Views/Channel/ChannelMenu.ascx"); %> 
  <div class="roundedBorders" id="content">
    <div class="botmodify">     
    <h2>渠道增加</h2>
    <%: Html.ValidationSummary() %>
     <% using (Html.BeginForm(new { Action = "ChannelAdd" }))
     { %>
        <label>渠道Id:&nbsp;</label><%=Html.TextBoxFor(m => Model.channelId)%><br/>
        <label>渠道名称:&nbsp;</label><%=Html.TextAreaFor(m => Model.channelName)%><br/>                
        <label>分成:&nbsp;</label><%=Html.TextBoxFor(m => Model.proportion)%><br/>        
        <p>
        <input type="submit" value="新增" onclick='this.form.action="<%=Url.Action("ChannelAdd")%>"' />
        <input type="submit" value="取消" onclick='this.form.action="<%=Url.Action("ChannelAddCancleAction")%>"' />
        </p>
     <% } %>
    </div>
</div>
</asp:Content>
