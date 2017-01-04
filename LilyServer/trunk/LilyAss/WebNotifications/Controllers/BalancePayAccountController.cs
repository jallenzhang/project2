using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Permission;
using DataPersist;

namespace WebNotifications.Controllers
{
    [Description("平账查询(底下权限可以继承当前权限)")]
    public class BalancePayAccountController : BaseController
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();
        //
        // GET: /BalancePayAccount/

        [Description("平账列表页面")]
        public ActionResult Index()
        {
            return View();
        }

        [Description("平账查询动作")]
        public ActionResult ShowAllData() {
            // 每日赠送筹码总数
            long? v1 = BindToViewData("tbEveryDaySendChips", GetBankOutChips((int)BankActionType.Award));
            // 注册奖励筹码总数
            long? v2 = BindToViewData("tbRegisterSendChips", GetBankOutChips((int)BankActionType.Register));
            // 完成成就奖励筹码总数
            long? v3 = BindToViewData("tbAchivementSendChips", GetBankOutChips((int)BankActionType.Achievement));
            // 升级奖励筹码总数
            long? v4 = BindToViewData("tbLevelUpSendChips", GetBankOutChips((int)BankActionType.LevelUp));
            // 购买筹码总数
            long? v5 = BindToViewData("tbBuyChipsSendChips", GetBankOutChips((int)BankActionType.BuyChips));
            // 机器人带走筹码总数
            long? v6 = BindToViewData("tbRobotTakenSendChips", GetBankOutChips((int)BankActionType.RobotTaken));
            // 总共银行支出筹码总数
            BindToViewData("tbAllOutChips", SumLong(v1, v2, v3, v4, v5, v6));

            // 购买礼物扣除筹码总数
            long? i1 = BindToViewData("tbBuyChips", GetBankInChips((int)BankActionType.BuyGift));
            // 玩家创建游戏扣除筹码总数
            long? i2 = BindToViewData("tbUserCreateGameChips", GetBankInChips((int)BankActionType.CreatGame));
            // 玩家每局牌局税收扣除筹码总数
            long? i3 = BindToViewData("tbUserTaxChips", GetBankInChips((int)BankActionType.GameTax));
            // 世界喊话扣除筹码总数
            long? i4 = BindToViewData("tbBroastCastChips", GetBankInChips((int)BankActionType.Broadcast));
            // 系统创建游戏扣除筹码总数
            long? i5 = BindToViewData("tbSystemCreateGameChips", GetBankInChips((int)BankActionType.CreatGameSystem));
            // 系统每局牌局筹码总数
            long? i6 = BindToViewData("tbSystemTaxChips", GetBankInChips((int)BankActionType.GameTaxSystem));
            // 总共银行支出筹码总数
            BindToViewData("tbAllInChips", SumLong(i1, i2, i3, i4, i5, i6));

            //玩家购买筹码总数
            BindToViewData("tbUserAllBuyChips", GetAllUserBuyChips());  
            // 玩家目前持有的筹码总数
            BindToViewData("tbUserAllChips", GetUserAllChips());        
            // 机器人目前持有的筹码总数
            BindToViewData("tbBotsAllChips", GetBotsAllChips());          

            return View();
        }

        private long? BindToViewData(string tag, long? result) {
            ViewData[tag] = CodeHelper.GetNumberOrZero(result);

            return result;
        }

        private long? GetBankOutChips(int types) {
            var result = (from m in dataContext.banks
                          where m.optype == types
                          select m.bankout).Sum();
            return result;
        }

        private long? GetBankInChips(int types)
        {
            var result = (from m in dataContext.banks
                          where m.optype == types
                          select m.bankin).Sum();
            return result;
        }

        private long SumLong(params long?[] args) {
            long res = 0;
            foreach(var item in args){
                res += CodeHelper.GetNumberOrZero(item);
            }
            return res;
        }

        private long? GetUserAllChips() {
            var result = (from m in dataContext.users
                          select m.chips).Sum();
            return result;
        }

        private long? GetBotsAllChips()
        {
            var result = (from m in dataContext.bots
                          select m.chips).Sum();
            return result;
        }

        private long? GetAllUserBuyChips() { 
            int type = (int)ItemType.Chip;
            var result = (from m in dataContext.userpayments
                          where m.type == type
                          select m.itemid).Sum();

            return result;
        }
    }
}
