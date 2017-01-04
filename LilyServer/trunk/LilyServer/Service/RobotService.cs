using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using LilyServer.Operations;
using DataPersist;
using LilyServer.Helper;
using System.Threading;
using LilyServer.Model;
using Lite.Messages;
using System.Configuration;
using ExitGames.Logging;

namespace LilyServer
{
    class RobotService
    {
        private const int ROBOT_CAPACITY = 500;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static int ROBOT_STARTID = Convert.ToInt32(ConfigurationManager.AppSettings["RobotIdStart"]);
        private static int ROBOT_ENDID = Convert.ToInt32(ConfigurationManager.AppSettings["RobotIdEnd"]);

        private static RobotService robotService;

        private LilyEntities lilyEntities = new LilyEntities();
        private object lockObj = new object();
        private Thread thread;
        private AutoResetEvent autoResetEvent;

        public ConcurrentQueue<Lite.Room> MessageQueue { get; set; }
        public List<RobotQueue> RobotQueues { get; set; }

        public static RobotService Singleton{
            get{
                if (robotService == null)
                {
                    robotService = new RobotService();
                }
                return robotService;
            }
        }

        private RobotService()
        {
            RobotQueues = new List<RobotQueue>();
            lilyEntities = new LilyEntities();
            MessageQueue = new ConcurrentQueue<Lite.Room>();
            this.autoResetEvent = new AutoResetEvent(false);
            this.thread = new Thread(new ThreadStart(ProcessMessage));
            this.thread.Start();
        }

        public void Request(Lite.Room roomId)
        {

            log.DebugFormat("career Request Robot: roomId:{0} ",roomId.Name);
            MessageQueue.Enqueue(roomId);
            this.autoResetEvent.Set();
        }

        private void ProcessMessage()
        {
            while (true)
            {
                autoResetEvent.WaitOne();
                if (RobotQueues.Count < 1 || RobotQueues[RobotQueues.Count - 1].IsEmpty)
                {
                    if (!this.TryAllocRobots())
                    {
                        break;
                    }
                }
                else
                {
                    DeallocRobots();
                }

                while (MessageQueue.Count > 0)
                {
                    Lite.Room roomId;
                    if (MessageQueue.TryDequeue(out roomId))
                    {
                        //Lite.Caching.RoomReference gr;
                        LilyGame game = roomId as LilyGame;
                        //if (LilyGameCache.Instance.TryGetRoomReference(roomId, out gr))
                        //{
                        //    game = gr.Room as LilyGame;
                        //}
                        if (game != null && game.PokerGame != null 
                                //&& game.PokerGame.Table.Players.Count == 1 
                                && !game.OnlyFriend)
                        {


                            //先执行 离开策略
                            if(game.PokerGame.GameType!=GameType.Career){
                            int leaveCount = RobotHelper.MoveRobotCount(game.PokerGame.Table.Players.Count);
                            Random x = new Random();
                            List<PlayerInfo> newList = new List<PlayerInfo>();
                            foreach (PlayerInfo item in game.PokerGame.Table.Players)
                            {
                                newList.Insert(x.Next(newList.Count), item);
                            }
                            if (leaveCount > 0)
                                foreach (PlayerInfo player in newList)
                                {
                                    if (player.IsRobot)
                                        if (--(player as RobotInfo).Alive == 0 || (game.PokerGame.State == TypeState.WaitForPlayers && leaveCount > 0))
                                        {
                                            game.PokerGame.LeaveGame(player);
                                            leaveCount--;
                                        }

                                    if (leaveCount <= 0)
                                    {
                                        break;
                                    }
                                }
                            }                       

                            // then add robot in.....
                            //int robotCount = RobotHelper.CreateRobotCount();
                            int robotCount = game.PokerGame.GameType == GameType.Career
                                                 ? 1
                                                 : RobotHelper.CreateRobotCount(game.PokerGame.Table.Players.Count);
                            //Random random = new Random();
                            for (int i = 0; i < robotCount; i++)
                            {
                                int second = RobotHelper.RandomRange(3,30);
                                //Thread.Sleep(second * 1000);
                                game.ScheduleMessage(new RoomMessage((byte)LilyMessageCode.AddRobotInGame, game),second*1000);                                
                                //game.EnqueueMessage(new RoomMessage((byte)LilyMessageCode.AddRobotInGame, game));
                            }
                        }
                    }
                }
            }
        }

        public void AddRobotInGame(LilyGame game)
        {
            AddRobotInGame(game, -1);
        }


