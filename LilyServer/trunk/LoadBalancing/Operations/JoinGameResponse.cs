﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinGameResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinGameResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    #region

    using Photon.SocketServer.Rpc;

    #endregion

    public class JoinGameResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterCode.Address, IsOptional = false)]
        public string Address { get; set; }

        [DataMember(Code = (byte)ParameterCode.NodeId)]
        public byte NodeId { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameId)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)DataPersist.LilyOpKey.ErrorCode)]
        public int ErrorCode { get; set; }

        #endregion
    }
}