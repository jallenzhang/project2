namespace LilyServer
{
    using Lite;
    using LiteLobby;
    using LiteLobby.Caching;

    public class LilyGameCache : LiteLobbyGameCache 
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static readonly LilyGameCache Instance = new LilyGameCache();

        /// <summary>
        /// The create room.
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// a <see cref="LiteLobbyGame"/>
        /// </returns>
        protected override Room CreateRoom(string roomId, params object[] args)
        {
            var lobbyName = (string)args[0];
            return new LilyGame(roomId, lobbyName);
        }      
    }

    /// <summary>
    /// The lite lobby room cache.
    /// </summary>
    public class LilyRoomCache : LiteLobbyRoomCache
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly LilyRoomCache Instance = new LilyRoomCache();

        /// <summary>
        /// The create room.
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// a <see cref="LiteLobbyRoom"/>  
        /// </returns>
        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new LilyRoom(roomId);
        }
    }
}
