using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using DataPersist;

namespace PokerWorld.Game
{
    public class RandomDealer : AbstractDealer
    {
        public override GameCard[] DealHoles(PlayerInfo p)
        {
            GameCard[] set = new GameCard[2];
            set[0] = m_Deck.Pop();
            set[1] = m_Deck.Pop();
            return set;
        }

        public override GameCard[] DealFlop()
        {
            GameCard[] set = new GameCard[3];
            //set[0] = m_Deck.Pop();
            //set[1] = m_Deck.Pop();
            //set[2] = m_Deck.Pop();
            if (m_Cheat[0]==null||m_Cheat[0].Id < 0)
                SetCheat();
            set[0] = m_Cheat[0];
            set[1] = m_Cheat[1];
            set[2] = m_Cheat[2];
            return set;
        }

        public override GameCard DealTurn()
        {
            //return m_Deck.Pop();
            return m_Cheat[3];
        }

        public override GameCard DealRiver()
        {
            //return m_Deck.Pop();
            return m_Cheat[4];
        }

        public override void FreshDeck()
        {
            m_Deck = GameCardUtility.GetShuffledDeck(false);
            m_Cheat = new GameCard[5];
        }

        public override void SetCheat() {
            m_Cheat[0] = m_Deck.Pop();
            m_Cheat[1] = m_Deck.Pop();
            m_Cheat[2] = m_Deck.Pop();
            m_Cheat[3] = m_Deck.Pop();
            m_Cheat[4] = m_Deck.Pop();
        }

    }
}
