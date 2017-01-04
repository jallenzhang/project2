using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Logging;
using LilyServer.Model;
using LilyServer.Helper;
using DataPersist;
using System.Data.Objects;

namespace LilyServer
{   
    public class UserMessageService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        //private LilyEntities lilyEntities = new LilyEntities();
        private object lockObj = new object();

        private static UserMessageService userMessageService=null;
        public static UserMessageService Singleton { 
            get 
            {
                if (userMessageService == null)
                {
                    userMessageService = new UserMessageService();
                }
                return userMessageService; 
            } 
        }

        public UserMessageService()
        {
            //lilyEntities.usermessage.MergeOption = MergeOption.OverwriteChanges;
        }

        public void SaveMessage(string senderId, string receiverId, MessageType messageType)
        {
            SaveMessage(senderId, receiverId, messageType,"");
        }

        public void SaveMessage(string senderId,string receiverId,MessageType messageType,string content)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                if (messageType == MessageType.RequestFriend &&
                lilyEntities.usermessage.Any(rs => rs.sender == senderId && rs.receiver == receiverId))
                {
                    return;
                }

                if (receiverId.StartsWith("2_"))
                {
                    return;
                }

                usermessage message = new usermessage
                {
                    sender = senderId,
                    receiver = receiverId,
                    messagetype = (byte)messageType,
                    content = content,
                    time = DateHelper.GetNow()
                };
                lock (lockObj)
                {
                    lilyEntities.usermessage.AddObject(message);
                    lilyEntities.SaveChanges();
                }
            }
            
        }


        public bool TryGetMessages(string receiverId) {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                usermessage userMessages = lilyEntities.usermessage.FirstOrDefault(rs => rs.receiver == receiverId && rs.messagetype == (byte)MessageType.AppScore);
                if (userMessages == null)
                {
                    return false;
                }
                lilyEntities.usermessage.DeleteObject(userMessages);
                lock (lockObj)
                {
                    lilyEntities.SaveChanges();
                }
                return true;
            }            
        }

        public bool TryGetMessages(string receiverId,out List<UserMessage> messages)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                messages = new List<UserMessage>();
                if (lilyEntities.usermessage.Any(rs => rs.receiver == receiverId))
                {
                    List<usermessage> userMessages = lilyEntities.usermessage.Where(rs => rs.receiver == receiverId).ToList();
                    int count = userMessages.Count;
                    for (int i = 0; i < count; i++)
                    {
                        usermessage message = userMessages[i];
                        UserMessage userMessage = new UserMessage();
                        userMessage.MessageType = (MessageType)message.messagetype;
                        userMessage.Content = message.content;
                        userMessage.Sender = lilyEntities.user.FirstOrDefault(rs => rs.userid == message.sender).ToUserData();
                        messages.Add(userMessage);
                        lilyEntities.usermessage.DeleteObject(message);
                    }
                    lock (lockObj)
                    {
                        lilyEntities.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
