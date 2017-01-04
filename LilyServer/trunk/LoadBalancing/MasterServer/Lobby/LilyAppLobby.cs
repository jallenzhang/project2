using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Photon.LoadBalancing.Operations;
using Photon.LoadBalancing.MasterServer.GameServer;
using LilyServer;
using LilyServer.Events;
using Photon.LoadBalancing.Events;
using DataPersist;
using ExitGames.Logging;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    public class LilyAppLobby:AppLobby
    {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, IncomingGameServerPeer> roomList = new Dictionary<string, IncomingGameServerPeer>();

        public LilyAppLobby(LoadBalancer<IncomingGameServerPeer> loadBalancer)
            : this(loadBalancer, 0, TimeSpan.FromSeconds(5))
        {
        }

        public LilyAppLobby(LoadBalancer<IncomingGameServerPeer> loadBalancer, int maxPlayersDefault, TimeSpan joinTimeOut):base(loadBalancer,maxPlayersDefault,joinTimeOut)
        {
        }

        //protected virtual OperationResponse HandleGetGameServer(MasterClientPeer peer, OperationRequest operationRequest)
        //{
        //    HandleJoinLobby(peer, operationRequest);
        //    //var response= HandleCreateGame(peer, operationRequest);
        //    IncomingGameServerPeer gameServer=null;

        //    if (roomList.ContainsKey(peer.UserId))
        //    {
        //        gameServer = roomList[peer.UserId];
        //        roomList.Remove(peer.UserId);
        //    }

        //    //OperationResponse response = new OperationResponse(operationRequest.OperationCode);
        //    //Dictionary<byte, object> param = new Dictionary<byte, object>();
        //    if (gameServer == null)
        //    {
        //        //推测上次断线位置及牌桌信息
        //        if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.StepID))
        //        {
        //            int sessionId=peer.ConnectionId;
        //            int stepid = (int)operationRequest.Parameters[(byte)LilyOpKey.StepID];
        //            string gameid = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;
        //            int tableroud = (int)operationRequest.Parameters[(byte)LilyOpKey.TableRound];
        //            string serverId = operationRequest.Parameters[(byte)LilyOpKey.GameServerAddress] as string;
        //            if (!string.IsNullOrEmpty(serverId))
        //            {
        //                IncomingGameServerPeer lastGameServer = null;
        //                this.LoadBalancer.TryGetServer(serverId, out lastGameServer);
        //                if (lastGameServer != null) {
        //                    gameServer = lastGameServer;
        //                    lastGameServer.AskForGameHistoryActions(gameid, tableroud, stepid, sessionId);
        //                }
        //            }
        //        }
        //        if(gameServer==null)
        //        if (!this.LoadBalancer.TryGetServer(out gameServer))
        //        {
        //            OperationResponse errorResponse = new OperationResponse(operationRequest.OperationCode)
        //                {
        //                    ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.ServerFull,
        //                    DebugMessage = "Failed to get server instance."
        //                };
        //            return errorResponse;
        //        }
        //    }

        //    GetGameServerResponse gameServerResponse = new GetGameServerResponse();
        //    gameServerResponse.GameId = gameServer.ServerId.ToString();
        //    //gameServerResponse.Address = gameServer.NetworkProtocol == NetworkProtocolType.Tcp ? gameServer.TcpAddress : gameServer.UdpAddress;
        //    gameServerResponse.Address = peer.NetworkProtocol == NetworkProtocolType.Tcp ? gameServer.TcpAddress : gameServer.UdpAddress;
        //    gameServerResponse.NodeId = gameServer.NodeId;
        //    gameServerResponse.ErrorCode = (int)DataPersist.ErrorCode.Sucess;
        //    peer.GameServerId = gameServer.ServerId.ToString();
        //    // return response;
        //    var response = new OperationResponse(operationRequest.OperationCode, gameServerResponse);
        //    return response;
        //}

        protected override object GetCreateGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new GetGameServerResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer), NodeId = gameState.NodeId, ErrorCode = (int)DataPersist.ErrorCode.Sucess };
        }

        protected override void ExecuteOperation(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse response;
            switch ((DataPersist.LilyOpCode)operationRequest.OperationCode)
            {
                //case DataPersist.LilyOpCode.GetGameServer:
                //    response = this.HandleGetGameServer(peer, operationRequest);
                //    //response = this.HandleCreateGame(peer, operationRequest);
                //    break;
                case DataPersist.LilyOpCode.GetFriendGameServer:
                    response = this.GetFriendGameServer(peer, operationRequest);
                    break;
                default:
                    base.ExecuteOperation(peer, operationRequest, sendParameters);
                    return;
            }

            if (response != null)
            {
                peer.SendOperationResponse(response, sendParameters);
            }
        }


        protected OperationResponse GetFriendGameServer(MasterClientPeer peer, OperationRequest operationRequest)
        {

            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            //HandleJoinLobby(peer, operationRequest);
            //var response = HandleCreateGame(peer, operationRequest);
            //peer.GameServerId = response.Parameters[(byte)LilyOpKey.GameId] as string;

            IncomingGameServerPeer serverpeer = null;
            //search from registered  offline roomlist
            if (roomList.ContainsKey(userId))
            {
                serverpeer = roomList[userId];
            }
            // then search from lilyserver.actors
            if (serverpeer == null)
            {
                bool needRegister = false;
                Lite.Actor actor = LilyServer.LilyServer.Actors.FirstOrDefault(r => (r.Peer as LilyPeer).UserId == userId);
                PeerBase firendpeer = null;
                if (actor != null)
                {
                    firendpeer = actor.Peer;
                }
                else
                {
                    //friend offline , register room into roomlist;
                    needRegister = true;
                }
                if (firendpeer == null)
                {
                    firendpeer = peer;
                }
                string gameid = (firendpeer as MasterClientPeer).GameServerId;
                this.LoadBalancer.TryGetServer(gameid, out serverpeer);
                if (serverpeer == null)
                    this.LoadBalancer.TryGetServer(out serverpeer);

                if (needRegister && !roomList.ContainsKey(userId))
                    roomList.Add(userId, serverpeer);
            }

            OperationResponse response = new OperationResponse((byte)LilyOpCode.GetFriendGameServer);
            response.Parameters = new Dictionary<byte, object>();
            response.Parameters.Add((byte)ParameterCode.Address, serverpeer.NetworkProtocol == NetworkProtocolType.Tcp ? serverpeer.TcpAddress : serverpeer.UdpAddress);
            response.Parameters.Add((byte)ParameterCode.GameId, serverpeer.ServerId.ToString());
            response.Parameters.Add((byte)ParameterCode.NodeId, serverpeer.NodeId);
            response.Parameters.Add((byte)LilyOpKey.ErrorCode, DataPersist.ErrorCode.Sucess);
            peer.GameServerId = serverpeer.ServerId.ToString();
            return response;
        }

        protected override OperationResponse HandleJoinLobby(MasterClientPeer peer, OperationRequest operationRequest)
        {
            if (this.peers.Contains(peer))
            {
                LilyPeer peerToRemove = this.peers.FirstOrDefault(rs=>(rs as LilyPeer).UserId==peer.UserId) as LilyPeer;
                //SameAccountLoginEvent e = new SameAccountLoginEvent();
                //EventData eventData = new EventData(e.Code, e);
                //peerToRemove.SendEvent(eventData,new SendParameters());

                //GameState gameState;
                //this.GameList.TryGetGame("", out gameState);
                //Dictionary<byte, object> evParams = new Dictionary<byte, object>();
                //evParams[(byte)LilyEventKey.UserId] = peerToRemove.UserId;
                //EventData ed = new EventData((byte)EventCode.RemovePeerFromRoom, evParams);
                //gameState.GameServer.SendEvent(ed, new SendParameters());
            }

            // check if peer already joined
            if (this.peers.Add(peer) == false)
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (int)Photon.LoadBalancing.Operations.ErrorCode.OperationDenied, DebugMessage = "Peer already joined the lobby" };
            }

            // publish game list to peer after the response has been sent
            //this.ExecutionFiber.Enqueue(() => this.PublishGameList(peer));

            return new OperationResponse(operationRequest.OperationCode);
        }
    }
}
