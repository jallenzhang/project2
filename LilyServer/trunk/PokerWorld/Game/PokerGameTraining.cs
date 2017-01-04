using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PokerWorld.Game
{
    public class PokerGameTraining : PokerGame
    {
        public TableInfoTraining TrainingTable
        {
            get { return (TableInfoTraining)m_Table; }
        }
        public PokerGameTraining()
            : base()
        {
        }
        public PokerGameTraining(AbstractDealer dealer) :
            base(dealer)
        {
        }

        public PokerGameTraining(TableInfoTraining table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base( table,  wtaPlayerAction,  wtaBoardDealed,  wtaPotWon)
        {
        }

        public PokerGameTraining(AbstractDealer dealer, TableInfoTraining table, int wtaPlayerAction, int wtaBoardDealed, int wtaPotWon)
            : base(dealer,table, wtaPlayerAction, wtaBoardDealed, wtaPotWon)
        {
        }
    }
}
