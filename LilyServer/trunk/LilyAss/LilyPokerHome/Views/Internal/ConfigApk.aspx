<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    配置android下载路径
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="roundedBorders" id="content">
        <div>     
          <% using (Html.BeginForm(new { Action = "ConfigApk" }))
            { %>
            <p>
            <label>android 下载路径: </label><%= Html.TextArea("tbUrl") %>
            <input type="submit" value="添加" />
            </p>         
            <% } %>  
       </div>   
    </div>
</asp:Content>
