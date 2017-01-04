using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using PokerWorld.HandEvaluator;

namespace DataPersist
{
    public class PlayerInfo
    {
        // INFO
        private String m_Name; // Nom du joueur
        private int m_NoSeat; // Position du joueur autour de la table
        
        // CARDS
        public GameCard[] m_Cards {get;set;} // Cartes du joueur
        
        // MONEY
        private long m_MoneyInitAmnt; // Argent du joueur au moment ou il s'installe a la table
        private long m_MoneySafeAmnt; // Argent du joueur qu'il a en sa pocession, non-jouee
        private long m_MoneyBetAmnt; // Argent du joueur qu'il a jouee depuis le debut de la round
        
        // STATES
        public bool m_IsPlaying { get; set; } // Est-il en train de jouer ? Faux si Folded, AllIn or NotPlaying
        public bool m_IsAllIn { get; set; } // Est-il All-in ? Vrai si All-in
        private bool m_IsShowingCards; // Montre-il ses cartes ? Vrai si showdown
        private bool m_Zombie; // Est-ce que le vrai player a quitté la partie ? Vrai si zombie

        public bool IsRobot { get; set; }
        public bool HasPlayed { get; set; }
        public ExpType exp { get; set; }
        public int GiftId { get; set; }

        public Int64 HandValue { get; set; }
        public Hand.HandTypes HandType { get; set; }


        public long Chips { get; set; }
        public int Level { get; set; }
        public long LevelExp { get; set; }

        public long WinAmnt { get; set; }


        public int autofold { get; set; }

        public bool Greedy { get; set; }

        public int Kicked { get; set; }


        //牌局奖励 props
        private DateTime LastOnlineAwardTimestamp { get; set; }
        private int OnlineAwardTimes { get; set; }
        private long MoneyTotalAmnt { get; set; }
        
        //stepid
        public int StepID { get; set; }

        public byte Avator { get; set; }


        #region 牌局结算属性
        /// <summary>
        /// 在当前牌桌内已赢局数
        /// </summary>
        public int WinsInTable { get; set; }
        /// <summary>
        /// 在当前牌桌内已玩局数
        /// </summary>
        public int PlayedInTable { get; set; }

        /// <summary>
        /// 比赛排名
        /// </summary>
        private int Ranking { get; set; }
        #endregion

        public string Name
        {
            get
            {
                return m_Name;
            }
            set { m_Name = value; }
        }
        public int NoSeat
        {
            get { return m_NoSeat; }
            set { m_NoSeat = value; }
        }
        public GameCard[] Cards
        {
            get
            {
                GameCard[] cards = new GameCard[2] { m_Cards[0], m_Cards[1] };
                for (int i = 0; i < 2; ++i)
                {
                    if (cards[i] == null)//|| !(m_IsPlaying || m_IsAllIn)
                    {
                        cards[i] = new GameCard(GameCardSpecial.Null);
                    }
                }

                return cards;
            }
            set
            {

                if (m_Cards == null)
                {
                    m_Cards = new GameCard[2];
                }

                if (value != null && value.Length == 2)
                {
                    m_Cards[0] = value[0];
                    m_Cards[1] = value[1];
                }
            }
        }
        
        public long MoneyInitAmnt
        {
            get { return m_MoneyInitAmnt; }
            set { m_MoneyInitAmnt = value; }
        }
        public long MoneySafeAmnt
        {
            get { return m_MoneySafeAmnt; }
            set { m_MoneySafeAmnt = value; }
        }
        public long MoneyBetAmnt
        {
            get { return m_MoneyBetAmnt; }
            set { m_MoneyBetAmnt = value; }
        }
        public long MoneyAmnt
        {
            get { return MoneyBetAmnt + MoneySafeAmnt; }
        }
        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set
            {
                m_IsPlaying = value;
                m_IsAllIn = false;
            }
        }
        public bool IsAllIn
        {
            get { return m_IsAllIn; }
            set
            {
                m_IsPlaying = false;
                m_IsAllIn = value;
            }
        }
        //the network is available
        public bool IsZombie
        {
            get { return m_Zombie; }
            set { m_Zombie = value; }
        }
        public bool IsShowingCards
        {
            get { return m_IsShowingCards; }
            set { m_IsShowingCards = value; }
        }
        public bool CanPlay
        {
            get { return NoSeat >= 0 && MoneySafeAmnt > 0; }
        }

