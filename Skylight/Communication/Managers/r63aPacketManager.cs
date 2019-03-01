using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r63a;
using SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake;
using SkylightEmulator.Communication.Messages.Outgoing.r63a;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
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
        public Dictionary<OutgoingPacketsEnum, OutgoingPacket> OutgoingPackets;
        public Dictionary<Type, OutgoingHandlerPacket> NewOutgoingPackets;

        public r63aPacketManager()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.OutgoingPackets = new Dictionary<OutgoingPacketsEnum, OutgoingPacket>();
            this.NewOutgoingPackets = new Dictionary<Type, OutgoingHandlerPacket>();

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
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPoints, new UpdateActivityPointsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateAchievement, new UpdateAchievementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Badges, new BadgesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.InfoRetrieve, new InfoRetrieveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPointsSilence, new UpdateActivityPointsSilenceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Friends, new FriendsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Requests, new RequestsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.EnterPrivateRoom, new EnterPrivateRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LoadingRoomInfo, new LoadingRoomInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ApplyRoomEffect, new ApplyRoomEffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRoomRights, new GiveRoomRightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.IsRoomOwner, new IsRoomOwnerMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomRating, new RoomRatingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FlatCats, new FlatCatsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Heightmap, new HeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RelativeHeightmap, new RelativeHeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.SetRoomUser, new SetRoomUserMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromMod, new NotifFromModMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateUserState, new UpdateUserStateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AchievementUnlocked, new AchievementUnlockedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Doorbell, new DoorbellMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.DoorbellNoAnswer, new DoorbellNoAnswerMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerUpdate, new MessengerUpdateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomError, new RoomErrorMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LeaveRoom, new LeaveRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomErrorOnEnter, new RoomErrorOnEnterMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromAdmin, new NotifFromAdminMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BadgePointLimits, new BadgePointLimitsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.TypingStatus, new TypingStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Chat, new ChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Shout, new ShoutMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerSearch, new MessengerSearchMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogPage, new CatalogPageMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserLeaved, new UserLeavedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogIndexes, new CatalogIndexesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BuyInfo, new BuyInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddItemToInventory, new AddItemToInventoryMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewItems, new NewItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveItemFromHand, new RemoveItemFromHandMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddFloorItemToRoom, new AddFloorItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateFloorItem, new UpdateFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddWallItemToRoom, new AddWallItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateWallItem, new UpdateWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveFloorItem, new RemoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveWallItem, new RemoveWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Ping, new PingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MoveFloorItem, new MoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Idle, new IdleMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Effect, new EffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.PublicItems, new PublicItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSearch, new RoomSearchMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.PrivateChat, new PrivateChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MessengerChatError, new MessengerChatErrorMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewFriendRequest, new NewFriendRequestMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Wave, new WaveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RollerMovement, new RollerMovementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Handitem, new HanditemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRespect, new GiveRespectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSettingsOK, new RoomSettingsOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomUpdateOK, new RoomUpdateOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomVIPSettings, new RoomVIPSettingsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomData, new RoomDataMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.StartTrade, new StartTradeMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ActiveBadges, new ActiveBadgesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Flood, new FloodMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Dance, new DanceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ClubMembership, new ClubMembershipMessageResponse());

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
            this.IncomingPackets.Add(r63aIncoming.GoToFlat, new GoToFlatMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.FollowFriend, new FollowFriendMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Quit, new QuitMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.KickUser, new KickUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.BanUser, new BanUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Wave, new WaveMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Dance, new DanceMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetClubOffers, new GetClubOffersMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UpdateFigure, new UpdateFigureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UpdateMotto, new UpdateMottoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetWarbode, new GetWarbodeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SaveWarbode, new SaveWarbodeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetBadges, new GetBadgesMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SetActiveBadges, new SetActiveBadgesMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetQuests, new GetQuestsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.LatencyTest, new LatencyTestMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.LatencyTestReport, new LatencyTestReportMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetMarketplaceConfiguration, new GetMarketplaceConfigurationMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetMarketplaceCanMakeOffer, new GetMarketplaceCanMakeOfferMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.BuyMarketplaceTokens, new BuyMarketplaceTokensMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetMarketplaceItemStats, new GetMarketplaceItemStatsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MakeOffer, new MakeOfferMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.StartTyping, new StartTypingMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.StopTyping, new StopTypingMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetOffers, new GetOffersMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.BuyOffer, new BuyOfferMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetOwnOffers, new GetOwnOffersMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RedeemCredits, new RedeemCreditsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CancelOffer, new CancelOfferMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RedeemFurni, new RedeemFurniMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.LookTo, new LookToMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorGetUserInfo, new ModeratorGetUserInfoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorGetRoomInfo, new ModeratorGetRoomInfoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorBanUser, new ModeratorBanUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorCaution, new ModeratorCautionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFAQs, new GetFAQsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetAllFAQs, new GetAllFAQsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFAQCategorys, new GetFAQCategorysMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFAQ, new GetFAQMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CallForHelp, new CallForHelpMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorPickIssue, new ModeratorPickIssueMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorCloseIssue, new ModeratorCloseIssueMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CallGuideBot, new CallGuideBotMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.DeletePendingCall, new DeletePendingCallMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorReleaseIssue, new ModeratorReleaseIssueMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorSendMessage, new ModeratorSendMessageMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorKickUser, new ModeratorKickUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorGetRoomChatlog, new ModeratorGetRoomChatlogMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorGetRoomVisits, new ModeratorModeratorGetRoomVisits());
            this.IncomingPackets.Add(r63aIncoming.ModeratorGetUserChatlog, new ModeratorGetUserChatlogMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorRoomAction, new ModeratorRoomActionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModeratorAction, new ModeratorActionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SetHomeRoom, new SetHomeRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CanCreateEvent, new CanCreateEventMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CreateEvent, new CreateEventMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.EditEvent, new EditEventMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.EndEvent, new EndEventMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SearchEvents, new SearchEventsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetRoomsWithHigestScore, new GetRoomsWithHigestScoreMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUsedTagsOnRooms, new GetUsedTagsOnRoomsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RoomTagSearch, new RoomTagSearchMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RoomSearch, new RoomSearchMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MyRoomHistory, new MyRoomHistoryMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.WhereMyFriensAre, new WhereMyFriensAreMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RoomsOwnedByFriends, new RoomsOwnedByFriendsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.AddFavouriteRoom, new AddFavouriteRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RemoveFavouriteRoom, new RemoveFavouriteRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetMyFavouriteRooms, new GetMyFavouriteRoomsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetAchievements, new GetAchievementsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUserSelectedBadges, new GetUserSelectedBadgesMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUserTags, new GetUserTagsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SetSoundSettings, new SetSoundSettingsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.DeleteRoom, new DeleteRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GiveRoomRights, new GiveRoomRightsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RemoveRoomRights, new RemoveRoomRightsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RemoveAllRoomRights, new RemoveAllRoomRightsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetSellablePetBreeds, new GetSellablePetBreedsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ValidatePetName, new ValidatePetNameMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.IsOfferGiftable, new IsOfferGiftableMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetGiftWrappingConfiguration, new GetGiftWrappingConfigurationMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.BuyAsGift, new BuyAsGiftMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ApplyRoomEffect, new ApplyRoomEffectMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PlacePostIt, new PlacePostItMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetPostItData, new GetPostItDataMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SavePostItData, new SavePostItDataMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.DeletePostIt, new DeletePostItMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetRoomDimmerPresets, new GetRoomDimmerPresetsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetRoomDimmerChangeState, new GetRoomDimmerChangeStateMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RoomDimmerSavePreset, new RoomDimmerSavePresetMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetPetInventory, new GetPetInventoryMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PlacePet, new PlacePetMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetPetInfo, new GetPetInfoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.PickupPet, new PickupPetMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetPetCommands, new GetPetCommandsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseDice, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.OffDice, new OffDiceMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseHabbowheel, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseLoveshuffler, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UpdateWiredTrigger, new UpdateWiredTriggerMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UpdateWiredAction, new UpdateWiredActionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UpdateWiredCondition, new UpdateWiredConditionMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Whisper, new WhisperMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.StartTrade, new StartTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CancelTrade, new CancelTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.OfferItem, new OfferItemMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.AcceptTrade, new AcceptTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ModifyTrade, new ModifyTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ConfirmAcceptTrade, new ConfirmAcceptTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RemoveItem, new RemoveItemMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.DeclineAcceptTrade, new DeclineAcceptTradeMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SecretKey, new SecretKeyMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Variables, new VariablesMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.MachineId, new MachineIdMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.OpenGift, new OpenGiftMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RedeemVoucher, new RedeemVoucherMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GiveRespect, new GiveRespectMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Pong, new PongMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RespectPet, new RespectPetMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.CheckUsername, new CheckUsernameMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ChangeUsername, new ChangeUsernameMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UseOnWayGate, new UseFurnitureMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RateRoom, new RateRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.IgnoreUser, new IgnoreUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.UnignoreUser, new UnignoreUserMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.Login, new LoginMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SetObjectData, new SetObjectDataMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.SetFootballGateData, new SetFootballGateDataMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ToggleStaffPick, new ToggleStaffPickMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.ToggleFriendStream, new ToggleFriendStreamMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetFriendStream, new GetFriendStreamMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetJukeboxPlaylist, new GetJukeboxPlaylistMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetUserDiscs, new GetUserDiscsMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetSongInfo, new GetSongInfoMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.AddDiscToJukebox, new AddDiscToJukeboxMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.RemoveDiscToJukebox, new RemoveDiscToJukeboxMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetNowPlaying, new GetNowPlayingMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetIgnoredUsers, new GetIgnoredUsersMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.GetPublicRoom, new GetPublicRoomMessageEvent());
            this.IncomingPackets.Add(r63aIncoming.OpenConnection, new OpenConnectionMessageEvent());
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

            this.OutgoingPackets = null;
        }

        public override ServerMessage GetNewOutgoing(OutgoingHandler handler)
        {
            OutgoingHandlerPacket packet;
            if (!this.NewOutgoingPackets.TryGetValue(handler.GetType(), out packet))
            {
                throw new Exception("New Outgoing packet not found: " + handler.GetType());
            }

            return handler.Handle(packet);
        }
    }
}
