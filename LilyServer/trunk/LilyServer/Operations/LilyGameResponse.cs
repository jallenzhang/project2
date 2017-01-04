// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LilyGameResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LilyGameResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LilyApplication.Operations
{
    #region

    using Photon.SocketServer.Rpc;

    #endregion

    public class LilyGameResponse
    {
        [DataMember(Code = (byte)LilyParameterCodes.Response, IsOptional = false)]
        public string Response { get; set; }
    }
}