        public PlayerInfo()
        {
            m_Name = "Anonymous Player";
            m_NoSeat = -1;
            m_MoneySafeAmnt = 0;
            m_MoneyBetAmnt = 0;
            m_MoneyInitAmnt = 0;
            m_Cards = new GameCard[2];
        }
        public PlayerInfo(String name)
            : this()
        {
            m_Name = name;
        }
        public PlayerInfo(int seat)
            : this()
        {
            m_NoSeat = seat;
        }
        public PlayerInfo(String name, int money)
            : this(name)
        {
            m_MoneySafeAmnt = money;
            m_MoneyInitAmnt = money;
        }
        public PlayerInfo(int seat, String name, int money):
            this(name, money)
        {
            m_NoSeat = seat;
        }

        public uint EvaluateCards(GameCard[] boardCards)
        {
            try
            {
                if (boardCards == null || boardCards.Length != 5 || m_Cards == null || m_Cards.Length != 2)
                    return 0;
                string pocket = String.Format("{0} {1}", m_Cards[0], m_Cards[1]);
                string board = String.Format("{0} {1} {2} {3} {4}", boardCards[0], boardCards[1], boardCards[2], boardCards[3], boardCards[4]);

                Hand phand = new Hand(pocket, board);
                this.HandValue = phand.HandValue;
                this.HandType = phand.HandTypeValue;

                return (uint)HandValue;
            }
            catch (ArgumentException ex) {

                HandValue = 0;
                HandType =Hand.HandTypes.HighCard;
                return (uint)HandValue;
            }
        }

        public ulong EvaluateCardsMaskValue(GameCard[] boardCards)
        {
            if (boardCards == null || boardCards.Length != 5 || m_Cards == null || m_Cards.Length != 2)
                return 0;
            string pocket = String.Format("{0} {1}", m_Cards[0], m_Cards[1]);
            string board = String.Format("{0} {1} {2} {3} {4}", boardCards[0], boardCards[1], boardCards[2], boardCards[3], boardCards[4]);

            return new Hand(pocket, board).MaskValue;
        }

        public bool CanBet(long amnt)
        {
            return amnt <= m_MoneySafeAmnt;
        }

        public bool TryBet(long amnt)
        {
            if (!CanBet(amnt))
            {
                return false;
            }
            m_MoneySafeAmnt -= amnt;
            m_MoneyBetAmnt += amnt;
            return true;
        }


        public long GetOnlineAwards() {
            if (DateTime.Now>=LastOnlineAwardTimestamp.AddSeconds(160))
            {
                OnlineAwardTimes++;
                long onlineawards = 0;
                onlineawards =Convert.ToInt64(MoneyTotalAmnt * (0.01 + 0.01 * OnlineAwardTimes));
                if (onlineawards < 100)
                    onlineawards = 100;
                if (onlineawards > 10000)
                    onlineawards = 10000;
                LastOnlineAwardTimestamp = DateTime.Now;
                return onlineawards;
            }
            return 0;
        }

        public bool CanGetOnlineAwards() {
            return DateTime.Now >= LastOnlineAwardTimestamp.AddSeconds(180);
        }
        public void ResetStatus() {
            LastOnlineAwardTimestamp = DateTime.Now;
            OnlineAwardTimes = 0;
            MoneyTotalAmnt = Chips;
            m_MoneyBetAmnt = 0;
            autofold = 0;
        }
    }
}
