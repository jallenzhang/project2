using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Photon.SocketServer;
using PokerWorld.Game;
using ExitGames.Logging;
using LilyServer.Helper;
using DataPersist;
using System.Collections;

namespace LilyServer
{
    public class StatusTipsMsgHander
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void LoginRequest(OperationRequest operationRequest, SendParameters sendParameters, string userId)
        {
            try
            {
                StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.FriendUp, userId);
            }
            catch (Exception ex) {
                log.Error(ex.ToString());
            }
        }

        public static void BuyItemRequest(OperationRequest operationRequest, SendParameters sendParameters, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return;

                string nickname = UserService.getInstance().QueryUserByUserId(userId).nickname;
                ItemType itemType = (ItemType)operationRequest.Parameters[(byte)LilyOpKey.ItemType];
                int myItemId = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                object[] myparams = new object[] { nickname, myItemId };
                switch (itemType)
                {
                    case ItemType.Room:
                        StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.BuyScence, myparams);
                        break;
                    case ItemType.Chip:
                        StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.BuyChips, myparams);
                        break;
                    case ItemType.Avator:
                        StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.BuyAvator, myparams);
                        break;
                    case ItemType.Jade:
                        StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.KangxiJade, myparams);
                        break;
                    case ItemType.Lineage:
                        StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.KangxiLineage, myparams);
                        break;
                }

                BeVipRequest(operationRequest, sendParameters, userId);
            }
            catch (Exception ex) {
                log.Error(ex.ToString());
            }
        }

        public static void lilyGame_GameEnded(object sender, GameEndEventArgs e)
        {
            LogPrint("Lee test lilyGame_GameEnded called");

            PokerGame pokerGame = sender as PokerGame;
            object[] myparams = null;
            if (pokerGame != null)
            {      
                foreach (PlayerInfo p in pokerGame.Table.Players)
                {
                    if (p.IsRobot) continue;

                    if (p.HandType == PokerWorld.HandEvaluator.Hand.HandTypes.StraightFlush && (p.IsPlaying || p.IsAllIn ))
                    {
                        LogPrint("Lee test take StraightFlush");

                        myparams = new object[] { p.Name };
                        // 皇家同花顺
                        if (p.HandValue == 135004160) 
                        {
                            StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.RoyalStraightFlush, myparams);
                        }   
                        else // 同花顺
                        {                         
                            StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.StraightFlush, myparams);
                        }
                        //p.HandType = PokerWorld.HandEvaluator.Hand.HandTypes.HighCard;
                    }                   
                }
            }
        }

        public static void BeVipRequest(OperationRequest operationRequest, SendParameters sendParameters, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;
            if (!operationRequest.Parameters.ContainsKey((byte)LilyOpKey.IAPMoney)) return;

            var mydata = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            ItemType itemType = (ItemType)operationRequest.Parameters[(byte)LilyOpKey.ItemType];

            int myMoney = (int)operationRequest.Parameters[(byte)LilyOpKey.IAPMoney];
            if ((mydata.Money - myMoney) > 0) return; // be first to vip user
            
            string nickname = UserService.getInstance().QueryUserByUserId(userId).nickname;            
            object[] myparams = new object[] { nickname };
            switch (itemType)
            { 
                case ItemType.Chip:                    
                case ItemType.Jade:
                case ItemType.Lineage:
                    StatusTipsSendMsgHelper.Instance.SendMessage(StatusTipsType.BeVipPlayer, myparams);
                    break;
            }
        }

        public static void LogPrint(string s)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(s);
            }
        }
    }
}
