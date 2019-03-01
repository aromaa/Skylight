using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Headers
{
    public class r63bIncoming
    {
        public static readonly uint VersionCheck = 4000;
        public static readonly uint InitCryptoMessage = 2996;
        public static readonly uint SSOTicket = 442;
        public static readonly uint EventHappend = 3870;
        public static readonly uint GetUserData = 3183;
        public static readonly uint GetCredits = 1359;
        public static readonly uint GetUserClubMembership = 2757;
        public static readonly uint GetSoundSettings = 3823;
        public static readonly uint InitMessenger = 3789;
        public static readonly uint GetFlatCats = 2570;
        public static readonly uint GetFriendRequests = 1671;
        public static readonly uint GetBadgePointLimits = 2384;
        public static readonly uint OpenFlatConnection = 1650;
        public static readonly uint GetRoomEntryData = 2373;
        public static readonly uint UserMove = 703;
        public static readonly uint GuestRoomInfo = 858;
        public static readonly uint StartTyping = 1123;
        public static readonly uint GetUserSelectedBadges = 2756;
        public static readonly uint GetUserTags = 3030;
        public static readonly uint SecretKey = 840;
        public static readonly uint Variables = 3561;
        public static readonly uint MachineId = 3570;
        public static readonly uint GetPrizes = 2718;
        public static readonly uint GetAchievementCompetion = 2888;
        public static readonly uint GetAchievementCompetionHOF = 400;
        public static readonly uint GetCommunityGoal = 648;
        public static readonly uint GetGames = 2498;
        public static readonly uint GetGameAchievements = 823;
        public static readonly uint GetGame = 39;
        public static readonly uint GetGame2 = 3509;
        public static readonly uint GetWeeklyLeaderboard = 117;
        public static readonly uint GetGame3 = 3322;
        public static readonly uint GetAccountGameStatus = 3039;
        public static readonly uint GetGame4 = 806;
        public static readonly uint GetGame5 = 793;
        public static readonly uint GetIgnoredUsers = 1114;
        public static readonly uint StopTyping = 3916;
        public static readonly uint Speak = 1548;
        public static readonly uint Shout = 1953;
        public static readonly uint MyRooms = 3350;
        public static readonly uint GetPublicRooms = 1930;
        public static readonly uint GetRooms = 2958;
        public static readonly uint HigestRatedRooms = 2390;
        public static readonly uint RecentRooms = 557;
        public static readonly uint CanCreateRoom = 2115;
        public static readonly uint CreateRoom = 2431;
        public static readonly uint AddUserToRoom = 2346;
        public static readonly uint AddUserToRoom2 = 1188;
        public static readonly uint MoveOrRotate = 2792;
        public static readonly uint OpenInventory = 964;
        public static readonly uint AddFloorItem = 3078;
        public static readonly uint PickupItem = 2805;
        public static readonly uint GetCatalogIndex = 3038;
        public static readonly uint GetCatalogPage = 1132;
        public static readonly uint PurchaseCatalogItem = 3487;
        public static readonly uint StartGame = 1231;
        public static readonly uint GameClosed = 2504;
    }
}
