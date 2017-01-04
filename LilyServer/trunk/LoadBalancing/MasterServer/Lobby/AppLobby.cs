// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLobby.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AppLobby type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.LoadBalancing.Events;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using DataPersist;
    using System.Linq;
    using LilyServer;

    #endregion

    public class AppLobby
    {
        #region Constants and Fields

        public readonly TimeSpan JoinTimeOut = TimeSpan.FromSeconds(10);

        public readonly int MaxPlayersDefault; 

        protected readonly GameList GameList;

        protected readonly LoadBalancer<IncomingGameServerPeer> LoadBalancer;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected readonly HashSet<PeerBase> peers = new HashSet<PeerBase>();

        private IDisposable schedule;

        #endregion

        #region Constructors and Destructors

        public AppLobby(LoadBalancer<IncomingGameServerPeer> loadBalancer)
            : this(loadBalancer, 0, TimeSpan.FromSeconds(10))
        {
        }

        public AppLobby(LoadBalancer<IncomingGameServerPeer> loadBalancer, int maxPlayersDefault, TimeSpan joinTimeOut)
        {
            this.MaxPlayersDefault = maxPlayersDefault;
            this.JoinTimeOut = joinTimeOut;

            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();

            this.LoadBalancer = loadBalancer;
            this.GameList = new GameList();

            this.ExecutionFiber.Schedule(this.CheckJoinTimeOuts, (long)joinTimeOut.TotalMilliseconds / 2);
        }

        #endregion

        #region Properties

        public PoolFiber ExecutionFiber { get; private set; }

        #endregion

        #region Public Methods

        public void EnqueueOperation(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }

        public void RemoveGame(string gameId)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemoveGameState(gameId));
        }

        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemoveGameServer(gameServer));
        }

        public void RemovePeer(MasterClientPeer serverPeer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemovePeer(serverPeer));
        }

        public void AddPeer(MasterClientPeer serverPeer) {
            this.ExecutionFiber.Enqueue(()=>this.peers.Add(serverPeer));
        }

        public void UpdateGameState(UpdateGameEvent operation)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleUpdateGameState(operation));
        }

        public void AddGameState(UpdateGameEvent operation, IncomingGameServerPeer gameServer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleAddGameState(operation, gameServer));
        }

        #endregion

        #region Methods

        protected virtual void ExecuteOperation(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse response;
            switch ((OperationCode)operationRequest.OperationCode)
            {
                default:
                    response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = -1, DebugMessage = "Unknown operation code" };
                    break;

                case OperationCode.JoinLobby:
                    response = this.HandleJoinLobby(peer, operationRequest);
                    break;

                case OperationCode.LeaveLobby:
                    response = this.HandleLeaveLobby(peer, operationRequest);
                    break;
                //case OperationCode.HistoryAction:
                 //   response = this.HandleHistoryAction(peer, operationRequest);
                //    break;
                case OperationCode.CreateGame:
                    response = this.HandleCreateGame(peer, operationRequest);
                    break;

                case OperationCode.JoinGame:
                    response = this.HandleJoinGame(peer, operationRequest);
                    break;

                case OperationCode.JoinRandomGame:
                    response = this.HandleJoinRandomGame(peer, operationRequest);
                    break;

                case OperationCode.CancelJoinRandomGame:
                    response = this.HandleCancelJoinRandomGame(peer, operationRequest);
                    break;                
            }          

            if (response != null)
            {
                peer.SendOperationResponse(response, sendParameters);
            }
        }

        public OperationResponse HandleHistoryAction(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            OperationResponse response;
            var operation = new CreateGameRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            // if no gameId is specified by the client generate a unique id 
            if (string.IsNullOrEmpty(operation.GameId))
            {
                operation.GameId = Guid.NewGuid().ToString();
            }
            else
            {
                string gameid = operation.GameId;
                // check if a game with the specified id already exists
                if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.StepID))
                {
                    int sessionid = peer.ConnectionId;//peer.UserId;
                    int stepid = (int)operationRequest.Parameters[(byte)LilyOpKey.StepID];
                    gameid = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;
                    int tableroud = (int)operationRequest.Parameters[(byte)LilyOpKey.TableRound];
                    string mail = operationRequest.Parameters[(byte)LilyOpKey.Mail] as string;
                    if (this.GameList.ContainsGameId(gameid))
                    {
                        GameState gameStateHistory;
                        // try to find game by id
                        if (this.GameList.TryGetGame(gameid, out gameStateHistory) == false)
                        {
                            return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.GameIdNotExists, DebugMessage = "Game does not exist" };
                        }
                        IncomingGameServerPeer lastGameServer = gameStateHistory.GameServer;
                        if (lastGameServer != null)
                        {
                            lastGameServer.AskForGameHistoryActions(gameid, tableroud, stepid, sessionid, mail);
                            log.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@get AskForGameHistoryActions request");
                            
                        }
                    }
                }
            }
            return null;
        }

        protected virtual object GetCreateGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new CreateGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer), NodeId = gameState.NodeId,ErrorCode=(int)DataPersist.ErrorCode.Sucess};
        }

        protected virtual object GetJoinGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer), NodeId = gameState.NodeId, ErrorCode = (int)DataPersist.ErrorCode.Sucess };
        }

        protected virtual object GetJoinRandomGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinRandomGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer), NodeId = gameState.NodeId };
        }

        protected virtual OperationResponse HandleCancelJoinRandomGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.OperationDenied, DebugMessage = "Invalid operation" };
        }

        protected virtual OperationResponse HandleCreateGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            OperationResponse response;
            var operation = new CreateGameRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            // if no gameId is specified by the client generate a unique id 
            if (string.IsNullOrEmpty(operation.GameId))
            {
                operation.GameId = Guid.NewGuid().ToString();
            }
            else
            {
                string gameid = operation.GameId;
                // check if a game with the specified id already exists
                //if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.StepID))
                //{
                //    int sessionId = peer.ConnectionId;
                //    int stepid = (int)operationRequest.Parameters[(byte)LilyOpKey.StepID];
                //    gameid = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;
                //    int tableroud = (int)operationRequest.Parameters[(byte)LilyOpKey.TableRound];
                //    if (this.GameList.ContainsGameId(gameid))
                //    {
                //        GameState gameStateHistory;
                //        // try to find game by id
                //        if (this.GameList.TryGetGame(gameid, out gameStateHistory) == false)
                //        {
                //            return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.GameIdNotExists, DebugMessage = "Game does not exist" };
                //        }
                //        IncomingGameServerPeer lastGameServer = gameStateHistory.GameServer;
                //        if (lastGameServer != null)
                //        {
                //            lastGameServer.AskForGameHistoryActions(gameid, tableroud, stepid, sessionId);
                //        }
                //    }                        
                //}

                if (this.GameList.ContainsGameId(gameid))
                {
                    
                    //if a game with the specified id already exists, then return exist game response
                    response = this.HandleJoinGame(peer,operationRequest);
                    return response;
                }
            }

            // try to create game
            GameState gameState;
            if (!this.TryCreateGame(operation, operation.GameId, operation.GameProperties, out gameState, out response))
            {
                return response;
            }

            // add peer to game
            gameState.AddPeer(peer);

            // publish operation response
            object createGameResponse = this.GetCreateGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, createGameResponse);
        }

        protected virtual OperationResponse HandleJoinGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            var operation = new JoinGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            GameState gameState;

            // try to find game by id
            if (this.GameList.TryGetGame(operation.GameId, out gameState) == false)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.GameIdNotExists, DebugMessage = "Game does not exist" };
            }

            // check if max players of the game is already reached
            if (gameState.MaxPlayer > 0 && gameState.PlayerCount >= gameState.MaxPlayer)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.GameFull, DebugMessage = "Game full" };
            }

            // check if the game is open
            if (gameState.IsOpen == false)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.GameClosed, DebugMessage = "Game closed" };
            }

            // check if user is blocked by peers allready in the game
            if (gameState.IsBlocked(peer))
            {
                //return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.UserBlocked, DebugMessage = "User is blocked" };
                object joinResponse1 = this.GetJoinGameResponse(peer, gameState);
                return new OperationResponse(operationRequest.OperationCode, joinResponse1);
            }
            
            // add peer to game
            if (gameState.TryAddPeer(peer) == false)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.UserBlocked, DebugMessage = "Already joined the specified game." };
            }

            // publish operation response
            object joinResponse = this.GetJoinGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleJoinLobby(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // check if peer already joined
            if (this.peers.Add(peer) == false)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.OperationDenied, DebugMessage = "Peer already joined the lobby" };
            }
            
            // publish game list to peer after the response has been sent
            //this.ExecutionFiber.Enqueue(() => this.PublishGameList(peer));



            return new OperationResponse(operationRequest.OperationCode);
        }

        protected virtual OperationResponse HandleJoinRandomGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            var operation = new JoinRandomGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            GameState game;
            if (this.GameList.TryGetRandomGame(peer, operation.GameProperties, out game) == false)
            {
                response = new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.NoMatchFound, DebugMessage = "No match found" };
                return response;
            }

            // match found, add peer to game and notify the peer
            game.AddPeer(peer);

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Found match: connectionId={0}, userId={1}, gameId={2}", peer.ConnectionId, peer.UserId, game.Id);
            }

            object joinResponse = this.GetJoinRandomGameResponse(peer, game);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleLeaveLobby(MasterClientPeer peer, OperationRequest operationRequest)
        {
            if (this.peers.Remove(peer))
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode };
            }

            return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.OperationDenied, DebugMessage = "lobby not joined" };
        }

        protected virtual void OnGameStateChanged(GameState gameState)
        {
        }

        protected virtual void OnRemovePeer(MasterClientPeer peer)
        {
        }

        private void CheckJoinTimeOuts()
        {
            this.GameList.CheckJoinTimeOuts(this.JoinTimeOut);
            this.ExecutionFiber.Schedule(this.CheckJoinTimeOuts, (long)this.JoinTimeOut.TotalMilliseconds / 2);
        }

        private void HandleRemoveGameServer(IncomingGameServerPeer gameServer)
        {
            this.GameList.RemoveGameServer(gameServer);

            if (this.schedule == null)
            {
                this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, 1000);
            }
        }

        private void HandleRemoveGameState(string gameId)
        {
            GameState gameState;
            if (this.GameList.TryGetGame(gameId, out gameState) == false)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("HandleRemoveGameState: Game not found - gameId={0}", gameId);
                }

                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("HandleRemoveGameState: gameId={0}, joiningPlayers={1}", gameId, gameState.JoiningPlayerCount);
            }

            this.GameList.RemoveGameState(gameId);

            if (this.schedule == null)
            {
                this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, 1000);
            }
        }

        private void HandleRemovePeer(MasterClientPeer peer)
        {
            this.peers.Remove(peer);
            this.OnRemovePeer(peer);
        }

        private void HandleUpdateGameState(UpdateGameEvent operation)
        {
            GameState gameState;
            if (this.GameList.UpdateGameState(operation, out gameState) == false)
            {
                // HandleAddGameState(operation, gameServer);
                return;
            }

            if (this.schedule == null)
            {
                this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, 1000);
            }

            this.OnGameStateChanged(gameState);
        }

        private void HandleAddGameState(UpdateGameEvent operation, IncomingGameServerPeer gameServer)
        {
            // try to create create new game state
            if (!this.GameList.ContainsGameId(operation.GameId))
            {
                GameState gameState = new GameState(operation.GameId, (byte)this.MaxPlayersDefault, gameServer);

                //这个是错的，不能新建个空的peer
                MasterClientPeer new_peer = new MasterClientPeer();
                new_peer.UserId = operation.NewUsers[0];
                gameState.TryAddPeer(new_peer);

                this.GameList.AddGameState(gameState);
                log.Info("-------------------GameList:" + this.GameList.Count + ",gameStateMaxPlayers:" + gameState.MaxPlayer);
                if (this.schedule == null)
                {
                    this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, 1000);
                }
            }
        }

        private void PublishGameChanges()
        {
            return; //暂时没用，先注掉
            this.schedule = null;

            if (this.GameList.ChangedGamesCount > 0)
            {
                Hashtable changedGames = this.GameList.GetChangedGames();

                var e = new GameListUpdateEvent { Data = changedGames };
                var eventData = new EventData((byte)EventCode.GameListUpdate, e);
                eventData.SendTo(this.peers, new SendParameters());
            }
        }

        protected virtual void PublishGameList(PeerBase peer)
        {
            var e = new GameListEvent { Data = this.GameList.GetAllGames() };
            var eventData = new EventData((byte)EventCode.GameList, e);
            peer.SendEvent(eventData, new SendParameters());
        }

        private bool TryCreateGame(Operation operation, string gameId, Hashtable properties, out GameState gameState, out OperationResponse errorResponse)
        {
            // try to get a game server instance from the load balancer            
            IncomingGameServerPeer gameServer;
            if (!this.LoadBalancer.TryGetServer(out gameServer))
            {
                errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                    {
                        ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.ServerFull,
                        DebugMessage = "Failed to get server instance."
                    };
                gameState = null;
                return false;
            }

            // try to create create new game state
            gameState = new GameState(gameId, (byte)this.MaxPlayersDefault, gameServer);
            if (properties != null)
            {
                bool changed;
                string debugMessage;

                if (!gameState.TrySetProperties(properties, out changed, out debugMessage))
                {
                    errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                        {
                            ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.OperationInvalid,
                            DebugMessage = debugMessage
                        };
                    return false;
                }
            }

            this.GameList.AddGameState(gameState);
            log.Info("-------------------GameList:" + this.GameList.Count+",gameStateMaxPlayers:"+gameState.MaxPlayer);
            if (this.schedule == null)
            {
                this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, 1000);
            }

            errorResponse = null;
            return true;
        }

        #endregion



        #region LilyMethod

        public void SendHistoryActionResponse(OperationResponse response) {
            this.ExecutionFiber.Enqueue(()=>this.HandleSendHistoryActionResponse(response));
        }

        private void HandleSendHistoryActionResponse(OperationResponse response) {
            response.OperationCode = (byte)LilyOpCode.HistoryAction;
            int nodeId = (int)response.Parameters[(byte)LilyOpKey.UserId];

            //if (LilyServer.Actors.Exists(r => r.Peer.ConnectionId == nodeId))
            //if (Exists(r => r.Peer.ConnectionId == nodeId))
            {
                //PeerBase peer=LilyServer.Actors.FirstOrDefault(r => r.Peer.ConnectionId == nodeId).Peer;
                PeerBase peer = peers.FirstOrDefault(r => r.ConnectionId == nodeId);
                if (peer!=null)
                {
                    //if (!response.Parameters.ContainsKey((byte)DataPersist.LilyOpKey.HistoryAction))
                    //{
                        
                    //    OperationRequest request=new OperationRequest((byte)LilyOpCode.HistoryAction);
                    //    request.Parameters.Add((byte)LilyOpKey.GameId,(string)response.Parameters[(byte)LilyOpKey.GameId]);
                    //    response=this.HandleCreateGame(peer as MasterClientPeer, request);

                    //}
                    peer.SendOperationResponse(response, new SendParameters());
                }
            }
        }

        #endregion
    }
}