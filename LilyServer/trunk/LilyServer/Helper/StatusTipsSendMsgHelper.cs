using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using DataPersist;
using LilyServer.Events;
using DataPersist.HelperLib;
using Lite;
using ExitGames.Logging;

namespace LilyServer.Helper
{
    public class StatusTipsSendMsgHelper
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private PoolFiber ExecutionFiber { get; set; }
        private List<StatusTipsType> timelyMsgList = new List<StatusTipsType>();
        private static readonly object padlock = new object();
        private static StatusTipsSendMsgHelper myHelper = null;
        private static long intervalTime = 2 * 1000; // 2 seconds
        private static Queue<Hashtable> tipsTask = new Queue<Hashtable>();

        public static StatusTipsSendMsgHelper Instance
        {
            get
            {
                if (myHelper == null)
                {
                    lock (padlock)
                    {
                        if (myHelper == null) {
                            myHelper = new StatusTipsSendMsgHelper();
                        }
                    }
                }
                return myHelper;
            }
        }

        private StatusTipsSendMsgHelper()
        {
            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();

            timelyMsgList.Add(StatusTipsType.FriendUp);
            this.ExecutionFiber.ScheduleOnInterval(this.SendMsgOnInterval, 0, intervalTime);        
        }

        public void SendMessage(StatusTipsType tipsType, params object[] tipsParams)
        {
            if (timelyMsgList.Contains(tipsType)) {
                SendMsgTimely(tipsType, tipsParams);                
            } else {
                StoreCurrentParams(tipsType, tipsParams);
            }            
        }

        private void SendMsgTimely(StatusTipsType tipsType, params object[] tipsParams)
        { 
            if(tipsType == StatusTipsType.FriendUp){
                ExecutionFiber.Enqueue(() => this.SendMsgFirendUp(tipsParams));
            }            
        }

        private void StoreCurrentParams(StatusTipsType tipsType, params object[] tipsParams)
        { 
            Hashtable ht = new Hashtable();
            ht.Add(0, tipsType);
            ht.Add(1, tipsParams);
            tipsTask.Enqueue(ht);
        }

        private void SendMsgOnInterval() 
        {
            if (tipsTask.Count > 0) {
                Hashtable ht = tipsTask.Dequeue();
                StatusTipsType tipsType = (StatusTipsType)ht[0];
                object[] tipsParams = (object[])ht[1];
                this.NoticeToAllActors(tipsType, tipsParams);
            }
        }

        private void NoticeToAllActors(StatusTipsType tipsType, params object[] tipsParams)
        {
            StatusTipsSendEvent e = new StatusTipsSendEvent(tipsType, tipsParams);
            EventData eventData = new EventData(e.Code, e);            

            eventData.SendTo(LilyServer.Actors.Select(rs => rs.Peer), GetCustomSendParameters());
        }

        /// <summary>
        /// index 0: userId;
        /// </summary>
        /// <param name="tipsParams"></param>
        private void SendMsgFirendUp(params object[] tipsParams)
        {
            string userid = tipsParams[0] as string;
            if (string.IsNullOrEmpty(userid)) return;

            // get all friend user data which is on line
            string user_nickname = UserService.getInstance().QueryUserByUserId(userid).nickname;
            var myFriendIdList = FriendService.getInstance().GetFriends(userid).Select(rs => rs.userid);

            var actorList = LilyServer.Actors.FindAll(rs => myFriendIdList.Contains((rs.Peer as LilyPeer).UserId));
            if (actorList == null || actorList.Count == 0) return;

            string[] myStrArray = new string[] { user_nickname };
            StatusTipsSendEvent e = new StatusTipsSendEvent(StatusTipsType.FriendUp, myStrArray);

            EventData eventData = new EventData(e.Code, e);
            eventData.SendTo(actorList.Select(rs => rs.Peer), GetCustomSendParameters());
        }

        private SendParameters GetCustomSendParameters() {
            SendParameters myParameters = new SendParameters()
            {
                ChannelId = 1,
                Unreliable = true
            };

            return myParameters;
        }
    }
}
