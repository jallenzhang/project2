using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DataPersist;

namespace PokerWorld.Game
{
    public class PokerGameCareer : PokerGame
    {
        public TableInfoCareer CareerTable
        {
            get { return (TableInfoCareer)m_Table; }
        }
        public PokerGameCareer()
            : base()
        {
        }
        public PokerGameCareer(AbstractDealer dealer) :
            base(dealer)
        {
        }

        public PokerGameCareer(TableInfoCareer table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base( table,  wtaPlayerAction,  wtaBoardDealed,  wtaPotWon)
        {
        }

        public PokerGameCareer(AbstractDealer dealer, TableInfoCareer table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base(dealer,table, wtaPlayerAction, wtaBoardDealed, wtaPotWon)
        {
        }
    }
}
