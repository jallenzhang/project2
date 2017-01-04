<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<li class="inactive"><a class="button" title="Posts" href="#"><strong><span class="title">
    产品数据分析</span> <span class="expand expanded"></span></strong></a>
    <ul style="display: block;">
        <li><a href="/ProductData/V1RegistChips">1. 注册奖励筹码数</a> </li>
        <li><a href="/ProductData/V2EveryChips">2. 每日奖励筹码数</a> </li>
        <li><a href="/ProductData/V3UserBuyChips">3. 用户购买的筹码数量</a> </li>
        <li><a href="/ProductData/V4ActiveSendChips">4. 活动赠送的筹码量</a> </li>
        <li><a href="/ProductData/V5CreateRoomChips">5. 用户建房筹码数</a> </li>
        <li><a href="/ProductData/V6TableTax">6. 牌桌税收</a> </li>
        <li><a href="/ProductData/V7CostOfPalyerBuyChips">7. 玩家购买筹码花费</a> </li>
        <li><a href="/ProductData/V8GiftsBuyTimes">8. 各类礼物被购买次数</a> </li>
        <li><a href="/ProductData/V9SceneUsedTimes">9. 场景使用人数</a> </li>
        <li><a href="/ProductData/V10RoleUsedTimes">10. 角色使用人数</a> </li>
        <%--<li><a href="/ProductData/V11LanguageUsedTimes">11. 牌局默认语句使用次数统计</a> </li>--%>
        <li><a href="/ProductData/V12WorldCallUsedTimes">11. 世界喊话功能使用次数</a> </li>
        <li><a href="#">12. 语音系统使用次数</a> </li>
        <li><a href="/ProductData/V23TodayRoundTimesAndDate">13. 今日对局数和对局时间</a> </li>
    </ul>
</li>
