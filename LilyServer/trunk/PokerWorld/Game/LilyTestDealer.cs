using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using DataPersist;
using System.IO;

namespace PokerWorld.Game
{
    public class LilyTestDealer : AbstractDealer
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
            //m_Deck = GameCardUtility.GetShuffledDeck(false);
            m_Deck = GetCustomDeck(false);
            m_Cheat = new GameCard[5];
        }
        public override void SetCheat()
        {
            m_Cheat[0] = m_Deck.Pop();
            m_Cheat[1] = m_Deck.Pop();
            m_Cheat[2] = m_Deck.Pop();
            m_Cheat[3] = m_Deck.Pop();
            m_Cheat[4] = m_Deck.Pop();
        }

        private Stack<GameCard> GetCustomDeck(bool jokers)
        {
            string path = System.Environment.CurrentDirectory + "\\bin\\Dealer\\dealer.txt";
            List<GameCard> deck = new List<GameCard>();
            StreamReader sr = File.OpenText(path);
            string s = sr.ReadLine();
            while (s != null)
            {
                string[] sa = s.Split(' ');
                int id = Convert.ToInt32(sa[1]);
                GameCard card = new GameCard(id);
                deck.Add(card);
                s = sr.ReadLine();
            }
            sr.Close();            
            //for (int i = 0; i < 4; ++i)
            //    for (int j = 0; j < 13; ++j)
            //        deck.Add(new GameCard((GameCardKind)i, (GameCardValue)j));
            //if (jokers)
            //{
            //    deck.Add(new GameCard(GameCardSpecial.JokerColor));
            //    deck.Add(new GameCard(GameCardSpecial.JokerDark));
            //}
            Stack<GameCard> deckStack = new Stack<GameCard>();

            while (deck.Count>0)
            {
                int id = deck.Count - 1;
                deckStack.Push(deck[id]);
                deck.RemoveAt(id);
            }

            return deckStack;
        }
    }
}
