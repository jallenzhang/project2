<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<WebNotifications.Models.Custom.BuyItemObject>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    付费道具购买详情
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
  <h2>付费道具购买详情</h2>
    <div class="roundedBorders right" id="content">
      <% using (Html.BeginForm(new { Action = "BuyItemInfoDownload" }))
     { %>
        <input type="submit" value="导出表格" />         
     <% } %>
        <div class="scroll">       
            <table>
            <tr>
                <th>名称</th>
                <th>购买次数</th>
                <th>购买人数</th>   
            </tr>
            <% foreach (var m in Model)
                { %>
                <tr>
                    <td><%= m.name %></td>
                    <td><%= m.buyTimes %></td>
                    <td><%= m.peopleCount %></td>       
                </tr>
            <% } %>
            </table>
        </div>
   </div>      
</asp:Content>
