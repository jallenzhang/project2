using System;
using System.Collections.Generic;
using System.Text;

namespace PokerWorld.Game
{
    public interface IPokerViewer
    {
        void SetGame(IPokerGame c, int s);
        void Start();
    }
}
