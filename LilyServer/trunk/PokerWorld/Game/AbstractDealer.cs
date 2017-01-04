using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using DataPersist;

namespace PokerWorld.Game
{
    public abstract class AbstractDealer
    {
        protected Stack<GameCard> m_Deck;
        public GameCard[] m_Cheat;
       
        public AbstractDealer()
        {
            FreshDeck();
        }

        public abstract GameCard[] DealHoles(PlayerInfo p);
        public abstract GameCard[] DealFlop();
        public abstract GameCard DealTurn();
        public abstract GameCard DealRiver();

        public abstract void FreshDeck();

        public abstract void SetCheat();
    }
}
