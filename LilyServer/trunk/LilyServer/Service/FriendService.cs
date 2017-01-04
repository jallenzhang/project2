using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LilyServer.Model;
using DataPersist;
using ExitGames.Logging;
using LilyServer.Helper;
using System.Collections;

namespace LilyServer
{
    internal class FriendService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        //private LilyEntities lilyEntities = new LilyEntities();
        private object lockObj = new object();

        private static FriendService _instance = null;

        public static FriendService getInstance()
        {
            if (_instance == null) { _instance = new FriendService(); 
            }

            return _instance;
        }

        public FriendService() {
            //lilyEntities.user.MergeOption = System.Data.Objects.MergeOption.NoTracking;
        }

        public Dictionary<byte, object> CheckFriend(string userA,string userB) {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            using (LilyEntities lilyEntities = new LilyEntities())
            {
                try
                {
                    int count = (from _f in lilyEntities.friend
                                 where
                                     (_f.userA.CompareTo(userA) == 0 && _f.userB.CompareTo(userB) == 0) ||
                                     (_f.userA.CompareTo(userB) == 0 && _f.userB.CompareTo(userA) == 0)
                                 select _f).Count<friend>();
                    if (count > 0)
                    {
                        dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.FriendExist);
                        return dic;
                    }
                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.Sucess);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.SystemError);
                }
            }
            return dic;
        }

        public Dictionary<byte, object> Add(FriendData data)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            using (LilyEntities lilyEntities = new LilyEntities())
            {
                try
                {
                    log.InfoFormat("FriendService Add UserA={0},UserB={1}", data.UserA, data.UserB);
                    //using (LilyEntities lilyEntities = new LilyEntities())
                    //{

                    int count = (from _f in lilyEntities.friend
                                 where
                                     (_f.userA.CompareTo(data.UserA) == 0 && _f.userB.CompareTo(data.UserB) == 0) ||
                                     (_f.userA.CompareTo(data.UserB) == 0 && _f.userB.CompareTo(data.UserA) == 0)
                                 select _f).Count<friend>();
                    if (count > 0)
                    {
                        dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.FriendExist);
                        return dic;
                    }

                    friend f = new friend {userB = data.UserB, userA = data.UserA};

                    lock (lockObj)
                    {
                        lilyEntities.AddObject("friend", f);
                        lilyEntities.SaveChanges();
                    }

                    user u = (from _u in lilyEntities.user
                              where _u.userid.CompareTo(data.UserB) == 0
                              select _u).Single<user>();
                    UserData userData = u.ToUserData();
                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.Sucess);
                    dic.Add((byte) LilyOpKey.UserData, userData.Tobyte());
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.SystemError);
                }
            }

            return dic;
        }      

        public Dictionary<byte, object> Search(string nickName,string userId)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            using (LilyEntities lilyEntities = new LilyEntities())
            {
                try
                {
                    log.InfoFormat("FriendService Search nickName={0}", nickName);

                    int rowLimit = ConfigurationHelp.GetRowLimit();

                    int status = UserStatus.Offline.getInt32Value();

                    // List<user> users = (from _u in _entities.Users
                    //                     where _u.nickname.Contains(nickName) && _u.status != status
                    // select _u).Take<user>(rowLimit).ToList<user>();
                    List<user> users = null;
                    //using (LilyEntities lilyEntities = new LilyEntities())
                    //{
                    //users = (from actor in LilyServer.Actors
                    //            join u in lilyEntities.user on (actor.Peer as LilyPeer).UserId equals u.userid
                    //                where u.nickname.Contains(nickName) && u.userid != userId &&
                    //                !(from friendData in lilyEntities.friend where friendData.userA==userId select friendData.userB).Contains(u.userid) &&
                    //                !(from friendData in lilyEntities.friend where friendData.userB==userId select friendData.userA).Contains(u.userid)
                    //                select u).ToList();
                    //lilyEntities.user.MergeOption = System.Data.Objects.MergeOption.NoTracking;               

                    users = lilyEntities.user.Where(rs =>
                                                    //(rs.usertype??0)!=(byte)UserType.Guest&&
                                                    (rs.userid != userId) &&
                                                    !(from friendData in lilyEntities.friend
                                                      where friendData.userA == userId
                                                      select friendData.userB).Contains(rs.userid) &&
                                                    !(from friendData in lilyEntities.friend
                                                      where friendData.userB == userId
                                                      select friendData.userA).Contains(rs.userid) &&
                                                    rs.nickname.ToLower().Contains(nickName.ToLower())).Take(rowLimit).
                        ToList();

                    //}
                    if (users.Count == 0)
                    {
                        dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.NoResult);
                        return dic;
                    }

                    List<UserData> datas = new List<UserData>(users.Count);

                    users.ForEach(u => datas.Add(u.ToUserData()));

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.Sucess);
                    dic.Add((byte) LilyOpKey.FriendList, datas.Tobyte());
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.SystemError);
                }
            }

            return dic;
        }

        public Dictionary<byte, object> List(string userId)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                log.InfoFormat("FriendService List userId={0}", userId);

                List<user> users = GetFriends(userId);

                if (users.Count == 0)
                {
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NoResult);
                    return dic;
                }

                List<UserData> datas = new List<UserData>(users.Count);

                users.ForEach(u => datas.Add(u.ToUserData()));

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.FriendList, datas.Tobyte());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        public List<user> GetFriends(string userId)
        {
            int rowLimit = ConfigurationHelp.GetRowLimit();
            List<user> users = null;
            //using (LilyEntities lilyEntities = new LilyEntities())
            //{
            
            //lilyEntities.user.MergeOption = System.Data.Objects.MergeOption.NoTracking; 
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                users = (from _u in lilyEntities.user
                         join _f in lilyEntities.friend
                             on _u.userid equals _f.userB
                         where _f.userA.CompareTo(userId) == 0
                         select _u).Union<user>(from _u in lilyEntities.user
                                                join _f in lilyEntities.friend
                                                    on _u.userid equals _f.userA
                                                where _f.userB.CompareTo(userId) == 0
                                                select _u).Take<user>(rowLimit).ToList<user>();
                //}
            }
            return users;
        }

        public Dictionary<byte, object> Access(string userId)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            using (LilyEntities lilyEntities = new LilyEntities())
            {
                try
                {
                    log.InfoFormat("FriendService Access userId={0}", userId);
                    user u = null;
                    //using (LilyEntities lilyEntities = new LilyEntities())
                    //{
                    u = (from _u in lilyEntities.user
                         where _u.userid.CompareTo(userId) == 0
                         select _u).Single<user>();
                    //}

                    UserData data = u.ToUserData();

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.Sucess);
                    dic.Add((byte) LilyOpKey.UserData, data.Tobyte());
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);

                    dic.Add((byte) LilyOpKey.ErrorCode, ErrorCode.SystemError);
                }
            }

            return dic;
        }

        public Dictionary<byte, object> Delete(FriendData friendData)
        {
            Dictionary<byte, object> paramters = new Dictionary<byte, object>();
            //using (LilyEntities lilyEntities = new LilyEntities())
            //{
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (lockObj)
                {
                    if (lilyEntities.friend.Any(rs => (rs.userA == friendData.UserA && rs.userB == friendData.UserB)))
                    {
                        friend friend = lilyEntities.friend.FirstOrDefault(
                            rs => rs.userA == friendData.UserA && rs.userB == friendData.UserB);
                        lilyEntities.friend.DeleteObject(friend);
                        lilyEntities.SaveChanges();
                        paramters[(byte) LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                    }
                    else if (lilyEntities.friend.Any(rs => (rs.userA == friendData.UserB && rs.userB == friendData.UserA)))
                    {
                        friend friend = lilyEntities.friend.FirstOrDefault(
                            rs => rs.userA == friendData.UserB && rs.userB == friendData.UserA);
                        lilyEntities.friend.DeleteObject(friend);
                        lilyEntities.SaveChanges();
                        paramters[(byte) LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                    }
                    else
                    {
                        paramters[(byte) LilyOpKey.ErrorCode] = ErrorCode.FriendNotExist;
                    }
                }
            }
            //}
            return paramters;
        }


        public bool ExistFriend(string userA,string userB) {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                return lilyEntities.friend.Any(
                        r => ((r.userA == userA && r.userB == userB) || (r.userA == userB && r.userB == userA)));
            }
        }
    }
}
