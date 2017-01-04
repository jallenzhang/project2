using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EricUtility;
using DataPersist;
using System.Collections;
using System.Linq;

namespace PokerWorld.Game
{
    public abstract class PokerGame : IPokerGame
    {
        // States of the Game in each Round
        public enum TypeRoundState
        {
            Cards,
            Betting,
            Cumul
        }

        public event EventHandler EverythingEnded = delegate { };
        public event EventHandler GameBlindNeeded = delegate { };
        public event EventHandler<GameEndEventArgs> GameEnded = delegate { };
        public event EventHandler GameStarted = delegate { };

        public event EventHandler GameGenerallyUpdated = delegate { };
        public event EventHandler<RoundEventArgs> GameBettingRoundStarted = delegate { };
        public event EventHandler<RoundEventArgs> GameBettingRoundEnded = delegate { };
        public event EventHandler<PlayerInfoEventArgs> PlayerJoined = delegate { };
        public event EventHandler<PlayerInfoEventArgs> PlayerLeaved = delegate { };
        public event EventHandler<HistoricPlayerInfoEventArgs> PlayerActionNeeded = delegate { };
        public event EventHandler<PlayerMoneyChangedEventArgs> PlayerMoneyChanged = delegate { };
        public event EventHandler<PlayerInfoEventArgs> PlayerHoleCardsChanged = delegate { };
        public event EventHandler<PlayersShowCardsArgs> PlayersShowCards = delegate { };


        public event EventHandler<PlayerActionEventArgs> PlayerActionTaken = delegate { };
        public event EventHandler<PotWonEventArgs> PlayerWonPot = delegate { };

        //
        public event EventHandler<PotWonImproveEventArgs> PlayerWonPotImprove = delegate { };
        //robot event 
        public event EventHandler<PlayerInfoEventArgs> RobotActionNeeded = delegate { };
        public event EventHandler DecideWinnersEnded = delegate { };

        public  Action RealPlayerCanJoinNow = delegate { };


        protected readonly TableInfo m_Table; // La table
        protected readonly AbstractDealer m_Dealer; // Dealer

        // WAITING TIME
        protected readonly int m_WaitingTimeAfterPlayerAction; // Attente apres chaque player action (ms)
        protected readonly int m_WaitingTimeAfterBoardDealed; // Attente apres chaque board dealed (ms)
        protected readonly int m_WaitingTimeAfterPotWon; // Attente apres chaque pot won ! (ms)

        // STATES
        protected TypeState m_State; // L'etat global de la game
        protected TypeRoundState m_RoundState; // L'etat de la game pour chaque round


        protected int startdelay=0;//
        protected int m_WaitingTimeAfterGameEnd = 7;

        //牌桌税率
        public double gameTax { get; set; }

        public bool isFastForward { get; set; }
        public TypeRound stopFastForwardFlag { get; set; }

        public GameType GameType { get; set; }


        protected object lockobj = new object();

        public TableInfo Table
        {
            get { return m_Table; }
        }
        public TypeRound Round
        {
            get { return m_Table.Round; }
        }

        public virtual bool IsRunning
        {
            get { 
                return m_State != TypeState.End;
            }
        }

        public TypeState State
        {
            get { return m_State; }
        }

        public string Encode
        {
            get
            {
                // 0 : Assume que les game sont en real money (1)
                // 1 : Assume que c'est tlt du Texas Hold'em (0)
                // 2 : Assume que c'Est tlt des Ring game (0)
                // 3 : Assume que c'Est tlt du NoLimit (0)
                // 4 : GameRound (0,1,2,3)
                return string.Format("{0}{1}{2}{3}{4}",1,0,0,0,(int)m_Table.Round);
            }
        }

        public PokerGame()
            : this(new RandomDealer())
        {
        }
        public PokerGame(AbstractDealer dealer) :
            this(new RandomDealer(), new TableInfo(), 0, 0, 0)
        {
        }

