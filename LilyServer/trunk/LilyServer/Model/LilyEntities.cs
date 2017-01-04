using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Configuration;

namespace LilyServer.Store
{
    public class LilyEntities : ObjectContext
    {
        private static String connString = ConfigurationManager.ConnectionStrings["LilyEntities"].ConnectionString;

        private static LilyEntities _instance = null;

        /// <summary>
        /// Initializes a new express_empireEntities object using the connection string found in the 'express_empireEntities' section of the application configuration file.
        /// </summary>
        public LilyEntities()
            : base(connString, "LilyEntities") //base("name=express_empireEntities", "express_empireEntities")
        {
            try
            {
                _users = CreateObjectSet<user>();
                _friends = CreateObjectSet<friend>();
                _feedbacks = CreateObjectSet<feedback>();
            }
            catch (Exception ex)
            {
                throw ex;               
            }
        }

        public static LilyEntities getInstance()
        {
            if (_instance == null) _instance = new LilyEntities();

            return _instance;
        }

        public ObjectSet<user> Users
        {
            get
            {
                return _users;
            }
        }
        private ObjectSet<user> _users;

        public ObjectSet<friend> Friends
        {
            get
            {
                return _friends;
            }
        }
        private ObjectSet<friend> _friends;

        public ObjectSet<feedback> Feedbacks
        {
            get
            {
                return _feedbacks;
            }
        }
        private ObjectSet<feedback> _feedbacks;
    }
}
