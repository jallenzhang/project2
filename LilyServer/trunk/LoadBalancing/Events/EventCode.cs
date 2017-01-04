﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    public enum EventCode
    {
        RemovePeerFromRoom=1,

        GameList = 230,
        GameListUpdate = 229,
        QueueState = 228,
        AppStats = 226
    }
}