        public PokerGame(TableInfo table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : this(new RandomDealer(), table, wtaPlayerAction, wtaBoardDealed, wtaPotWon)
        {
        }

        public PokerGame(AbstractDealer dealer, TableInfo table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon,GameType gameType = GameType.User)
        {
            m_Dealer = dealer;
            m_Table = table;
            m_State = TypeState.Init;
            m_RoundState = TypeRoundState.Cards;
            m_WaitingTimeAfterPlayerAction = wtaPlayerAction;
            m_WaitingTimeAfterBoardDealed = wtaBoardDealed;
            m_WaitingTimeAfterPotWon = wtaPotWon;
            gameTax = 0;
            GameType = gameType;
        }

        ~PokerGame()
        {
            while (this.Table.Players.Count > 0)
            {
                this.LeaveGame(this.Table.Players[0]);
            }
        }

        protected void NextState(TypeState state)
        {
            TypeState oldState = m_State;

            if (m_State == TypeState.End)
                return;

            if ((int)state - (int)oldState != 1 && m_Table.NbPlaying()>1)
                return;

            m_State = state;

            switch (m_State)
            {
                case TypeState.Init:
                    break;
                case TypeState.WaitForPlayers:
                    TryToBegin();
                    break;
                case TypeState.WaitForBlinds:
                    m_Table.HigherBet = 0;
                    break;
                case TypeState.Playing:
                    m_Table.Round = TypeRound.Preflop;
                    m_RoundState = TypeRoundState.Cards;
                    StartRound();
                    break;
                case TypeState.Showdown:
                    ShowAllCards();
                    break;
                case TypeState.DecideWinners:
                    DecideWinners();
                    break;
                case TypeState.DistributeMoney:
                    DistributeMoney();
                    break;
                case TypeState.End:
                    EverythingEnded(this, new EventArgs());
                    break;
            }
        }
        private void NextRound(TypeRound round)
        {

            TypeRound oldRound = m_Table.Round;

            if (m_State != TypeState.Playing)
                return;

            if ((int)round - (int)oldRound != 1)
                return;

            m_RoundState = TypeRoundState.Cards;
            int noSeat = m_Table.NoSeatDealer;
            if (m_Table.GetPlayer(noSeat)==null)
            {
                PlayerInfo playerDealer = m_Table.GetPlayingOrAllInPlayerNextTo(noSeat);
                if (playerDealer == null)
                    noSeat = m_Table.Players[0].NoSeat;
                else
                    noSeat = playerDealer.NoSeat;
            }

            m_Table.NoSeatCurrPlayer = noSeat;

            m_Table.Round = round;
            StartRound();
        }
        private void NextRoundState(TypeRoundState roundState)
        {

            TypeRoundState oldRoundState = m_RoundState;

            if (m_State != TypeState.Playing)
                return;

            if ((int)roundState - (int)oldRoundState != 1)
                return;

            m_RoundState = roundState;
            StartRound();
        }

        public void Start()
        {
            NextState(TypeState.WaitForPlayers);
        }

        private bool JoinGame(PlayerInfo p)
        {
            if (m_State == TypeState.Init || m_State == TypeState.End)
            {
                LogManager.Log(LogLevel.Error, "PokerGame.JoinGame", "Can't join, bad timing: {0}", m_State);
                return false;
            }
            return m_Table.JoinTable(p);
        }

        protected virtual void BeforeJoin(PlayerInfo p)
        {
        }

        public void SitInGame(PlayerInfo p)
        {
            BeforeJoin(p);
            if (!JoinGame(p))return;
            PlayerJoined(this, new PlayerInfoEventArgs(p)); // to do ..
            //if (m_State == TypeState.WaitForPlayers)
            //    TryToBegin();
        }

        public void TryStartGame() {
            if (m_State != TypeState.WaitForPlayers)
            {
                return;
            }

            double delay = m_WaitingTimeAfterGameEnd + startdelay;

            SetTimeout(delay * 1000d, () =>
            {
                if (m_State == TypeState.WaitForPlayers)
                    TryToBegin();
            });             
        }

        public void TryStartGame2() {
            if (m_State == TypeState.WaitForPlayers)
                TryToBegin(); 
        }


        public bool StandUp(PlayerInfo p) {
            //int leaveAmnt = p.MoneyBetAmnt;
            //if (p.NoSeat == this.Table.NoSeatCurrPlayer)
            //    PlayNext();

            if (m_Table.LeaveTable(p))
            {
                //if (this.Table.Pots.Count > 0)
                //    m_Table.ManagePotsLeaved(p);
                this.Table.addBystander(p);
                PlayerLeaved(this, new PlayerInfoEventArgs(p,PlayerLeaveType.Stand));
                if (m_Table.NbPlayingAndAllIn() == 1)
                {
                    //ShowAllCards();
                    //DecideWinners();
                    //DistributeMoney();
                    if (m_State == TypeState.Playing)
                    {
                        PlayerInfo lastplayer = m_Table.Players[0];
                        PlayerActionTaken(this, new PlayerActionEventArgs(lastplayer, TypeAction.DoNothing, 0));
                        m_Table.ManagePotsRoundEnd();
                        m_State = TypeState.DistributeMoney;
                        DistributeMoney();
                    }
                    //m_Table.Players[0].MoneyBetAmnt = 0;
                    //m_State = TypeState.WaitForPlayers;
                }

                return true;
            }
            return false;
        }

        public bool LeaveGame(PlayerInfo p)
        {
            return LeaveGame(p,PlayerLeaveType.Leave);
        }

        public bool LeaveGame(PlayerInfo p, PlayerLeaveType leaveType)
        {
            return LeaveGame(p,leaveType,string.Empty);
        }

        public bool LeaveGame(PlayerInfo p, PlayerLeaveType leaveType,string messageContent)
        {
            //int leaveAmnt = p.MoneyBetAmnt;
            this.Table.deleteBystander(p);
            //if (p.NoSeat == this.Table.NoSeatCurrPlayer)
            //PlayNext();
            if (m_Table.LeaveTable(p))
            {
                //if(this.Table.Pots.Count>0)
                //    m_Table.ManagePotsLeaved(p);
                PlayerLeaved(this, new PlayerInfoEventArgs(p, leaveType,messageContent));
                if (m_Table.PlayersAndBystander.Count == 0)
                    m_State = TypeState.End;
                if (m_Table.NbPlayingAndAllIn() == 1)
                {
                    //ShowAllCards();
                    //DecideWinners();
                    //DistributeMoney();
                    if (m_State == TypeState.Playing)
                    {
                        PlayerInfo lastplayer = m_Table.Players[0];
                        PlayerActionTaken(this, new PlayerActionEventArgs(lastplayer, TypeAction.DoNothing, 0));
                        m_Table.ManagePotsRoundEnd();
                        m_State = TypeState.DistributeMoney;
                        DistributeMoney();
                    }
                    //m_Table.Players[0].MoneyBetAmnt = 0;
                    //m_State = TypeState.WaitForPlayers;
                }

                return true;
            }
            return false;
        }

        public bool autoFold(PlayerInfo p)
        {
            const int amount = -1;
            bool willLeave = p.autofold >= 1;
            bool ret = PlayMoney(p, amount);
            p.autofold++;
            if (willLeave)
                this.LeaveGame(p,PlayerLeaveType.KickFoldTwice);
            return ret;
        }

        public bool PlayMoneyUser(PlayerInfo p, long amount) {
            if (p==null)
            {
                return false;
            }
            p.autofold = 0;
            return PlayMoney(p, amount);
        }

        public bool PlayMoney(PlayerInfo p, long amount)
        {
            long amnt = Math.Min(amount, p.MoneySafeAmnt);
            LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} is playing {1} money on state: {2}", p.Name, amnt, m_State);
            if (m_State == TypeState.WaitForBlinds)
            {
                LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Total blinds needed is {0}", m_Table.TotalBlindNeeded);
                LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "{0} is putting blind of {1}", p.Name, amnt);
                long needed = m_Table.GetBlindNeeded(p);
                if (amnt != needed)
                {
                    if (p.CanBet(amnt + 1))
                    {
                        LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} needed to put a blind of {1} and tried {2}", p.Name, needed, amnt);
                        return false;
                    }
                    LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Player now All-In !");
                    p.IsAllIn = true;
                    m_Table.NbAllIn++;
                    m_Table.AddAllInCap(p.MoneyBetAmnt + amnt);
                }else
                {
                    if (p.MoneySafeAmnt-amnt== 0)
                    {
                        LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Player now All-In !");
                        p.IsAllIn = true;
                        m_Table.NbAllIn++;
                        m_Table.AddAllInCap(p.MoneyBetAmnt + amnt);
                    }
                }
                if (!p.TryBet(amnt))
                {
                    LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                    return false;
                }
                m_Table.TotalPotAmnt += amnt;
                PlayerMoneyChanged(this, new PlayerMoneyChangedEventArgs(p,amnt));
                m_Table.AddBlindNeeded(p, 0);

