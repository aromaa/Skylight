using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Managers
{
    public class r63aPacketManager : PacketManager
    {
        public Dictionary<uint, IncomingPacket> IncomingPackets;

        public r63aPacketManager()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.IncomingPackets.Add(r63aIncoming.InitCryptoMessage, new InitCryptoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetSessionParameters, new GetSessionParametersMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SSOTicket, new SSOTicketMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.EventHappend, new EventMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUserData, new InfoRetrieveMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetCredits, new GetCreditsInfoEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUserClubMembership, new GetUserClubMembershipMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetSoundSettings, new GetSoundSettingsEvent());
            this.IncomingPackets.Add(r63aIncoming.InitMessenger, new MessengerInitMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MessengerSearch, new HabboSearchMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RequestFriend, new RequestBuddyMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.AcceptFriendRequest, new AcceptBuddyMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SendChatMessage, new SendMsgMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MessengerRemoveFriends, new RemoveBuddyMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.DeclineFriendRequest, new DeclineBuddyMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.FriendListUpdate, new FriendsListUpdateEvent());
            this.IncomingPackets.Add(r63aIncoming.MessengerSendRoomInvite, new SendRoomInviteMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetOfficalRooms, new GetOfficialRoomsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetMyRooms, new MyRoomsSearchMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CanCreateRoom, new CanCreateRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CreateRoom, new CreateFlatMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFlatCats, new GetUserFlatCatsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFriendRequests, new GetBuddyRequestsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetBadgePointLimits, new GetBadgePointLimitsEvent());
            this.IncomingPackets.Add(r63aIncoming.OpenFlatConnection, new OpenFlatConnectionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFurniAlias, new GetFurnitureAliasesMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetRoomEntryData, new GetRoomEntryDataMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Say, new ChatMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Shout, new ShoutMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UserMove, new MoveAvatarMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CatalogIndex, new GetCatalogIndexMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CatalogPage, new GetCatalogPageMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PurchaseItem, new PurchaseItemMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RequestInventory, new RequestInventoryMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PlaceObject, new PlaceObjectMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GuestRoomInfo, new GetGuestRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseFloorFurniture, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseWallFurniture, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MoveFloorItem, new MoveObjectMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MoveWallItem, new MoveWallItemMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PickupRoomItem, new PickupObjectMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetRoomSettings, new GetRoomSettingsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SaveRoomSettings, new SaveRoomSettingsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SaveRoomThumbnail, new SaveRoomThumbnailMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PopularRoomSearch, new PopularRoomsSearchMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.LetUserIn, new LetUserInMessageEvent());
        }

        public override void Initialize()
        {

        }

        public override bool HandleIncoming(uint id, out IncomingPacket packet)
        {
            if (this.IncomingPackets.ContainsKey(id))
            {
                packet = this.IncomingPackets[id];
                return true;
            }
            else
            {
                packet = null;
                return false;
            }
        }

        public override bool PacketExits(uint id)
        {
            return this.IncomingPackets.ContainsKey(id);
        }
    }
}
