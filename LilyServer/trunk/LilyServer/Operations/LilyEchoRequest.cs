// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LilyEchoRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LilyEchoRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LilyApplication.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class LilyEchoRequest : Operation
    {
        #region Constructors and Destructors

        public LilyEchoRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)LilyParameterCodes.Text, IsOptional = false)]
        public string Text { get; set; }

        #endregion
    }
}