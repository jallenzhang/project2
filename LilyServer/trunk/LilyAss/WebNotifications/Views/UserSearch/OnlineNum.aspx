<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<WebNotifications.Models.Custom.OnlineCaculateObject>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    在线实时人数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Scripts.Render("~/bundles/switchTabJs")%>
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
  <h2>在线实时人数</h2>
    <% using (Html.BeginForm(new { Action = "OnlineNum" }))
     { %>
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("OnlineNumDownload")%>"' /> 
        <p><label>开始时间:&nbsp;</label><%=Html.TextBox("tbStartTime")%>
        <label>结束时间:&nbsp;</label><%=Html.TextBox("tbEndTime")%>
        </p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("OnlineNum")%>"' />               
     <% } %>
    <div class="roundedBorders right" id="content">
        <div class="tabbed_area">
            <ul class="tabs">
                <li><a href="#" title="content_1" class="tab active">列表</a></li>
                <li><a href="#" title="content_2" class="tab">图表</a></li>
            </ul>

            <div id="content_1" class="tabcontent" style="display: block; ">
        	    <% if( null != Model) { %>
                <% foreach (var m in Model) { %>
                        <label>时间: <%= m.CurrentDate %></label>
                        <label>人数: <%= m.UserNum %></label>
                        <br/>         
                <% } %>
                <% } %>
            </div>

            <div id="content_2" class="tabcontent" style="display: none; ">
        	    <div class="scroll">     
                   <%: Scripts.Render("~/bundles/openchartJs")%>    
	                <script type="text/javascript">
	                    var content_flag = "content_2";
	                    var is_first = true;
	                    function printFlash() {
	                        swfobject.embedSWF(
	                            //"http://teethgrinder.co.uk/open-flash-chart/open-flash-chart.swf",
	                            "/Content/Chart/open-flash-chart.swf",                                
			                    "my_chart",
			                    "650",
			                    "400",
			                    "9.0.0",
			                    "expressInstall.swf",
			                    { "data-file": "%2FUserSearch%2FGetOnlinePeopleChart" }
		                    );	                       
	                    }
	                </script>
	                <div id="my_chart"></div>
                </div>
              </div>

        </div>    
     </div>    
</asp:Content>
