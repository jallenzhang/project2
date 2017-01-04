using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using LilyServer.Model;
using System.Web.Caching;
using DataPersist;
using ExitGames.Logging;

namespace LilyServer.Helper
{
    public class ConfigurationHelp
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();


        public static double SENDCHIPSTAX = 0.003;
        public static double GAMETAX = 0;
        public static long BROADCASTFEE = 0;
        public static long RegisterAwards = 3000;
        public static long RegisterAwardsGuest = 1000;
        public static long UpgradeAwards = 500;
        public static long CreateGameFEE = 24;
        public static string SystemNotice = "健康游戏，快乐生活！欢迎关注我们的微博，地址：";
        public static double GameWaitingTime = 5000d;

        public static string ClientVersion = "1.0.0.0";


        private const string CACHKEY1 = "LilySystemConfig";
        private const string CACHKEY2 = "LilyRobotStrategy";

        private static string DependencyDB = ConfigurationManager.AppSettings["DependencyDB"];
        private static string RobotTable = ConfigurationManager.AppSettings["RobotTable"];
        private static string SystemTable = ConfigurationManager.AppSettings["SystemTable"];



        public static List<Props> AllProps;

        public static int GetRowLimit()
        {
            return ConfigurationManager.AppSettings["RowsLimit"].getInt32();
        }


        static ConfigurationHelp() {
            var obj = CacheHelper.Get(CACHKEY1);
            if (obj==null)
            {
                buildConfigCache();
            }
        }


        private static void OnCacheRemoved(string key,object value,CacheItemRemovedReason r) { 
            //CacheHelper.Get(

            //log.DebugFormat("%%%%%%%%%%%%%%%%% cache removed  key : {0} , reason : {1}",key,r);
            buildConfigCache();
        }



        private static void buildConfigCache() {
            var obj = CacheHelper.Get(CACHKEY1);
            if (obj != null) return;
            string cacheKey = CACHKEY1;
            List<configSystem> cs = ConfigService.getInstance().configSystem();
            try
            {
                SqlCacheDependency dependecy = new SqlCacheDependency(DependencyDB, SystemTable);
                CacheHelper.Insert(cacheKey, cs, dependecy, CacheItemPriority.Normal, OnCacheRemoved);
            }
            catch (Exception ex)
            {
                //log

                CacheHelper.Insert(cacheKey, cs, 2 * 60 * 60, CacheItemPriority.Normal, OnCacheRemoved);
            }

            SystemNotice = (from _c in cs
                                where _c.id==(int)CacheKeys.SystemNotice
                                select _c).FirstOrDefault().valuestr;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'SystemNotice' is set to : {0}", SystemNotice);

            SENDCHIPSTAX = (double)(from _c in cs
                            where _c.id == (int)CacheKeys.SendChipsFee
                            select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'SENDCHIPSTAX' is set to : {0}", SENDCHIPSTAX);

            GAMETAX = (double)(from _c in cs
                               where _c.id == (int)CacheKeys.GameTax
                               select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'GAMETAX' is set to : {0}", GAMETAX);

            BROADCASTFEE = (long)(from _c in cs
                                    where _c.id == (int)CacheKeys.BroadcastMessage
                                    select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'BROADCASTFEE' is set to : {0}", BROADCASTFEE);

            RegisterAwards = (long)(from _c in cs
                                    where _c.id == (int)CacheKeys.RegisterAwards
                                    select _c).FirstOrDefault().value;

            RegisterAwardsGuest = (long)(from _c in cs
                                         where _c.id == (int)CacheKeys.RegisterAwardsGuest
                                         select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'RegisterAwards' is set to : {0}", RegisterAwards);
            UpgradeAwards = (long)(from _c in cs
                                   where _c.id == (int)CacheKeys.UpgradeAwards
                                    select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'UpgradeAwards' is set to : {0}", UpgradeAwards);

            CreateGameFEE = (long)(from _c in cs
                                   where _c.id == (int)CacheKeys.CreateGame
                                   select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'CreateGameFEE' is set to : {0}", CreateGameFEE);
            GameWaitingTime = (double)(from _c in cs
                                     where _c.id == (int)CacheKeys.GameWaitingTime
                                     select _c).FirstOrDefault().value;

            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'GameWaitingTime' is set to : {0}", GameWaitingTime);


            ClientVersion = (from _c in cs
                                       where _c.id == (int)CacheKeys.ClientVersion
                                       select _c).FirstOrDefault().valuestr;
            //log.DebugFormat("%%%%%%%%%%%%%%%%% property 'ClientVersion' is set to : {0}", ClientVersion);
        }


        public static List<configRobotStrategy> getRobotStrategy() {

            List <configRobotStrategy> strategies;
            string cacheKey = CACHKEY2;
            strategies=(List <configRobotStrategy>)CacheHelper.Get(cacheKey);
            if (strategies==null)
            {
                strategies = ConfigService.getInstance().RobotStrategy();
                try
                {
                    SqlCacheDependency dependecy = new SqlCacheDependency(DependencyDB, RobotTable);
                    CacheHelper.Insert(cacheKey, strategies, dependecy);
                }
                catch(Exception ex) { 
                    //log
                    CacheHelper.Insert(cacheKey, strategies, 2 * 60 * 60);
                }
            }
            return strategies;
        }

        //public static string getUpgradeUrl() {
        //    string url = string.Empty;
        //    url = ConfigService.getInstance().upgradeurl();

        //    return url;
        //}

    }
}
