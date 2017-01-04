using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
   public class PlayerAction
   {
        public int Id { get; set; }
        public int TableRound { get; set; }
        public TypeAction TypeAction { get;  set; }
        public int NoSeat { get;  set; }
        public long AmountPlayed { get;  set; }
        public long MoneySafeAmnt { get;  set; }
        public TableInfo Table { get; set; }

        public PlayerAction() { }

        public PlayerAction(int id,TypeAction typeaction,int noseat,long amountplayed,long moneysafeamnt) {
            this.Id = id;
            this.TypeAction = typeaction;
            this.NoSeat = noseat;
            this.AmountPlayed = amountplayed;
            this.MoneySafeAmnt = moneysafeamnt;
        }
        public PlayerAction(int id, TypeAction typeaction, int noseat, long amountplayed, long moneysafeamnt,TableInfo table,int tableround)
            :this(id,typeaction,noseat,amountplayed,moneysafeamnt)
        {
            this.Table = table;
            this.TableRound = tableround;
        }

    }
}
