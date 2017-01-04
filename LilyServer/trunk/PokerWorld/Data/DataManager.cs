using System;
using System.Collections.Generic;
using System.Text;

namespace PokerWorld.Data
{
    public static class DataManager
    {
        public static IDataPersistance Persistance = new DummyPersistance();
    }
}
