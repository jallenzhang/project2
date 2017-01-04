using System.Linq;
using System.Collections.Generic;
using System;

using PokerWorld.Game;
using Lite;
using DataPersist;
using Photon.SocketServer;
using ExitGames.Logging;

namespace LilyServer
{
    public class PokerGameCareer:PokerGameUsers
    {
        public event Action<byte[]> PlayerRankChanged = delegate { };
        public event EventHandler PokerGameCareer_EverythingEnded = delegate { };

        private static readonly int Max_Players = 5;
        public GameGrade GameGrade { get; set; }
        private List<PlayerInfo> obsoletePlayer = new List<PlayerInfo>();

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override bool IsRunning
        {
            get { return Table.TotalRounds > 0||Table.Players.Count>4; }
        }

        public PokerGameCareer(AbstractDealer dealer, Room room, string tableName, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon,GameGrade gamegrade)
            : base(dealer,room,new TableInfo(tableName,1000,5,10), wtaPlayerAction, wtaBoardDealed, wtaPotWon,GameType.Career)
        {
            m_WaitingTimeAfterGameEnd = 3;
            GameGrade = gamegrade;
        }


        /// <summary>
        /// 比赛场必须凑齐5个人才开始，且盲注随比赛局数的增加而改变
        /// </summary>
        protected override void TryToBegin()
        {
            lock (lockobj)
            {
                

                if (!IsRunning&& m_Table.Players.Count < 5)
                {
                    if (m_Table.Players.Count == 4)
                        room.ExecutionFiber.Schedule(() => RobotService.Singleton.AddRobotInGame((LilyGame)room), 30000);
                    //if (m_Table.Players.Count == 1)
                    //    RobotService.Singleton.Request(room);
                    return;
                }
                foreach (PlayerInfo p in m_Table.Players)
                {

                    if (p.MoneySafeAmnt ==0)
                    {
                        LeaveGame(p, p.IsRobot ? PlayerLeaveType.Kick : PlayerLeaveType.Stand);
                        continue;
                    }

                    if (p.CanPlay)
                    {
                        p.IsPlaying = true;
                        p.HasPlayed = true;
                        p.exp = ExpType.Normal;
                        p.IsShowingCards = false;
                        p.MoneyBetAmnt = 0;
                        p.Greedy = false;
                    }
                    else
                    {
                        p.IsPlaying = false;
                        p.IsAllIn = false;
                    }
                }
                if (m_Table.NbPlaying() > 1)
                {
                    m_WaitingTimeAfterGameEnd = 7;
                    m_Table.BigBlindAmnt = GetBlind(m_Table.TotalRounds);
                    m_Table.SmallBlindAmnt = m_Table.BigBlindAmnt/2;

                    m_Table.InitTable();
                    m_Dealer.FreshDeck();
                    NextState(TypeState.WaitForBlinds);
                    m_Table.TotalRounds++;

                    GameStart();

                    PlayBlindMoney();
                    //GameBlindNeeded(this, new EventArgs()); will be happen atfer sb&bb
                }
                else
                {
                    //m_Table.NoSeatDealer = -1;
                    m_Table.NoSeatCurrPlayer = -1;
                    m_Table.NoSeatSmallBlind = -1;
                    m_Table.NoSeatSmallBlind = -1;
                }
            }     
        }

        private int GetBlind(int tableRound)
        {
            try
            {
                GameBlind[] blinds = XmlResources.AllGameBlind;
                if (tableRound+1>blinds.Count())
                {
                    var lastOrDefault = blinds.LastOrDefault();
                    if (lastOrDefault != null) return lastOrDefault.BigBlind;
                }else
                {
                    return blinds[tableRound].BigBlind;
                }
                
            }
            catch
            {
                //Console.WriteLine(e);
                //return 1000;
            }
            return 1000;
        }

        protected override void BeforeJoin(PlayerInfo p)
        {
            p.MoneySafeAmnt = 100000;
            p.MoneyInitAmnt = 100000;
            p.NoSeat = -1;
        }

