using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Lite.Events;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace LilyServer.Operations
{
      /// <summary>
    /// This class implements the Join operation.
    /// </summary>
    public class SitRequest : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public SitRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {

        }

        ///// <summary>
        ///// Gets or sets custom actor properties.
        ///// </summary>
        //[DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        //public Hashtable ActorProperties { get; set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether the actor properties
        ///// should be included in the <see cref="JoinEvent"/> event which 
        ///// will be sent to all clients currently in the room.
        ///// </summary>
        //[DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        //public bool BroadcastActorProperties { get; set; }

        ///// <summary>
        ///// Gets or sets the name of the game (room).
        ///// </summary>
        //[DataMember(Code = (byte)ParameterKey.GameId)]
        //public string GameId { get; set; }

        ///// <summary>
        ///// Gets or sets custom game properties.
        ///// </summary>
        ///// <remarks>
        ///// Game properties will only be applied for the game creator.
        ///// </remarks>
        //[DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        //public Hashtable GameProperties { get; set; }
    }
}



