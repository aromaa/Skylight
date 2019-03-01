using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Headers
{
    class r63cOutgoing
    {
        public static readonly uint MachineID = 2935;
        public static readonly uint Fuserights = 1862;
        public static readonly uint AvaiblityStatus = 2468;
        public static readonly uint AuthenicationOK = 1442;
        public static readonly uint ModTool = 2545;
        public static readonly uint Logging = 3282;
        public static readonly uint HomeRoom = 3175;
        public static readonly uint FavouriteRooms = 604;
        public static readonly uint UpdateCredits = 3604;
        public static readonly uint MOTD = 1829;
        public static readonly uint NewItemAdded = 2725;
        public static readonly uint UpdateActivityPoints = 606;
        public static readonly uint UpdateActivityPointsSilence = 1911;
        public static readonly uint AchievementUpdate = 305;
        public static readonly uint AddBadges = 154;
        public static readonly uint SendUserInfo = 1823;
        public static readonly uint EnterPrivateRoom = 1329;
        public static readonly uint LoadingRoomInfo = 2029;
        public static readonly uint ApplyRoomEffect = 1328;
        public static readonly uint GiveRoomRights = 1425;
        public static readonly uint IsRoomOwner = 495;
        public static readonly uint RoomRating = 3464;
        public static readonly uint FloorItems = 3521;
        public static readonly uint WallItems = 2335;
        public static readonly uint SetRoomUser = 2422;
        public static readonly uint RoomVIPSettings = 3786;
        public static readonly uint RoomOwner = 3378;
        public static readonly uint RoomData = 2224;
        public static readonly uint Dance = 845;
        public static readonly uint Sleeps = 3852;
        public static readonly uint Handitem = 2623;
        public static readonly uint Effect = 2662;
        public static readonly uint Flood = 1197;
        public static readonly uint Heightmap = 1112;
        public static readonly uint RelativeHeightmap = 207;
        public static readonly uint SendNotifFromMod = 3720;
        public static readonly uint UserStatues = 3153;
        public static readonly uint FlatCats = 377;
        public static readonly uint InventoryItems = 2183;
        public static readonly uint CatalogIndexes = 2018;
        public static readonly uint CatalogPage = 3477;
        public static readonly uint AddFloorItemToRoom = 505;
        public static readonly uint RemoveRoomUser = 2841;
        public static readonly uint BuyInfo = 2843;
        public static readonly uint AddItemToHand = 176;
        public static readonly uint NewItems = 2725;
        public static readonly uint RemoveItemFromHand = 1903;
        public static readonly uint Say = 3821;
        public static readonly uint UpdateFloorItem = 273;
        public static readonly uint AddWallItemToRoom = 1841;
        public static readonly uint UpdateWallItem = 2933;
        public static readonly uint RemoveFloorItem = 85;
        public static readonly uint RemoveWallItem = 762;
        public static readonly uint UserPerks = 2807;
        public static readonly uint NewNavigatorMetaData = 371;
        public static readonly uint NewNavigatorLiftedRooms = 761;
        public static readonly uint NewNavigatorCollapsedCategories = 1263;
        public static readonly uint NewNavigatorSavedSearches = 508;
        public static readonly uint NewNavigatorEventCategories = 1109;
        public static readonly uint NewNavigatorSearchResults = 815;
        public static readonly uint Ping = 3014;
        public static readonly uint RoomCreated = 1621;
        public static readonly uint RoomSettings = 633;
        public static readonly uint UserClub = 2811;
        public static readonly uint RoomUpdateOK = 3297;
        public static readonly uint RoomChatSettings = 2006;
        public static readonly uint TypingStatus = 2854;
        public static readonly uint Pong = 3014;
        public static readonly uint AchievementUnlocked = 1887;
        public static readonly uint HotelViewData = 3234;
        public static readonly uint Games = 2481;
        public static readonly uint AccountGameStatus = 139;
        public static readonly uint LoadGame = 1403;
        public static readonly uint LeaveGameQueue = 2164;
        public static readonly uint GameButtonStatus = 549;
        public static readonly uint GiveRespect = 474;
        public static readonly uint RoomSettingsOK = 948;
        public static readonly uint RollerMovement = 1143;
        public static readonly uint Shout = 909;
        public static readonly uint ValidUsername = 3801;
        public static readonly uint ClubData = 1500;
        public static readonly uint UpdateUser = 32;

        public static readonly uint MessengerInit = 391;
        public static readonly uint MessengerUserSearchResults = 214;
        public static readonly uint MessengerSendFriendRequestError = 915;
        public static readonly uint MessengerReceivePrivateMessage = 2121;
        public static readonly uint MessengerSendPrivateMessageError = 2964;
        public static readonly uint MessengerReceiveFriendRequest = 2981;
        public static readonly uint MessengerUpdateFriends = 1611;
        public static readonly uint MessengerFollowError = 1170;
        public static readonly uint MessengerFollowUser = 1963;
        public static readonly uint MessengerReceivedRoomInvite = 3942;
        public static readonly uint MessengerFriends = 3394;
        public static readonly uint MessengerFriendRequests = 2757;

        public static readonly uint TradeStartError = 2876;
        public static readonly uint TradeStart = 2290;
        public static readonly uint TradeCancel = 2068;
        public static readonly uint TradeUpdate = 2277;
        public static readonly uint TradeAccepted = 1367;
        public static readonly uint TradeRequireConfirm = 1959;
        public static readonly uint TradeWindowClose = 2369;

        public static readonly uint InventoryRefresh = 506;

        public static readonly uint VerifyMobilePhoneWindow = 1941;
        public static readonly uint VerifyMobilePhoneCodeWindow = 340;
        public static readonly uint VerifyMobilePhoneDone = 1904;

        public static readonly uint SendQuests = 664;

        public static readonly uint GuideTool = 224;
        public static readonly uint GuideRequestError = 2089;
        public static readonly uint GuideSessionAttached = 2054;
        public static readonly uint GuideSessionDetached = 733;
        public static readonly uint GuideSessionEnd = 1219;
        public static readonly uint GuideStartSession = 1628;
        public static readonly uint GuideSessionPartnerTyping = 2346;
        public static readonly uint GuideSessionReceiveMessage = 1355;
        public static readonly uint GuideSendToRoom = 421;
        public static readonly uint GuideSendInvite = 1642;

        public static readonly uint BullyReportStart = 2094;
        public static readonly uint BullyReport = 453;
        public static readonly uint NewBullyReport = 150;
        public static readonly uint BullyReportDetached = 192;
        public static readonly uint BullyReportAttached = 3796;
        public static readonly uint BullyVoteResults = 3245;
        public static readonly uint BullyReportClose = 1804;
        public static readonly uint BullyReportResults = 2081;

        //Misc
        public static readonly uint SendTalentTrack = 3614;
        public static readonly uint SendQuiz = 2152;
        public static readonly uint QuizResults = 3379;
        public static readonly uint AchievementList = 509;
        public static readonly uint SendUserProfile = 3872;
        public static readonly uint SendUserRelations = 1589;
        public static readonly uint SendUserActiveBadges = 1123;
        public static readonly uint SendDiscount = 3322;
        public static readonly uint SendCatalogMode = 3424;
        public static readonly uint SendUserTags = 774;
        public static readonly uint SendMySettings = 2921;
        public static readonly uint UpdateRoomUser = 32;
        public static readonly uint UpdateUserLook = 3632;
        public static readonly uint UserAchievementScore = 3710;
        public static readonly uint AchievementRequirements = 2066;
        public static readonly uint SendWardrobe = 2760;
        public static readonly uint NewbieIdentity = 1581;
        public static readonly uint ReceiveWhisper = 2280;
        public static readonly uint UserAction = 179;
        public static readonly uint IgnoreUser = 2394;
    }
}
