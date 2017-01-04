using System;
using System.Collections.Generic;
using System.Text;
using DataPersist.CardGame;
using DataPersist;

namespace PokerWorld.Game
{
    public class TableInfoCareer: TableInfo
    {

        public TableInfoCareer()
            : base()
        {
        }

        public TableInfoCareer(int nbSeats)
            : base(nbSeats)
        {
        }

        public TableInfoCareer(string name, int bigBlind, int nbSeats, TypeBet limit)
            : base(name, bigBlind, nbSeats, limit)
        {
        }
    }
}
