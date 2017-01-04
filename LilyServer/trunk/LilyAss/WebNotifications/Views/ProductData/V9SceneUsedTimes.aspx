<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    9 场景使用人数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Scripts.Render("~/bundles/switchTabJs")%>
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>9 场景使用人数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V9SceneUsedTimesAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V9SceneUsedTimesAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV9SceneUsedTimesAction")%>"' />
     <% } %>
     </p>
     
     <div class="roundedBorders right" id="Div1">
        <div class="tabbed_area">
            <ul class="tabs">
                <li><a href="#" title="content_1" class="tab active">列表</a></li>
                <li><a href="#" title="content_2" class="tab">图表</a></li>
            </ul>

            <div id="content_1" class="tabcontent" style="display: block; ">
        	    <p><label>高尚富人小区(默认)：</label><%=Html.DisplayText("tbRoomType1")%></p>    
                 <p><label>埃及法老墓室：</label><%=Html.DisplayText("tbRoomType2")%></p>    
                 <p><label>夏威夷风情：</label><%=Html.DisplayText("tbRoomType4")%></p>    
                 <p><label>日本情调桃源：</label><%=Html.DisplayText("tbRoomType8")%></p>    
                 <p><label>西部牛仔酒馆：</label><%=Html.DisplayText("tbRoomType16")%></p>    
                 <p><label>古典浪漫庭院：</label><%=Html.DisplayText("tbRoomType32")%></p>   
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
	                            "/Content/Chart/open-flash-chart-SimplifiedChinese.swf",
			                    "my_chart",
			                    "650",
			                    "400",
			                    "9.0.0",
			                    "expressInstall.swf",
			                    { "data-file": "%2FProductData%2FGetV9SceneUsedTimesActionChart" }
		                    );
	                    }
	                </script>
	                <div id="my_chart"></div>
                </div>
              </div>

        </div>    
     </div>    
     
      
   </div>   
<a href="/DataManager/Index">返回</a> </div>   
</asp:Content>