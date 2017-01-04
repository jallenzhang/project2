<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Notification
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h2>测试推送ios</h2>
    </hgroup>
   <%-- <% using (Html.BeginForm(new { Action="ShowCardsAction" }))
       { %>
         <p><input type="submit" value="轮到你出牌了" /></p>         
     <% } %>
     <% using (Html.BeginForm(new { Action = "FriendRequestAction" }))
       { %>
         <p><input type="submit" value="好友邀请" /><label>消息：</label><%=Html.TextBox("tbMsg")%></p>         
     <% } %>
     <% using (Html.BeginForm(new { Action = "ChipsReceivedAction" }))
       { %>
         <p><input type="submit" value="收到筹码" /><label>消息：</label><%=Html.TextBox("tbMsg")%></p>         
     <% } %>
     <% using (Html.BeginForm(new { Action = "FriendVisitAction" }))
       { %>
         <p><input type="submit" value="被邀请串门" /><label>消息：</label><%=Html.TextBox("tbMsg")%></p>         
     <% } %>
     <% using (Html.BeginForm(new { Action = "FirendJoinGameAction" }))
       { %>
         <p><input type="submit" value="参加牌局" /><label>消息：</label><%=Html.TextBox("tbMsg")%></p>         
     <% } %>--%>
     <% using (Html.BeginForm(new { Action = "SystemInfoAction" }))
       { %>
         <p><input type="submit" value="系统消息" /><label>消息：</label><%=Html.TextBox("tbMsg")%></p>         
     <% } %>
</asp:Content>