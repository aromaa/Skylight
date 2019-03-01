using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Headers
{
    public class r26Incoming
    {
        public static readonly uint InitCryptoMessage = 206;
        public static readonly uint SomeKeysMaybe = 2002;
        public static readonly uint SSOTicket = 204;
        public static readonly uint GetUserClubMembership = 26;
        public static readonly uint GetUserData = 7;
        public static readonly uint GetCredits = 8;
        public static readonly uint GetRooms = 150;
        public static readonly uint GuestRoomInfo = 21;
        public static readonly uint LoadAdvertisement = 182;
        public static readonly uint GetRoomState = 2;
        public static readonly uint EnterCheckRoom = 57;
        public static readonly uint EnterRoom = 59;
        public static readonly uint EventsData = 321;
        public static readonly uint RoomModel = 60;
        public static readonly uint RoomItems = 61;
        public static readonly uint GetGroupsAndSkillLevels = 62;
        public static readonly uint RoomWallItems = 63;
        public static readonly uint GetRoomAdvertisement = 126;
        public static readonly uint EntryRoom = 64;
        public static readonly uint InitMessenger = 12;
        public static readonly uint UserMove = 75;
        public static readonly uint StartTyping = 317;
        public static readonly uint StopTyping = 318;
        public static readonly uint Say = 52;
        public static readonly uint Shout = 55;
        public static readonly uint GetBadges = 157;
        public static readonly uint RandomRooms = 264;
        public static readonly uint MessengerSearch = 41;
        public static readonly uint RequestFriend = 39;
        public static readonly uint AcceptFriendRequest = 37;
        public static readonly uint FriendListUpdate = 15;
        public static readonly uint MessengerRemoveFriends = 40;
        public static readonly uint Ping = 196;
        public static readonly uint Wave = 94;
        public static readonly uint Quit = 53;
        public static readonly uint GetFlatCats = 151;
        public static readonly uint GetEvents = 321;
        public static readonly uint GetRecycle = 222;
        public static readonly uint GetRecycleSession = 223;
        public static readonly uint SetSwimsuit = 116;
        public static readonly uint ContorlWobble = 114;
        public static readonly uint CreateRoom = 29;
        public static readonly uint CreateRoomPhaseTwo = 25;
        public static readonly uint ModifyRoomCategory = 153;
        public static readonly uint GetOwnRooms = 16;
        public static readonly uint GetCatalogIndexes = 101;
        public static readonly uint GetCatalogPage = 102;
        public static readonly uint Purchase = 100;
        public static readonly uint GetHand = 65;
        public static readonly uint Kick = 95;
        public static readonly uint KickBan = 320;
        public static readonly uint RoomSearch = 17;
        public static readonly uint GameboardData = 117;
        public static readonly uint OpenStickyOrPhoto = 83;
        public static readonly uint Variables = 1170;
        public static readonly uint MachineId = 813;
        public static readonly uint GetSessionParameters = 1817;
        public static readonly uint RemoveStatus = 88;
        public static readonly uint PlaceItem = 90;
        public static readonly uint MoveFloorItem = 73;
        public static readonly uint PickupItem = 67;
        public static readonly uint ToggleItem = 74;
        public static readonly uint SendInstantMessage = 33;
        public static readonly uint DeclineFriendRequest = 38;
        public static readonly uint Follow = 262;
    }
}
