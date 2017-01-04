using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPersist;
using PokerWorld.Game;
using LilyServer.Helper;
using System.Threading;
using ExitGames.Logging;
using Lite.Messages;
using Lite;
using LilyServer.Model;
using DataPersist.CardGame;
using PokerWorld.HandEvaluator;

namespace LilyServer
{
    public class PokerGameUsers : PokerGame
    {

        protected static Dictionary<long, int> robotChips = new Dictionary<long, int>(){
                                                                                                    {5000,9},
                                                                                                    {10000,12},
                                                                                                    {15000,12},
                                                                                                    {20000,14},
                                                                                                    {25000,14},
                                                                                                    {30000,14},
                                                                                                    {35000,16},
                                                                                                    {40000,16},
                                                                                                    {45000,18},
                                                                                                    {50000,21},
                                                                                                    {100000,21},
                                                                                                    {150000,23},
                                                                                                    {200000,23},
                                                                                                    {250000,25},
                                                                                                    {300000,28},
                                                                                                    {350000,28},
                                                                                                    {400000,30},
                                                                                                    {450000,30},
                                                                                                    {500000,35}};

        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public List<PlayerAction> HistoryActions
        {
            get;
            set;
        }

        private int StepCount { get; set; }
        private Room room;

        public int AddAction(PlayerAction p)
        {
            if (this.HistoryActions==null)
            {
                this.HistoryActions = new List<PlayerAction>();
            }
            this.HistoryActions.Add(p);
            this.StepCount++;
            return this.StepCount;
        }

        public int GetActionId() {
            
            return this.StepCount;
        }


        public List<PlayerAction> getHistoricActions(int step)
        {
            return HistoryActions.FindAll(ha=>ha.Id>step);
        }

        public List<PlayerAction> getHistoricActions(int step,int tableround)
        {
            return HistoryActions.FindAll(ha => ha.Id > step &&ha.TableRound.CompareTo(tableround)==0);
        }


        public PokerGameUsers()
            : base()
        {
        }
        public PokerGameUsers(AbstractDealer dealer) :
            base(dealer)
        {
        }

