using System;
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
    }
}
