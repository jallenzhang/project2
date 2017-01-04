using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using DataPersist;
using LilyServer.Model;
using LilyServer.Helper;
using System.Xml;

namespace LilyServer
{
    public class AwardManager
    {
        private static AwardManager awardManager;
        public static AwardManager Singleton {
            get
            {
                if (awardManager == null)
                {
                    awardManager = new AwardManager();
                }
                return awardManager;
            }
        }

        //public Award[] Awards { get; private set; }

        public Award[] Awards { get; private set; }

        public AwardManager()
        {
            //Awards = new Award[3];
            //Awards[0] = new Award { Id = 1,Chip = 864 };
            //Awards[1] = new Award { Id = 2, Chip = 1728 };
            //Awards[2] = new Award { Id = 3, Chip = 2592 };
        }

        public void LoadFile(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Award[]), new XmlRootAttribute("Awards"));
            StreamReader streamReader = new StreamReader(path);
            this.Awards = xmlSerializer.Deserialize(streamReader) as Award[];
            streamReader.Close();
        }

        //public Award GetAward(user user)
        //{
        //    if (user.logintime.Value.Date == DateHelper.GetNow().Date)
        //    {
        //        return new Award { Chip = 0 };
        //    }
        //    Award award = null;
        //    if (user.usertype == (byte)UserType.Guest)
        //    {
        //        award= this.Awards.FirstOrDefault(rs => rs.Id == 1);
        //    }
        //    else
        //    {
        //        if (user.hasbuy??false)
        //        {
        //            award= this.Awards.FirstOrDefault(rs => rs.Id == 3);
        //        }
        //        else
        //        {
        //            award= this.Awards.FirstOrDefault(rs => rs.Id == 2);
        //        }
        //    }
        //    return award;
        //}

        public string SignIn(user user)
        {
            if (user.logintime.Value.Date == DateHelper.GetNow().Date)
            {
                return user.awards;
            }

            byte awardType;
            if((user.usertype??0)==(byte)UserType.Guest)
            {
                awardType=(byte)AwardType.Guest;
            }
            else
            {
                awardType=(byte)((user.money??0)!=0?AwardType.Pay:AwardType.Normal);
            }

            string awards = null;
            if (string.IsNullOrEmpty(user.awards)||user.awards.Split('|').Length>=Awards.Length||DateHelper.GetNow().Date-user.logintime.Value>TimeSpan.FromDays(1))
            {
                awards = awardType.ToString()+","+(user.awardsadd??0m).ToString("F2");
            }
            else
            {
                awards = user.awards + "|" + awardType.ToString()+","+(user.awardsadd??0m).ToString("F2");
            }
            
            return awards;
        }

        public string InitAwards(UserData userData)
        {
            string awards = null;
            if (userData.UserType == UserType.Guest)
            {
                awards = ((byte)AwardType.Guest).ToString()+","+userData.AwardAdds.ToString("F2");
            }
            else
            {
                if (userData.Money!=0)
                {
                    awards = ((byte)AwardType.Pay).ToString()+","+userData.AwardAdds.ToString("F2");
                }
                else
                {
                    awards = ((byte)AwardType.Normal).ToString()+","+userData.AwardAdds.ToString("F2");
                }
            }
            return awards;
        }

        public Dictionary<byte, object> GetAwards(string userId)
        {
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            UserData userData = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            string awardsOfUser = userData.Awards;
            if (!string.IsNullOrEmpty(awardsOfUser))
            {
                string[] awards = awardsOfUser.Split('|');
                long chip = 0;
                for(int i=0;i<awards.Length;i++)
                {
                    string[] values = awards[i].Split(',');
                    AwardType awardType=(AwardType)Convert.ToByte(values[0]);
                    float awardAdds = Convert.ToSingle(values[1]);
                    switch (awardType)
                    {
                        case AwardType.Guest:
                            chip +=Convert.ToInt64( AwardManager.Singleton.Awards.FirstOrDefault(rs => rs.Id == i+1).Guest*(1f+awardAdds));
                            break;
                        case AwardType.Normal:
                            chip += Convert.ToInt64(AwardManager.Singleton.Awards.FirstOrDefault(rs => rs.Id == i + 1).Normal * (1f+awardAdds));
                            break;
                        case AwardType.Pay:
                            chip += Convert.ToInt64(AwardManager.Singleton.Awards.FirstOrDefault(rs => rs.Id == i + 1).Pay * (1f+awardAdds));
                            break;
                    }
                }
                if (chip != 0)
                {
                    UserService.getInstance().ChangeUserChips(userId, chip);
                    UserService.getInstance().SetAwardsNone(userId);
                    // add bank record
                    BankService.getInstance().addRecord(chip, BankActionType.Award,userId);
                    opParams[(byte)LilyOpKey.Chip] = UserService.getInstance().QueryUserByUserId(userId).chips ?? 0;
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                }
                else
                {
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.NoAwards;
                }
            }
            else
            {
                opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.NoAwards;
            }

            return opParams;
        }
    }
}
