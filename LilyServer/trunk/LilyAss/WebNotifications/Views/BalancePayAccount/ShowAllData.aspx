<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    平账 总数据
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.RenderPartial("~/Views/DataManager/DataLeftMenu.ascx"); %> 
    <div class="roundedBorders" id="content">
    <h2>总数据</h2>
        <div>      
            <h3>筹码平衡</h3>      
            <p><label>每日赠送筹码总数(+)：</label><%=Html.DisplayText("tbEveryDaySendChips")%></p>    
            <p><label>注册奖励筹码总数(+)：</label><%=Html.DisplayText("tbRegisterSendChips")%></p>    
            <p><label>完成成就奖励筹码总数(+)：</label><%=Html.DisplayText("tbAchivementSendChips")%></p>    
            <p><label>升级奖励筹码总数(+)：</label><%=Html.DisplayText("tbLevelUpSendChips")%></p>    
            <p><label>购买筹码总数(+)：</label><%=Html.DisplayText("tbBuyChipsSendChips")%></p>  
            <p><label>机器人带走筹码总数(+)：</label><%=Html.DisplayText("tbRobotTakenSendChips")%></p>
            <p><label>总共银行支出筹码总数(+)：</label><%=Html.DisplayText("tbAllOutChips")%></p>
            <br/>  
            <p><label>购买礼物扣除筹码总数(-)：</label><%=Html.DisplayText("tbBuyChips")%></p>    
            <p><label>玩家创建游戏扣除筹码总数(-)：</label><%=Html.DisplayText("tbUserCreateGameChips")%></p>  
            <p><label>玩家每局牌局税收扣除筹码总数(-)：</label><%=Html.DisplayText("tbUserTaxChips")%></p>  
            <p><label>世界喊话扣除筹码总数(-)：</label><%=Html.DisplayText("tbBroastCastChips")%></p>  
            <p><label>系统创建游戏扣除筹码总数(-)：</label><%=Html.DisplayText("tbSystemCreateGameChips")%></p>  
            <p><label>系统每局牌局筹码总数(-)：</label><%=Html.DisplayText("tbSystemTaxChips")%></p>   
            <p><label>总共银行收入筹码总数(-)：</label><%=Html.DisplayText("tbAllInChips")%></p>   
            <br/>

            <p><label>玩家购买筹码总数：</label><%=Html.DisplayText("tbUserAllBuyChips")%></p>
            <br/>

            <p><label>玩家目前持有的筹码总数：</label><%=Html.DisplayText("tbUserAllChips")%></p>
            <p><label>机器人目前持有的筹码总数：</label><%=Html.DisplayText("tbBotsAllChips")%></p>
         </div>     
        <a href="/DataManager/Index">返回</a>
    </div>
</asp:Content>
