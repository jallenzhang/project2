using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
    public class Props
    {
        #region Constructor
        public Props()
		{

		}
		#endregion

        #region public Properties
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
        public int ItemId { get; set;}
        public int Duration { get; set; }
        public string ItemName { get; set; }
        #endregion
    }
}
