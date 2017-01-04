// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameClientPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GamePeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using ExitGames.Logging;

    using Lite;
    using Lite.Caching;
    using Lite.Messages;
    using Lite.Operations;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;
    using LilyServer;
    using DataPersist;
    using System.Collections.Generic;

    #endregion

    public class GameClientPeer : LilyPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private readonly GameLobby lobby;

        #endregion

        #region Constructors and Destructors

        public GameClientPeer(InitRequest initRequest,GameLobby _lobby)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            this.PeerId = string.Empty;
            this.lobby = _lobby;
        }

        #endregion

        #region Properties

        public string PeerId { get; protected set; }

        #endregion

        #region Public Methods

        public void OnJoinFailed(Operations.ErrorCode result)
        {
            this.RequestFiber.Enqueue(() => this.OnJoinFailedInternal(result));
        }

        #endregion

        #region Methods

        protected override RoomReference GetRoomReference(JoinRequest joinRequest)
        {
            return GameCache.Instance.GetRoomReference(joinRequest.GameId);
        }

        protected virtual void HandleCreateGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // The JoinRequest from the Lite application is also used for create game operations to support all feaures 
            // provided by Lite games. 
            // The only difference is the operation code to prevent games created by a join operation. 
            // On "LoadBalancing" game servers games must by created first by the game creator to ensure that no other joining peer 
            // reaches the game server before the game is created.
            var createRequest = new JoinRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(createRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom();

            // try to create the game
            RoomReference gameReference;
            if (this.TryCreateRoom(createRequest.GameId, out gameReference) == false)
            {
                var response = new OperationResponse
                    {
                        OperationCode = (byte)OperationCode.CreateGame,
                        ReturnCode = (short)Operations.ErrorCode.GameIdAlreadyExists,
                        DebugMessage = "Game already exists"
                    };

                this.SendOperationResponse(response, sendParameters);
                return;
            }

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        /// <summary>
        ///   Handles the <see cref = "JoinRequest" /> to enter a <see cref = "Game" />.
        ///   This method removes the peer from any previously joined room, finds the room intended for join
        ///   and enqueues the operation for it to handle.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleJoinGameOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // create join operation
            var joinRequest = new JoinRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(joinRequest, sendParameters) == false)
            {
                return;
            }

            // remove peer from current game
            this.RemovePeerFromCurrentRoom();

            // try to get the game reference from the game cache 
            RoomReference gameReference;
            if (this.TryGetRoomReference(joinRequest.GameId, out gameReference) == false)
            {
                var response = new OperationResponse
                    {
                        OperationCode = (byte)OperationCode.JoinGame,
                        ReturnCode = (short)Operations.ErrorCode.GameIdNotExists,
                        DebugMessage = "Game does not exists"
                    };

                this.SendOperationResponse(response, sendParameters);
                return;
            }

            // save the game reference in the peers state                    
            this.RoomReference = gameReference;

            // finally enqueue the operation into game queue
            gameReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
        }

        protected override void OnDisconnect()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnDisconnect: conId={0},userId={1}", this.ConnectionId,this.UserId);
            }

            if (this.RoomReference == null)
            {
                return;
            }

            var message = new RoomMessage((byte)GameMessageCodes.RemovePeerFromGame, this);
            this.RoomReference.Room.EnqueueMessage(message);
            this.RoomReference.Dispose();
            this.RoomReference = null;
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                if (request.OperationCode != 10)
                {
                    log.DebugFormat("OnOperationRequest: conId={0}, opCode={1}, userId={2}", this.ConnectionId, request.OperationCode, this.UserId);
                }
            }

            switch (request.OperationCode)
            {
                case (byte)OperationCode.Authenticate:
                    this.HandleAuthenticateOperation(request, sendParameters);
                    return;

                case (byte)OperationCode.CreateGame:
                    this.HandleCreateGameOperation(request, sendParameters);
                    return;

                case (byte)OperationCode.JoinGame:
                    this.HandleJoinGameOperation(request, sendParameters);
                    return;

                case (byte)Lite.Operations.OperationCode.Leave:
                    //RemovePeerFromGameState(this.RoomReference);
                    this.HandleLeaveOperation(request, sendParameters);
                    return;

                case (byte)Lite.Operations.OperationCode.Ping:
                    this.HandlePingOperation(request, sendParameters);
                    return;
                case (byte)Lite.Operations.OperationCode.RaiseEvent:
                case (byte)Lite.Operations.OperationCode.GetProperties:
                case (byte)Lite.Operations.OperationCode.SetProperties:
                    this.HandleGameOperation(request, sendParameters);
                    return;
                //quick start
                case (byte)LilyOpCode.JoinCareerGame:
                case (byte)LilyOpCode.QuickStart:
                    this.lobby.EnqueueOperation(this, request, sendParameters);

                    //QuickStartRequest(request, sendParameters);
                    return;
                case (byte)LilyOpCode.TryJoinTable:
                    {
                        string gameId = request.Parameters[(byte)LilyOpKey.GameId] as string;
                        string uid = request.Parameters[(byte)LilyOpKey.UserId] as string;
                        UserData user = base.QueryUserByUserId(uid);
                        Dictionary<byte, object> dic = new Dictionary<byte, object>();

                        //Lite.Caching.RoomReference roomr = this.RoomReference;
                        Room room = null;
                        GameCache.Instance.TryGetRoomWithoutReference(gameId, out room);
                        if (room == null)
                        {
                            dic.Add((byte)LilyOpKey.ErrorCode, DataPersist.ErrorCode.GameIdNotExists);
                        }
                        else
                        {
                            dic = TryJoinGameResult(room, user);
                        }
                        var or = new OperationResponse(request.OperationCode, dic);
                        this.SendOperationResponse(or, sendParameters);
                        return;
                    }
                default:
                    base.OnOperationRequest(request, sendParameters);
                    return;
            }

            string message = string.Format("Unknown operation code {0}", request.OperationCode);
            var response = new OperationResponse { ReturnCode = -1, DebugMessage = message, OperationCode = request.OperationCode };
            this.SendOperationResponse(response, sendParameters);
        }

        public UserData GameQueryUserByUserId(string userId) {
            return base.QueryUserByUserId(userId);
        }

        public void GameOnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            OnOperationRequest(operationRequest,sendParameters);
        }

        protected virtual bool TryCreateRoom(string gameId, out RoomReference roomReference)
        {
            return GameCache.Instance.TryCreateRoom(gameId, out roomReference);
        }

        protected virtual bool TryGetRoomReference(string gameId, out RoomReference roomReference)
        {
            return GameCache.Instance.TryGetRoomReference(gameId, out roomReference);
        }

        protected virtual void HandleAuthenticateOperation(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var request = new AuthenticateRequest(this.Protocol, operationRequest);
            if (this.ValidateOperation(request, sendParameters) == false)
            {
                return;
            }

            if (request.UserId != null)
            {
                this.PeerId = request.UserId;
            }

            var response = new OperationResponse { OperationCode = operationRequest.OperationCode };
            this.SendOperationResponse(response, sendParameters);
        }

        private void OnJoinFailedInternal(Operations.ErrorCode result)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnJoinFailed: {0}", result);
            }

            // if join operation failed -> release the refrence to the room
            if (result != Operations.ErrorCode.Ok)
            {
                this.RoomReference.Dispose();
                this.RoomReference = null;
            }
        }

        #endregion


        #region Join Method
        protected override void HandleJoinGameWithLobby(LiteLobby.Operations.JoinRequest joinOperation, SendParameters sendParameters)
        {

            log.DebugFormat("@@@@@@@@@@@@@@@@@@@@@@@ HandleJoinGameWithLobby, joinOperation.GameId:{0}", joinOperation.GameId);
            // remove the peer from current game if the peer
            // allready joined another game
            this.RemovePeerFromCurrentRoom();

            // get a game reference from the game cache 
            // the game will be created by the cache if it does not exists allready
            string userid = joinOperation.OperationRequest.Parameters[(byte)LilyOpKey.UserId].ToString();
            RoomReference gameReference = GameCache.Instance.GetRoomReference(joinOperation.GameId, joinOperation.LobbyId,userid);

            // save the game reference in peers state                    
            this.RoomReference = gameReference;

            // enqueue the operation request into the games execution queue
            gameReference.Room.EnqueueOperation(this, joinOperation.OperationRequest, sendParameters);
        }

        protected override void HandleJoinLobby(LiteLobby.Operations.JoinRequest joinRequest, SendParameters sendParameters)
        {
            // remove the peer from current game if the peer
            // allready joined another game
            this.RemovePeerFromCurrentRoom();

            // get a lobby reference from the game cache 
            // the lobby will be created by the cache if it does not exists allready
            RoomReference lobbyReference = GameCache.Instance.GetRoomReference(joinRequest.GameId);
            // save the lobby(room) reference in peers state                    
            this.RoomReference = lobbyReference;

            // enqueue the operation request into the games execution queue
            lobbyReference.Room.EnqueueOperation(this, joinRequest.OperationRequest, sendParameters);
        }
        #endregion
    }
}