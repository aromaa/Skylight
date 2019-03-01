﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Headers
{
    public class r63aIncoming
    {
        public static readonly uint InitCryptoMessage = 206;
        public static readonly uint GetSessionParameters = 1817;
        public static readonly uint SSOTicket = 415;
        public static readonly uint EventHappend = 482;
        public static readonly uint GetUserData = 7;
        public static readonly uint GetCredits = 8;
        public static readonly uint GetUserClubMembership = 26;
        public static readonly uint GetSoundSettings = 228;
        public static readonly uint InitMessenger = 12;
        public static readonly uint MessengerSearch = 41;
        public static readonly uint RequestFriend = 39;
        public static readonly uint AcceptFriendRequest = 37;
        public static readonly uint SendChatMessage = 33;
        public static readonly uint MessengerRemoveFriends = 40;
        public static readonly uint DeclineFriendRequest = 38;
        public static readonly uint FriendListUpdate = 15;
        public static readonly uint MessengerSendRoomInvite = 34;
        public static readonly uint GetOfficalRooms = 380;
        public static readonly uint GetMyRooms = 434;
        public static readonly uint CanCreateRoom = 387;
        public static readonly uint CreateRoom = 29;
        public static readonly uint GetFlatCats = 151;
        public static readonly uint GetFriendRequests = 233;
        public static readonly uint GetBadgePointLimits = 3032;
        public static readonly uint OpenFlatConnection = 391;
        public static readonly uint GetFurniAlias = 215;
        public static readonly uint GetRoomEntryData = 390;
        public static readonly uint Say = 52;
        public static readonly uint Shout = 55;
        public static readonly uint UserMove = 75;
        public static readonly uint CatalogIndex = 101;
        public static readonly uint CatalogPage = 102;
        public static readonly uint PurchaseItem = 100;
        public static readonly uint RequestInventory = 404;
        public static readonly uint PlaceObject = 90;
        public static readonly uint GuestRoomInfo = 385;
        public static readonly uint UseFloorFurniture = 392;
        public static readonly uint UseWallFurniture = 393;
        public static readonly uint MoveFloorItem = 73;
        public static readonly uint MoveWallItem = 91;
        public static readonly uint PickupRoomItem = 67;
        public static readonly uint GetRoomSettings = 400;
        public static readonly uint SaveRoomSettings = 401;
        public static readonly uint SaveRoomThumbnail = 386;
        public static readonly uint PopularRoomSearch = 430;
        public static readonly uint LetUserIn = 98;
        public static readonly uint GoToFlat = 59;
        public static readonly uint FollowFriend = 262;
        public static readonly uint Quit = 53;
        public static readonly uint KickUser = 95;
        public static readonly uint BanUser = 320;
        public static readonly uint Wave = 94;
        public static readonly uint Dance = 93;
        public static readonly uint GetClubOffers = 3031;
        public static readonly uint UpdateFigure = 44;
        public static readonly uint UpdateMotto = 484;
        public static readonly uint GetWarbode = 375;
        public static readonly uint SaveWarbode = 376;
        public static readonly uint GetBadges = 157;
        public static readonly uint SetActiveBadges = 158;
        public static readonly uint GetQuests = 3101;
        public static readonly uint LatencyTest = 315;
        public static readonly uint LatencyTestReport = 316;
        public static readonly uint GetMarketplaceConfiguration = 3011;
        public static readonly uint GetMarketplaceCanMakeOffer = 3012;
        public static readonly uint BuyMarketplaceTokens = 3013;
        public static readonly uint GetMarketplaceItemStats = 3020;
        public static readonly uint MakeOffer = 3010;
        public static readonly uint StartTyping = 317;
        public static readonly uint StopTyping = 318;
        public static readonly uint GetOffers = 3018;
        public static readonly uint BuyOffer = 3014;
        public static readonly uint GetOwnOffers = 3019;
        public static readonly uint RedeemCredits = 3016;
        public static readonly uint CancelOffer = 3015;
        public static readonly uint RedeemFurni = 183;
        public static readonly uint LookTo = 79;
        public static readonly uint ModeratorGetUserInfo = 454;
        public static readonly uint ModeratorGetRoomInfo = 459;
        public static readonly uint ModeratorBanUser = 464;
        public static readonly uint ModeratorCaution = 461;
        public static readonly uint GetFAQs = 416;
        public static readonly uint GetAllFAQs = 417;
        public static readonly uint GetFAQCategorys = 420;
        public static readonly uint GetFAQ = 418;
        public static readonly uint CallForHelp = 453;
        public static readonly uint ModeratorPickIssue = 450;
        public static readonly uint ModeratorCloseIssue = 452;
        public static readonly uint CallGuideBot = 440;
        public static readonly uint DeletePendingCall = 238;
        public static readonly uint ModeratorReleaseIssue = 451;
        public static readonly uint ModeratorSendMessage = 462;
        public static readonly uint ModeratorKickUser = 463;
        public static readonly uint ModeratorGetRoomChatlog = 456;
        public static readonly uint ModeratorGetRoomVisits = 458;
        public static readonly uint ModeratorGetUserChatlog = 455;
        public static readonly uint ModeratorRoomAction = 460;
        public static readonly uint ModeratorAction = 200;
        public static readonly uint SetHomeRoom = 384;
        public static readonly uint CanCreateEvent = 345;
        public static readonly uint CreateEvent = 346;
        public static readonly uint EditEvent = 348;
        public static readonly uint EndEvent = 347;
        public static readonly uint SearchEvents = 439;
        public static readonly uint GetRoomsWithHigestScore = 431;
        public static readonly uint GetUsedTagsOnRooms = 382;
        public static readonly uint RoomTagSearch = 438;
        public static readonly uint RoomSearch = 437;
        public static readonly uint MyRoomHistory = 436;
        public static readonly uint WhereMyFriensAre = 433;
        public static readonly uint RoomsOwnedByFriends = 432;
        public static readonly uint AddFavouriteRoom = 19;
        public static readonly uint RemoveFavouriteRoom = 20;
        public static readonly uint GetMyFavouriteRooms = 435;
        public static readonly uint GetAchievements = 370;
        public static readonly uint GetUserSelectedBadges = 159;
        public static readonly uint GetUserTags = 263;
        public static readonly uint SetSoundSettings = 229;
        public static readonly uint DeleteRoom = 23;
        public static readonly uint GiveRoomRights = 96;
        public static readonly uint RemoveRoomRights = 97;
        public static readonly uint RemoveAllRoomRights = 155;
        public static readonly uint GetSellablePetBreeds = 3007;
        public static readonly uint ValidatePetName = 42;
        public static readonly uint IsOfferGiftable = 3030;
        public static readonly uint GetGiftWrappingConfiguration = 473;
        public static readonly uint BuyAsGift = 472;
        public static readonly uint ApplyRoomEffect = 66;
        public static readonly uint PlacePostIt = 3254;
        public static readonly uint GetPostItData = 83;
        public static readonly uint SavePostItData = 84;
        public static readonly uint DeletePostIt = 85;
        public static readonly uint GetRoomDimmerPresets = 341;
        public static readonly uint GetRoomDimmerChangeState = 343;
        public static readonly uint RoomDimmerSavePreset = 342;
        public static readonly uint GetPetInventory = 3000;
        public static readonly uint PlacePet = 3002;
        public static readonly uint GetPetInfo = 3001;
        public static readonly uint PickupPet = 3003;
        public static readonly uint GetPetCommands = 3004;
        public static readonly uint UseDice = 76;
        public static readonly uint OffDice = 77;
        public static readonly uint UseHabbowheel = 247;
        public static readonly uint UseLoveshuffler = 314;
        public static readonly uint UpdateWiredTrigger = 3050;
        public static readonly uint UpdateWiredAction = 3051;
        public static readonly uint UpdateWiredCondition = 3052;
        public static readonly uint Whisper = 56;
        public static readonly uint StartTrade = 71;
        public static readonly uint CancelTrade = 70;
        public static readonly uint OfferItem = 72;
        public static readonly uint AcceptTrade = 69;
        public static readonly uint ModifyTrade = 68;
        public static readonly uint ConfirmAcceptTrade = 402;
        public static readonly uint RemoveItem = 405;
        public static readonly uint DeclineAcceptTrade = 403;
        public static readonly uint SecretKey = 2002;
        public static readonly uint Variables = 1170;
        public static readonly uint MachineId = 813;
        public static readonly uint OpenGift = 78;
        public static readonly uint RedeemVoucher = 129;
        public static readonly uint GiveRespect = 371;
        public static readonly uint Pong = 196;
        public static readonly uint RespectPet = 3005;
        public static readonly uint CheckUsername = 471;
        public static readonly uint ChangeUsername = 470;
        public static readonly uint UseOnWayGate = 232;
        public static readonly uint RateRoom = 261;
        public static readonly uint IgnoreUser = 319;
        public static readonly uint UnignoreUser = 322;
        public static readonly uint Login = 756;
        public static readonly uint SetObjectData = 74;
        public static readonly uint SetFootballGateData = 480;
        public static readonly uint ToggleStaffPick = 483;
        public static readonly uint ToggleFriendStream = 501;
        public static readonly uint GetFriendStream = 500;
        public static readonly uint GetJukeboxPlaylist = 258;
        public static readonly uint GetUserDiscs = 259;
        public static readonly uint GetSongInfo = 221;
        public static readonly uint AddDiscToJukebox = 255;
        public static readonly uint RemoveDiscToJukebox = 256;
        public static readonly uint GetNowPlaying = 249;
        public static readonly uint GetIgnoredUsers = 321;
        public static readonly uint GetPublicRoom = 388;
        public static readonly uint OpenConnection = 2;
    }
}
