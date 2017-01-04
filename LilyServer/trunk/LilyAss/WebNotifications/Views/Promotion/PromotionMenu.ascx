<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%: Styles.Render("~/Content/customCss")%>
<%: Scripts.Render("~/bundles/customJs")%>
<div class="bImg" id="sidebar">
    <div class="bInner">
        <span class="bTR"></span><span class="bBL"></span>
        <ul id="side-nav">
            <li class="active"><a class="button" title="Posts" href="#"><strong><span class="title">
                推广列表页面</span> <span class="expand expanded"></span></strong></a>
                <ul style="display: block;">
                    <li><a href="/Promotion/ListParter">合作商列表</a> </li>
                    <li><a href="/Promotion/AddParter">添加合作商</a> </li>
                    <li><a href="/Promotion/GetUrlView">例子：合作商推广用户 URL</a> </li>                    
                </ul>
            </li>
        </ul>
    </div>
</div>
