using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
    public enum LilyOpCode : byte
    {
        // ask for master server to assign a game server
        GetFriendGameServer = 99,
        GetGameServer = 100,

        //LilyOpCode
        Register = 101,
        Login = 102,
        Logout = 103,
        SearchUser,
        RequestFriend,
        AcceptFriend,
		InviteFriend,
        AccessFriend,
        AddFriend,
        DeleteFriend,
        FriendList,
        Feedback,
        SyncDate,
        QuickStart,
        JoinTable,
        LeaveTable,
        BroadcastMessage,
		BroadcastMessageInTable,
        Sit,
        SaveUserInfo,        
        FindPassword,
        Fold,
        Call,
        Raise,
        Check,
        HistoryAction,
		RequestRobot,
        BuyItem,
        SendChip,
		SendGift,
        StandUp,
        GetClientVersion,
        Suspend,
		Achieve,
        QueryUserById,
        Setting,
        GuestUpgrade,
        SystemNotice,
        TryJoinTable,
        PublishInGame,
        UpgradeUrl,
        SyncGameData,
        GetAward,
        GetOnlineAwards,
        GetUserProps,
        BuyitemByChips,
        //LiteOpOcde
        ExchangeKeysForEncryption = 250,
        Join = 255,
        Leave = 254,
        RaiseEvent = 253,
        SetProperties = 252,
        GetProperties = 251,
        //LoadBalancing
        Authenticate = 230,
        JoinLobby = 229,
        LeaveLobby = 228,
        CreateGame = 227,
        JoinGame = 226,
        JoinRandomGame = 225,
        CancelJoinRandomGame = 224,
        Latency = 223 // should not be here
    }
	
	public enum LilyOpKey:byte {
		//LilyOpCode
		ErrorCode=1,
		UserData,
		UserId,
		FriendList,
        FriendData,
        NickName,
		Feedback,
		RoomData,
        SyncDate,
		UserList,
		Message,
        TableInfo,
		NoSeat,
        Money,
        StepID,
        TableRound,
        HistoryAction,
        GameSettingBigBlind, 
        GameSettingThinkingTime,
        GameSettingMaxPlayers,
        GameSettingFriendsOnly,
        PlayingNoSeats,
        AllInNoSeats,
        ItemType,
        ItemId,
        Messages,
        Chip,
		Destination,
        TakenAmnt,
		GiftId,
        ClientVersion,
		AchievementNumber,
        Notification,
        SystemNotification,
        FriendNotification,
        IAPString,
        IAPMoney,
        SystemNotice,
        UserStatus,
        UpgradeUrl,
		InviterId,
        Mail,
        PassWord,
        DeviceType,
        DeviceToken,
        UserType,
        GameTypeState,
        PlayerCardId,
		GameCardIds,        
        GameServerAddress,
        Avator,
        BackGroundType,
        LivingRoomType,
        PayWay,
        DayOfAward,
        UserProps,

        //ParamterCode
        Address = 230, 
        PeerCount = 229, 
        GameCount = 228, 
        MasterPeerCount = 227, 
        ApplicationId = 224, 
        Position = 223, 
        GameList = 222, 
        Secret = 221, 
        AppVersion = 220,
        NodeId = 219,
		
		//LiteOpKey
		GameId = 255,
		ActorNr = 254,
		TargetActorNr = 253,
		ActorList = 252,
		Properties = 251,
		Broadcast = 250,
		ActorProperties = 249,
		GameProperties = 248,
		Cache = 247,
		ReceiverGroup = 246,
		Data = 245,
		Code = 244,
		LobbyId=242
	}
	
	public enum LilyEventCode:byte
	{
		//LilyEventCode
		RequestFriend=1,
		AddFriend,
        DeleteFriend,
		InviteFriend,
		AcceptFriend,
		BroadcastMessage,
		BroadcastMessageInTable,
        Sit,
        PlayerTurnEnded,
        BetTurnStarted,
        BetTurnEnded,
        GameStarted,
        PlayerHoleCardsChanged,
        GameEnded,
        PlayerWonPot,
        PlayerMoneyChanged,
        TableClosed,
        PlayerTurnBegan,
        PlayerJoined,
        PlayerLeaved,
        SameAccountLogin,
        SendChip,
        ExperienceAdded,
        SendGift,
        Achievement,
        RoomTypeChanged,
        PlayerWonPotImprove,
        PlayersShowCards,
		
		//LiteEventCode
		Join = 255,
		Leave = 254,
		PropertiesChanged = 253
	}

    public enum LilyParameterKey : byte
    {
        UserDatas=1,
        UserData
    }
	
	public enum LilyEventKey:byte
	{
		//LilyEventKey
		UserId=1,
		UserData,
		Message,
        Sit,
        Bystander,
        PlayerInfo,
        Amounts,
        TypeRound,
        NoSeatDealer,
        NoSeatSmallBlind,
        NoSeatBigBlind,
        TotalPortAmnt,
        Action,
        AmountPlayed,
        NoSeat,
        MoneyPotId,
        AmountWon,
        MoneySafeAmnt,
        LastNoSeat,
        GameCardIds,
        IsPlaying,
        MoneyBetAmnt,
        TotalRounds,
        StepID,
        NickName,
        Chip,
        MessageContent,
        Level,
        LevelExp,
        SendNoSeat,
        ReceiverNoSeat,
        GiftId,
        PlayingNoSeats,
        AchievementNumber,
        RoomType,
        RoomID,
        GameServerAddress,
        TaxAmnt,

        WinnerSeats,
        AttachedPlayerSeats,
        IsKick,
        HigherBet,
		
		//LiteEventKey
		ActorNr = 254,
		TargetActorNr = 253,
		ActorList = 252,
		Properties = 251,
		ActorProperties = 249,
		GameProperties = 248,
		Data = 245,
		CustomContent = 245
	}
	
    public enum ErrorCode
    {
        Sucess = 201,

        SystemError = 500,

        NoResult = 505,
         
        // user
        AuthenticationFail = 10101,
        UserExist = 10102,
        UserNotExist = 10103,
        NoAwards=10104,

        //friend
        FriendExist = 20101,
		FriendNotExist=20102,
        FriendOffline=20103,
		
		//Room
		RoomFull=30101,
		TableFull=30102,
        TableNotExist=30103,
        OnlyFriendsCanJoin=30104,
        GameEnded=30105,
        ChipsNotEnough = 30106,

        //Item
        ItemExist = 40101,

        //payment
        VerifyFail=50101,

        //BuyItem(chips)
        BuyItemNotFound=50201,
        BuyItemChipNotEnough=50202,
        BuyItemUserNotExist=50203,


        OperationDenied = -3, 
        OperationInvalid = -2, 
        InternalServerError = -1, 

        Ok = 0, 

        InvalidAuthentication = 0x7FFF, // codes start at short.MaxValue 
        GameIdAlreadyExists = 0x7FFF - 1,
        GameFull = 0x7FFF - 2,
        GameClosed = 0x7FFF - 3,
        AlreadyMatched = 0x7FFF - 4,
        ServerFull = 0x7FFF - 5,
        UserBlocked = 0x7FFF - 6,
        NoMatchFound = 0x7FFF - 7,
        RedirectRepeat = 0x7FFF - 8,
        GameIdNotExists = 0x7FFF - 9
    }

    public enum UserType:byte
    {
        Normal = 1,
        GameCenter = 2,
        Facebook = 3,
        QQ = 4,
        SinaWeibo = 5,
        RenRen = 6,
        Guest = 7
    }
	
	public enum RoomType
	{
		Common=1,
		Egypt=2,
		Hawaii=4,
		Japan=8,
		West=16,
		China=32
	}

    public enum ChipType
    {
        Chip1=1,
        Chip2,
        Chip3,
        Chip4,
        Chip5
    }

    public enum UserStatus:byte
    {
        Offline = 1,
        Playing = 2,
        Idle = 3,
        Suspend=4
    }
	
	public enum PetType:byte
	{
		FatCat,
		SharPei,
		Lizard,
		Parrot
	}
    /// <summary>
    /// 道具类型
    /// </summary>
    public enum ItemType : byte
    {
        Room=1,
        Chip,
        Avator,
        Jade,
        Lineage
    }

    public enum VIPItem
    {
        Days30 = 1,
        Days90,
        Days180,
        Days365,
        Forever
    }

    public enum MessageType:byte
    {
        RequestFriend = 1,
        SendChip,

        // User use cash to buy 
        BuyChips = 4
    }

    public enum ExpType { 
        Fold=1,
        Win=4,
        Normal=2
    }
	
	public enum GiftType
	{
        None=0,
		Gift1,
		Gift2,
		Gift3,
		Gift4,
		Gift5
	}

    public enum DeviceType { 
        UNKNOW=0,
        IOS,
        ANDROID
    }

    public enum NotificationType { 
        System,
        ActionNeeded,
        AddFriend,
        SendChips,
        InviteFriendGame,
        InviteFriendRoom
    }

    public enum BankActionType
    {
        RobotTaken = 1,
        Register,
        BuyChips,
        LevelUp,
        Achievement,
        Award,

        BuyGift=101,
        CreatGame,
        GameTax,
        Broadcast,
        CreatGameSystem,
        GameTaxSystem

    }

    public enum GameType { 
        User,
        System,
        Training,
        Career
    }


    public enum CacheKeys {
        SystemNotice=0,
        BroadcastMessage,
        RegisterAwards,
        UpgradeAwards,
        CreateGame,
        GameTax,
        GameWaitingTime,
        SendChipsFee,
        RegisterAwardsGuest,
        ClientVersion,
        UpgradeUrl
    }

    // Global States of the Game
    public enum TypeState
    {
        Init,
        WaitForPlayers,
        WaitForBlinds,
        Playing,
        Showdown,
        DecideWinners,
        DistributeMoney,
        End
    }

    public enum PayWay
    { 
        UnKown = 0,
        Alipay,
        IAP
    }

    public enum AwardType:byte
    {
        None,
        Guest,
        Normal,
        Pay
    }
}
