using System;
using System.Collections.Generic;
using DataPersist;

namespace PokerWorld.Game
{
    public interface IPokerGame
    {
        event EventHandler EverythingEnded; //suo you de ren dou cong zhuo shang li kai le.
        event EventHandler GameBlindNeeded; //yi chang game kai shi, chu shi hua zhuo zi, chu shi hua da xiao mang zhu he zhuang jia weizhi.
        event EventHandler<GameEndEventArgs> GameEnded;       //fen chu sheng fu zhi hou, fen wan qian, yi chang game jie su.
        event EventHandler GameGenerallyUpdated;//??

        event EventHandler<RoundEventArgs> GameBettingRoundStarted;
        event EventHandler<RoundEventArgs> GameBettingRoundEnded;

        event EventHandler<PlayerInfoEventArgs> PlayerJoined;
        event EventHandler<PlayerInfoEventArgs> PlayerLeaved;
        event EventHandler<HistoricPlayerInfoEventArgs> PlayerActionNeeded;
        event EventHandler<PlayerMoneyChangedEventArgs> PlayerMoneyChanged;
        event EventHandler<PlayerInfoEventArgs> PlayerHoleCardsChanged;

        event EventHandler<PlayerActionEventArgs> PlayerActionTaken;

        event EventHandler<PotWonEventArgs> PlayerWonPot;

        TableInfo Table { get; }

        bool PlayMoney(PlayerInfo p, long amnt);
        bool LeaveGame(PlayerInfo p);

        string Encode { get; }
    }
}
