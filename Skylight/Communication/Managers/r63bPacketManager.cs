using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r63b;
using SkylightEmulator.Communication.Messages.Outgoing.r63b;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Managers
{
    class r63bPacketManager : PacketManager
    {
        public Dictionary<uint, IncomingPacket> IncomingPackets;
        public Dictionary<OutgoingPacketsEnum, OutgoingPacket> OutgoingPackets;

        public override void Initialize()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.OutgoingPackets = new Dictionary<OutgoingPacketsEnum, OutgoingPacket>();

            this.OutgoingPackets.Add(OutgoingPacketsEnum.Fuserights, new FuserightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AvaiblityStatus, new AvaiblityStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AuthOk, new AuthOkMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ShowNotifications, new ShowNotificationsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.HomeRoom, new HomeRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FavouriteRooms, new FavouriteRoomsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ModTool, new ModToolMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateCredits, new UpdateCreditsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MOTD, new MOTDMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UnseenItem, new UnseenItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateAchievement, new UpdateAchievementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Badges, new BadgesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.InfoRetrieve, new InfoRetrieveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPoints, new UpdateActivityPointsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPointsSilence, new UpdateActivityPointsSilenceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BadgePointLimits, new BadgePointLimitsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Friends, new FriendsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Requests, new RequestsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FlatCats, new FlatCatsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.EnterPrivateRoom, new EnterPrivateRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LoadingRoomInfo, new LoadingRoomInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ApplyRoomEffect, new ApplyRoomEffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRoomRights, new GiveRoomRightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.IsRoomOwner, new IsRoomOwnerMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomRating, new RoomRatingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromMod, new NotifFromModMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Heightmap, new HeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RelativeHeightmap, new RelativeHeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateUserState, new UpdateUserStateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.SetRoomUser, new SetRoomUserMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateFloorItem, new UpdateFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Idle, new IdleMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserPerks, new UserPerksMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserLeaved, new UserLeavedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.TypingStatus, new TypingStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Chat, new ChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Shout, new ShoutMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Wave, new WaveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AchievementUnlocked, new AchievementUnlockedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MoveFloorItem, new UpdateFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddFloorItemToRoom, new AddFloorItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveItemFromHand, new RemoveItemFromHandMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveFloorItem, new RemoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddItemToInventory, new AddItemToInventoryMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveWallItem, new RemoveWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogPage, new CatalogPageMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BuyInfo, new BuyInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewItems, new NewItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RollerMovement, new RollerMovementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Handitem, new HanditemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRespect, new GiveRespectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSettingsOK, new RoomSettingsOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomUpdateOK, new RoomUpdateOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.StartTrade, new StartTradeMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Effect, new EffectMessageResponse());

            this.IncomingPackets.Add(r63bIncoming.VersionCheck, new VersionCheckMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.InitCryptoMessage, new InitCryptoMessageMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.SecretKey, new SecretKeyMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.Variables, new VariablesMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.MachineId, new MachineIdMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.SSOTicket, new SSOTicketMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetUserData, new GetUserDataMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.EventHappend, new EventHappendMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetCredits, new GetCreditsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetUserClubMembership, new GetUserClubMembershipMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetBadgePointLimits, new GetBadgePointLimitsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetSoundSettings, new GetSoundSettingsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetPrizes, new GetPrizesMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetAchievementCompetion, new GetAchievementCompetionMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetAchievementCompetionHOF, new GetAchievementCompetionHOFMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetCommunityGoal, new GetCommunityGoalMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGames, new GetGamesMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGameAchievements, new GetGameAchievementsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGame, new GetGameMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGame2, new GetGame2MessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetWeeklyLeaderboard, new GetWeeklyLeaderboardMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGame3, new GetGame3MessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetAccountGameStatus, new GetAccountGameStatusMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGame4, new GetGame4MessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetGame5, new GetGame5MessageEvent());
            this.IncomingPackets.Add(r63bIncoming.InitMessenger, new InitMessengerMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetIgnoredUsers, new GetIgnoredUsersMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetFlatCats, new GetFlatCatsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.OpenFlatConnection, new OpenFlatConnectionMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetRoomEntryData, new GetRoomEntryDataMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.AddUserToRoom, new GetRoomEntryDataMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.AddUserToRoom2, new GetRoomEntryDataMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetUserSelectedBadges, new GetUserSelectedBadgesMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetUserTags, new GetUserTagsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GuestRoomInfo, new GuestRoomInfoMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.UserMove, new UserMoveMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.StartTyping, new StartTypingMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.StopTyping, new StopTypingMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.Speak, new SpeakMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.Shout, new ShoutMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.MyRooms, new MyRoomsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetPublicRooms, new GetPublicRoomsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetRooms, new GetRoomsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.HigestRatedRooms, new HigestRatedRoomsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.RecentRooms, new RecentRoomsMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.CanCreateRoom, new CanCreateRoomMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.CreateRoom, new CreateRoomMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.MoveOrRotate, new MoveOrRotateMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.OpenInventory, new OpenInventoryMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.AddFloorItem, new AddFloorItemMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.PickupItem, new PickupItemMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetCatalogIndex, new GetCatalogIndexMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GetCatalogPage, new GetCatalogPageMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.PurchaseCatalogItem, new PurchaseCatalogItemMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.StartGame, new StartGameMessageEvent());
            this.IncomingPackets.Add(r63bIncoming.GameClosed, new GameClosedMessageEvent());
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
