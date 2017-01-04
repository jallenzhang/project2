using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersist
{
    /// <summary>
    /// 
    /// </summary>
    public class MoneyPot
    {
        /************ VARIABLES MEMBRES ************/
        private int m_Id; // ID du pot
        private long m_Amount; // Quantite d'argent
        private readonly List<PlayerInfo> m_AttachedPlayers = new List<PlayerInfo>(); // Liste de joueurs attaches


        public List<PlayerInfo> OriginalAttachedPlayers = new List<PlayerInfo>();

        /************ PROPRIETES ************/
        /// <summary>
        /// ID du pot
        /// </summary>
        public int Id
        {
            get { return m_Id; }
			set { m_Id=value;}
        }

        /// <summary>
        /// Quantite d'argent
        /// </summary>
        public long Amount
        {
            get { return m_Amount; }
            set { m_Amount = value; }
        }

        /// <summary>
        /// Liste de joueurs attaches
        /// </summary>
        public PlayerInfo[] AttachedPlayers
        {
            get { return m_AttachedPlayers.ToArray(); }
        }

        /************ CONSTRUCTEURS ************/
		public MoneyPot()
		{
		}
        /// <summary>
        /// MoneyPot avec amount initial a 0
        /// </summary>
        /// <param name="id">ID du pot</param>
        public MoneyPot(int id)
            : this(id, 0)
        {
        }
		

        /// <summary>
        /// MoneyPot avec amount initial
        /// </summary>
        /// <param name="id">ID du pot</param>
        /// <param name="amount">Quantite d'argent</param>
        public MoneyPot(int id, long amount)
        {
            m_Id = id;
            m_Amount = amount;
        }

        /************ METHODES ************/
        /// <summary>
        /// Attache un joueur au POT
        /// </summary>
        /// <param name="p"></param>
        public void AttachPlayer(PlayerInfo p)
        {
            m_AttachedPlayers.Add(p);
        }

        /// <summary>
        /// Detache un joueur du POT
        /// </summary>
        /// <param name="p"></param>
        public void DetachPlayer(PlayerInfo p)
        {
            m_AttachedPlayers.Remove(p);
        }

        /// <summary>
        /// Detache tous les joueurs du POT
        /// </summary>
        public void DetachAllPlayers()
        {
            m_AttachedPlayers.Clear();
        }

        /// <summary>
        /// Ajoute un montant d'argent au POT
        /// </summary>
        /// <param name="added"></param>
        public void AddAmount(long added)
        {
            m_Amount += added;
        }
    }
}
