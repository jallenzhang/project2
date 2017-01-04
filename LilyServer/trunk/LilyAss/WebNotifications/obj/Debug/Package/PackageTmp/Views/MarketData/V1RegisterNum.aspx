<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    注册  人数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Scripts.Render("~/bundles/switchTabJs")%>
    <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
    <div>     
    <h2>注册人数</h2>
     <% using (Html.BeginForm(new { Action = "V1RegisterNumAction" }))
     { %>
         <p>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V1RegisterNumAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV1RegisterNumAction")%>"' />
        </p>
         <p><label>开始时间(e; 2012/08/12):</label><%=Html.TextBox("tbStartDate")%> --- <label>结束时间(e; 2012/09/08):</label><%=Html.TextBox("tbEndDate")%></p>
     <% } %>     

     <div class="roundedBorders right" id="Div1">
        <div class="tabbed_area">
            <ul class="tabs">
                <li><a href="#" title="content_1" class="tab active">列表</a></li>
                <li><a href="#" title="content_2" class="tab">图表</a></li>
            </ul>

            <div id="content_1" class="tabcontent" style="display: block; ">
        	    <% if(Model != null) { %>
                     <% foreach(var item in Model)
                     { %>
                        <p><label>时间: <%= item.Name%></label>
                        <label>累计注册人数: <%= item.Count%></label><label>&nbsp;(累计游客: <%= item.Count2%>)</label>
                        </p>                
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
			                    { "data-file": "%2FMarketData%2FGetRegisterNumChart" }
		                    );
	                    }
	                </script>
	                <div id="my_chart"></div>
                </div>
              </div>

        </div>    
     </div>    

   </div>   
    </div>
</asp:Content>
