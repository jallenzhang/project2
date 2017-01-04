// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LilyGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LilyGameRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LilyApplication.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class LilyGameRequest : Operation
    {
        #region Constructors and Destructors

        public LilyGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        #endregion
    }
}