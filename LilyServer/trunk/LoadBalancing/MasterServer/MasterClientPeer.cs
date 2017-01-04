// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterClientPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterClientPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer
{
    #region using directives

    using System.Collections.Generic;

    using ExitGames.Logging;

    using Photon.LoadBalancing.MasterServer.Lobby;
    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using DataPersist;
    using Lite;

    #endregion

    public class MasterClientPeer : LilyServer.LilyPeer, ILobbyPeer
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly AppLobby lobby;


        /// <summary>
        ///   The actor number counter is increase whenever a new <see cref = "Actor" /> joins the game.
        /// </summary>
        private int actorNumberCounter;

        #endregion

        #region Constructors and Destructors

        public MasterClientPeer(InitRequest initRequest, AppLobby lobby)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            this.lobby = lobby;

            this.lobby.AddPeer(this);
            this.actorNumberCounter++;
            //Actor actor = new Actor(this);
            //actor.ActorNr = this.actorNumberCounter;
            //LilyServer.LilyServer.Actors.Add(actor);
        }

        public MasterClientPeer() : base(null, null) { 
        }

        #endregion

        

        #region Properties

        public HashSet<string> BlockedUsers { get; set; }

        //public string UserId { get; set; }

        #endregion

        #region Public Methods

        public bool IsBlocked(ILobbyPeer lobbyPeer)
        {
            return this.BlockedUsers != null && this.BlockedUsers.Contains(lobbyPeer.UserId);
        }

        #endregion

        #region Methods

        protected override void OnDisconnect()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Disconnect: pid={0},userId={1}", this.ConnectionId,this.UserId);
            }

            LilyServer.LilyServer.Actors.RemoveActorByPeer(this);
            this.lobby.RemovePeer(this);
            base.OnDisconnect();
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnOperationRequest: pid={0}, op={1},userId={2}", this.ConnectionId, request.OperationCode,this.UserId);
            }
            //switch ((OperationCode)request.OperationCode)
            //{
            //    default:
            //        var response = new OperationResponse(request.OperationCode) { ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = "Unknown operation code" };
            //        this.SendOperationResponse(response, sendParameters);
            //        break;

            //    case OperationCode.Authenticate:
            //        OperationResponse authenticateResponse = this.HandleAuthenticate(request);
            //        this.SendOperationResponse(authenticateResponse, sendParameters);
            //        break;

            //    case OperationCode.CreateGame:
            //    case OperationCode.JoinLobby:
            //    case OperationCode.LeaveLobby:
            //    case OperationCode.JoinRandomGame:
            //    case OperationCode.JoinGame:
            //        this.lobby.EnqueueOperation(this, request, sendParameters);
            //        break;
            //}
            switch ((DataPersist.LilyOpCode)request.OperationCode)
            {
                case LilyOpCode.Authenticate:
                    OperationResponse authenticateResponse = this.HandleAuthenticate(request);
                    this.SendOperationResponse(authenticateResponse, sendParameters);
                    break;

                case LilyOpCode.CreateGame:
                case LilyOpCode.JoinLobby:
                case LilyOpCode.LeaveLobby:
                case LilyOpCode.JoinRandomGame:
                case LilyOpCode.HistoryAction:
                case LilyOpCode.JoinGame:
                    this.lobby.EnqueueOperation(this, request, sendParameters);
                    break;
                case LilyOpCode.GetGameServer:
                    this.lobby.EnqueueOperation(this, request, sendParameters);
                    break;
                case LilyOpCode.GetFriendGameServer:
                    this.lobby.EnqueueOperation(this,request,sendParameters);
                    break;
                case LilyOpCode.Login:

                    this.lobby.ExecutionFiber.Enqueue(() => this.lobby.HandleHistoryAction(this, new OperationRequest((byte)LilyOpCode.HistoryAction, request.Parameters)));
                    //this.lobby.EnqueueOperation(this,new OperationRequest((byte)LilyOpCode.HistoryAction,request.Parameters),sendParameters);
                    base.OnOperationRequest(request,sendParameters);
                    break;
                //case LilyOpCode.InviteFriend:
                //    {                       
                //        this.lobby.EnqueueOperation(this,request,sendParameters);
                //        break;
                //    }
                default:
                    base.OnOperationRequest(request, sendParameters);
                    break;                
            }
        }

        private OperationResponse HandleAuthenticate(OperationRequest operationRequest)
        {
            OperationResponse response;

            var request = new AuthenticateRequest(this.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, log, out response))
            {
                return response;
            }

            this.UserId = request.UserId;

            // publish operation response
            var responseObject = new AuthenticateResponse { QueuePosition = 0 };
            return new OperationResponse(operationRequest.OperationCode, responseObject);
        }

        #endregion

        #region Join Method
        protected override void HandleJoinGameWithLobby(LiteLobby.Operations.JoinRequest joinOperation, SendParameters sendParameters)
        {
            base.HandleJoinGameWithLobby(joinOperation, sendParameters);
        }

        protected override void HandleJoinLobby(LiteLobby.Operations.JoinRequest joinRequest, SendParameters sendParameters)
        {
            base.HandleJoinLobby(joinRequest, sendParameters);
        }
        #endregion
    }
}