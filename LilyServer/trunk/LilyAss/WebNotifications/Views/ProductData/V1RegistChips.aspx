<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    1 注册奖励筹码数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>1 注册奖励筹码数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V1RegistChipsAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V1RegistChipsAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV1RegistChipsAction")%>"' />
     <% } %>
     </p>
     <p><label>登陆总人数：</label><%=Html.DisplayText("tbLoginCount")%></p>    
     <p><label>Guest总人数：</label><%=Html.DisplayText("tbGusetCount")%></p>
     <p><label>注册用户数：</label><%=Html.DisplayText("tbRegistCount")%></p>
     <p><label>Guest用户奖励总筹码数：</label><%=Html.DisplayText("tbGuestChips")%></p>
     <p><label>注册用户奖励总筹码数：</label><%=Html.DisplayText("tbRegistChips")%></p>
     <p><label>总计奖励筹码数：</label><%=Html.DisplayText("tbTotalChips")%></p>     
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>