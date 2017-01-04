using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Logging;
using LilyServer.Model;

namespace LilyServer
{
    public class ConfigService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static ConfigService _instance = null;
        public static ConfigService getInstance()
        {
            if (_instance == null) _instance = new ConfigService();
            return _instance;
        }


        public List<configRobotStrategy> RobotStrategy() {
            List<configRobotStrategy> result = new List<configRobotStrategy>();
            using (LilyEntities robotstrategy=new LilyEntities())
            {
                result = robotstrategy.configRobotStrategy.Execute(System.Data.Objects.MergeOption.NoTracking).ToList();
            }
            return result;
        }

        public List<configSystem> configSystem() {
            List<configSystem> result = new List<configSystem>();
            using (LilyEntities robotstrategy = new LilyEntities())
            {
                result = robotstrategy.configSystem.Execute(System.Data.Objects.MergeOption.NoTracking).ToList();
            }
            return result;
        }

        public string upgradeurl(int id) {
            string result = string.Empty;
            using (LilyEntities robotstrategy = new LilyEntities())
            {
                result = robotstrategy.configSystem.FirstOrDefault(r => r.id ==id).valuestr;
            }
            return result;
        }

    }
}
