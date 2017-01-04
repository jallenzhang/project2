<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    10 角色使用人数
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%: Scripts.Render("~/bundles/switchTabJs")%>
 <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
<h2>10 角色使用人数
</h2>
   <div>
     <p>
     <% using (Html.BeginForm(new { Action = "V10RoleUsedTimesAction" }))
     { %>
        <input type="submit" value="查询" onclick='this.form.action="<%=Url.Action("V10RoleUsedTimesAction")%>"' />
        <input type="submit" value="导出表格" onclick='this.form.action="<%=Url.Action("DownloadV10RoleUsedTimesAction")%>"' />
     <% } %>
     </p>

     <div class="roundedBorders right" id="Div1">
        <div class="tabbed_area">
            <ul class="tabs">
                <li><a href="#" title="content_1" class="tab active">列表</a></li>
                <li><a href="#" title="content_2" class="tab">图表</a></li>
            </ul>

            <div id="content_1" class="tabcontent" style="display: block; ">
        	    <p><label>默认：</label><%=Html.DisplayText("tbRoleType0")%></p>    
                 <p><label>石油大亨：</label><%=Html.DisplayText("tbRoleType1")%></p>    
                 <p><label>饶舌歌星：</label><%=Html.DisplayText("tbRoleType2")%></p>    
                 <p><label>海贼王：</label><%=Html.DisplayText("tbRoleType3")%></p>    
                 <p><label>欧洲王储：</label><%=Html.DisplayText("tbRoleType4")%></p>    
                 <p><label>意大利黑手党千金：</label><%=Html.DisplayText("tbRoleType5")%></p>    
                 <p><label>俄罗斯富商大老婆：</label><%=Html.DisplayText("tbRoleType6")%></p>    
                 <p><label>宋朝官员的姨太太：</label><%=Html.DisplayText("tbRoleType7")%></p>    
                 <p><label>有钱的小萝莉：</label><%=Html.DisplayText("tbRoleType8")%></p>     
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
			                    { "data-file": "%2FProductData%2FGetV10RoleUsedTimesActionChart" }
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