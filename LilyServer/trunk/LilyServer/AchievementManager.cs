using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite;
using DataPersist;
using LilyServer.Helper;
using System.Xml.Serialization;
using System.IO;
using DataPersist.CardGame;
using PokerWorld.HandEvaluator;
//using ExitGames.Logging;

namespace LilyServer
{
    public class AchievementManager
    {
        private static AchievementManager achievementService;
        // lee add
        //private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static byte[] AchieveList2 = { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

        public static AchievementManager Singleton
        {
            get
            {
                if (achievementService == null)
                {
                    achievementService = new AchievementManager();
                }
                return achievementService;
            }
        }

        private Achievement[] achievements;

        private AchievementManager()
        {
        }

        public void LoadFile(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Achievement[]), new XmlRootAttribute("Achievements"));
            StreamReader streamReader = new StreamReader(path);
            this.achievements = xmlSerializer.Deserialize(streamReader) as Achievement[];
        }

        private bool CheckAndAchieve(string userId, byte achievementNumber)
        {
            if (UserService.getInstance().HasAchieved(userId, achievementNumber))
            {
                return false;
            }
            UserService.getInstance().Achieve(userId, achievementNumber);
            return true;
        }

        public bool TryAchieve1(string userId)
        {
            if (UserService.getInstance().HasAchieved(userId, 1))
            {
                return false;
            }
            if (FriendService.getInstance().GetFriends(userId).Count >= 30)
            {
                UserService.getInstance().Achieve(userId, 1);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve2(string userId)
        {
            return CheckAndAchieve(userId, 2);
        }

        public bool TryAchieve3(string inviterId, string userId)
        {
            if (FriendService.getInstance().ExistFriend(inviterId, userId))
            {
                return CheckAndAchieve(inviterId, 3);
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve4(string userId, List<PlayerInfo> players)
        {
            if (UserService.getInstance().HasAchieved(userId, 4))
            {
                return false;
            }
            string[] userIds = players.Where(rs => (rs as UserData).UserId != userId).Select(rs => (rs as UserData).UserId).ToArray();
            foreach (string friendId in userIds)
            {
                if (FriendService.getInstance().ExistFriend(userId, friendId))
                {
                    UserService.getInstance().Achieve(userId, 4);
                    return true;
                }
            }
            return false;
        }

        public bool TryAchieve5(string userId, List<PlayerInfo> players)
        {
            if (UserService.getInstance().HasAchieved(userId, 5))
            {
                return false;
            }
            if (players.Any(rs => rs.MoneySafeAmnt == 0))
            {
                string[] friendIds = players.Where(rs => rs.MoneySafeAmnt == 0 && (rs as UserData).UserId != userId).Select(rs => (rs as UserData).UserId).ToArray();
                foreach (string friendId in friendIds)
                {
                    if (FriendService.getInstance().ExistFriend(userId, friendId))
                    {
                        UserService.getInstance().Achieve(userId, 5);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve6(string userId)
        {
            if (UserService.getInstance().HasAchieved(userId, 6))
            {
                return false;
            }
            byte[] numbers = new byte[] { 1, 2, 3, 4, 5 };
            if (UserService.getInstance().HasAchieved(userId, numbers))
            {
                UserService.getInstance().Achieve(userId, 6);
                Achievement achievement = this.achievements.FirstOrDefault(rs => rs.Number == 6);
                UserService.getInstance().ChangeChipAndExp(userId, achievement.Chip, achievement.Exp);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve7(UserData userData)
        {
            if (UserService.getInstance().HasAchieved(userData.UserId, 7))
            {
                return false;
            }
            if (userData.LivingRoomType != RoomType.Common)
            {
                UserService.getInstance().Achieve(userData.UserId, 7);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve8(string userId)
        {
            return CheckAndAchieve(userId, 8);
        }

        public bool TryAchieve9(string userId, DateTime lastLoginTime)
        {
            if (UserService.getInstance().HasAchieved(userId, 9))
            {
                return false;
            }
            if (lastLoginTime.AddDays(1d).Date == DateHelper.GetNow().Date)
            {
                UserService.getInstance().Achieve(userId, 9);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryAchieve10(string userId)
        {
            return CheckAndAchieve(userId, 10);
        }

        public bool TryAchieve11(string userId)
        {
            if (UserService.getInstance().HasAchieved(userId, 11))
            {
                return false;
            }
            //byte[] numbers = this.achievements.Select(rs => rs.Number).ToArray();
            byte[] numbers = this.achievements.Select(rs => rs.Number).Where(m => m != 11).ToArray();

            if (UserService.getInstance().HasAchieved(userId, numbers))
            {
                UserService.getInstance().Achieve(userId, 11);
                Achievement achievement = this.achievements.FirstOrDefault(rs => rs.Number == 11);
                UserService.getInstance().ChangeChipAndExp(userId, achievement.Chip, achievement.Exp);

                return true;
            }
            else
            {
                return false;
            }
        }


        #region GameAchievement

        /// <summary>
        /// 小财主
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>bool</returns>
        public void TryAchieve12(LilyPeer peer, string userId, long chips)
        {
            if (chips >= 4000)
            {
                if (CheckAndAchieve(userId, 12))
                {
                    if (peer != null)
                        peer.SendAchievementEvent(12);
                    goto AchieveALLA;
                }
            }
            return;
        AchieveALLA:
            TryAchieve32(peer, userId);
        }

        /// <summary>
        /// 成就13,14,15,16   20
        /// </summary>
        /// <param name="peer"></param>
        public void TryAchieveRounds(LilyPeer peer, PlayerInfo p)
        {
            string userId = (p as UserData).UserId;
            UserData curUser = p as UserData;//UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            int Lose = curUser.TotalGame - curUser.Wins;
            int Wins = curUser.Wins;
            List<byte> myList = new List<byte>();
            if (Lose >= 100)
                myList.Add(16);

            if (Lose >= 10)
                myList.Add(15);

            if (Wins >= 10)
                myList.Add(13);

            if (Wins >= 100)
                myList.Add(14);

            if ((p.exp == ExpType.Normal || p.exp == ExpType.Fold) && p.IsAllIn)
                myList.Add(20);

            bool isAchive = false;
            foreach (var item in myList)
            {
                if (CheckAndAchieve(userId, item))
                {
                    if (peer != null)
                        peer.SendAchievementEvent(item);
                    isAchive = true;
                }
            }

            if (isAchive)
                TryAchieve32(peer, userId);
        }

        /// <summary>
        /// 24,25,27,28,29,30,31,18
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="porket"></param>
        /// <param name="boad"></param>
        public void TryAchieveWins(LilyPeer peer, string userId, long moneySafeAmnt, GameCard[] porket, GameCard[] boad, bool isWin)
        {

            HandTypes handType = HandStrengthHelper.GetHandStrength(porket, boad);
            byte achieveNumber = 0;
            //for 皇家同花顺, 同花顺, 
            List<byte> achieveNumber_list = new List<byte>();
            bool isAchived = false;
            switch (handType)
            {
                case HandTypes.HighCard:
                    achieveNumber = 24;
                    break;
                case HandTypes.Straight:
                    achieveNumber = 27;
                    break;
                case HandTypes.Flush:
                    achieveNumber = 28;
                    break;
                case HandTypes.FullHouse:
                    achieveNumber = 29;
                    break;
                case HandTypes.FourOfAKind:
                    achieveNumber = 30;
                    break;
                case HandTypes.StraightFlush:
                    achieveNumber = 31;
                    achieveNumber_list.Add(27);//顺子
                    break;
                case HandTypes.BiggestStraightFlush:
                    achieveNumber = 18;
                    achieveNumber_list.Add(28);//同花
                    achieveNumber_list.Add(27);//顺子
                    achieveNumber_list.Add(31);//同花顺
                    break;
                default:
                    break;
            }

            if (moneySafeAmnt <= 100)
            {
                // 咸鱼翻身
                achieveNumber_list.Add(25);
            }

            if (achieveNumber != 0)
            {
                if (!isWin && achieveNumber == 18) //发到一次皇家同花顺 this is special
                {
                    if (CheckAndAchieve(userId, achieveNumber))
                    {
                        if (peer != null)
                            peer.SendAchievementEvent(achieveNumber);
                        isAchived = true;
                    }
                }
                else if (isWin)
                {
                    if (CheckAndAchieve(userId, achieveNumber))
                    {
                        if (peer != null)
                            peer.SendAchievementEvent(achieveNumber);
                        isAchived = true;
                    }
                }
            }

            if (isWin)
            {
                foreach (var item in achieveNumber_list)
                {
                    if (CheckAndAchieve(userId, item))
                    {
                        if (peer != null)
                        {
                            peer.SendAchievementEvent(item);
                        }
                        isAchived = true;
                    }
                }
            }

            if (isAchived)
                TryAchieve32(peer, userId);
        }


        public void TryAchieveHole(LilyPeer peer, string userId, GameCard[] porket)
        {
            if (porket.Length < 2 || porket[0].Id < 0 || porket[1].Id < 0)
                return;

            List<byte> achiveList = new List<byte>();
            if (porket[0].Kind == porket[1].Kind)
            {
                //23 底牌同花
                achiveList.Add(23);
            }

            int diff = Math.Abs((int)porket[0].Value - (int)porket[1].Value);
            if (diff == 0)
            {
                //22 底牌对子
                achiveList.Add(22);
            }
            else if (diff == 1)
            {
                //21 底牌顺子
                achiveList.Add(21);
            }
            else if (diff == 12) // A 2
            {
                //21 底牌顺子
                if (porket[0].Kind == porket[1].Kind)
                    achiveList.Add(21);
            }

            if (achiveList.Count > 0)
            {
                foreach (var item in achiveList)
                {
                    if (CheckAndAchieve(userId, item))
                    {
                        if (peer != null)
                            peer.SendAchievementEvent(item);
                    }
                }

                goto AchieveALLA;
            }

            return;
        AchieveALLA:
            TryAchieve32(peer, userId);

        }

        /// <summary>
        /// ken die 17,26
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="porket"></param>
        /// <param name="boad"></param>
        /// <param name="ht"></param>
        public void TryAchieveKenDie(LilyPeer peer, GameCard[] porket, GameCard[] boad, Hand.HandTypes ht, Dictionary<string, int> achivemnt17Times)
        {
            string userId = peer.UserId;
            //GameCard[] nonep = { GameCard.NO_CARD,GameCard.NO_CARD,GameCard.NO_CARD,GameCard.NO_CARD,GameCard.NO_CARD};
            GameCard[] tmp_porket = { boad[0], boad[1] };
            GameCard[] tmp_board = { boad[2], boad[3], boad[4], GameCard.NO_CARD, GameCard.NO_CARD };

            //if (HandStrengthHelper.GetHandStrength(boad,nonep)==HandTypes.BiggestStraightFlush)
            if (HandStrengthHelper.GetHandStrength(tmp_porket, tmp_board) == HandTypes.BiggestStraightFlush)
            {
                if (CheckAndAchieve(peer.UserId, 26))
                {
                    if (peer != null)
                        peer.SendAchievementEvent(26);
                    goto AchieveALLA;
                }
            }

            //if ((byte)ht == (int)HandTypes.HighCard)
            //{
            if (HandStrengthHelper.GetHandStrength(porket, boad) == HandTypes.HighCard)
            {
                if (!achivemnt17Times.ContainsKey(peer.UserId))
                    achivemnt17Times.Add(peer.UserId, 0);
                achivemnt17Times[peer.UserId]++;
                if (achivemnt17Times[peer.UserId] != 2) return;

                if (CheckAndAchieve(peer.UserId, 17))
                {
                    if (peer != null)
                        peer.SendAchievementEvent(17);
                    goto AchieveALLA;
                }
            }
            //}
            else
            {
                if (achivemnt17Times.ContainsKey(peer.UserId))
                {
                    achivemnt17Times[peer.UserId] = 0;
                }
            }

            return;

        AchieveALLA:
            TryAchieve32(peer, userId);
        }

        /// <summary>
        /// 我叫维克托·拉斯提格 ,19
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="players"></param>
        public void TryAchieveVictor(LilyPeer peer, string userId, PlayerInfo currentPlayer, PokerWorld.Game.PokerGame pokergame, Dictionary<string, int> times)
        {
            int count = 3;
            GameCard[] cards = pokergame.Table.Cards;
            List<PlayerInfo> players = pokergame.Table.Players;
            if (players == null || players.Count <= count) return;

            for (int i = 0; i < 5; i++)
            {
                if (cards[i].Id == -1)
                {
                    cards[i] = null;
                }
            }

            try
            {
                string board = String.Format("{0} {1} {2} {3} {4}", cards[0], cards[1], cards[2], cards[3], cards[4]).Trim();
                string currentPocket = String.Format("{0} {1}", currentPlayer.Cards[0], currentPlayer.Cards[1]).Trim();
                uint handvalue = (new Hand(currentPocket, board)).HandValue;

                foreach (var p in players)
                {
                    if ((p as UserData).UserId != userId && p.Cards != null)
                    {
                        if (p.Cards[0].Id < 0 || p.Cards[1].Id < 0) continue;

                        string pocket = String.Format("{0} {1}", p.Cards[0], p.Cards[1]).Trim();
                        uint value = (new Hand(pocket, board)).HandValue;
                        if (value <= handvalue) continue;

                        count--;
                        if (count != 0) continue;

                        if (!times.ContainsKey(userId))
                            times.Add(userId, 0);
                        times[userId]++;
                        if (times[userId] != 2) return;

                        if (CheckAndAchieve(userId, 19))
                        {
                            if (peer != null)
                                peer.SendAchievementEvent(19);
                            TryAchieve32(peer, userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 高端职业玩家
        /// </summary>
        /// <param name="peer"></param>
        private void TryAchieve32(LilyPeer peer, string userId)
        {
            byte[] numbers = AchieveList2;
            if (UserService.getInstance().HasAchieved(userId, numbers))
            {
                byte my32number = 32;
                if (CheckAndAchieve(userId, my32number))
                {
                    Achievement achievement = this.achievements.FirstOrDefault(rs => rs.Number == my32number);
                    UserService.getInstance().ChangeChipAndExp(userId, achievement.Chip, achievement.Exp);
                    UserData curUser = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
                    if (peer != null)
                        peer.SendAchievementEvent(my32number, curUser.Chips, curUser.Level, curUser.LevelExp);

                    // 完成所有成就
                    if (this.TryAchieve11(userId))
                    {
                        if (peer != null)
                        {
                            curUser = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
                            peer.SendAchievementEvent(11, curUser.Chips, curUser.Level, curUser.LevelExp);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
