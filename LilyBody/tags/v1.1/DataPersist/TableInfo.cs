using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using EricUtility;

namespace DataPersist
{
    public class TableInfo
    {
        // INFO
        public string m_Name{get;set;} // Nom de la table
        protected TypeBet m_BetLimit; // Type de betLimit (NO_LIMIT, POT_LIMIT, etc.)

        // CARDS
        protected GameCard[] m_Cards = new GameCard[5]; // Cartes du board

        // SEATS
        protected int m_NbMaxSeats; // Nombe de siege total autour de la table
        //protected Stack<int> m_RemainingSeats = new Stack<int>(); // LIFO contenant les sieges non utilises

        protected SortedList<int,int> m_RemainingSeats2 = new SortedList<int,int>();

        // PLAYERS
        protected PlayerInfo[] m_Players; // Joueurs autour de la table

        protected Dictionary<string,PlayerInfo> m_Bystander=new Dictionary<string,PlayerInfo>(); // stupid guys


        // POTS
        protected List<MoneyPot> m_Pots = new List<MoneyPot>(); // La liste des POTS
        protected long m_TotalPotAmnt; // Le montant total en jeu (Tous les pots + l'argent en jeu)
        protected List<long> m_AllInCaps = new List<long>(); // Les differents CAPS ALL_IN de la ROUND

        // BLINDS
        protected int m_SmallBlindAmnt; // Montant a donner lorsqu'on est small blind
        protected int m_BigBlindAmnt; // Montant a donner lorsqu'on est big blind
        protected Dictionary<PlayerInfo, long> m_BlindNeeded = new Dictionary<PlayerInfo, long>(); // Hashmap contenant les blinds necessaire pour chaque player
        protected long m_TotalBlindNeeded; // Montant total necessaire pour les blinds

        // STATES
        protected int m_NbPlayed; // Nombre de joueur ayant joues de cette ROUND
        protected int m_NbAllIn; // Nombre de joueurs ALL-IN
        protected long m_HigherBet; // Le bet actuel qu'il faut egaler
        protected TypeRound m_Round; // La round actuelle
        public int m_NoSeatDealer { get; set; } // La position actuelle du Dealer
        public int m_NoSeatSmallBlind { get; set; } // La position actuelle du SmallBlind
        public int m_NoSeatBigBlind { get; set; }// La position actuelle du BigBlind
        public int m_NoSeatCurrPlayer { get; set; } // La position du joueur actuel
        public int m_CurrPotId { get; set; } // L'id du pot qu'on travail actuellement avec
        protected int m_NoSeatLastRaise; // L'id du dernier player qui a raiser ou du premier a jouer

        protected int m_TotalRounds=0;

        public DateTime GameStart { get; set; }


        public int ThinkingTime { get; set; }
        public bool OnlyFriend { get; set; }


        public int TotalRounds
        {
            get { return m_TotalRounds; }
            set { m_TotalRounds = value; }
        }
        public string Name()
        {
            return m_Name;
        }

        public TypeBet BetLimit
        {
            get { return m_BetLimit; }
            set { m_BetLimit = value; }
        }

        public GameCard[] Cards
        {
            get
            {
                GameCard[] cards = new GameCard[5];
                for (int i = 0; i < 5; ++i)
                {
                    if (m_Cards[i] == null)
                    {
                        cards[i] = GameCard.NO_CARD;
                    }
                    else
                        cards[i] = m_Cards[i];
                }
                return cards;
            }
            set
            {
                if (value != null && value.Length == 5)
                {
                    for (int i = 0; i < 5; ++i)
                        m_Cards[i] = value[i];
                }
            }
        }
        public int NbMaxSeats()
        {
            return m_NbMaxSeats; 
        }

        public List<PlayerInfo> Bystander {
            get
            {
                List<PlayerInfo> list = new List<PlayerInfo>();                
                foreach (var p in m_Bystander)
                {
                    list.Add(p.Value);
                }
                return list;
            }
        }

        /// <summary>
        /// contains standing 
        /// </summary>
        public List<PlayerInfo> PlayersAndBystander
        {
            get {
                List<PlayerInfo> list = new List<PlayerInfo>();
                for (int i = 0; i < m_NbMaxSeats; ++i)
                {
                    if (m_Players[i] != null)
                    {
                        list.Add(m_Players[i]);
                    }
                }
                foreach (var p in m_Bystander)
                {
                    list.Add(p.Value);
                }
                return list;
            }
        }

