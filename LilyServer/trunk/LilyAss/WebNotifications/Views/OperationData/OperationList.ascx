<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<li class="inactive"><a class="button" title="Posts" href="#"><strong><span class="title">
    运营数据分析</span> <span class="expand expanded"></span></strong></a>
    <ul style="display: block;">
        <li><a href="/OperationData/V5UserLevelDist">1. 用户等级分布</a> </li>
        <li><a href="/OperationData/V6ActiveName">2. 活动名称</a> </li>
        <li><a href="/OperationData/V7RechargeNum">3. 充值用户数量</a> </li>
        <li><a href="/OperationData/V8SingleRechargeValue">4. 单个用户充值金额</a> </li>
        <li><a href="/OperationData/V9PayPercent">5. 付费率</a> </li>
        <li><a href="/OperationData/V10ARPUValue">6. ARPU值</a> </li>
        <li><a href="/OperationData/V11PayValueTotal">7. 付费总量</a> </li>
        <li><a href="/OperationData/V12ActiveName">8. 单个玩家最高充值数</a> </li>
    </ul>
</li>
