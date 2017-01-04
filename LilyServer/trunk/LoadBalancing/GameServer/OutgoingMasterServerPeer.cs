// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutgoingMasterServerPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OutgoingMasterServerPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;

    using ExitGames.Logging;

    using Photon.LoadBalancing.LoadShedding;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.LoadBalancing.ServerToServer.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using PhotonHostRuntimeInterfaces;

    using OperationCode = Photon.LoadBalancing.ServerToServer.Operations.OperationCode;
    using Photon.LoadBalancing.Events;
    using DataPersist;
    using LilyServer;
    using System.Linq;

    #endregion

    public class OutgoingMasterServerPeer : ServerPeerBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication application;

        private bool redirected;

        private IDisposable updateLoop;

        #endregion

        #region Constructors and Destructors

        public OutgoingMasterServerPeer(IRpcProtocol protocol, IPhotonPeer nativePeer, GameApplication application)
            : base(protocol, nativePeer)
        {
            this.application = application;
            log.InfoFormat("connection to master at {0}:{1} established (id={2})", this.RemoteIP, this.RemotePort, this.ConnectionId);
            this.RequestFiber.Enqueue(this.Register);
        }

        #endregion

        #region Properties

        protected bool IsRegistered { get; set; }

        #endregion

        #region Public Methods

        public void RemoveGameState(string gameId)
        {
            if (!this.IsRegistered)
            {
                return;
            }

            var parameter = new Dictionary<byte, object> { { (byte)ParameterCode.GameId, gameId }, };
            var eventData = new EventData { Code = (byte)ServerEventCode.RemoveGameState, Parameters = parameter };
            this.SendEvent(eventData, new SendParameters());
        }

        public void AddGameState(string gameId, string userId, byte actorCount) {
            if (!this.IsRegistered)
            {
                return;
            }
            string[] newUsers = new string[] { userId };
            var e = new UpdateGameEvent { GameId = gameId, ActorCount = actorCount, NewUsers = newUsers };

            var eventData = new EventData((byte)ServerEventCode.AddGameState, e);
            this.SendEvent(eventData, new SendParameters());
        }

        public void UpdateGameState(string gameId, byte actorCount, Hashtable changedProperties, List<string> newUsers, List<string> removedUsers)
        {
            if (!this.IsRegistered)
            {
                return;
            }

            var e = new UpdateGameEvent { GameId = gameId, ActorCount = actorCount, GameProperties = changedProperties };

            if (newUsers != null)
            {
                e.NewUsers = newUsers.ToArray();
            }

            if (removedUsers != null)
            {
                e.RemovedUsers = removedUsers.ToArray();
            }

            var eventData = new EventData((byte)ServerEventCode.UpdateGameState, e);
            this.SendEvent(eventData, new SendParameters());
        }

        public void UpdateServerState(FeedbackLevel workload, int peerCount)
        {
            if (!this.IsRegistered)
            {
                return;
            }

            var contract = new UpdateServerEvent { LoadIndex = (byte)workload, PeerCount = peerCount };
            var eventData = new EventData((byte)ServerEventCode.UpdateServer, contract);
            this.SendEvent(eventData, new SendParameters());
        }

        #endregion

        #region Methods

        protected virtual void HandleRegisterGameServerResponse(OperationResponse operationResponse)
        {
            var contract = new RegisterGameServerResponse(this.Protocol, operationResponse);
            if (!contract.IsValid)
            {
                if (operationResponse.ReturnCode != (short)Photon.LoadBalancing.Operations.ErrorCode.Ok)
                {
                    log.ErrorFormat("RegisterGameServer returned with err {0}: {1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
                }

                log.Error("RegisterGameServerResponse contract invalid: " + contract.GetErrorMessage());
                this.Disconnect();
                return;
            }

            switch (operationResponse.ReturnCode)
            {
                case (short)Photon.LoadBalancing.Operations.ErrorCode.Ok:
                    {
                        log.Info("Successfully registered at master server");
                        this.IsRegistered = true;
                        this.StartUpdateLoop();
                        break;
                    }

                case (short)Photon.LoadBalancing.Operations.ErrorCode.RedirectRepeat:
                    {
                        // TODO: decide whether to connect to internal or external address (config)
                        // use a new peer since we otherwise might get confused with callbacks like disconnect
                        var address = new IPAddress(contract.InteralAddress);
                        log.InfoFormat("Connected master server is not the leader; Reconnecting to node {0} at IP {1}...", contract.MasterNode, address);
                        this.Reconnect(address, 0); // don't use proxy for direct connections

                        // enable for external address connections
                        //// var address = new IPAddress(contract.ExternalAddress);
                        //// log.InfoFormat("Connected master server is not the leader; Reconnecting to node {0} at IP {1}...", contract.MasterNode, address);
                        //// this.Reconnect(address, contract.MasterNode);
                        break;
                    }

                default:
                    {
                        log.WarnFormat("Failed to register at master: err={0}, msg={1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
                        this.Disconnect();
                        break;
                    }
            }
        }

        protected override void OnDisconnect()
        {
            this.IsRegistered = false;
            this.StopUpdateLoop();

            // if RegisterGameServerResponse tells us to connect somewhere else we don't need to reconnect here
            if (this.redirected)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} disconnected from master server", this.ConnectionId);
                }
            }
            else
            {
                log.InfoFormat("connection to master closed (id={0})", this.ConnectionId);
                this.application.ReconnectToMaster();
            }
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            switch ((EventCode)eventData.Code)
            {
                case EventCode.RemovePeerFromRoom:
                    string userId = eventData.Parameters[(byte)LilyOpKey.UserId] as string;
                    if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == userId))
                    {
                        LilyPeer peer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == userId).Peer as LilyPeer;
                        peer.RemovePeer();
                    }
                    break;
            }
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Received unknown operation code {0}", request.OperationCode);
            }

            OperationResponse response;
            switch ((OperationCode)request.OperationCode)
            {
                case OperationCode.RegisterGameServer:
                    response = new OperationResponse { OperationCode = request.OperationCode, ReturnCode = -1, DebugMessage = "Unknown operation code" };
                    break;
                case OperationCode.GameHistoryActions:
                    {
                        log.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@get AskForGameHistoryActions request");
                        Dictionary<byte, object> dic = new Dictionary<byte, object>();
                        dic.Add((byte)LilyOpKey.ErrorCode, DataPersist.ErrorCode.Sucess);
                        if (request.Parameters.ContainsKey((byte)LilyOpKey.StepID))
                        {
                            int stepid = (int)request.Parameters[(byte)LilyOpKey.StepID];
                            string gameid = request.Parameters[(byte)LilyOpKey.GameId] as string;
                            int tableroud = (int)request.Parameters[(byte)LilyOpKey.TableRound];
                            int sessionId = (int)request.Parameters[(byte)LilyOpKey.UserId];
                            string mail = request.Parameters[(byte)LilyOpKey.Mail] as string;
                            //UserData user = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
                            //string nodeId = request.Parameters[(byte)LilyOpKey.NodeId] as string;
                            Lite.Room rrf = GameCache.Instance.getAllRooms().FirstOrDefault(r => r.Name == gameid);                           
                            if (rrf != null)
                            {
                                LilyGame game = rrf as LilyGame;

                                //if(stepid==-1)
                                //{
                                //    if (game.PokerGame != null && game.PokerGame.Table.Players.Any(p => ((UserData) p).Mail == mail))
                                //    {
                                //        dic.Add((byte)LilyOpKey.HistoryAction, null);
                                //        dic.Add((byte)LilyOpKey.GameId, gameid);
                                //    }

                                //}else 
                                if (game.PokerGame != null && game.PokerGame.HistoryActions != null && game.PokerGame.Table.Players.Any(p =>((UserData) p).Mail == mail))
                                {
                                    dic.Add((byte)LilyOpKey.HistoryAction,DataPersist.HelperLib.SerializeHelper.Serialize(game.PokerGame.GetHistoricActions(stepid, tableroud)));
                                    dic.Add((byte)LilyOpKey.GameId, gameid);
                                }
                            }
                            dic.Add((byte)LilyOpKey.UserId, sessionId);
                            //Lite.Caching.RoomReference rrf = null;
                            //if (LilyGameCache.Instance.TryGetRoomReference(gameid, out rrf))
                            //{
                            //    LilyGame game = rrf.Room as LilyGame;
                            //    if (game.PokerGame != null && game.PokerGame.HistoryActions != null && game.PokerGame.Table.Players.Any(p => p.Name == user.Name))
                            //    {
                            //        dic.Add((byte)LilyOpKey.HistoryAction, game.PokerGame.getHistoricActions(stepid, tableroud).Tobyte());
                            //    }
                            //}
                        }
                        response = new OperationResponse(request.OperationCode, dic);
                    }
                    break;
                default:
                    response = new OperationResponse { OperationCode = request.OperationCode, ReturnCode = -1, DebugMessage = "Unknown operation code" };
                    break;
            }

            //var response = new OperationResponse { OperationCode = request.OperationCode, ReturnCode = -1, DebugMessage = "Unknown operation code" };
            this.SendOperationResponse(response, sendParameters);
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            switch ((OperationCode)operationResponse.OperationCode)
            {
                default:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Received unknown operation code {0}", operationResponse.OperationCode);
                        }

                        break;
                    }

                case OperationCode.RegisterGameServer:
                    {
                        this.HandleRegisterGameServerResponse(operationResponse);
                        break;
                    }
            }
        }

        protected void Reconnect(IPAddress address, byte masterNode)
        {
            this.redirected = true;

            GameApplication.Instance.ConnectToMaster(new IPEndPoint(address, this.RemotePort), masterNode);
            this.Disconnect();
            this.Dispose();
        }

        protected virtual void Register()
        {
            var contract = new RegisterGameServer
                {
                    GameServerAddress = this.application.PublicIPAddress.ToString(), 
                    TcpPort = this.application.GamingTcpPort, 
                    UdpPort = this.application.GamingUdpPort, 
                    ServerId = GameApplication.ServerId
                };
            log.InfoFormat("Registering game server with address {0}, TCP {1}, UDP {2}, ServerID {3}", contract.GameServerAddress, contract.TcpPort, contract.UdpPort, contract.ServerId);
            var request = new OperationRequest((byte)OperationCode.RegisterGameServer, contract);
            this.SendOperationRequest(request, new SendParameters());
        }

        protected void StartUpdateLoop()
        {
            if (this.updateLoop != null)
            {
                log.Error("Update Loop already started! Duplicate RegisterGameServer response?");
                this.updateLoop.Dispose();
            }

            this.updateLoop = this.RequestFiber.ScheduleOnInterval(this.UpdateServerState, 1000, 1000);
            GameApplication.Instance.WorkloadController.FeedbacklevelChanged += this.WorkloadController_OnFeedbacklevelChanged;
        }

        protected void StopUpdateLoop()
        {
            if (this.updateLoop != null)
            {
                this.updateLoop.Dispose();
                this.updateLoop = null;

                GameApplication.Instance.WorkloadController.FeedbacklevelChanged -= this.WorkloadController_OnFeedbacklevelChanged;
            }
        }

        private void UpdateServerState()
        {
            this.UpdateServerState(GameApplication.Instance.WorkloadController.FeedbackLevel, GameApplication.Instance.PeerCount);
        }

        private void WorkloadController_OnFeedbacklevelChanged(object sender, EventArgs e)
        {
            this.UpdateServerState(GameApplication.Instance.WorkloadController.FeedbackLevel, GameApplication.Instance.PeerCount);
        }

        #endregion
    }
}