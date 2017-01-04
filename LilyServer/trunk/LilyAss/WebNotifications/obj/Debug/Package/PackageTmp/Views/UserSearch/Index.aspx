<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    用户查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
    <div>     
    <h2>用户查询</h2>
     <% using (Html.BeginForm(new { Action = "UserSearchAction" }))
     { %>
     <p>
     <label>用户昵称</label><%=Html.TextBox("tbnickname")%> <br />
     <label>用户邮箱</label><%=Html.TextBox("tbmail")%> <br />
     <input type="submit" value="查询" />
     </p>     
     <% } %>     
     <p><label>user-id：</label><%=Html.DisplayText("userid")%></p>    
     <p><label>用户类型：</label><%=Html.DisplayText("usertype")%></p>    
     <p><label>经验值：</label><%=Html.DisplayText("userexp")%></p>  
     <p><label>筹码：</label><%=Html.DisplayText("userchip")%></p>  
     <p><label>总共游戏场数：</label><%=Html.DisplayText("totalgames")%></p>  
     <p><label>赢牌场数：</label><%=Html.DisplayText("wingames")%></p>    
     <p><label>最好牌型：</label><%=Html.DisplayText("besthand")%></p>  
     <p><label>拥有金钱：</label><%=Html.DisplayText("ownmoney")%></p>    
     <p><label>设备类型：</label><%=Html.DisplayText("devicestype")%></p>
   </div>   
   <a href="/DataManager/Index">返回</a>
    </div>
</asp:Content>