        private void AddRobotInGame(LilyGame game, int noSeat) {
            if (game.PokerGame != null)
            {
                RobotQueue robotQueue = this.RobotQueues.FirstOrDefault(rs => !rs.IsEmpty);
                RobotInfo robot;
                if (robotQueue == null || !robotQueue.TryDequeue(out robot))
                {
                    return;
                }
                robot.Alive =RobotHelper.RandomRange(4, 12);

                int bigBlind = game.PokerGame.Table.BigBlindAmnt;

                long takenAmnt = bigBlind * RobotHelper.RandomRange(30, 50);

                long chipsdiff = takenAmnt * 10 - robot.Chips;


                //if (robot.Chips<bigBlind*50)
                //{
                //long needMoney = bigBlind * 50 - robot.Chips;
                PokerGameCareer cgame = game.PokerGame as PokerGameCareer;
                if(cgame!=null)
                {
                    chipsdiff += cgame.GameGrade.Tickets;
                }

                robot.Chips = robot.Chips + chipsdiff;
                UserService.getInstance().ChipsChanged(robot as UserData, chipsdiff);
                BankService.getInstance().addRecord(chipsdiff, BankActionType.RobotTaken, robot.UserId);
                // }

                robot.MoneyInitAmnt = takenAmnt;
                robot.MoneySafeAmnt = takenAmnt;
                if (noSeat > -1)
                    robot.NoSeat = noSeat;

                game.PokerGame.SitInGame(robot);


                log.DebugFormat("career Request Robot: roomId:{0},robot:{1},{2} ", game.Name,robot.NoSeat,robot.NickName);
                if (robot.NoSeat == -1)
                {
                    robotQueue.Enqueue(robot);
                    return;
                }
                game.PokerGame.TryStartGame();
            }
        }

        private bool TryAllocRobots()
        {
            lock (lockObj)
            {
                int robotCount=RobotQueues.Sum(rs=>rs.RobotCapacity);
                if (lilyEntities.bots.Count() - robotCount > 0)
                {
                    RobotQueue robotQueue = new RobotQueue();
                    IQueryable<bots> bots = (from _b in lilyEntities.bots
                                             where _b.id >= ROBOT_STARTID && _b.id < ROBOT_ENDID
                                             orderby _b.id
                                             select _b).Skip(robotCount).Take(ROBOT_CAPACITY);
                                            
                        
                        //lilyEntities.bots.SelectMany(rs => rs.id >= ROBOT_STARTID&&rs.id<ROBOT_ENDID).OrderBy(rs => rs.id).Skip(robotCount).Take(ROBOT_CAPACITY);
                    robotQueue.RobotCapacity = bots.Count();
                    Random random = new Random();
                    List<int> temp = new List<int>();
                    for (int i = 0; i < robotQueue.RobotCapacity; i++)
                    {
                        temp.Add(i);
                    }
                    int[] positions = new int[robotQueue.RobotCapacity];
                    for (int i = 0; i < robotQueue.RobotCapacity; i++)
                    {
                        int index = random.Next(0, temp.Count);
                        positions[i] = temp[index];
                        temp.RemoveAt(index);
                    }

                    bots[] robots = bots.ToArray();
                    for (int i = 0; i < robotQueue.RobotCapacity; i++)
                    {
                        int pos = positions[i];
                        RobotInfo robot = robots[pos].ToRobotInfo();
                        robot.QueueIndex = RobotQueues.Count;
                        robotQueue.Enqueue(robot);
                    }
                    RobotQueues.Add(robotQueue);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void DeallocRobots()
        {
            lock (lockObj)
            {
                int count=this.RobotQueues.Count;
                if (count > 2 && this.RobotQueues[count - 1].CanDealloc&& this.RobotQueues[count - 2].CanDealloc)
                {
                    this.RobotQueues.RemoveAt(count - 1);
                }
            }
        }



        public void AddRobotInSystemGameBeforePlayerJoined(LilyGame game) {
            int botNum = RobotHelper.CreateRobotCount();
            botNum = Math.Max(botNum,2);
            if (game.PokerGame != null&&botNum>0)
            {

                List<int> randSeat = Enumerable.Range(0, 8)
                                    .Select(x => new {v = x, k = Guid.NewGuid().ToString() }).ToList()
                                    .OrderBy(x => x.k)
                                    .Select(x => x.v)
                                    .Take(botNum).ToList();
                foreach (int noSeat in randSeat)
                {
                    AddRobotInGame(game,noSeat);
                }
                //while (botNum>0)
                //{
                //    AddRobotInGame(game);
                //    botNum--;
                //}
                game.PokerGame.TryStartGame2();
            }

        }

        public void AddRobotInCareerGame(LilyGame game)
        {
            //to do 
        }
    }
}
