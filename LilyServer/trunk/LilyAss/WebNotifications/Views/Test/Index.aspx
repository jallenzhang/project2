<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    测试
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>测试</h2>

    <%: Scripts.Render("~/bundles/openchartJs")%>    
	<script type="text/javascript">
	    swfobject.embedSWF(
			"/Content/Chart/open-flash-chart.swf",
			"my_chart",
			"550",
			"200",
			"9.0.0",
			"expressInstall.swf",
			{ "data-file": "/Chart/MyData" }
		);
	</script>
	<div id="my_chart"></div>

</asp:Content>