                if (amnt == m_Table.SmallBlindAmnt)
                {
                    LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} POSTED SMALL BLIND", p.Name);
                    PlayerActionTaken(this, new PlayerActionEventArgs(p, TypeAction.PostSmallBlind, amnt));
                }
                else
                {
                    LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} POSTED BIG BLIND", p.Name);
                    PlayerActionTaken(this, new PlayerActionEventArgs(p, TypeAction.PostBigBlind, amnt));
                }

                m_Table.TotalBlindNeeded -= needed;

                if (amnt > m_Table.HigherBet)
                    m_Table.HigherBet = amnt;

                if (m_Table.TotalBlindNeeded == 0)
                {
                    GameBlindNeeded(this, new EventArgs());
                    NextState(TypeState.Playing);

                }


                LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Total blinds still needed is {0}", m_Table.TotalBlindNeeded);
                return true;
            }

            else if (m_State == TypeState.Playing && m_RoundState == TypeRoundState.Betting)
            {
                LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Currently, we need {0} minimum money from this player", m_Table.CallAmnt(p));
                if (p.NoSeat != m_Table.NoSeatCurrPlayer)
                {
                    LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just played but it wasn't his turn", p.Name);
                    return false;
                }

                if (amnt == -1)
                {
                    LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} FOLDED", p.Name);

                    p.exp = ExpType.Fold;

                    FoldPlayer(p);
                    ContinueBettingRound();
                    return true;
                }
                long amntNeeded = m_Table.CallAmnt(p);
                if (amnt < amntNeeded)
                {
                    if (p.CanBet(amnt + 1))
                    {
                        LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} needed to play at least {1} and tried {2}", p.Name, amntNeeded, amnt);
                        return false;
                    }
                    amntNeeded = amnt;
                }
                if (!p.TryBet(amnt))
                {
                    LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} just put more money than he actually have ({1} > {2})", p.Name, amnt, p.MoneySafeAmnt);
                    return false;
                }
                PlayerMoneyChanged(this, new PlayerMoneyChangedEventArgs(p,amnt));
                if (p.MoneySafeAmnt == 0)
                {
                    LogManager.Log(LogLevel.MessageVeryLow, "PokerGame.PlayMoney", "Player now All-In !");
                    p.IsAllIn = true;
                    m_Table.NbAllIn++;
                    m_Table.AddAllInCap(p.MoneyBetAmnt);
                }
                if (amnt == amntNeeded)
                {
                    LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} CALLED WITH ${1}", p.Name, amnt);
                    m_Table.TotalPotAmnt += amnt;
                    CallPlayer(p, amnt);
                    ContinueBettingRound();
                    return true;
                }
                LogManager.Log(LogLevel.MessageLow, "PokerGame.PlayMoney", "{0} RAISED WITH ${1}", p.Name, amnt);
                m_Table.TotalPotAmnt += amnt;
                RaisePlayer(p, amnt);
                ContinueBettingRound();
                return true;
            }
            LogManager.Log(LogLevel.Warning, "PokerGame.PlayMoney", "{0} played money but the game is not it the right state", p.Name);
            return false;
        }
        private void StartRound()
        {
            switch (m_RoundState)
            {
                case TypeRoundState.Cards:
                    StartCardRound();
                    break;
                case TypeRoundState.Betting:
                    StartBettingRound();
                    break;
                case TypeRoundState.Cumul:
                    StartCumulRound();
                    break;
            }
        }
        private void StartCumulRound()
        {
            m_Table.ManagePotsRoundEnd();
            GameBettingRoundEnded(this, new RoundEventArgs(m_Table.Round));
            if (m_Table.NbPlayingAndAllIn() <= 1)
            {
                NextState(TypeState.Showdown);
            }
            else
            {
                switch (m_Table.Round)
                {
                    case TypeRound.Preflop:
                        NextRound(TypeRound.Flop);
                        break;
                    case TypeRound.Flop:
                        NextRound(TypeRound.Turn);
                        break;
                    case TypeRound.Turn:
                        NextRound(TypeRound.River);
                        break;
                    case TypeRound.River:
                        NextState(TypeState.Showdown);
                        break;
                }
            }
        }
        private void StartBettingRound()
        {
            GameBettingRoundStarted(this, new RoundEventArgs(m_Table.Round));
            m_Table.NbPlayed = 0 + m_Table.NbAllIn;
            m_Table.NoSeatLastRaise = m_Table.GetPlayingPlayerNextTo(m_Table.NoSeatCurrPlayer).NoSeat;
            WaitALittle(m_WaitingTimeAfterBoardDealed);

            if (m_Table.NbPlaying() <= 1)
                EndBettingRound();
            else
                ContinueBettingRound();
        }
        private void StartCardRound()
        {
            switch (m_Table.Round)
            {
                case TypeRound.Preflop:
                    m_Table.NoSeatCurrPlayer = m_Table.NoSeatBigBlind;
                    DealHole();
                    if(!isFastForward)
                        WaitALittle(1000);
                    break;
                case TypeRound.Flop:
                    DealFlop();
                    break;
                case TypeRound.Turn:
                    DealTurn();
                    break;
                case TypeRound.River:
                    DealRiver();
                    break;
            }    
            if (isFastForward && m_Table.Round == stopFastForwardFlag)
            {
                isFastForward = false;
                //NextRoundState(TypeRoundState.Betting);
                //if (RealPlayerCanJoinNow != null)
                //    RealPlayerCanJoinNow();
            }
            //else
                NextRoundState(TypeRoundState.Betting);
           
        }
        private void DealRiver()
        {
            m_Table.AddCards(m_Dealer.DealRiver());
        }
        private void DealTurn()
        {
            m_Table.AddCards(m_Dealer.DealTurn());
        }
        private void DealFlop()
        {
            m_Table.AddCards(m_Dealer.DealFlop());
        }
        private void DealHole()
        {
            try
            {
                foreach (PlayerInfo p in m_Table.PlayingOrAllinPlayers())
                {
                    p.Cards = m_Dealer.DealHoles(p);
                    p.PlayedInTable++;
                    PlayerHoleCardsChanged(this, new PlayerInfoEventArgs(p));
                }
                //同时发送消息给站起的人
                PlayerInfo player = new PlayerInfo();
                player.NoSeat = -1;
                player.IsPlaying = false;
                PlayerHoleCardsChanged(this, new PlayerInfoEventArgs(player));
            }
            catch { 
                
            }
            m_Dealer.SetCheat();
        }
        private void ShowAllCards()
        {
            if (m_Table.NbPlayingAndAllIn()>1)
            {
                Dictionary<int, string> playerlist = new Dictionary<int, string>();
                //foreach (PlayerInfo p in m_Table.Players)
                //    if (p.IsPlaying || p.IsAllIn)
                //    {
                //        p.IsShowingCards = true;
                //        string cards = string.Join(" ",p.Cards.Select(r=>r.ToString()).ToArray());
                //        playerlist.Add(p.NoSeat, cards);
                //        PlayerHoleCardsChanged(this, new PlayerInfoEventArgs(p));
                //    }
                //需要重新调整顺序，以小盲注位置开始
                int noSeatd = Table.NoSeatDealer;
                int maxSeat = Table.NbMaxSeats();
                for (int i = 0; i < maxSeat; i++)
                {
                    int j = (noSeatd + 1 + i) % maxSeat;
                    PlayerInfo p = Table.GetPlayer(j);
                    if (p != null && (p.IsPlaying || p.IsAllIn))
                    {
                        p.IsShowingCards = true;
                        string cards = string.Join(" ", p.Cards.Select(r => r.ToString()).ToArray());
                        playerlist.Add(p.NoSeat, cards);
                        //PlayerHoleCardsChanged(this, new PlayerInfoEventArgs(p));
                    }
                }

                if (playerlist.Count>0)
                {
                    PlayersShowCardsArgs arg = new PlayersShowCardsArgs();
                    arg.ShowPlayers = playerlist;
                    PlayersShowCards(this, arg);
                }
            }            
            NextState(TypeState.DecideWinners);
        }
        private void FoldPlayer(PlayerInfo p)
        {
            p.IsPlaying = false;
            WaitALittle(m_WaitingTimeAfterPlayerAction);
            PlayerActionTaken(this, new PlayerActionEventArgs(p, TypeAction.Fold, -1));
        }
        private void CallPlayer(PlayerInfo p, long played)
        {
            m_Table.NbPlayed++;
            WaitALittle(m_WaitingTimeAfterPlayerAction);
            PlayerActionTaken(this, new PlayerActionEventArgs(p, TypeAction.Call, played));
        }
        private void RaisePlayer(PlayerInfo p, long played)
        {
            int count = m_Table.NbAllIn;
            if (!p.IsAllIn)
                count++;
            m_Table.NoSeatLastRaise = p.NoSeat;
            m_Table.NbPlayed = count;
            m_Table.HigherBet = p.MoneyBetAmnt;
            WaitALittle(m_WaitingTimeAfterPlayerAction);
            PlayerActionTaken(this, new PlayerActionEventArgs(p, TypeAction.Raise, played));
        }
        private void ContinueBettingRound()
        {
            if (m_Table.NbPlayingAndAllIn() == 1 || m_Table.NbPlayed >= m_Table.NbPlayingAndAllIn())
                EndBettingRound();
            else
                PlayNext();
        }
        private void EndBettingRound()
        {
            NextRoundState(TypeRoundState.Cumul);
        }
        private void PlayNext()
        {
            PlayerInfo old = m_Table.GetPlayer(m_Table.NoSeatCurrPlayer);
            PlayerInfo player = m_Table.GetPlayingPlayerNextTo(m_Table.NoSeatCurrPlayer);
            m_Table.NoSeatCurrPlayer = player.NoSeat;
            PlayerActionNeeded(this, new HistoricPlayerInfoEventArgs(player,old,Table.HigherBet));
            if (player.IsZombie)
            {
                if (m_Table.CanCheck(player))
                    PlayMoney(player, 0);
                else
                    //PlayMoney(player, -1);
                    autoFold(player);
            }
            if (player.IsRobot)
            {
                // robot ai
                RobotActionNeeded(this, new PlayerInfoEventArgs(player));
            }

        }
        private void DistributeMoney()
        {
            if (m_State!= TypeState.DistributeMoney)
            {
                return;
            }
            //m_Table.Pots.Sort(delegate(MoneyPot a, MoneyPot b) {
            //    int _temp = a.Id - b.Id;
            //    if (_temp < 0) return 1;
            //    if (_temp > 0) return -1;
            //    return 0;
            //});

            startdelay = 0;
            long taxAmount = 0;
            List<MoneyPot> lmp = m_Table.Pots;
            foreach (MoneyPot pot in lmp)
            {
                ArrayList winner = new ArrayList();
                ArrayList winamount = new ArrayList();
                //ArrayList attachedplayers = new ArrayList();
                PlayerInfo[] players = pot.AttachedPlayers;
                double taxAmountPot = pot.Amount * gameTax;
                long taxAmountPotInt = 0;
                if (taxAmountPot >= 1)
                    taxAmountPotInt = Convert.ToInt64(taxAmountPot);

                if (players.Length > 0)
                {
                    long wonAmount = (pot.Amount - taxAmountPotInt) / players.Length;
                    if (wonAmount > 0)
                    {
                        foreach (PlayerInfo p in players)
                        {
                            p.MoneySafeAmnt += wonAmount;
                            if (pot.Id == 0)
                            {
                                p.exp = ExpType.Win;
                                p.WinsInTable++;
                            }
                            PlayerMoneyChanged(this, new PlayerMoneyChangedEventArgs(p, wonAmount * -1));
                            PlayerWonPot(this, new PotWonEventArgs(p, pot.Id, wonAmount));

                            winner.Add(p.NoSeat);
                            winamount.Add(wonAmount);
                            //if(waiting)WaitALittle(m_WaitingTimeAfterPotWon);
                        }
                    }
                }
                taxAmount += taxAmountPotInt;
                if(winner.Count>0)
                PlayerWonPotImprove(this, new PotWonImproveEventArgs(pot.Id,(int[])winner.ToArray(typeof(int)),(long[])winamount.ToArray(typeof(long)),pot.OriginalAttachedPlayers.Select(r=>r.NoSeat).ToArray()));
            }

            if (lmp.Count>1)
            {
                startdelay = (int)((lmp.Count - 1) * 2.5);
            }

            EndRound(taxAmount);
            foreach (PlayerInfo p in Table.Players)
            {
                p.IsPlaying = false;
            }
            m_Table.m_NoSeatCurrPlayer = -1;
            //if (waiting) WaitALittle(5000);

            //SetTimeout(4000, () => { m_State = TypeState.WaitForPlayers; });
            m_State = TypeState.WaitForPlayers;
            //TryStartGame2();
            //SetTimeout(5000, () => { TryStartGame(); });
            TryStartGame();
            
        }
        private void DecideWinners()
        {
            m_Table.CleanPotsForWinning();
            DecideWinnersEnded(this, new EventArgs());
            NextState(TypeState.DistributeMoney);

            
        }
        private void WaitALittle(int waitingTime)
        {
            Thread.Sleep(waitingTime);
        }
        protected virtual void TryToBegin()
        {
            lock (lockobj)
            {
                if (m_Table.Players.Count < 2) return;
                foreach (PlayerInfo p in m_Table.Players)
                {
                    if (p.MoneySafeAmnt < Table.BigBlindAmnt)
                    {
                        LeaveGame(p, p.IsRobot ? PlayerLeaveType.Kick : PlayerLeaveType.Stand);

                        continue;
                    }


                    if (p.IsZombie)
                    {
                        LeaveGame(p);
                        p.IsPlaying = false;
                    }
                    else if (p.CanPlay)
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
                    m_Table.InitTable();
                    m_Dealer.FreshDeck();
                    NextState(TypeState.WaitForBlinds);
                    m_Table.TotalRounds++;

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


        protected void PlayBlindMoney()
        {

            TableInfo t = Table;
            if (t.NbPlaying() < 2)
                return;
            if (this.State != TypeState.WaitForBlinds)
                return;
            //int totalp = t.Players.Count;
            int small = t.NoSeatSmallBlind;
            int big = t.NoSeatBigBlind;
            if (t.GetPlayer(small) == null || t.GetPlayer(big) == null)
                this.Table.PlaceButtons();
            this.PlayMoney(t.GetPlayer(t.NoSeatSmallBlind), t.SmallBlindAmnt);
            this.PlayMoney(t.GetPlayer(t.NoSeatBigBlind), t.BigBlindAmnt);
            //Log.Info("##### SmalBlind " + t.GetPlayer(t.NoSeatSmallBlind).Name +" "+t.SmallBlindAmnt);
            //Log.Info("##### BigBlind " + t.GetPlayer(t.NoSeatBigBlind).Name + " " + t.BigBlindAmnt);
            //Log.Info("##### blindneeded " + t.TotalBlindNeeded);
        }


        private void SetTimeout(double interval, Action action)
        {
            if (Math.Abs(interval - 0d) < 1) interval = 1000d;
            System.Timers.Timer time = new System.Timers.Timer(interval);
            time.Elapsed += delegate
                                {
                time.Enabled = false;
                action();
                time.Dispose();
            };
            time.AutoReset = false;
            time.Enabled = true;
        }

        protected virtual void EndRound(long taxAmount)
        {
            GameEnded(this, new GameEndEventArgs(taxAmount, startdelay));
        }

        protected void GameStart()
        {
            GameStarted(this,new EventArgs());
        }
    }
}
