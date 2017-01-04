using System;
using System.Collections.Generic;
using System.Text;
using DataPersist;

namespace PokerWorld.Game
{
    public class PlayerInfoEventArgs : EventArgs
    {
        private readonly PlayerInfo m_Player;
        public PlayerInfo Player { get { return m_Player; } }
        public PlayerLeaveType LeaveType { get; set; }

        public string LeaveMessage { get; set; }

        public PlayerInfoEventArgs(PlayerInfo p)
        {
            m_Player = p;
            LeaveType = PlayerLeaveType.Leave;
        }
        public PlayerInfoEventArgs(PlayerInfo p, PlayerLeaveType s)
            :this(p,s,string.Empty)
        {
            
        }

        public PlayerInfoEventArgs(PlayerInfo p, PlayerLeaveType s,string leavemessage)
        {
            m_Player = p;
            LeaveType = s;
            LeaveMessage = leavemessage;
        }                
    }
    public class PlayerMoneyChangedEventArgs : PlayerInfoEventArgs
    {
        public long ChangedAmnt { get; set; }
        public PlayerMoneyChangedEventArgs(PlayerInfo p,long changedamnt) 
            :base(p)
        {
            ChangedAmnt = changedamnt;
        }
    }

    public class HistoricPlayerInfoEventArgs : PlayerInfoEventArgs
    {
        private readonly PlayerInfo m_Last;
        public PlayerInfo Last { get { return m_Last; } }
        public long HighBet{get;set;}

        public HistoricPlayerInfoEventArgs(PlayerInfo p, PlayerInfo l,long highbet)
            : base(p)
        {
            m_Last = l;
            HighBet = highbet;
        }
    }
    public class RoundEventArgs : EventArgs
    {
        private readonly TypeRound m_Round;
        public TypeRound Round { get { return m_Round; } }

        public RoundEventArgs(TypeRound r)
        {
            m_Round = r;
        }
    }
    public class PlayerActionEventArgs : PlayerInfoEventArgs
    {
        private readonly TypeAction m_Action;
        private readonly long m_AmountPlayed;

        public TypeAction Action { get { return m_Action; } }
        public long AmountPlayed { get { return m_AmountPlayed; } }

        public PlayerActionEventArgs(PlayerInfo p, TypeAction action, long amnt)
            : base(p)
        {
            m_Action = action;
            m_AmountPlayed = amnt;
        }
    }
    public class PotWonEventArgs : PlayerInfoEventArgs
    {
        private readonly int m_Id;
        private readonly long m_AmountWon;

        public int Id { get { return m_Id; } }
        public long AmountWon { get { return m_AmountWon; } }

        public PotWonEventArgs(PlayerInfo p, int id, long amntWon)
            : base(p)
        {
            m_Id = id;
            m_AmountWon = amntWon;
        }
    }

    public class PotWonImproveEventArgs : EventArgs {
        public int Id { get; set; }
        public int[] Winner { get; set; }
        public long[] WinAmnt { get; set; }
        public int[] AttachedPlayer { get; set; }

        public PotWonImproveEventArgs(int id,int[] winner,long[] winamnt,int[] attachedplayer) {
            this.Id = id;
            this.Winner = winner;
            this.WinAmnt = winamnt;
            this.AttachedPlayer = attachedplayer;
        }
    }


    public class PlayersShowCardsArgs : EventArgs {
        public Dictionary<int, string> ShowPlayers;


    }

    public class GameEndEventArgs : EventArgs {
        public long gameTax { get; set; }

        public int Delay { get; set; }

        public GameEndEventArgs(long money,int delay) {
            this.gameTax = money;
            this.Delay = delay;
        }
    }
}
