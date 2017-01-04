// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LilyApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Example application to show how to extend the Lite application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LilyServer
{
    using System.Reflection;

    using LiteLobby;

    //using log4net;

    using Photon.SocketServer;

    using Photon.SocketServer.ServerToServer;
    using PhotonHostRuntimeInterfaces;
    using System.Net;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Lite;
    using DataPersist;
    using Photon.SocketServer.Diagnostics;
    using Lite.Diagnostics;

    /// <summary>
    ///   Example application to show how to extend the Lite application.
    /// </summary>
    public class LilyServer : LiteLobbyApplication
    {
        static object objLock = new object();
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILog" /> instance used to log messages to the log4net framework.
        /// </summary>
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Properties
        public static ActorCollection Actors { get; set; }
        public static string BinaryPathLily { get; set; }
        #endregion

        #region Methods

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new LilyPeer(initRequest.Protocol, initRequest.PhotonPeer);
        }

        protected override void Setup()
        {
            Actors = new ActorCollection();
            AchievementManager.Singleton.LoadFile(this.BinaryPath + "/Resources/Achievement.xml");
            AwardManager.Singleton.LoadFile(this.BinaryPath + "/Resources/Awards.xml");
            GiftManager.Singleton.LoadFile(this.BinaryPath + "/Resources/Gifts.xml");

            BinaryPathLily = this.BinaryPath;
            //XMLResources.InitXMLResources(this.BinaryPath);
            //Helper.WordsFilter.path = this.BinaryPath;
            //游戏计数器test
            //CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Counter), "LilyServer");
            base.Setup();
        }
        #endregion
    }
}