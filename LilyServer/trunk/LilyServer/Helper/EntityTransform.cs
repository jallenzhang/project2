using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPersist;
using LilyServer.Model;


namespace LilyServer.Helper
{
    public static class EntityTransform
    {
        public static UserData ToUserData(this user u)
        {
            UserData data = new UserData();

            if(u.avator != null) data.Avator = (byte)u.avator;
            if(u.backgroundtype != null) data.BackgroundType = (byte)u.backgroundtype.Value;
            data.BestHand = u.besthandtype;
            if (u.besthandvalue != null) data.BestHandValue = u.besthandvalue.Value;
            if(u.maximumwinnote != null) data.BiggestWin = u.maximumwinnote.Value;
            if (u.exp != null)
            {
                data.Exp = u.exp.Value;
                int[] level = RobotHelper.getLevel((double)data.Exp, 1d);
                data.Level = level[0];
                if (data.Level == 0) data.Level = 1;
                data.LevelExp = level[1];
            }
            else {
                data.Level = 1;
            }
            if(u.totalgame != null) data.HandsPlayed = u.totalgame.Value;
            if(u.wins != null) data.HandsWon = u.wins.Value;

            if (u.totalgame > 0) data.WinRatio =(float)Math.Round((double)data.HandsWon / (double)data.HandsPlayed, 2);
            //if(u.besthandtype != null) data.LivingRoomType = u.livingroomtype;
            if(u.money != null) data.Money = u.money.Value;
            if (u.chips != null) data.Chips = u.chips.Value;
            
            data.Name = u.nickname;
            data.LivingRoomType = (RoomType)u.livingroomtype;
            data.NickName = u.nickname;
            data.Password = u.password;
            data.Mail = u.mail;
            if (u.status != null) data.Status = (UserStatus)u.status.Value;
            data.UserId = u.userid;
            if (u.usertype != null) data.UserType = u.usertype.getUserType();
            if (u.winningpercentage != null) data.WinRatio = (float)u.winningpercentage;

            //if (u.chips != null) {
            //    data.MoneyInitAmnt = u.chips.Value;
            //    data.MoneySafeAmnt = u.chips.Value;                
            //}

            data.OwnRoomTypes = (RoomType)u.ownroomtypes;
            data.Achievements = u.achievements ?? 0;
            data.HasBuy = u.hasbuy ?? false;

            data.SystemNotify = u.systempn ?? true;
            data.FriendNotify = u.friendpn ?? true;

            data.DeviceType = (DeviceType)(u.devicetype ?? 0);
            data.DeviceToken = u.devicetoken;
            data.Wins = u.wins ?? 0;
            data.TotalGame = (int)(u.totalgame ?? 0);
            data.Awards = u.awards;
            data.AwardAdds = Convert.ToSingle(u.awardsadd ?? 0m);
            data.ExpAdds = u.expadd ?? 0;
            data.WinAdds = Convert.ToSingle(u.winadd ?? 0m);

            data.VIP = (u.money ?? 0) > 0 ? 1 : 0;

            data.Jade = data.AwardAdds > 0;
            data.LineAge = data.WinAdds > 0;


            return data;
        }

        public static RobotInfo ToRobotInfo(this bots bot)
        {
            RobotInfo data = new RobotInfo();

            if (bot.avator != null) data.Avator = (byte)bot.avator;
            if (bot.backgroundtype != null) data.BackgroundType = (byte)bot.backgroundtype.Value;
            data.BestHand = bot.besthandtype;
            if (bot.besthandvalue != null) data.BestHandValue = bot.besthandvalue.Value;
            if (bot.maximumwinnote != null) data.BiggestWin = bot.maximumwinnote.Value;
            if (bot.exp != null)
            {
                data.Exp = bot.exp.Value;
                int[] level = RobotHelper.getLevel((double)data.Exp, 1d);
                data.Level = level[0];
                if (data.Level == 0) data.Level = 1;
                data.LevelExp = level[1];
            }
            else
            {
                data.Level = 1;
            }
            if (bot.totalgame != null) data.HandsPlayed = bot.totalgame.Value;
            if (bot.wins != null) data.HandsWon = bot.wins.Value;
            //if(u.besthandtype != null) data.LivingRoomType = u.livingroomtype;
            if (bot.money != null) data.Money = bot.money.Value;
            data.Chips = bot.chips ?? 0;
            data.Name = bot.nickname;

            data.NickName = bot.nickname;
            data.Password = bot.password;
            data.Mail = bot.mail;
            if (bot.status != null) data.Status = (UserStatus)bot.status.Value;
            data.UserId = bot.userid;
            if (bot.usertype != null) data.UserType = bot.usertype.getUserType();
            if (bot.winningpercentage != null) data.WinRatio = (float)bot.winningpercentage;

            //if (bot.chips != null)
            //{
            //    int i_money = int.Parse(bot.chips.ToString());
            //    data.MoneyInitAmnt = i_money;
            //    data.MoneySafeAmnt = i_money;
            //}

            data.IsRobot = true;
            data.OwnRoomTypes = RoomType.Common;

            return data;
        }


        public static Props ToUserProps(this userprops props) {
            Props pp = new Props();
            pp.Id = props.Id;
            pp.ItemId = props.ItemId;
            pp.ItemName = props.ItemName;
            pp.ItemType = (ItemType)props.ItemType;
            pp.Duration = props.Duration;
            return pp;
        }

        //public static UserMessage ToUserMessage(this usermessage message)
        //{
        //    UserMessage userMessage = new UserMessage();
        //    userMessage.SenderId = message.sender;
        //    userMessage.MessageType = (MessageType)message.messagetype;
        //    userMessage.Content = message.content;
        //    return userMessage;
        //}
    }
}