        /// <summary>
        /// players in table
        /// </summary>
        public List<PlayerInfo> Players
        {
            get
            {
                List<PlayerInfo> list = new List<PlayerInfo>();
                for (int i = 0; i < m_NbMaxSeats; ++i)
                {
                    if (m_Players[i] != null)
                    {
                        list.Add(m_Players[i]);
                    }
                }
                return list;
            }
            set
            {
				for(int i=0;i<value.Count;i++)
				{
                	m_Players[value[i].NoSeat] = value[i];
				}
            }
        }

        public List<PlayerInfo> PlayingPlayers()
        {
            return PlayingPlayersFrom(0); 
        }

        public List<PlayerInfo> PlayingPlayersFromNext()
        {
            if (GetPlayingPlayerNextTo(m_NoSeatCurrPlayer) != null)
                return PlayingPlayersFrom(GetPlayingPlayerNextTo(m_NoSeatCurrPlayer).NoSeat);
            
            return null;
        }

        public List<PlayerInfo> PlayingPlayersFromCurrent()
        {
            return PlayingPlayersFrom(m_NoSeatCurrPlayer);
        }

        public List<PlayerInfo> PlayingPlayersFromLastRaise()
        {
            return PlayingPlayersFrom(m_NoSeatLastRaise);
        }

        public List<PlayerInfo> PlayingPlayersFromFirst()
        {
            if (m_Round == TypeRound.Preflop)
            {
                if (GetPlayingPlayerNextTo(m_NoSeatBigBlind) != null)
                    return PlayingPlayersFrom(GetPlayingPlayerNextTo(m_NoSeatBigBlind).NoSeat);

                return null;
            }

            if (GetPlayingPlayerNextTo(m_NoSeatDealer) != null)
                return PlayingPlayersFrom(GetPlayingPlayerNextTo(m_NoSeatDealer).NoSeat);

            return null;
        }

        public List<MoneyPot> Pots
        {
            get {return m_Pots;}
            set { m_Pots = value; }
        }

        public long TotalPotAmnt
        {
            get { return m_TotalPotAmnt; }
            set { m_TotalPotAmnt = value; }
        }

        public int SmallBlindAmnt
        {
            get {return m_SmallBlindAmnt;}
			set {m_SmallBlindAmnt=value;}
        }

        public int BigBlindAmnt
        {
            get {return m_BigBlindAmnt;}
			set {m_BigBlindAmnt=value;}
        }

        public long TotalBlindNeeded
        {
            get { return m_TotalBlindNeeded; }
            set { m_TotalBlindNeeded = value; }
        }

        public int NoSeatDealer
        {
            get { return m_NoSeatDealer; }
            set { m_NoSeatDealer = value; }
        }

        public int NoSeatSmallBlind
        {
            get { return m_NoSeatSmallBlind; }
            set { m_NoSeatSmallBlind = value; }
        }

        public int NoSeatBigBlind
        {
            get { return m_NoSeatBigBlind; }
            set { m_NoSeatBigBlind = value; }
        }

        public int NoSeatCurrPlayer
        {
            get { return m_NoSeatCurrPlayer; }
            set { m_NoSeatCurrPlayer = value; }
        }

        public int NoSeatLastRaise
        {
            get { return m_NoSeatLastRaise; }
            set { m_NoSeatLastRaise = value; }
        }

        public int NbPlayed
        {
            get { return m_NbPlayed; }
            set { m_NbPlayed = value; }
        }

        public int NbAllIn
        {
            get { return m_NbAllIn; }
            set { m_NbAllIn = value; }
        }

        public int NbPlaying()
        {
            return PlayingPlayers().Count;
        }

        public int NbPlayingAndAllIn()
        {
            return NbPlaying() + NbAllIn;
        }
        public long HigherBet
        {
            get { return m_HigherBet; }
            set { m_HigherBet = value; }
        }
        public TypeRound Round
        {
            get { return m_Round; }
            set { m_Round = value; }
        }

        public TableInfo()
            : this(10)
        {
        }

