using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using System.Collections;
using DataPersist;
using Photon.SocketServer.Rpc;

namespace LilyServer
{
    public class SitEvent : LiteEventBase
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="JoinEvent"/> class.
        /// </summary>
        /// <param name="actorNr">
        /// The sender actor nr.
        /// </param>
        /// <param name="actors">
        /// The actors in the game.
        /// </param>
        public SitEvent(int actorNr)
            : base(actorNr)
        {
            this.Code = (byte)LilyEventCode.Sit;
            //this.Actors = actors;
        }

        ///// <summary>
        ///// Gets or sets the actor properties of the joined actor.
        ///// </summary>
        //[DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        //public Hashtable ActorProperties { get; set; }

        ///// <summary>
        ///// Gets or sets the actors in the game.
        ///// </summary>
        //[DataMember(Code = (byte)ParameterKey.Actors)]
        //public int[] Actors { get; set; }
    }
}
