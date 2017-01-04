<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<WebNotifications.Models.configRobotStrategy>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    上传文件
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="roundedBorders right" id="content">
        <div>     
    <h2>上传Android Apk</h2>
        <%=ViewData["Message"]%>
         <% using (Html.BeginForm("UploadAndroidApk", "Poker", FormMethod.Post, new { enctype = "multipart/form-data" }))
         { %>
             <p>
            <input type="file" name="uploadFile"/>
            <input type="submit" value="上传android apk"' />
            </p>
         <% } %><br/>
    </div>
    </div>
</asp:Content>