        public TableInfo(int nbSeats)
            : this("Anonymous Table", 10, nbSeats, TypeBet.NoLimit)
        {
        }

        public TableInfo(string name, int bigBlind, int nbSeats, TypeBet limit)
        {
            m_NbMaxSeats = nbSeats;
            m_Players = new PlayerInfo[nbSeats];
            m_Name = name;
            m_BigBlindAmnt = bigBlind;
            m_SmallBlindAmnt = bigBlind / 2;
            m_NoSeatDealer = -1;
            m_NoSeatSmallBlind = -1;
            m_NoSeatBigBlind = -1;
            m_NoSeatCurrPlayer = -1;
            m_BetLimit = limit;
            m_RemainingSeats2.Clear();
            for (int i = 1; i <= m_NbMaxSeats; ++i)
            {
                //m_RemainingSeats.Push(m_NbMaxSeats - i);
                m_RemainingSeats2.Add(m_NbMaxSeats - i, m_NbMaxSeats - i);
            }
        }
        public void InitCards()
        {
            Cards = new GameCard[5];
        }
        public void SetCards(GameCard c1, GameCard c2, GameCard c3, GameCard c4, GameCard c5)
        {
            Cards = new GameCard[5] { c1, c2, c3, c4, c5 };
        }
        public void AddCards(params GameCard[] c)
        {
            int i = 0;
            for (; m_Cards[i] != null && m_Cards[i].ToString() != new GameCard(GameCardSpecial.Null).ToString(); ++i) ;
            for (int j = i; j < Math.Min(5, c.Length + i); ++j)
                m_Cards[j] = c[j - i];
        }
        public PlayerInfo GetPlayer(int seat)
        {
			if(seat<0||seat>=m_Players.Length)
			{
				return null;
			}
            return m_Players[seat];
        }

        public PlayerInfo GetPlayer(string name) {
            foreach (PlayerInfo p in Players)
                if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return p;
            return null;
        }

        public PlayerInfo GetPlayerNextTo(int seat)
        {
            if (seat < 0)
                return null;

            for (int i = 0; i < m_NbMaxSeats; ++i)
            {
                int j = (seat + 1 + i) % m_NbMaxSeats;
                if (m_Players[j] != null)
                {
                    return m_Players[j];
                }
            }
            return m_Players[seat];
        }
        public PlayerInfo GetPlayingPlayerNextTo(int seat)
        {
            if (seat < 0) {
                seat = -1;
            }

            for (int i = 0; i < m_NbMaxSeats; ++i)
            {
                int j = (seat + 1 + i) % m_NbMaxSeats;
                if (m_Players[j] != null && m_Players[j].IsPlaying)
                {
                    return m_Players[j];
                }
            }
            return m_Players[seat];
        }

        public PlayerInfo GetPlayingOrAllInPlayerNextTo(int seat) {
            if (seat < 0)
            {
                seat = -1;
            }
            for (int i = 0; i < m_NbMaxSeats; ++i)
            {
                int j = (seat + 1 + i) % m_NbMaxSeats;
                if (m_Players[j] != null && (m_Players[j].IsPlaying||m_Players[j].IsAllIn))
                {
                    return m_Players[j];
                }
            }
            return m_Players[seat];
        }

