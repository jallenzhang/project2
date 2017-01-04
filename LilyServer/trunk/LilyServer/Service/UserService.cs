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
using System.Text.RegularExpressions;

namespace LilyServer
{
    internal class UserService
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
 
        //private LilyEntities lilyEntities = new LilyEntities();
        private readonly object _lockObj = new object();

        private static UserService _instance = null;

        private static long PRESENT = ConfigurationHelp.RegisterAwards;
        private static long GUESTPRESENT = ConfigurationHelp.RegisterAwardsGuest;



        public static UserService getInstance()
        {
            if (_instance == null)
                _instance = new UserService();
            return _instance;
        }


        public Dictionary<byte, object> GuestUpgrade(UserData userData, LilyPeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte,object>();
            try
            {
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                    //UserData userData = ht.ToUserData();
                    user guest = (from _u in lilyEntities.user
                                  where _u.userid.CompareTo(userData.UserId) == 0
                                  select _u).FirstOrDefault();
                    if (guest != null)
                    {

                        //vUsers count = (from _u in lilyEntities.vUsers
                        //             where _u.mail.CompareTo(userData.Mail) == 0 //|| _u.nickname.CompareTo(userData.NickName) == 0
                        //             select _u).FirstOrDefault();
                        //if (count != null)
                        //{
                        //    //dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserExist);
                        //    //return dic;

                        //    //if (count.mail.CompareTo(userData.Mail) == 0)
                        //        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.MailExist);
                        //    //else if (count.nickname.CompareTo(userData.NickName) == 0)
                        //    //    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NickNameExist);
                        //    return dic;
                        //}
                        bool isValid = JavApiService.UpGrade(userData.UserId, userData.Mail, guest.username);
                        if (!isValid)
                        {
                            dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserExist);
                            return dic;
                        }

                        isValid = JavApiService.UpGradeChangePassword(userData.Mail,userData.Password);
                        if (!isValid)
                        {
                            //dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserExist);
                            //return dic;
                        }

                        long upgradePresent = Math.Max(PRESENT - GUESTPRESENT, 0);
                        guest.usertype = (int)userData.UserType;
                        guest.mail = userData.Mail;
                        guest.password = userData.Password;
                        guest.nickname = userData.NickName;
                        guest.chips += upgradePresent;
                        guest.avator = userData.Avator;

                        BankService.getInstance().addRecord(upgradePresent, BankActionType.GuestUpgrade,guest.userid);

                        lock (_lockObj)
                        {
                            lilyEntities.SaveChanges();
                        }
                        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                        dic.Add((byte)LilyOpKey.UserData, guest.ToUserData().Tobyte());
                    }
                    else
                    {
                        dic = this.Register(userData, peer);
                    }
                }

            }
            catch (Exception ex) {
                Log.Error(ex.Message,ex);
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }
            return dic;
        }


        public Dictionary<byte, object> Register(UserData userData, LilyPeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                //UserData userData = ht.ToUserData();
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                    if (userData.UserType == UserType.Normal)
                    {

                        int isValid = JavApiService.CreateAccount(userData);
                        if (isValid<0)
                        {
                            dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserExist);
                            return dic;
                        }

                        userData.UserId = isValid.ToString();
                        dic = RegisterNormalUser(userData, peer);
                    }
                    else
                    {                        
                        dic = this.Register3rdUser(userData, peer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        private Dictionary<byte, object> RegisterNormalUser(UserData userData,LilyPeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                Log.InfoFormat("UserService RegisterNormalUser userData.Mail={0},userData.NickName={1}", userData.Mail, userData.NickName);

                //vUsers count = (from _u in lilyEntities.vUsers 
                //                where _u.mail.CompareTo(userData.Mail) == 0 || _u.nickname.CompareTo(userData.NickName) == 0
                //                select _u ).FirstOrDefault();
                //if (count != null)
                //{
                //    //dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserExist);
                //    //return dic;

                //    if (count.mail.CompareTo(userData.Mail) == 0)
                //        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.MailExist);
                //    else if(count.nickname.CompareTo(userData.NickName) == 0)
                //        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NickNameExist);
                //    return dic;
                //}               


                user u = new user
                {
                    userid=userData.UserId,
                    usertype = (int)UserType.Normal,
                    nickname = userData.NickName,
                    password = userData.Password,
                    mail = userData.Mail,
                    avator = userData.Avator,
                    status = UserStatus.Idle.getInt32Value(),
                    logintime = DateHelper.GetNow(),
                    backgroundtype=1,
                    livingroomtype = 1,
                    ownroomtypes=1,
                    chips=PRESENT,
                    money=0,
                    devicetoken=userData.DeviceToken,
                    devicetype=(int)userData.DeviceType,
                    awards=AwardManager.Singleton.InitAwards(userData)
                };


                lock (_lockObj)
                {
                    lilyEntities.AddObject("user", u);
                    lilyEntities.SaveChanges();                    

                    //u.userid = GenerateUserId(u.id);
                    //lilyEntities.ObjectStateManager.ChangeObjectState(u, EntityState.Modified);
                    //lilyEntities.SaveChanges();

                    BankService.getInstance().addRecord(PRESENT, BankActionType.Register, u.userid);
                }


                UserData data = u.ToUserData();
                peer.UserId = data.UserId;

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserData, data.Tobyte());
               }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        private Dictionary<byte, object> Register3rdUser(UserData userData,LilyPeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                Log.InfoFormat("UserService Register3rdUser userData.Mail={0},userData.NickName={1},userData.UserName={2}, userData.UserType={3}", string.IsNullOrEmpty(userData.Mail) ? "" : userData.Mail, userData.NickName, userData.UserName, userData.UserType == null ? "null" : userData.UserType.ToString());

                int userType = userData.UserType.getInt32Value();

                int count = (from _u in lilyEntities.vUsers
                                where _u.nickname.CompareTo(userData.NickName) == 0 //&& _u.usertype==userType //|| _u.nickname.CompareTo(userData.NickName) == 0
                                select _u).Count<vUsers>();
                if (count > 0)
                {
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NickNameExist);
                    return dic;
                }

                int isValid = JavApiService.QuickCreateAccount(userData);
                if (isValid < 0)
                {
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NickNameExist);
                    return dic;
                }

                if (string.IsNullOrEmpty(userData.Mail))
                    userData.Mail = Guid.NewGuid().ToString();

                user u = new user
                {
                    userid=isValid.ToString(),
                    usertype = userData.UserType.getInt32Value(),
                    nickname = userData.NickName,
                    // password = userData.Password,
                    username = userData.UserName,
                    mail =userData.Mail,//userData.Mail,
                    avator = userData.Avator,
                    status = UserStatus.Idle.getInt32Value(),
                    logintime = DateHelper.GetNow(),
                    backgroundtype=1,
                    livingroomtype = 1,
                    ownroomtypes = 1,
                    chips = userData.UserType == UserType.Guest ? GUESTPRESENT : PRESENT,
                    money = 0,
                    awards = AwardManager.Singleton.InitAwards(userData)
                };

                lock(_lockObj)
                {
                    lilyEntities.user.AddObject(u);
                    lilyEntities.SaveChanges();

                    //u.userid = GenerateUserId(u.id);
                    //if (u.usertype == UserType.Guest.getInt32Value()&&string.IsNullOrEmpty(u.nickname))
                    //{
                    //    u.nickname = string.Format("来宾{0}", u.id);
                    //    //u.mail = Guid.NewGuid().ToString();
                    //    //userData.Mail = u.mail;
                    //}

                    //lilyEntities.ObjectStateManager.ChangeObjectState(u, EntityState.Modified);
                    //lilyEntities.SaveChanges();

                    BankService.getInstance().addRecord(GUESTPRESENT, BankActionType.Register, u.userid);
                }

                UserData data = u.ToUserData();
                peer.UserId = data.UserId;
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserData, data.Tobyte());
               }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        private string GenerateUserId(Int64 id)
        {
            return string.Format("1_{0}",id);
        }

        public Dictionary<byte,object> CheckMail(string email)
        {
            Dictionary<byte,object> opParams=new Dictionary<byte,object>();
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                if (lilyEntities.user.Any(rs => rs.mail == email))
                {
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.UserExist;
                }
                else
                {
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                }
            }
            return opParams;
        }

        public UserData QueryUserOrBotsById(string userid) {
            bool robot = userid.Substring(0, 1) == "2";
            UserData userData = null;
            lock (_lockObj)
            {
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                    if (robot)
                    {
                        bots bot = (from _u in lilyEntities.bots
                                    where _u.userid.CompareTo(userid) == 0
                                    select _u).FirstOrDefault();
                        userData = bot.ToRobotInfo();
                    }
                    else
                    {
                        user user = (from _u in lilyEntities.user
                                     where _u.userid.CompareTo(userid) == 0
                                     select _u).FirstOrDefault();
                        userData = user.ToUserData();
                    }
                }
            }
            return userData;
        }

        public user QueryUserByUserId(string userId)
        {
            user result = null;
            using (LilyEntities lilyEntities = new LilyEntities())
            {
            Log.InfoFormat("QueryUserDataByUserId userId={0}", userId);

            lock (_lockObj)
            {
                IQueryable<user> users = (from _u in lilyEntities.user
                                          where _u.userid.CompareTo(userId) == 0
                                          select _u);

                if (!users.Any())
                {
                    Log.InfoFormat("QueryUserDataByUserId can't find the user by userId={0}", userId);
                    return null;
                }

                result = users.First<user>();
              }
            }
            return result;
        }

        public Dictionary<byte, object> Login(Hashtable ht,LilyPeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (ht == null) {

                Log.Error("#############       FUCK  USERDATA==NULL");

                dic.Add((byte)LilyOpKey.ErrorCode,ErrorCode.AuthenticationFail);
                return dic;
            }                

            try
            {
                //if (lilyEntities.Connection.State!=ConnectionState.Open)
                //{
                //    lilyEntities.Connection.Open();
                //}

                using (LilyEntities lilyEntities = new LilyEntities())
                {
                UserData userData = ht.ToUserData();
                Log.InfoFormat("UserService Login userData.Mail={0},userData.UserName={1}", string.IsNullOrEmpty(userData.Mail) ? "" : userData.Mail, string.IsNullOrEmpty(userData.NickName) ? "" : userData.NickName);

                user u = null;
                if (userData.UserType == UserType.Normal)
                {

                    bool isValid = JavApiService.UserLoginGetInfo(userData);
                    if (!isValid)
                    {
                        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.AuthenticationFail);
                        return dic;
                    }

                    IQueryable<user> users = (from _u in lilyEntities.user
                                              where _u.mail.CompareTo(userData.Mail) == 0 //&& _u.password.CompareTo(userData.Password) == 0
                                              select _u);
                    if (users.Count<user>() > 0)
                    {
                        u = users.First<user>();
                    }
                    else
                    {
                        userData.UserType = UserType.Normal;
                        userData.Avator = 1;
                        RegisterNormalUser(userData,peer);


                       u= (from _u in lilyEntities.user
                         where _u.mail.CompareTo(userData.Mail) == 0 //&& _u.password.CompareTo(userData.Password) == 0
                         select _u).FirstOrDefault();
                        //IQueryable<user> users2pwd2 = (from _u in lilyEntities.user
                        //                               where _u.mail.CompareTo(userData.Mail) == 0 && _u.password2.CompareTo(userData.Password) == 0
                        //                               select _u);
                        //if (users2pwd2.Count<user>() > 0)
                        //{
                        //    lock (_lockObj)
                        //    {
                        //        u = users2pwd2.First<user>();
                        //        u.password = u.password2;
                        //        u.password2 = null;
                        //        lilyEntities.SaveChanges();
                        //    }

                        //}
                        //else
                        //{
                        //    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.AuthenticationFail);
                        //    return dic;
                        //}
                    }
                }
                else
                {
                    IQueryable<user> users = (from _u in lilyEntities.user
                                              where _u.mail.CompareTo(userData.Mail) == 0 &&_u.usertype==(int)userData.UserType 
                                              select _u);
                    if (users.Count<user>() > 0&&!string.IsNullOrEmpty(userData.Mail))
                    {
                        u = users.First<user>();
                    }
                    else
                    {
                        if (userData.UserType == UserType.Guest)
                        {
                            bool isValid = JavApiService.checkNickName(userData.NickName);
                            if (!isValid)
                            {
                                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NickNameExist);
                                return dic;
                            }


                            this.Register3rdUser(userData, peer);
                            u = (from _u in lilyEntities.user
                                 where _u.mail.CompareTo(userData.Mail) == 0
                                 select _u).First<user>();
                        }
                        else
                        {
                            dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.The3rdUserNotRegistered);
                            return dic;
                        }
                    }

                }

                //Award award = AwardManager.Singleton.GetAward(u);
                //long awardChip = notification ? award.Chip : award.Chip / 2;
                DateTime? lastLoginTime = u.logintime;

                lock (_lockObj)
                {
                    u.awards = AwardManager.Singleton.SignIn(u);
                    u.status = UserStatus.Idle.getInt32Value();
                    u.logintime = DateHelper.GetNow();
                    u.devicetype = (int)userData.DeviceType;
                    u.devicetoken = userData.DeviceToken;
                    //u.chips = (u.chips??0)+awardChip;
                    lilyEntities.ObjectStateManager.ChangeObjectState(u, EntityState.Modified);
                    lilyEntities.SaveChanges();

                    login addlogin = new login
                    {
                        useridstr = u.userid,
                        logintime = DateHelper.GetNow()
                    };
                    lilyEntities.AddObject("login", addlogin);
                    lilyEntities.SaveChanges();
                }

                //if (awardChip > 0)
                //    BankService.getInstance().addRecord(awardChip, BankActionType.Award, u.userid);


                UserData data = u.ToUserData();
                peer.UserId = data.UserId;
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserData, data.Tobyte());
                //dic[(byte)LilyOpKey.Chip] = awardChip;

                //List<user> friends = FriendService.getInstance().GetFriends(u.userid);

                //if (friends.Count > 0)
                //{
                //    List<UserData> friendDatas = new List<UserData>(friends.Count);
                //    friends.ForEach(f => friendDatas.Add(f.ToUserData()));
                //    dic.Add((byte)LilyOpKey.FriendList, friendDatas.Tobyte());
                //}
                //disable in v1.0
                if (lastLoginTime != null &&
                    AchievementManager.Singleton.TryAchieve9(peer.UserId, lastLoginTime.Value))
                {
                    peer.SendAchievementEvent(9);
                    if (AchievementManager.Singleton.TryAchieve11(peer.UserId))
                    {
                        userData = UserService.getInstance().QueryUserByUserId(peer.UserId).ToUserData();
                        peer.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                    }
                }
               }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                dic.Clear();
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }
            //finally {
            //    lilyEntities.Connection.Close();
            //}

            return dic;
        }

        public Dictionary<byte, object> Logout(string userId)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                Log.InfoFormat("UserService Logout userId={0}", userId);

                IQueryable<user> users = (from _u in lilyEntities.user
                                            where _u.userid.CompareTo(userId) == 0
                                            select _u);

                if (!users.Any())
                {
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserNotExist);
                    return dic;
                }

                user u = users.First<user>();
                lock (_lockObj)
                {
                    u.status = UserStatus.Offline.getInt32Value();
                    u.devicetoken = string.Empty;
                    lilyEntities.ObjectStateManager.ChangeObjectState(u, EntityState.Modified);
                    lilyEntities.SaveChanges();
                }

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
               }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        public Dictionary<byte, object> Feedback(Hashtable ht)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                FeedbackData data = ht.ToFeedbackData();

                Log.InfoFormat("UserService Feedback UserId={0},Content={1}", data.UserId, data.Content);

                feedback _feedback = new feedback { content = data.Content, userid = data.UserId, time = DateHelper.GetNow() };
                using (LilyEntities lilyEntities = new LilyEntities())
                {
                lock (_lockObj)
                {
                    lilyEntities.feedback.AddObject(_feedback);
                    lilyEntities.SaveChanges();
                }
                }

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        public Dictionary<byte, object> GetRoommates(Dictionary<int, string>.ValueCollection userIds)
        {
            Dictionary<byte, object> roommates = new Dictionary<byte, object>();

            foreach (string userId in userIds)
            {

            }

            return roommates;
        }
        
        public Dictionary<byte, object> GetNow()
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            try
            {
                Log.Info("UserService GetNow");

                DateTime dt = DateHelper.GetNow();

                dic.Add((byte)LilyOpKey.SyncDate, dt.ToBinary());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
            }

            return dic;
        }

        public Dictionary<byte, object> GetPassword(string mailaddress)
        {
            
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                IQueryable<user> users = (from _u in lilyEntities.user
                                          where _u.mail.CompareTo(mailaddress) == 0
                                          select _u);
                if (!users.Any())
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserNotExist);
                else
                {

                    user u = users.First<user>();
                    Log.InfoFormat("UserService GetPassword Mail={0}", mailaddress);

                    string newPassword = Guid.NewGuid().ToString().Substring(0, 6);
                    string en_pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "MD5");

                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<div style='margin-left:0px;font-size:11pt;line-height:20px;'>");
                    mailBody.Append("亲爱的康熙德州扑克用户，您好：<br/><br/>");
                    mailBody.Append("以下是您康熙德州扑克账号重置后的登录密码：<br/>");
                    mailBody.AppendFormat("<p>{0}</p>", newPassword);
                    mailBody.Append("<p style='color:Red;'>请及时修改密码，以防邮件外泄导致账号被盗</p>");
                    mailBody.Append("<div style='font-size:10pt;color:Gray;'>");
                    mailBody.Append("修改密码注意事项：<br/>");
                    mailBody.Append("1、使用重置后的密码登录康熙德州扑克；<br/>");
                    mailBody.Append("2、点击游戏设置，选择密码修改，按要求操作即可对登录密码进行修改；</div>");
                    mailBody.Append("<p>请勿直接回复该邮件，有关康熙德州扑克的更多帮助信息，请关注官方微博： <a href='http://weibo.com/kxpoker'>http://weibo.com/kxpoker</a></p>");
                    mailBody.Append("康熙德州扑克运营团队敬上</div>");




                    MailHelper pwdMail = new MailHelper();
                    pwdMail.MailTo = mailaddress;
                    pwdMail.Subject = "您好，请确认对《康熙德州扑克》游戏账号的密码修改";
                    pwdMail.MailBody = mailBody.ToString();


                    u.password2 = en_pwd;
                    lock (_lockObj)
                    {
                        lilyEntities.SaveChanges();
                    }
                    try
                    {
                        pwdMail.send();
                        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                    }
                    catch (Exception)
                    {
                        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.SystemError);
                    }
                }
            }
            return dic;
        }

        public Dictionary<byte,object> Save(UserData userData,out bool isLivingRoomTypeChanged)
        {




            isLivingRoomTypeChanged = false;


           

            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                if (lilyEntities.user.Any(rs => rs.userid == userData.UserId))
                {
                    lock (_lockObj)
                    {
                        if (userData.IsRobot)
                        {
                            bots u = lilyEntities.bots.FirstOrDefault(rs => rs.userid == userData.UserId);
                            u.avator = userData.Avator;
                            u.password = userData.Password;
                            u.backgroundtype = userData.BackgroundType;
                            //if (userData.Wins > u.wins)
                            //    u.wins = userData.Wins;
                            u.livingroomtype = (byte)userData.LivingRoomType;
                            u.maximumwinnote = Math.Max(u.maximumwinnote.HasValue ? u.maximumwinnote.Value : 0, userData.BiggestWin);
                            lilyEntities.SaveChanges();
                        }
                        else
                        {
                            user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userData.UserId);

                            if (u.password!=userData.Password)
                            {
                                bool isValid = JavApiService.ChangePassword(u.mail, u.password, userData.Password);
                                if (!isValid)
                                {
                                    parameters.Add((byte)LilyOpKey.ErrorCode, ErrorCode.AuthenticationFail);
                                    return parameters;
                                }
                                    parameters.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                                    return parameters;
                            }


                            isLivingRoomTypeChanged = u.livingroomtype != (int)userData.LivingRoomType;
                            u.avator = userData.Avator;
                            u.password = userData.Password;
                            u.backgroundtype = userData.BackgroundType;
                            //if (userData.Wins > u.wins)
                            //    u.wins = userData.Wins;
                            u.livingroomtype = (byte)userData.LivingRoomType;
                            u.maximumwinnote = Math.Max(u.maximumwinnote.HasValue ? u.maximumwinnote.Value : 0, userData.BiggestWin);
                            lilyEntities.SaveChanges();
                        }
                    }
                    parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                }
                else
                {
                    parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.UserNotExist;
                }
            }
            return parameters;
        }

        public void SetBestHand(UserData userData) {

            using (LilyEntities lilyEntities = new LilyEntities())
            {
                if (userData.IsRobot)
                {
                    bots u = lilyEntities.bots.FirstOrDefault(rs => rs.userid == userData.UserId);
                    u.besthandvalue = userData.BestHandValue;
                    u.besthandtype = userData.BestHand;
                }
                else
                {
                    user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userData.UserId);
                    u.besthandvalue = userData.BestHandValue;
                    u.besthandtype = userData.BestHand;
                }
                lock (_lockObj)
                {
                    lilyEntities.SaveChanges();
                }
            }
        }

        public void SetWins(UserData userData) {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                if (userData.IsRobot)
                {
                    bots u = lilyEntities.bots.FirstOrDefault(rs => rs.userid == userData.UserId);
                    //if (userData.Wins > (u.wins??0))
                    u.wins = (u.wins ?? 0) + 1;
                    if (userData.BiggestWin > (u.maximumwinnote ?? 0))
                        u.maximumwinnote = userData.BiggestWin;
                }
                else
                {
                    user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userData.UserId);
                    //if (userData.Wins > (u.wins??0))
                    u.wins = (u.wins ?? 0) + 1;
                    if (userData.BiggestWin > (u.maximumwinnote ?? 0))
                        u.maximumwinnote = userData.BiggestWin;
                }
                lock (_lockObj)
                {
                    lilyEntities.SaveChanges();
                }
            }
        }

        public void SetTotalGame(List<UserData> userDatas) {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    foreach (UserData d in userDatas)
                    {
                        if (d.IsRobot)
                        {
                            bots b = lilyEntities.bots.FirstOrDefault(r => r.userid == d.UserId);
                            //b.wins = d.Wins;
                            b.totalgame = b.totalgame.HasValue ? b.totalgame + 1 : 1;
                        }
                        else
                        {
                            user u = lilyEntities.user.FirstOrDefault(r => r.userid == d.UserId);
                            u.totalgame = u.totalgame.HasValue ? u.totalgame + 1 : 1;
                            //u.wins = d.Wins;
                        }
                    }
                    lilyEntities.SaveChanges();
                }
            }
        }

        public void addExp(UserData userData,ExpType et) {
            addExp(userData, (int)et);          
        }

        public void addExp(UserData userData, int exp)
        {
            //if (userData.UserType == UserType.Guest && userData.Level >= 7)
            //    return;
            if (exp == 0)
                return;

            Log.InfoFormat("UserService addExp userData.NickName={0},addexp={1}",userData.NickName,exp);
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    if (userData.IsRobot)
                    {
                        bots u = lilyEntities.bots.FirstOrDefault(r => r.userid == userData.UserId);
                        u.exp = (u.exp ?? 0) + exp;
                        lilyEntities.SaveChanges();
                        int[] level = RobotHelper.getLevel((double)u.exp, 1d);
                        userData.Level = level[0];
                        userData.LevelExp = level[1];
                    }
                    else
                    {
                        user u = lilyEntities.user.FirstOrDefault(r => r.userid == userData.UserId);
                        u.exp = (u.exp ?? 0) + exp + (u.expadd ?? 0);
                        lilyEntities.SaveChanges();
                        int[] level = RobotHelper.getLevel((double)u.exp, 1d);
                        userData.Level = level[0];
                        userData.LevelExp = level[1];
                    }
                }
            }
        }


        public void ChipsChanged(UserData userData,long changeAmnt) {
            //user u = _entities.Users.FirstOrDefault(r=>r.userid==userData.UserId);
            //u.chips = userData.MoneySafeAmnt;
            //_entities.SaveChanges();
            if (changeAmnt == 0)
                return;
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    if (!userData.IsRobot)
                    {
                        user u = lilyEntities.user.FirstOrDefault(r => r.userid == userData.UserId);
                        u.chips += changeAmnt;
                        //if (u.usertype==(int)UserType.Guest&&u.chips>30000)
                        //{
                        //    u.chips = 30000;
                        //}
                        lilyEntities.SaveChanges();
                        userData.Chips = u.chips.Value;
                        Log.DebugFormat("ChipsChanged user:{0},changedamnt:{1}",userData.Name,changeAmnt);


                    }
                    else
                    {
                        bots bot = lilyEntities.bots.FirstOrDefault(rs => rs.nickname == userData.NickName);
                        bot.chips += changeAmnt;
                        lilyEntities.SaveChanges();
                        userData.Chips = bot.chips.Value;
                    }
                }
            }
        }

        public long ChangeUserChips(string userId, long chips)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                long retchips = 0;
                lock (_lockObj)
                {
                    user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                    if (u != null)
                    {
                        u.chips += chips;
                        lilyEntities.SaveChanges();
                        retchips = u.chips ?? 0;
                        Log.DebugFormat("ChipsChanged user:{0},changedamnt:{1}", u.nickname, chips);
                    }
                }
                return retchips;
            }
        }

        public void SetAward(string userId, AwardType awardType, int day)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                u.awards=Regex.Replace(u.awards,@"(?<=^(\d\|){"+(day-1).ToString()+@"})\d",((byte)awardType).ToString());
                lilyEntities.SaveChanges();
            }
        }

        public void SetAwards(string userId, AwardType awardType)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                u.awards = Regex.Replace(u.awards, @"(?=\d\,)\d", ((byte)awardType).ToString());
                lilyEntities.SaveChanges();
            }
        }

        public void SetAwardsNone(string userId)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                u.awards = Regex.Replace(u.awards, @"(?<num>(?=\d\,)[^0])", @"${num}0");
                lilyEntities.SaveChanges();
            }
        }

        public Dictionary<byte,object> BuyRoom(string userId, RoomType roomType,long? chip,long? money)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                if (chip!=null)
                    if (user.chips < chip)
                    {
                        opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.BuyItemChipNotEnough);
                        return opParams;
                    }

                if ((user.ownroomtypes & (int)roomType) == 0)
                {
                    lock (_lockObj)
                    {
                        user.ownroomtypes = user.ownroomtypes | (int)roomType;
                        user.hasbuy = true;
                        if (chip != null)
                            user.chips -= chip;
                        if (money!=null)
                            user.money = (user.money ?? 0) + money;
                        lilyEntities.SaveChanges();
                        opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                        opParams[(byte)LilyOpKey.Chip] = user.chips ?? 0;
                    }
                }
                else
                {
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.ItemExist;
                }
                return opParams;
            }
        }



        public Dictionary<byte,object> BuyChip(string userId,int chip,long money)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                lock (_lockObj)
                {
                    long ochips = user.chips ?? 0;
                    user.chips += chip;
                    user.hasbuy = true;
                    user.money = (user.money ?? 0) + money;
                    lilyEntities.SaveChanges();
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                    opParams[(byte)LilyOpKey.Chip] = user.chips;
                }
                return opParams;
            }
        }

        public void ChangeUserStatus(string userId, UserStatus us)
        {
            if (string.IsNullOrEmpty(userId))
                return;
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                    if (u != null)
                    {
                        u.status = us.getInt32Value();
                        lilyEntities.SaveChanges();
                    }
                }
            }
        }

        public bool HasAchieved(string userId,byte number)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                //user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                //return ((user.achievements ?? 0) & (1L << (number - 1))) == 1;
                user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                if (user == null) return true;
                return ((user.achievements ?? 0) & (1L << (number - 1))) == 1L << (number - 1);
            }
        }

        public bool HasAchieved(string userId, byte[] numbers)
        {
            //user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
            //long achievements = 0;
            //foreach (byte number in numbers)
            //{
            //    achievements |= 1L << (number - 1);
            //}
            //return (user.achievements & achievements) == achievements ? true : false;
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                if (user == null) return true;
                long achievements = 0;
                foreach (byte number in numbers)
                {
                    achievements |= 1L << (number - 1);
                }
                return (user.achievements & achievements) == achievements;
            }
        }

        public void Achieve(string userId, byte number)
        {
            bool isrobot = false;
            //if (userId.Substring(0, 1) == "2")
            //    isrobot = true;
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    if (isrobot)
                    {
                        bots bot = lilyEntities.bots.FirstOrDefault(rs => rs.userid == userId);
                        bot.achievements = (bot.achievements ?? 0) | (1L << (number - 1));
                        lilyEntities.SaveChanges();
                    }
                    else
                    {
                        user u = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                        u.achievements = (u.achievements ?? 0) | (1L << (number - 1));
                        lilyEntities.SaveChanges();
                    }
                }
            }
        }

        public void ChangeChipAndExp(string userId,long chip, long exp)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                lock (_lockObj)
                {
                    user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                    user.chips += chip;
                    user.exp += exp;
                    lilyEntities.SaveChanges();
                }
            }
        }


        public void SystemSetting(string userId, bool spn, bool fpn)
        {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userId);
                user.systempn = spn;
                user.friendpn = fpn;
                lock (_lockObj)
                {
                    lilyEntities.SaveChanges();
                }
            }
        }

        public List<Props> GetUserProps(string userId)
        {
            using (LilyEntities lilyEntities=new LilyEntities())
            {
                List<userprops> props = null;

                props = (from p in lilyEntities.userprops
                         where p.UserId==userId 
                              && p.Status>0
                         select p ).ToList();

                if (props!=null&&props.Count > 0)
                {
                    List<Props> userPropsDatas = new List<Props>(props.Count);
                    props.ForEach(f => userPropsDatas.Add(f.ToUserProps()));
                    return userPropsDatas;
                }
                return null;
            }
        }


        private Dictionary<byte, object> BuyProps(string userId, PropItem buyItem, long chip,long rmb) {
            using (LilyEntities lilyEntities = new LilyEntities())
            {
                Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                user u = lilyEntities.user.FirstOrDefault(r => r.userid == userId);
                if (u != null)
                {
                    if (u.chips > chip)
                    {
                        userprops paramUserprops = null;
                        userprops uprops = lilyEntities.userprops.FirstOrDefault(r => r.UserId == userId && r.ItemType == buyItem.ItemType && r.ItemId == buyItem.ItemId && r.Status > 0);

                        if (uprops != null)
                        {
                            //opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.BuyItemNotFound);
                            if (buyItem.Duration > 0)
                            {
                                u.chips -= chip;
                                if (rmb>0)
                                {
                                    u.money = (u.money ?? 0) + rmb;
                                }
                                uprops.Duration += buyItem.Duration;
                                //lilyEntities.SaveChanges();
                                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                                paramUserprops = uprops;
                                //opParams.Add((byte)LilyOpKey.UserProps, DataPersist.HelperLib.SerializeHelper.Serialize(uprops.ToUserProps()));
                            }
                            else {
                                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.ItemExist);
                                return opParams;
                            }
                        }
                        else
                        {
                            u.chips -= chip;
                            if (rmb > 0)
                            {
                                u.money = (u.money ?? 0) + rmb;
                            }
                            userprops newProp = new userprops();
                            newProp.ItemId = buyItem.ItemId;
                            newProp.ItemName = buyItem.ItemName;
                            newProp.ItemType = buyItem.ItemType;
                            newProp.PurchaseDate = DateHelper.GetNow();
                            newProp.Status = 1;
                            newProp.UserId = u.userid;
                            newProp.Duration = buyItem.Duration;
                            lilyEntities.AddTouserprops(newProp);
                            //lilyEntities.SaveChanges();
                            opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                            paramUserprops = newProp;
                            //opParams.Add((byte)LilyOpKey.UserProps, DataPersist.HelperLib.SerializeHelper.Serialize(newProp.ToUserProps()));
                        }                      
                        opParams.Add((byte)LilyOpKey.Chip, u.chips ?? 0);
                        lilyEntities.SaveChanges();
                        opParams.Add((byte)LilyOpKey.UserProps, DataPersist.HelperLib.SerializeHelper.Serialize(paramUserprops.ToUserProps()));
                    }
                    else
                    {
                        opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.BuyItemChipNotEnough);
                    }
                }
                else
                {
                    opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.BuyItemUserNotExist);
                }
                return opParams;
            }
        }


        public Dictionary<byte, object> BuyPropsByChips(string userId, PropItem buyItem,long chip)
        {
            return BuyProps(userId,buyItem,chip,0);
        }

        public Dictionary<byte, object> BuyPropsByRmb(string userId, PropItem buyItem) {

            return BuyProps(userId, buyItem, 0,buyItem.Price);
        }


        public void BindChannelID(string channelid,string userid) {
            using (LilyEntities lilyEntities=new LilyEntities())
            {
                lock (_lockObj)
                {
                    user user = lilyEntities.user.FirstOrDefault(rs => rs.userid == userid);
                    user.channelid = channelid;                
                    lilyEntities.SaveChanges();
                }
            }
        }
    }
}
