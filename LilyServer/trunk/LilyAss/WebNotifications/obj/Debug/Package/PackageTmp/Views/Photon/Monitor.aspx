<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Photon 监视
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>Photon 监视</h2>
<iframe src="<%: ViewBag.Message %>" scrolling="auto" width="100%" height="1800px"></iframe> 
</asp:Content>