        private bool ContainsPlayer(PlayerInfo p)
        {
            return Players.Contains(p);
        }
        public bool ContainsPlayer(String name)
        {
            foreach (PlayerInfo p in Players)
                if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">player nickname</param>
        /// <returns>noSeat</returns>
        private int ContainsPlayerLily(string name) {
            foreach (PlayerInfo p in Players)
                if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return p.NoSeat;
            return -1;
        }


        public void AddAllInCap(long val)
        {
            if (!m_AllInCaps.Contains(val))
                m_AllInCaps.Add(val);
        }
        public void AddBlindNeeded(PlayerInfo p, long amnt)
        {
            if (m_BlindNeeded.ContainsKey(p))
                m_BlindNeeded[p] = amnt;
            else
                m_BlindNeeded.Add(p, amnt);
        }
        public long GetBlindNeeded(PlayerInfo p)
        {
            if (m_BlindNeeded.ContainsKey(p))
                return m_BlindNeeded[p];
            else
                return 0;
        }

        public void InitTable()
        {
            Cards = new GameCard[5];
            GameStart = System.DateTime.Now;
            NbPlayed = 0;
            PlaceButtons();
            InitPots();
        }
        public bool ForceJoinTable(PlayerInfo p, int seat)
        {
            p.IsPlaying = false;
            p.NoSeat = seat;
            m_Players[seat] = p;
            return true;
        }
        public bool JoinTable(PlayerInfo p)
        {
            this.deleteBystander(p);
            if (m_RemainingSeats2.Count == 0&&!this.Players.Exists(rs=>rs.Name==p.Name))
            {
                LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Not enough seats to join!");
                return false;
            }

            //if (ContainsPlayer(p.Name)) //ContainsPlayer(p)
            //{
            //    LogManager.Log(LogLevel.Error, "TableInfo.JoinTable", "Already there someone with the same name!");
            //    return false;
            //}

            int checkSeat = ContainsPlayerLily(p.Name);
            if (checkSeat>-1)
            {
                PlayerInfo p2 = m_Players[checkSeat];
                p.IsPlaying = p2.IsPlaying;
                p.IsAllIn = p2.IsAllIn;
                p.NoSeat = p2.NoSeat;
                p.MoneyInitAmnt = p2.MoneyInitAmnt;
                p.MoneySafeAmnt = p2.MoneySafeAmnt;
                p.MoneyBetAmnt = p2.MoneyBetAmnt;
                p.IsZombie = false;
                p2.IsZombie = false;
                return true;
            }

            int seat = p.NoSeat; //m_RemainingSeats.Pop();
            if (seat == -1)
            {
                seat = m_RemainingSeats2.Keys[0];
                m_RemainingSeats2.Remove(seat);
            }
            else {
                if (m_RemainingSeats2.ContainsKey(seat))
                {
                    m_RemainingSeats2.Remove(seat);
                }
                else {
                    p.NoSeat = -1;
                    return false;
                }
            }
            p.IsPlaying = false;
            p.NoSeat = seat;
            m_Players[seat] = p;
            p.ResetStatus();
            return true;
        }
        public bool LeaveTable(PlayerInfo p)
        {
            if (!ContainsPlayer(p.Name))
                return false;



            int seat = p.NoSeat;

            if (seat<0||seat>=m_Players.Length)
            {
                p.NoSeat = -1;
                p.IsPlaying = false;
                return false;
            }
            
            p.IsPlaying = false;
            p.NoSeat = -1;
            m_Players[seat] = null;
            if (!m_RemainingSeats2.ContainsKey(seat))
            {
                m_RemainingSeats2.Add(seat, seat);
            }
            //m_RemainingSeats.Push(seat);
            return true;
        }

        public void addBystander(PlayerInfo p)
        {
            if (!m_Bystander.ContainsKey(p.Name))
                m_Bystander.Add(p.Name,p);
        }

        public void deleteBystander(PlayerInfo p) {
            if (m_Bystander.ContainsKey(p.Name))
                m_Bystander.Remove(p.Name);
        }



        public void InitPots()
        {
            TotalPotAmnt = 0;
            m_Pots.Clear();
            m_AllInCaps.Clear();
            m_Pots.Add(new MoneyPot(0));
            m_CurrPotId = 0;
            NbAllIn = 0;
        }
        public void PlaceButtons()
        {
            PlayerInfo p = GetPlayingPlayerNextTo(m_NoSeatDealer);
            m_NoSeatDealer = p == null ? this.PlayingPlayers()[0].NoSeat : p.NoSeat;
            int nbp = NbPlaying();
            m_NoSeatSmallBlind = nbp == 2 ? m_NoSeatDealer : GetPlayingPlayerNextTo(m_NoSeatDealer).NoSeat;
            m_NoSeatBigBlind = GetPlayingPlayerNextTo(m_NoSeatSmallBlind).NoSeat;
            m_BlindNeeded.Clear();
            m_BlindNeeded.Add(GetPlayer(m_NoSeatSmallBlind), SmallBlindAmnt);
            m_BlindNeeded.Add(GetPlayer(m_NoSeatBigBlind), BigBlindAmnt);
            m_TotalBlindNeeded = SmallBlindAmnt + BigBlindAmnt;
        }
        private void AddBet(PlayerInfo p, MoneyPot pot, long bet)
        {
            p.MoneyBetAmnt -= bet;
            pot.AddAmount(bet);
            if (bet > 0 && (p.IsPlaying || p.IsAllIn))
            {
                pot.AttachPlayer(p);
            }else if(bet==0 && p.IsPlaying)
                pot.AttachPlayer(p);
            
        }

        //public void ManagePotsLeaved(PlayerInfo p) {
        //    //p.IsPlaying = false;
        //    int currentTaken = 0;
        //    m_AllInCaps.Sort();
        //    while (m_AllInCaps.Count > 0)
        //    {
        //        MoneyPot pot = m_Pots[m_CurrPotId];
        //        pot.DetachAllPlayers();
        //        int aicf = m_AllInCaps[0];
        //        m_AllInCaps.RemoveAt(0);
        //        int cap = aicf - currentTaken;
               
        //        AddBet(p, pot, Math.Min(p.MoneyBetAmnt, cap));
                
        //        currentTaken += cap;
        //        m_CurrPotId++;
        //        m_Pots.Add(new MoneyPot(m_CurrPotId));
        //    }

        //    MoneyPot curPot = m_Pots[m_CurrPotId];
        //    curPot.DetachAllPlayers();
        //    AddBet(p, curPot, p.MoneyBetAmnt);
        //    m_HigherBet = 0;
        //}

        public void ManagePotsRoundEnd()
        {
            long currentTaken = 0;
            m_AllInCaps.Sort();
            while (m_AllInCaps.Count > 0)
            {
                MoneyPot pot = m_Pots[m_CurrPotId];
                pot.DetachAllPlayers();
                long aicf = m_AllInCaps[0];
                m_AllInCaps.RemoveAt(0);
                long cap = aicf - currentTaken;
                foreach (PlayerInfo p in Players)
                {
                    AddBet(p, pot, Math.Min(p.MoneyBetAmnt, cap));
                }
                currentTaken += cap;
                m_CurrPotId++;
                m_Pots.Add(new MoneyPot(m_CurrPotId));
            }

            MoneyPot curPot = m_Pots[m_CurrPotId];
            curPot.DetachAllPlayers();
            foreach (PlayerInfo p in Players)
            {
                AddBet(p, curPot, p.MoneyBetAmnt);
            }
            m_HigherBet = 0;
        }
        public void CleanPotsForWinning()
        {
            for (int i = 0; i <= m_CurrPotId; ++i)
            {
                MoneyPot pot = m_Pots[i];
                uint bestHand = 0;
                List<PlayerInfo> infos = new List<PlayerInfo>(pot.AttachedPlayers);
                pot.OriginalAttachedPlayers = infos;

                if (infos.Count > 1)
                {
                    foreach (PlayerInfo p in infos)
                    {
                        if (p.IsPlaying || p.IsAllIn) //踢除已经弃牌，但是还在早前的奖池中的人
                        {
                            uint handValue = p.EvaluateCards(Cards);
                            if (handValue > bestHand)
                            {
                                pot.DetachAllPlayers();
                                pot.AttachPlayer(p);
                                bestHand = handValue;
                            }
                            else if (handValue == bestHand)
                            {
                                pot.AttachPlayer(p);
                            }
                        }
                    }
                }
            }
        }
        public bool CanRaise(PlayerInfo p)
        {
            return HigherBet < p.MoneyAmnt;
        }
        public bool CanCheck(PlayerInfo p)
        {
            return HigherBet <= p.MoneyBetAmnt;
        }
        public long MinRaiseAmnt(PlayerInfo p)
        {
            return Math.Min(CallAmnt(p) + BigBlindAmnt, MaxRaiseAmnt(p));
        }
        public long MaxRaiseAmnt(PlayerInfo p)
        {
            return p.MoneySafeAmnt;
        }
        public long CallAmnt(PlayerInfo p)
        {
            return HigherBet - p.MoneyBetAmnt;
        }
        public List<PlayerInfo> PlayingPlayersFrom(int seat)
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            for (int i = 0; i < m_NbMaxSeats; ++i)
            {
                if (m_Players[i] != null && m_Players[i].IsPlaying)
                {
                    list.Add(m_Players[i]);
                }
            }
            return list;
        }
    }
}