        public PokerGameUsers(Room room,TableInfo table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base(new LilyTestDealer(),table,  wtaPlayerAction,  wtaBoardDealed,  wtaPotWon)
        {
            //this.Table.NoSeatDealer = 0;
            this.room = room;
            this.RobotActionNeeded += new EventHandler<PlayerInfoEventArgs>(PokerGameUsers_RobotActionNeeded);            
        }

        public PokerGameUsers(AbstractDealer dealer,Room room, TableInfo table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base(dealer,table, wtaPlayerAction, wtaBoardDealed, wtaPotWon)
        {
            this.room = room;
            this.RobotActionNeeded += new EventHandler<PlayerInfoEventArgs>(PokerGameUsers_RobotActionNeeded);  
        }


        //public void PlayBlindMoney() {

        //    TableInfo t = this.Table;
        //    if (t.NbPlaying() < 2)
        //        return;
        //    if (this.State != TypeState.WaitForBlinds)
        //        return;
        //    int totalp = t.Players.Count;
        //    int small = t.NoSeatSmallBlind;
        //    int big = t.NoSeatBigBlind;
        //    if (small >= totalp || big >= totalp || t.GetPlayer(small) == null || t.GetPlayer(big) == null)
        //        this.Table.PlaceButtons();
        //    this.PlayMoney(t.GetPlayer(t.NoSeatSmallBlind),t.SmallBlindAmnt);            
        //    this.PlayMoney(t.GetPlayer(t.NoSeatBigBlind), t.BigBlindAmnt);
        //    //Log.Info("##### SmalBlind " + t.GetPlayer(t.NoSeatSmallBlind).Name +" "+t.SmallBlindAmnt);
        //    //Log.Info("##### BigBlind " + t.GetPlayer(t.NoSeatBigBlind).Name + " " + t.BigBlindAmnt);
        //    //Log.Info("##### blindneeded " + t.TotalBlindNeeded);
        //}


        public TableInfo CareerTable
        {
            get { return (TableInfo)m_Table; }
        }

        public void WaitABit(int waitingTime)
        {
            Thread.Sleep(waitingTime*1000);
        }

        void PokerGameUsers_RobotActionNeeded(object sender, PlayerInfoEventArgs e)
        {
            room.EnqueueMessage(new RoomMessage((byte)LilyMessageCode.RobotAction,e.Player));
        }

        public void RobotLogic(PlayerInfo player)
        {

            try
            {

                HandTypes currentHand =//this.Table.Round==TypeRound.Preflop?
                                             //HandStrengthHelper.GetHandStrength(player.Cards)
                                             HandStrengthHelper.GetHandStrength(player.Cards, this.Table.Cards);

                //Log.Info("@@@@@@@@@@@  bot "+e.Player.Name+" hand:" + e.Player.Cards[0].ToString() + " " + e.Player.Cards[1].ToString()+"  "+currentHand.ToString());



                List<configRobotStrategy> stragegy = ConfigurationHelp.getRobotStrategy();


                configRobotStrategy crs = stragegy.Find(r => r.strategyid == (int)currentHand && r.typeround == (int)this.Table.Round);


                int minthinktime = (crs.delaymin ?? 2);
                int maxthinktime = (crs.delaymax ?? 4);
                int waitingTime = RobotHelper.RandomRange(minthinktime, maxthinktime);

                //WaitABit(waitingTime);
                if (isFastForward)
                    this.doAction(crs, player);
                else
                    TimerHelper.SetTimeout(waitingTime*1000,()=>this.doAction(crs,player));
                //doAction(crs,player);

            }
            catch {
                this.PlayMoney(player, -1);
            }
        }
        

        private void doAction(configRobotStrategy crs,PlayerInfo p) {

            int rate = RobotHelper.RandomRange(0, 99);
            long initChip = p.MoneyInitAmnt;
            int cheatrate = RobotHelper.RandomRange(1, 100);
            int cheatpoint = 0;
            bool willcheat = false;
            foreach (KeyValuePair<long, int> keyvalue in robotChips)
            {
                if (initChip > keyvalue.Key)
                    cheatpoint = keyvalue.Value;
                else
                    break;
            }          

            if (cheatrate<=cheatpoint)
            {
                willcheat = true;
                //p.Greedy = true;
            }
            long needAmnt = 0;            
            //开始淫荡了
            if (willcheat || p.Greedy)
            {
                int cheat = RobotHelper.RandomRange(0, 99);
                if (cheat < 70 || p.Greedy)
                {
                    
                    p.Greedy = true;
                    //for (int i = 0; i < pcount; i++)
                    //{
                    //     PlayerInfo p=
                    //}

                    bool heihei = true;
                    if (p.Greedy)
                    {
                        string boardCards = string.Join(" ", m_Dealer.m_Cheat.Select(r => r.ToString()));
                        uint myhandvalue = EvaluateCards(p.Cards, boardCards);
                        int pcount = Table.Players.Count;
                        foreach (PlayerInfo player in Table.Players)
                        {
                            if (player.IsRobot)
                                continue;
                            if (player.IsPlaying || player.IsAllIn)
                            {
                                uint phand = EvaluateCards(player.Cards, boardCards);
                                if (myhandvalue < phand)
                                {
                                    heihei = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (!heihei)
                        needAmnt = -1;
                    else
                    {

                        List<configRobotStrategy> stragegy = ConfigurationHelp.getRobotStrategy();
                        configRobotStrategy action4 = stragegy.Find(r => r.strategyid == (int)HandTypes.StraightFlush && r.typeround == (int)this.Table.Round);
                        int raseparam = RobotHelper.RandomRange(-25, 0);
                        int newrate = RobotHelper.RandomRange(0, 100);
                        if (newrate <= action4.callratio)
                        {
                            needAmnt = Table.CallAmnt(p);
                        }
                        else
                        {
                            int bigBlind = this.Table.BigBlindAmnt;
                            long higherBet = this.Table.HigherBet;
                            long pbet = higherBet > 0 ? higherBet : bigBlind;
                            int pm = (action4.rasiea ?? 0) + raseparam;
                            long safeAmnt = getMaxAllinValue(p);//p.MoneyAmnt;
                            long minAmnt = 0;
                            if ((2 * (higherBet - safeAmnt) + p.MoneyBetAmnt) == 0)
                                minAmnt = bigBlind;
                            else
                                minAmnt = (2 * (higherBet - p.MoneyBetAmnt) + p.MoneyBetAmnt);

                            int slider = RobotHelper.RandomRange(1, 100) + pm;
                            needAmnt = Math.Max(safeAmnt * slider / 100 + minAmnt, this.Table.CallAmnt(p));
                        }

                    }

                }
                else {
                    needAmnt = actionAmnt(crs, p, rate);
                }
            }
            else {
                needAmnt = actionAmnt(crs,p,rate);
            }
            this.PlayMoney(p, needAmnt);
        }

        private long getMaxAllinValue(PlayerInfo playerinforitem)
        {
            TableInfo info=this.Table;
            if (info != null)
            {
                //PlayerInfo playerinforitem = info.GetPlayer(noSeat);
                if (playerinforitem == null)
                    return 0;
                long max = 0;

                foreach (PlayerInfo item in info.Players)
                {
                    if (max < (item.MoneySafeAmnt + item.MoneyBetAmnt) && item.NoSeat != playerinforitem.NoSeat)
                    {
                        if (item.IsAllIn == true || item.IsPlaying == true)
                            max = (item.MoneySafeAmnt + item.MoneyBetAmnt);
                    }
                }

                if (max >= (playerinforitem.MoneySafeAmnt + playerinforitem.MoneyBetAmnt))
                    max = playerinforitem.MoneySafeAmnt;
                else
                    max = max - playerinforitem.MoneyBetAmnt;

                return max;
            }
            return 0;
        }

        private long actionAmnt(configRobotStrategy crs, PlayerInfo p,int rate)
        { 
            long needAmnt=0;
                    if (rate < crs.foldratio)
                    {
                        //进入弃牌或观望操作
                        if (this.Round == TypeRound.Preflop)
                        {
                            //this.PlayMoney(p, -1);
                            needAmnt = -1;
                        }
                        else
                        {
                            if (this.Table.CanCheck(p))
                            {
                                //this.PlayMoney(p, 0);
                                needAmnt = 0;
                            }
                            else
                            {
                                //this.PlayMoney(p, -1);
                                needAmnt = -1;
                            }
                        }
                        //return;
                    }
                    else if (rate >= crs.foldratio && rate < crs.foldratio + crs.callratio)
                    {
                        //跟注操作
                        needAmnt = Table.CallAmnt(p);
                    }
                    else
                    {
                        //加注操作
                        int bigBlind = this.Table.BigBlindAmnt;
                        long higherBet = this.Table.HigherBet;
                        //long pbet = higherBet > 0 ? higherBet : bigBlind;
                        int pm = higherBet - p.MoneyBetAmnt > 0 ? crs.rasieb ?? 0 : crs.rasiea ?? 0;
                        long safeAmnt = p.MoneyAmnt;
                        long minAmnt = 0;


                        if ((2 * (higherBet - safeAmnt) + p.MoneyBetAmnt) == 0)
                            minAmnt = bigBlind;
                        else
                            minAmnt = (2 * (higherBet - p.MoneyBetAmnt) + p.MoneyBetAmnt);

                        
                        int slider = RobotHelper.RandomRange(1, 100) + pm;

                        if (slider<1)
                        {
                            slider = 1;
                        }

                        needAmnt = Math.Max(safeAmnt * slider / 100 + minAmnt, this.Table.CallAmnt(p));

                        if (needAmnt<bigBlind)
                        {
                            needAmnt = bigBlind;
                        }
                        //Log.DebugFormat("####### 机器人加注搞飞机：{0}，加注金额：{1}，slider:{2},pm:{3}",rate,needAmnt,slider,pm);

                    }
            return needAmnt;
        }

        private uint EvaluateCards(GameCard[] pocket, string boardstr)
        { 
            uint handvalue=0;
            if (pocket[0].Id < 0)
                return 0;

            string pocketstr=string.Join(" ", pocket.Select(r => r.ToString()));
            handvalue = new Hand(pocketstr,boardstr).HandValue;

            return handvalue;
        }

        public int[] getGameCards(TypeRound tr) {

            int[] retInt = new int[5];
            switch (tr)
            {
                case TypeRound.Preflop:
                    retInt=new int[5]{-1,-1,-1,-1,-1};
                    break;
                case TypeRound.Flop:
                    {
                        //retInt = m_Dealer.m_Cheat.Select(r=>r.Id).ToArray();
                        GameCard[] fiveCards = m_Dealer.m_Cheat;
                        Table.SetCards(fiveCards[0], fiveCards[1], fiveCards[2], new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null));
                        retInt = Table.Cards.Select(r => r.Id).ToArray();
                        break;
                    }
                case TypeRound.Turn:
                    {
                        GameCard[] fiveCards = m_Dealer.m_Cheat;
                        Table.SetCards(fiveCards[0], fiveCards[1], fiveCards[2], fiveCards[3], new GameCard(GameCardSpecial.Null));
                        retInt = Table.Cards.Select(r => r.Id).ToArray();
                        break;
                    }
                case TypeRound.River:
                    {
                        GameCard[] fiveCards = m_Dealer.m_Cheat;
                        Table.SetCards(fiveCards[0], fiveCards[1], fiveCards[2], fiveCards[3], fiveCards[4]);
                        retInt = Table.Cards.Select(r => r.Id).ToArray();
                        break;
                    }
                default:
                    break;
            }
            return retInt;
        }
    }
}
