using System;
using System.Collections.Generic;
using System.Text;
using EricUtility;

namespace DataPersist.CardGame
{
    public static class GameCardUtility
    {
        public static List<GameCard> GetSortedDeck(bool jokers)
        {
            List<GameCard> deck = new List<GameCard>();
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 13; ++j)
                    deck.Add(new GameCard((GameCardKind)i, (GameCardValue)j));
            if (jokers)
            {
                deck.Add(new GameCard(GameCardSpecial.JokerColor));
                deck.Add(new GameCard(GameCardSpecial.JokerDark));
            }
            return deck;
        }

        public static Stack<GameCard> GetShuffledDeck(bool jokers)
        {
            Stack<GameCard> deck = new Stack<GameCard>();
            List<GameCard> restantes = GetSortedDeck(jokers);
            while (restantes.Count > 0)
            {
                int id = Hasard.RandomWithMax(restantes.Count - 1);
                deck.Push(restantes[id]);
                restantes.RemoveAt(id);
            }
            return deck;
        }

        public static char ValueChar(GameCardValue v)
        {
            string chars = "23456789TJQKA";
            return chars[(int)v];
        }

        public static char KindChar(GameCardKind k)
        {
            string chars = "cdhs";
            return chars[(int)k];
        }

        public static string SpecialStr(GameCardSpecial s)
        {
            string[] strs = { "", "--", "??", "xj", "sj" };
            return strs[(int)s];
        }
    }
}
