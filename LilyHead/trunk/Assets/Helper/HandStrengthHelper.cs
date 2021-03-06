using UnityEngine;
using System.Collections;
using PokerWorld.HandEvaluator;
using System.Text;
using DataPersist.CardGame;
using System;

public enum HandTypes
{
    HighCard = 0,
	
    Pair,
    
	TwoPair,
    
	Trips,
    
	Straight,
    
	Flush,
    
	FullHouse,
    
	FourOfAKind,
    
	StraightFlush,
	
	BiggestStraightFlush,
	
	Error
}

public class HandStrengthHelper {
	
	private const uint biggestHandValue = 135004160;

	public static HandTypes GetCurrentHandStrength(GameCard[] pocketCards, GameCard[] boardCards)
	{
		if (pocketCards == null || pocketCards.Length == 0)
			return HandTypes.Error;
		
		if (boardCards == null)
			return GetCurrentHandStrength(pocketCards);
		
		StringBuilder builder = new StringBuilder();
		const string blanckString = " ";
		string pocket = string.Empty;
		string board = string.Empty;
		
		if (pocketCards.Length > 1)
		{
			builder.Append(pocketCards[0].ToString());
			for (int i = 1; i < pocketCards.Length; i++)
			{
				if (pocketCards[i].ToString() == "--")
					continue;
				builder.Append(blanckString);
				builder.Append(pocketCards[i].ToString());
			}
			
			pocket = builder.ToString();
		}
		
		Debug.Log("here the pocket is: " + pocket);
		if (pocket == string.Empty || pocket == null)
		{
			return HandTypes.Error;
		}
		
		builder.Remove(0, builder.Length);		
		
		if (boardCards.Length > 1)
		{
			builder.Append(boardCards[0]);
			for (int i = 1; i < boardCards.Length; i++)
			{
				if (boardCards[i].ToString() == "--")
					continue;
				builder.Append(blanckString);
				builder.Append(boardCards[i].ToString());
			}
			board = builder.ToString();
		}
		
		//Debug.Log("here the table cards is : " + board);
		if (board == "--" || board == null)
			board = string.Empty;
		
		if (pocket == "--" || pocket == null)
			return HandTypes.Error;
		
		Hand hand = new Hand(pocket, board);
		
		int type = (int)hand.HandTypeValue;
		
		if (type == (int)HandTypes.StraightFlush)
		{
			if (hand.HandValue == biggestHandValue)
				type = (int)HandTypes.BiggestStraightFlush;
		}
		
		return (HandTypes)type;
	}
	
	public static HandTypes GetCurrentHandStrength(GameCard[] pocketCards)
	{
		int suit1 = 0, suit2=0;
		int rank1 = 0, rank2=0;
		
		if (pocketCards.Length == 2 && pocketCards[0].ToString() != "--" && pocketCards[1].ToString() != "--")
		{
			rank1 = AnalyseCard(pocketCards[0].ToString(), ref suit1);
			rank2 = AnalyseCard(pocketCards[1].ToString(), ref suit2);
			
			
			if (rank1 < 0 || rank2 < 0 || suit1 < 0 || suit2 < 0)
				return HandTypes.Error;
			
			//level1
			if (Math.Abs(rank1 - rank2) > 1 
			&& suit1 != suit2
			&& rank1 != Hand.RankAce
			&& rank2 != Hand.RankAce)
			{
				return HandTypes.Pair;
			}
			
			//level2
			if (rank1 == Hand.RankAce || rank2 == Hand.RankAce)
			{
				if (Math.Abs(rank1 - rank2) > 1 && suit1 != suit2)
				{
					return HandTypes.Trips;
				}
			}
			else
			{
				if (suit1 == suit2 && rank1 != rank2)
				{
					return HandTypes.Trips;
				}
			}
			
			//level3
			if (rank1 == Hand.RankAce || rank2 == Hand.RankAce)
			{
				if (Math.Abs(rank1 - rank2) > 1 && suit1 == suit2)
				{
					return HandTypes.Flush;
				}
			}
			else
			{
				if (Math.Abs(rank1 - rank2) == 1)
				{
					return HandTypes.Flush;
				}
			}
			
			//level4
			if (rank1 == Hand.RankAce || rank2 == Hand.RankAce)
			{
				if (suit1 != suit2 && (rank1 == Hand.RankKing || rank2 == Hand.RankKing))
				{
					return HandTypes.FourOfAKind;
				}
			}
			else
			{
				if (rank1 == rank2)
				{
					return HandTypes.FourOfAKind;
				}
			}
			
			//level5
			if (rank1 == Hand.RankAce || rank2 == Hand.RankAce)
			{
				if (suit1 == suit2 && Math.Abs(rank1 - rank2) == 1)
				{
					return HandTypes.BiggestStraightFlush;
				}
				else if (rank1 == rank2)
				{
					return HandTypes.BiggestStraightFlush;
				}
			}
		}
		
		return HandTypes.Error;
	}
	
	
	private static int AnalyseCard(string cards, ref int suit)
    {
        int rank = 0;
		int index = 0;

        // Remove whitespace
        while (index < cards.Length && cards[index] == ' ')
            index++;

        if (index >= cards.Length)
            return -1;

        // Parse cards
        if (index < cards.Length)
        {
            switch (cards[index++])
            {
                case '1':
                    try
                    {
                        if (cards[index] == '0')
                        {
                            index++;
                            rank = Hand.RankTen;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("Bad hand string");
                    }
                    break;
                case '2':
                    rank = Hand.Rank2;
                    break;
                case '3':
                    rank = Hand.Rank3;
                    break;
                case '4':
                    rank = Hand.Rank4;
                    break;
                case '5':
                    rank = Hand.Rank5;
                    break;
                case '6':
                    rank = Hand.Rank6;
                    break;
                case '7':
                    rank = Hand.Rank7;
                    break;
                case '8':
                    rank = Hand.Rank8;
                    break;
                case '9':
                    rank = Hand.Rank9;
                    break;
                case 'T':
                case 't':
                    rank = Hand.RankTen;
                    break;
                case 'J':
                case 'j':
                    rank = Hand.RankJack;
                    break;
                case 'Q':
                case 'q':
                    rank = Hand.RankQueen;
                    break;
                case 'K':
                case 'k':
                    rank = Hand.RankKing;
                    break;
                case 'A':
                case 'a':
                    rank = Hand.RankAce;
                    break;
                default:
                    return -2;
            }
        }
        else
        {
            return -2;
        }

        if (index < cards.Length)
        {
            switch (cards[index++])
            {
                case 'H':
                case 'h':
                    suit = Hand.Hearts;
                    break;
                case 'D':
                case 'd':
                    suit = Hand.Diamonds;
                    break;
                case 'C':
                case 'c':
                    suit = Hand.Clubs;
                    break;
                case 'S':
                case 's':
                    suit = Hand.Spades;
                    break;
                default:
                    return -2;
            }
        }
        else
        {
            return -2;
        }

        return rank;
    }
}
