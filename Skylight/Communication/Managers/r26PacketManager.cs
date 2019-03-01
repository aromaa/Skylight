using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r26;
using SkylightEmulator.Communication.Messages.Incoming.r26.Handshake;
using SkylightEmulator.Communication.Messages.Outgoing.r26;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Managers
{
    public class r26PacketManager : PacketManager
    {
        public Dictionary<uint, IncomingPacket> IncomingPackets;
        public Dictionary<OutgoingPacketsEnum, OutgoingPacket> OutgoingPackets;

        public r26PacketManager()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.OutgoingPackets = new Dictionary<OutgoingPacketsEnum, OutgoingPacket>();

            this.OutgoingPackets.Add(OutgoingPacketsEnum.MOTD, new MOTDResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromAdmin, new NotifFromAdminMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromMod, new NotifFromAdminMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Fuserights, new FuserightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AuthOk, new AuthOkMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.HomeRoom, new HomeRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateCredits, new UpdateCreditsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Badges, new BadgesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.EnterPrivateRoom, new EnterPrivateRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LoadingRoomInfo, new LoadingRoomInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ApplyRoomEffect, new ApplyRoomEffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRoomRights, new GiveRoomRightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.IsRoomOwner, new IsRoomOwnerMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomRating, new RoomRatingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Heightmap, new HeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateUserState, new UpdateUserStateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Friends, new FriendsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Requests, new RequestsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.SetRoomUser, new SetRoomUserMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.TypingStatus, new TypingStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Chat, new ChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Shout, new ShoutMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerSearch, new MessengerSearchMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerUpdate, new MessengerUpdateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Ping, new PingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Effect, new EffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Idle, new IdleMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserLeaved, new UserLeavedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Wave, new WaveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LeaveRoom, new LeaveRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.PublicItems, new PublicItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogPage, new CatalogPageMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BuyInfo, new BuyInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddItemToInventory, new AddItemToInventoryMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewItems, new NewItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSearch, new RoomSearchMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddFloorItemToRoom, new AddFloorItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveItemFromHand, new RemoveItemFromHandMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddWallItemToRoom, new AddWallItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MoveFloorItem, new MoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveFloorItem, new RemoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveWallItem, new RemoveWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateFloorItem, new UpdateFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.PrivateChat, new PrivateChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerChatError, new MessengerChatErrorMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewFriendRequest, new NewFriendRequestMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Handitem, new HanditemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRespect, new GiveRespectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSettingsOK, new RoomSettingsOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomUpdateOK, new RoomUpdateOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.StartTrade, new StartTradeMessageResponse());

            this.IncomingPackets.Add(r26Incoming.InitCryptoMessage, new InitCryptoMessageEvent());
            this.IncomingPackets.Add(r26Incoming.SomeKeysMaybe, new SomeKeysMaybeMessageEvent());
            this.IncomingPackets.Add(r26Incoming.SSOTicket, new SSOTicketMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetUserClubMembership, new GetUserClubMembershipMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetUserData, new GetUserDataMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetCredits, new GetCreditsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetRooms, new GetRoomsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GuestRoomInfo, new GuestRoomInfoMessageEvent());
            this.IncomingPackets.Add(r26Incoming.LoadAdvertisement, new LoadAdvertisementMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetRoomState, new GetRoomStateMessageEvent());
            this.IncomingPackets.Add(r26Incoming.EnterCheckRoom, new EnterCheckRoomMessageEvent());
            this.IncomingPackets.Add(r26Incoming.EnterRoom, new EnterRoomMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RoomModel, new RoomModelMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RoomItems, new GetRoomItemsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetGroupsAndSkillLevels, new GetGroupsAndSkillLevelsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RoomWallItems, new RoomWallItemsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetRoomAdvertisement, new GetRoomAdvertisementMessageEvent());
            this.IncomingPackets.Add(r26Incoming.EntryRoom, new EntryRoomMessageEvent());
            this.IncomingPackets.Add(r26Incoming.InitMessenger, new InitMessengerMessageEvent());
            this.IncomingPackets.Add(r26Incoming.UserMove, new UserMoveMessageEvent());
            this.IncomingPackets.Add(r26Incoming.StartTyping, new StartTypingMessageEvent());
            this.IncomingPackets.Add(r26Incoming.StopTyping, new StopTypingMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Say, new SayMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Shout, new ShoutMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetBadges, new GetBadgesMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RandomRooms, new RandomRoomsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.MessengerSearch, new MessengerSearchMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RequestFriend, new RequestFriendMessageEvent());
            this.IncomingPackets.Add(r26Incoming.AcceptFriendRequest, new AcceptFriendRequestMessageEvent());
            this.IncomingPackets.Add(r26Incoming.FriendListUpdate, new FriendListUpdateMessageEvent());
            this.IncomingPackets.Add(r26Incoming.MessengerRemoveFriends, new MessengerRemoveFriendsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Ping, new PingMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Wave, new WaveMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Quit, new QuitMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetFlatCats, new GetFlatCatsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetEvents, new GetEventsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetRecycle, new GetRecycleMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetRecycleSession, new GetRecycleSessionMessageEvent());
            this.IncomingPackets.Add(r26Incoming.SetSwimsuit, new SetSwimsuitMessageEvent());
            this.IncomingPackets.Add(r26Incoming.ContorlWobble, new ContorlWobbleMessageEvent());
            this.IncomingPackets.Add(r26Incoming.CreateRoom, new CreateRoomMessageEvent());
            this.IncomingPackets.Add(r26Incoming.CreateRoomPhaseTwo, new CreateRoomPhaseTwoMessageEvent());
            this.IncomingPackets.Add(r26Incoming.ModifyRoomCategory, new ModifyRoomCategoryMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetOwnRooms, new GetOwnRoomsMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetCatalogIndexes, new GetCatalogIndexesMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetCatalogPage, new GetCatalogPageMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Purchase, new PurchaseMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetHand, new GetHandMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Kick, new KickMessageEvent());
            this.IncomingPackets.Add(r26Incoming.KickBan, new KickBanMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RoomSearch, new RoomSearchMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GameboardData, new GameboardDataMessageEvent());
            this.IncomingPackets.Add(r26Incoming.OpenStickyOrPhoto, new OpenStickyOrPhotoMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Variables, new VariablesMessageEvent());
            this.IncomingPackets.Add(r26Incoming.MachineId, new MachineIdMessageEvent());
            this.IncomingPackets.Add(r26Incoming.GetSessionParameters, new GetSessionParametersMessageEvent());
            this.IncomingPackets.Add(r26Incoming.RemoveStatus, new RemoveStatusMessageEvent());
            this.IncomingPackets.Add(r26Incoming.PlaceItem, new PlaceItemMessageEvent());
            this.IncomingPackets.Add(r26Incoming.MoveFloorItem, new MoveFloorItemMessageEvent());
            this.IncomingPackets.Add(r26Incoming.PickupItem, new PickupItemMessageEvent());
            this.IncomingPackets.Add(r26Incoming.ToggleItem, new ToggleItemMessageEvent());
            this.IncomingPackets.Add(r26Incoming.SendInstantMessage, new SendInstantMessageMessageEvent());
            this.IncomingPackets.Add(r26Incoming.DeclineFriendRequest, new DeclineFriendRequestMessageEvent());
            this.IncomingPackets.Add(r26Incoming.Follow, new FollowMessageEvent());
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

        public override bool HandleOutgoing(OutgoingPacketsEnum id, out OutgoingPacket packet)
        {
            if (this.OutgoingPackets.ContainsKey(id))
            {
                packet = this.OutgoingPackets[id];
                return true;
            }
            else
            {
                packet = null;
                return false;
            }
        }

        public override IncomingPacket GetIncoming(uint id)
        {
            return this.IncomingPackets[id];
        }

        public override OutgoingPacket GetOutgoing(OutgoingPacketsEnum id)
        {
            return this.OutgoingPackets[id];
        }

        public override void Clear()
        {
            if (this.IncomingPackets != null)
            {
                this.IncomingPackets.Clear();
            }
            this.IncomingPackets = null;

            if (this.OutgoingPackets != null)
            {
                this.OutgoingPackets.Clear();
            }
            this.OutgoingPackets = null;
        }

        public override ServerMessage GetNewOutgoing(OutgoingHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