        protected override void EndRound(long taxAmount)
        {
            base.EndRound(taxAmount);

            LogPrint("Lee test Game Career EndRound");

            //比赛名次逻辑
            SetPlayInfoRankNum();

            GiveAwardToPlayer();

            SendRankingEvent();            
        }

        private void SetPlayInfoRankNum()
        { 
            if (this.Table == null) return;
            var myPlayers = this.Table.Players;

            myPlayers.Sort((x, y) => y.MoneySafeAmnt.CompareTo(x.MoneySafeAmnt));            
            
            int rankCnt = 1;
            for (int i = 0; i < myPlayers.Count; i++)
            {
                //if (!myPlayers[i].IsPlaying && !myPlayers[i].IsAllIn) continue;
                if (!ShouldToRank(myPlayers[i])) continue;

                if (i == 0) 
                {
                    myPlayers[i].Ranking = rankCnt;
                    rankCnt++;
                }
                else {
                    if (myPlayers[i].MoneySafeAmnt == myPlayers[i - 1].MoneySafeAmnt)
                    {
                        myPlayers[i].Ranking = myPlayers[i - 1].Ranking;
                    }
                    else {
                        myPlayers[i].Ranking = rankCnt;
                        rankCnt++;   
                    }  
                }
            }
            SaveObsoletePlayer(); // 将淘汰的选手保存起来
        }

        private void GiveAwardToPlayer() {
            if (!IsGameEnded()) return;
            if (this.Table == null) return;

            GameGrade grade = this.GameGrade;            
            int[] rankNum = new int[]{ 0, 0 };
            int[] rankMoney = new int[] { grade.First, grade.Second };
            for (int i = 0; i < rankNum.Length; i++)
			{
                rankNum[i] = this.Table.Players.Count(m => m.Ranking == (i+1));
                rankMoney[i] = rankNum[i] != 0 ? (rankMoney[i] / rankNum[i]) : 0;
			}

            foreach (var item in this.Table.Players)
            {
                if (item.IsRobot) return;
                if (item.Ranking >= 1 && item.Ranking <= rankNum.Length )
                {
                    int my_awards = rankMoney[item.Ranking - 1];
                    UserData user = item as UserData;
                    UserService.getInstance().ChangeUserChips(user.UserId, my_awards);

                    LogPrint(string.Format("Lee test User name {0} get awards chips {1}", user.UserName, my_awards));
                }               
            }

            ClearCurrentPokerGame();
            PokerGameCareer_EverythingEnded(this, new EventArgs());
        }

        private void SendRankingEvent()
        {
            if (this.Table == null) return;

            LogPrint("Lee test SendRankingEvent");

            byte[] ranklist = new byte[PokerGameCareer.Max_Players];
            for (int i = 0; i < this.Table.Players.Count; i++)
			{
                int seatNo = this.Table.Players[i].NoSeat;
                if(seatNo != -1)
                    ranklist[seatNo] = (byte)this.Table.Players[i].Ranking;
			}

            PlayerRankChanged(ranklist);            
        }

        private bool ShouldToRank(PlayerInfo player)
        {            
            foreach (var item in this.obsoletePlayer)
	        {
		        if(item.Name == player.Name) return false;
	        }
            return true;
        }

        private void SaveObsoletePlayer()
        {
            if (this.Table == null) return;

            foreach (var item in this.Table.Players)
            {                
                if (item.MoneySafeAmnt == 0)
                {
                    this.obsoletePlayer.Add(item);
                }
            }
        }

        private bool IsGameEnded()
        {
            if (this.Table == null) return true;

            int cnt = this.Table.Players.Count;
            if(cnt == 0 || cnt == 1) return true;

            // two or more people, but another money is 0
            if (cnt >= 2) {
                int leftWin = 0;
                foreach (var item in this.Table.Players)
                {
                    if (item.MoneySafeAmnt != 0)
                        leftWin++;                        
                }
                if (leftWin == 1) return true;
            }

            return false;
        }

        private void ClearCurrentPokerGame()
        {
            this.obsoletePlayer.Clear();
        }

        private void LogPrint(string s)
        {
            if (Log.IsDebugEnabled) {
                Log.Info(s);
            }
        }
    }
}
