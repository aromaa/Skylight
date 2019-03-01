using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Headers
{
    class r63cIncoming
    {
        public static readonly uint VersionCheck = 4000;
        public static readonly uint SwfVariables = 1600;
        public static readonly uint MachineId = 1471;
        public static readonly uint SSOLogin = 1778;
        public static readonly uint HabboData = 186;
        public static readonly uint GetCredits = 3697;
        public static readonly uint Event = 2386;
        public static readonly uint RequestRoom = 407;
        public static readonly uint RoomEntry = 2125;
        public static readonly uint InitMessenger = 2349;
        public static readonly uint FlatCats = 2506;
        public static readonly uint RequestInventoryItems = 352;
        public static readonly uint GetCatalogIndex = 2511;
        public static readonly uint GetCatalogPage = 39;
        public static readonly uint PurchaseItem = 2830;
        public static readonly uint PlaceObject = 579;
        public static readonly uint Move = 1737;
        public static readonly uint Talk = 670;
        public static readonly uint Ping = 1789;
        public static readonly uint MoveFloorItem = 1781;
        public static readonly uint MoveWallItem = 609;
        public static readonly uint UseFloorFurniture = 3846;
        public static readonly uint PickupItem = 636;
        public static readonly uint NewNavigatorRooms = 2722;
        public static readonly uint RequestRoomData = 1164;
        public static readonly uint RoomEntry2 = 2768;
        public static readonly uint CreateRoom = 3077;
        public static readonly uint GetRoomSettings = 1014;
        public static readonly uint RequestClub = 12;
        public static readonly uint SaveRoomSettings = 2074;
        public static readonly uint Sign = 2966;
        public static readonly uint HotelViewData = 3544;
        public static readonly uint GetGames = 2993;
        public static readonly uint GetAccountGameStatus = 2489;
        public static readonly uint JoinGame = 951;
        public static readonly uint LeaveGame = 3497;
        public static readonly uint Shout = 2101;
        public static readonly uint Dance = 645;
        public static readonly uint SetHomeRoom = 2501;
        public static readonly uint CheckUsername = 8;
        public static readonly uint ConfirmUsername = 1067;
        public static readonly uint Action = 3639;
        public static readonly uint Sit = 1565;
        public static readonly uint ClubData = 715;
        public static readonly uint Motto = 3515;

        public static readonly uint MessengerInit = 2151;
        public static readonly uint MessengerGetFriendRequests = 2485;
        public static readonly uint MessengerSearchUser = 3375;
        public static readonly uint MessengerSendFriendRequest = 3775;
        public static readonly uint MessengerSendPrivateMessage = 1981;
        public static readonly uint MessengerRemoveFriends = 698;
        public static readonly uint MessengerAcceptFriendRequest = 45;
        public static readonly uint MessengerDeclineFriendRequest = 835;
        public static readonly uint MessengerFollowFriend = 2280;
        public static readonly uint MessengerSendRoomInvite = 2694;
        public static readonly uint MessengerSetFriendRelation = 2112;
        public static readonly uint MessengerRequestUpdate = 2664;

        public static readonly uint TradeStart = 3313;
        public static readonly uint TradeOfferItem = 114;
        public static readonly uint TradeOfferMultipleItems = 2996;
        public static readonly uint TradeRemoveItem = 1033;
        public static readonly uint TradeAccept = 3374;
        public static readonly uint TradeModify = 1153;
        public static readonly uint TradeConfirm = 2399;
        public static readonly uint TradeClose = 2967;
        public static readonly uint TradeCloseConfirm = 2264;

        public static readonly uint GetPollData = 2580;

        public static readonly uint GetQuests = 2305;

        public static readonly uint VerifyMobilePhoneCode = 3113;

        public static readonly uint GetGuideTool = 3716;
        public static readonly uint SendGuideRequest = 491;
        public static readonly uint CancelGuideRequest = 3530;
        public static readonly uint HandleGuideRequest = 2975;
        public static readonly uint CloseGuideRequest = 1100;
        public static readonly uint GuideRecommendHelper = 116;
        public static readonly uint GuideTypingStatus = 3333;
        public static readonly uint GuideSesisonSendMessage = 3392;
        public static readonly uint GuideVisitUser = 3325;
        public static readonly uint GuideInviteUser = 918;

        public static readonly uint ReportUserBullyingStart = 2973;
        public static readonly uint ReportBully = 1803;
        public static readonly uint HandleBullyReport = 3668;
        public static readonly uint SkipBullyReport = 1006;
        public static readonly uint BullyReportVote = 1913;

        //Miscs
        public static readonly uint RequestTalentTrack = 1284;
        public static readonly uint CompletedQuiz = 2652;
        public static readonly uint AnsweredQuiz = 245;
        public static readonly uint GetAchievements = 2931;
        public static readonly uint GetUserProfile = 3591;
        public static readonly uint GetUserRelations = 866;
        public static readonly uint GerUserWearingBadges = 2226;
        public static readonly uint SetActiveBadges = 2752;
        public static readonly uint RequestDiscount = 1294;
        public static readonly uint RequestCatalogMode = 2267;
        public static readonly uint LookAtPoint = 3744;
        public static readonly uint GetUserTags = 1722;
        public static readonly uint GetMySettings = 3906;
        public static readonly uint SetPreferOldChat = 2006;
        public static readonly uint SetDisableRoomUnits = 1379;
        public static readonly uint SetDisableCameraFollow = 526;
        public static readonly uint SaveUserVolume = 3820;
        public static readonly uint SaveLook = 2560;
        public static readonly uint RequestAchievementConfiguration = 751;
        public static readonly uint SaveLookToWardrobe = 55;
        public static readonly uint GetWardrobe = 765;
        public static readonly uint Whisper = 878;
        public static readonly uint GiveRespect = 1955;
    }
}
