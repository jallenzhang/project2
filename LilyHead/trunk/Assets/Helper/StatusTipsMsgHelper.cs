using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class StatusTipsMsgHelper
{
    private static Dictionary<int, string> myScene = new Dictionary<int, string>();
    private static Dictionary<int, string> myAvator = new Dictionary<int, string>();
    private static Dictionary<int, string> myChips = new Dictionary<int, string>();
    private static Dictionary<int, string> myJade = new Dictionary<int, string>();
    private static Dictionary<int, string> myLineage = new Dictionary<int, string>();    

    public static string GetStatusTipsMsg(StatusTipsType myType, object[] myParams)
    {
        GlobalManager.Log("Lee test Enter GetStatusTipsMsg");     
        string myMsgFormat = string.Empty;
        string myMsg = string.Empty;        
        switch (myType)
        {
            case StatusTipsType.FriendUp:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_FRIEND_UP");
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
            case StatusTipsType.BuyChips:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_BUY_CHIPS");
                myMsg = string.Format(myMsgFormat, myParams[0], GetChipsString((int)myParams[1]));
                break;
            case StatusTipsType.BuyScence:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_BUY_SCENE");
                myMsg = string.Format(myMsgFormat, myParams[0], GetSceneString((int)myParams[1]));
                break;            
            case StatusTipsType.BuyAvator:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_BUY_AVATOR");
                myMsg = string.Format(myMsgFormat, myParams[0], GetAvatorString((int)myParams[1]));
                break;
            case StatusTipsType.BeVipPlayer:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_BE_VIP");
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
            case StatusTipsType.KangxiJade:
                myMsgFormat = GetJadeFormatStr((int)myParams[1]);
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
            case StatusTipsType.KangxiLineage:
                myMsgFormat = GetLineageFormatStr((int)myParams[1]);
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
            case StatusTipsType.StraightFlush:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_STRAIGHT_FLUSH");
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
            case StatusTipsType.RoyalStraightFlush:
                myMsgFormat = LocalizeHelper.Translate("STATUS_TIPS_MSG_ROYAL_STRAIGHT_FLUSH");
                myMsg = string.Format(myMsgFormat, myParams[0]);
                break;
        }

        GlobalManager.Log(myMsg);  

        return myMsg;
    }

    private static string GetSceneString(int id)
    {
        InitScene();
        string res = string.Empty;
        if (myScene.ContainsKey(id)) { 
            res = LocalizeHelper.Translate(myScene[id]);
        }

        return res;
    }

    private static string GetAvatorString(int id)
    {
        InitAvator();
        string res = string.Empty;
        if (myAvator.ContainsKey(id))
        {
            res = LocalizeHelper.Translate(myAvator[id]);
        }

        return res;
    }

    private static string GetChipsString(int id)
    {
        InitChips();
        string res = string.Empty;
        if (myChips.ContainsKey(id))
        {
            res = LocalizeHelper.Translate(myChips[id]);
        }

        return res;
    }

    private static string GetJadeFormatStr(int id) {
        InitJade();
        string res = string.Empty;
        if (myJade.ContainsKey(id))
        {
            res = LocalizeHelper.Translate(myJade[id]);
        }

        return res;
    }

    private static string GetLineageFormatStr(int id)
    {
        InitLineage();
        string res = string.Empty;
        if (myLineage.ContainsKey(id))
        {
            res = LocalizeHelper.Translate(myLineage[id]);
        }

        return res;
    }

    private static void InitScene()
    {
        if (myScene.Count == 0)
        {
            myScene.Add(1, "SCENE_COMMUNITY");
            myScene.Add(2, "SCENE_EGYPT");
            myScene.Add(4, "SCENE_HAWAII");
            myScene.Add(8, "SCENE_JAPAN");
            myScene.Add(16, "SCENE_WEST");
            myScene.Add(32, "SCENE_CLASSIC");
            //myScene.Add(64, "");
            //myScene.Add(128, "");
            //myScene.Add(256, "");
        }
    }

    private static void InitAvator()
    {
        if (myAvator.Count == 0)
        {
            myAvator.Add(1, "NICKNAME_OIL");
            myAvator.Add(2, "NICKNAME_BLACK");
            myAvator.Add(3, "NICKNAME_PIRATE");
            myAvator.Add(4, "NICKNAME_PRINCE");
            myAvator.Add(5, "NICKNAME_QIANJIN");
            myAvator.Add(6, "NICKNAME_WEALTHYWIFE");
            myAvator.Add(7, "NICKNAME_SONG_WOMEN");
            myAvator.Add(8, "NICKNAME_LOLI");
            //myAvator.Add(9, "");
            //myAvator.Add(10, "");
            //myAvator.Add(11, "");
            //myAvator.Add(12, "");
        }
    }

    private static void InitChips()
    {
        if (myChips.Count == 0)
        {
            myChips.Add(60000, "BUY_CHIPS_INDEX_60K_TIP");
            myChips.Add(120000, "BUY_CHIPS_INDEX_120K_TIP");
            myChips.Add(250000, "BUY_CHIPS_INDEX_250K_TIP");
            myChips.Add(300000, "BUY_CHIPS_INDEX_300K_TIP");
            myChips.Add(700000, "BUY_CHIPS_INDEX_700K_TIP");
            myChips.Add(3400000, "BUY_CHIPS_INDEX_3.4M_TIP");
            myChips.Add(6800000, "BUY_CHIPS_INDEX_6.8M_TIP");
            myChips.Add(36000000, "BUY_CHIPS_INDEX_36M_TIP");
            myChips.Add(72000000, "BUY_CHIPS_INDEX_72M_TIP");
        }
    }

    private static void InitJade() 
    {
        if (myJade.Count == 0)
        {
            myJade.Add(1, "STATUS_TIPS_MSG_BUY_JADE_MONTH");
            myJade.Add(4, "STATUS_TIPS_MSG_BUY_JADE_YEAR");
        }
    }

    private static void InitLineage()
    {
        if (myLineage.Count == 0)
        {
            myLineage.Add(1, "STATUS_TIPS_MSG_BUY_LINEAGE_MONTH");
            myLineage.Add(4, "STATUS_TIPS_MSG_BUY_LINEAGE_YEAR");
        }
    }
}

