using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r63c;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Quests;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade;
using SkylightEmulator.Communication.Messages.Outgoing.r63c;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Managers
{
    class r63cPacketManager : PacketManager
    {
        public Dictionary<uint, IncomingPacket> IncomingPackets;
        public Dictionary<OutgoingPacketsEnum, OutgoingPacket> OutgoingPackets;
        public Dictionary<Type, OutgoingHandlerPacket> NewOutgoingPackets;

        public r63cPacketManager()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.OutgoingPackets = new Dictionary<OutgoingPacketsEnum, OutgoingPacket>();
            this.NewOutgoingPackets = new Dictionary<Type, OutgoingHandlerPacket>();

            this.NewOutgoingPackets.Add(typeof(MessengerSearchUserResultsComposerHandler), new MessengerSearchUserResultsComposer<MessengerSearchUserResultsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerSendFriendRequestErrorComposerHandler), new MessengerSendFriendRequestErrorComposer<MessengerSendFriendRequestErrorComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerReceivePrivateMessageComposerHandler), new MessengerReceivePrivateMessageComposer<MessengerReceivePrivateMessageComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerSendPrivateMessageErrorComposerHandler), new MessengerSendPrivateMessageErrorComposer<MessengerSendPrivateMessageErrorComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerReceiveFriendRequestComposerHandler), new MessengerReceiveFriendRequestComposer<MessengerReceiveFriendRequestComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerUpdateFriendsComposerHandler), new MessengerUpdateFriendsComposer<MessengerUpdateFriendsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerFollowFriendErrorComposerHandler), new MessengerFollowFriendErrorComposer<MessengerFollowFriendErrorComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerFollowUserComposerHandler), new MessengerFollowUserComposer<MessengerFollowUserComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerReceivedRoomInviteComposerHandler), new MessengerReceivedRoomInviteComposer<MessengerReceivedRoomInviteComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerInitComposerHandler), new MessengerInitComposer<MessengerInitComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerFriendsComposerHandler), new MessengerFriendsComposer<MessengerFriendsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(MessengerFriendRequestsComposerHandler), new MessengerFriendRequestsComposer<MessengerFriendRequestsComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(TradeStartErrorComposerHandler), new TradeStartErrorComposer<TradeStartErrorComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeStartComposerHandler), new TradeStartComposer<TradeStartComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeCancelComposerHandler), new TradeCancelComposer<TradeCancelComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeUpdateComposerHandler), new TradeUpdateComposer<TradeUpdateComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeAcceptedComposerHandler), new TradeAcceptedComposer<TradeAcceptedComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeRequireConfirmComposerHandler), new TradeRequireConfirmComposer<TradeRequireConfirmComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(TradeWindowCloseComposerHandler), new TradeWindowCloseComposer<TradeWindowCloseComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(InventoryRefreshComposerHandler), new InventoryRefreshComposer<InventoryRefreshComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(SendQuestsComposerHandler), new SendQuestsComposer<SendQuestsComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(GuideToolComposerHandler), new GuideToolComposer<GuideToolComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideRequestErrorComposerHandler), new GuideRequestErrorComposer<GuideRequestErrorComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSessionAttachedComposerHandler), new GuideSessionAttachedComposer<GuideSessionAttachedComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSessionDetachedComposerHandler), new GuideSessionDetachedComposer<GuideSessionDetachedComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideCloseReasonCodeComposerHandler), new GuideCloseReasonCodeComposer<GuideCloseReasonCodeComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideStartSessionComposerHandler), new GuideStartSessionComposer<GuideStartSessionComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSessionPartnerTypingComposerHandler), new GuideSessionPartnerTypingComposer<GuideSessionPartnerTypingComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSessionReceiveMessageComposerHandler), new GuideSessionReceiveMessageComposer<GuideSessionReceiveMessageComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSendToRoomComposerHandler), new GuideSendToRoomComposer<GuideSendToRoomComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(GuideSendInviteComposerHandler), new GuideSendInviteComposer<GuideSendInviteComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(BullyReportStartComposerHandler), new BullyReportStartComposer<BullyReportStartComposerHandler>()); //This can also be used for tours, questions etc
            this.NewOutgoingPackets.Add(typeof(BullyReportCodeComposerHandler), new BullyReportCodeComposer<BullyReportCodeComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(NewBullyReportComposerHandler), new NewBullyReportComposer<NewBullyReportComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(BullyReportAttachedComposerHandler), new BullyReportAttachedComposer<BullyReportAttachedComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(BullyReportDetachedComposerHandler), new BullyReportDetachedComposer<BullyReportDetachedComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(BullyVoteResultsComposerHandler), new BullyVoteResultsComposer<BullyVoteResultsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(BullyReportCloseComposerHandler), new BullyReportCloseComposer<BullyReportCloseComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(BullyReportResultsComposerHandler), new BullyReportResultsComposer<BullyReportResultsComposerHandler>());

            this.NewOutgoingPackets.Add(typeof(SendTalentTrackComposerHandler), new SendTalentTrackComposer<SendTalentTrackComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendQuizComposerHandler), new SendQuizComposer<SendQuizComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(QuizResultsComposerHandler), new QuizResultsMessage<QuizResultsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendAchievementListComposerHandler), new SendAchievementListComposer<SendAchievementListComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendUserProfileComposerHandler), new SendUserProfileComposer<SendUserProfileComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendUserRelationsComposerHandler), new SendUserRelationsComposer<SendUserRelationsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendUserActiveBadgesComposerHandler), new SendUserActiveBadgesComposer<SendUserActiveBadgesComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendCatalogModeComposerHandler), new SendCatalogModeComposer<SendCatalogModeComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendUserTagsComposerHandler), new SendUserTagsComposer<SendUserTagsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendMySettingsComposerHandler), new SendMySettingsComposer<SendMySettingsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(UpdateRoomUserComposerHandler), new UpdateRoomUserComposer<UpdateRoomUserComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(UpdateUserLookComposerHandler), new UpdateUserLookComposer<UpdateUserLookComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(UserAchievementScoreComposerHandler), new UserAchievementScoreComposer<UserAchievementScoreComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(AchievementRequirementsComposerHandler), new AchievementRequirementsComposer<AchievementRequirementsComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(SendWardrobeComposerHandler), new SendWardrobeComposer<SendWardrobeComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(NewbieIdentityComposerHandler), new NewbieIdentityComposer<NewbieIdentityComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(ReceiveWhisperComposerHandler), new ReceiveWhisperComposer<ReceiveWhisperComposerHandler>());
            this.NewOutgoingPackets.Add(typeof(UserActionComposerHandler), new UserActionComposer<UserActionComposerHandler>());

            this.OutgoingPackets.Add(OutgoingPacketsEnum.Fuserights, new FuserightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AvaiblityStatus, new AvaiblityStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AuthOk, new AuthOkMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ModTool, new ModToolMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ShowNotifications, new ShowNotificationsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.HomeRoom, new HomeRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FavouriteRooms, new FavouriteRoomsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateCredits, new UpdateCreditsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MOTD, new MOTDMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UnseenItem, new UnseenItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPoints, new UpdateActivityPointsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPointsSilence, new UpdateActivityPointsSilenceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateAchievement, new UpdateAchievementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Badges, new BadgesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.EnterPrivateRoom, new EnterPrivateRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.LoadingRoomInfo, new LoadingRoomInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ApplyRoomEffect, new ApplyRoomEffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRoomRights, new GiveRoomRightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.IsRoomOwner, new IsRoomOwnerMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomRating, new RoomRatingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RelativeHeightmap, new RelativeHeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Heightmap, new HeightmapMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.SetRoomUser, new SetRoomUserMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Friends, new FriendsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Requests, new RequestsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NotifFromMod, new NotifFromModMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateUserState, new UpdateUserStateMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FlatCats, new FlatCatsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogPage, new CatalogPageMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddFloorItemToRoom, new AddFloorItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserLeaved, new UserLeavedMessageEvent());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.CatalogIndexes, new CatalogIndexesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.BuyInfo, new BuyInfoMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddItemToInventory, new AddItemToInventoryMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewItems, new NewItemsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveItemFromHand, new RemoveItemFromHandMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Chat, new ChatMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateFloorItem, new UpdateFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MoveFloorItem, new UpdateFloorItemMessageResponse()); //I'm lazy :)
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AddWallItemToRoom, new AddWallItemToRoomMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateWallItem, new UpdateWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveFloorItem, new RemoveFloorItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RemoveWallItem, new RemoveWallItemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserPerks, new UserPerksMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorMetaData, new NewNavigatorMetaDataMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorLiftedRooms, new NewNavigatorLiftedRoomsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorCollapsedCategories, new NewNavigatorCollapsedCategoriesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorSavedSearches, new NewNavigatorSavedSearchesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorEventCategories, new NewNavigatorEventCategoriesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.TypingStatus, new TypingStatusMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Ping, new PingMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Wave, new WaveMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Idle, new IdleMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AchievementUnlocked, new AchievementUnlockedMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Handitem, new HanditemMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.GiveRespect, new GiveRespectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomSettingsOK, new RoomSettingsOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomUpdateOK, new RoomUpdateOKMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.StartTrade, new StartTradeMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Effect, new EffectMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RollerMovement, new RollerMovementMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Shout, new ShoutMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Dance, new DanceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomChatSettings, new RoomChatSettingsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomVIPSettings, new RoomVIPSettingsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.RoomData, new RoomDataMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Action, new ActionMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ClubMembership, new ClubMembershipMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateUser, new UpdateUserMessageResponse());

            this.IncomingPackets.Add(r63cIncoming.VersionCheck, new VersionCheckIncomingMessage());
            this.IncomingPackets.Add(r63cIncoming.SwfVariables, new SwfVariablesIncomingMessage());
            this.IncomingPackets.Add(r63cIncoming.MachineId, new MachineIdIncomingMessage());
            this.IncomingPackets.Add(r63cIncoming.SSOLogin, new SSOLoginMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.HabboData, new HabboDataMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetCredits, new GetCreditsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Event, new EventMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestRoom, new RequestRoomMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RoomEntry, new RoomEntryMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RoomEntry2, new RoomEntryMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.InitMessenger, new InitMessengerMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.FlatCats, new FlatCatsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestInventoryItems, new RequestInventoryItemsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetCatalogIndex, new GetCatalogIndexMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetCatalogPage, new GetCatalogPageMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.PurchaseItem, new PurchaseItemMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.PlaceObject, new PlaceObjectMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Move, new MoveMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Talk, new TalkMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Ping, new PingMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.MoveFloorItem, new MoveFloorItemMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.MoveWallItem, new MoveWallItemMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.UseFloorFurniture, new UseFloorFurnitureMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.PickupItem, new PickupItemMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.NewNavigatorRooms, new NewNavigatorRoomsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestRoomData, new RequestRoomDataMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.CreateRoom, new CreateRoomMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetRoomSettings, new GetRoomSettingsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestClub, new RequestClubMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.SaveRoomSettings, new SaveRoomSettingsMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Sign, new SignMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.HotelViewData, new HotelViewDataMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetGames, new GetGamesMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GetAccountGameStatus, new GetAccountGameStatusMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.JoinGame, new JoinGameMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.LeaveGame, new LeaveGameMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Shout, new ShoutMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Dance, new DanceMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.SetHomeRoom, new SetHomeRoomMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.CheckUsername, new CheckUsernameMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.ConfirmUsername, new ConfirmUsernameMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Action, new ActionMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Sit, new SitMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.ClubData, new ClubDataMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.Motto, new MottoMessageEvent());

            this.IncomingPackets.Add(r63cIncoming.MessengerSearchUser, new MessengerSearchUserEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerSendFriendRequest, new MessengerSendFriendRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerSendPrivateMessage, new MessengerSendPrivateMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerRemoveFriends, new MessengerRemoveFriendsEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerAcceptFriendRequest, new MessengerAcceptFriendRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerDeclineFriendRequest, new MessengerDeclineFriendRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerFollowFriend, new MessengerFollowFriendEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerSendRoomInvite, new MessengerSendRoomInviteEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerInit, new MessengerInitEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerGetFriendRequests, new MessengerGetFriendRequestsEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerSetFriendRelation, new MessengerSetFriendRelationEvent());
            this.IncomingPackets.Add(r63cIncoming.MessengerRequestUpdate, new MessengerRequestUpdateEvent());

            this.IncomingPackets.Add(r63cIncoming.TradeStart, new TradeStartEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeOfferItem, new TradeOfferItemEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeOfferMultipleItems, new TradeOfferMultipleItemsEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeRemoveItem, new TradeRemoveItemEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeAccept, new TradeAcceptEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeModify, new TradeModifyEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeConfirm, new TradeConfirmEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeClose, new TradeCloseEvent());
            this.IncomingPackets.Add(r63cIncoming.TradeCloseConfirm, new TradeCloseConfirmEvent());

            //this.IncomingPackets.Add(r63cIncoming.GetPollData, new GetPollDataEvent());

            this.IncomingPackets.Add(r63cIncoming.GetQuests, new GetQuestsEvent());

            this.IncomingPackets.Add(r63cIncoming.GetGuideTool, new GetGuideToolEvent());
            this.IncomingPackets.Add(r63cIncoming.SendGuideRequest, new SendGuideRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.CancelGuideRequest, new CancelGuideRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.HandleGuideRequest, new HandleGuideRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.CloseGuideRequest, new CloseGuideRequestEvent());
            this.IncomingPackets.Add(r63cIncoming.GuideRecommendHelper, new GuideRecommendHelperEvent());
            this.IncomingPackets.Add(r63cIncoming.GuideTypingStatus, new GuideTypingStatusEvent());
            this.IncomingPackets.Add(r63cIncoming.GuideSesisonSendMessage, new GuideSesisonSendMessageEvent());
            this.IncomingPackets.Add(r63cIncoming.GuideVisitUser, new GuideVisitUserEvent());
            this.IncomingPackets.Add(r63cIncoming.GuideInviteUser, new GuideInviteUserEvent());

            this.IncomingPackets.Add(r63cIncoming.ReportUserBullyingStart, new ReportUserBullyingStartEvent());
            this.IncomingPackets.Add(r63cIncoming.ReportBully, new ReportBullyEvent());
            this.IncomingPackets.Add(r63cIncoming.HandleBullyReport, new HandleBullyReportEvent());
            this.IncomingPackets.Add(r63cIncoming.SkipBullyReport, new SkipBullyReportEvent());
            this.IncomingPackets.Add(r63cIncoming.BullyReportVote, new BullyReportVoteEvent());

            //Misc
            this.IncomingPackets.Add(r63cIncoming.RequestTalentTrack, new RequestTalentTrackEvent());
            this.IncomingPackets.Add(r63cIncoming.CompletedQuiz, new CompletedQuizEvent());
            this.IncomingPackets.Add(r63cIncoming.AnsweredQuiz, new AnsweredQuizEvent());
            this.IncomingPackets.Add(r63cIncoming.GetAchievements, new GetAchievementsEvent());
            this.IncomingPackets.Add(r63cIncoming.GetUserProfile, new GetUserProfileEvent());
            this.IncomingPackets.Add(r63cIncoming.GetUserRelations, new GetUserRelationsEvent());
            this.IncomingPackets.Add(r63cIncoming.GerUserWearingBadges, new GerUserWearingBadgesEvent());
            this.IncomingPackets.Add(r63cIncoming.SetActiveBadges, new SetActiveBadgesEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestDiscount, new RequestDiscountEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestCatalogMode, new RequestCatalogModeEvent());
            this.IncomingPackets.Add(r63cIncoming.LookAtPoint, new LookAtPointEvent());
            this.IncomingPackets.Add(r63cIncoming.GetUserTags, new GetUserTagsEvent());
            this.IncomingPackets.Add(r63cIncoming.GetMySettings, new GetMySettingsEvent());
            this.IncomingPackets.Add(r63cIncoming.SetPreferOldChat, new SetPreferOldChatEvent());
            this.IncomingPackets.Add(r63cIncoming.SetDisableRoomUnits, new SetDisableRoomUnitsEvent());
            this.IncomingPackets.Add(r63cIncoming.SetDisableCameraFollow, new SetDisableCameraFollowEvent());
            this.IncomingPackets.Add(r63cIncoming.SaveUserVolume, new SaveUserVolumeEvent());
            this.IncomingPackets.Add(r63cIncoming.SaveLook, new SaveLookEvent());
            this.IncomingPackets.Add(r63cIncoming.RequestAchievementConfiguration, new RequestAchievementConfigurationEvent());
            this.IncomingPackets.Add(r63cIncoming.SaveLookToWardrobe, new SaveLookToWardrobeEvent());
            this.IncomingPackets.Add(r63cIncoming.GetWardrobe, new GetWardrobeEvent());
            this.IncomingPackets.Add(r63cIncoming.Whisper, new WhisperEvent());
            this.IncomingPackets.Add(r63cIncoming.GiveRespect, new GiveRespectEvent());
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
            if (!this.OutgoingPackets.TryGetValue(id, out OutgoingPacket packet))
            {
                throw new Exception("Outgoing packet not found: " + id);
            }

            return packet;
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
            if (!this.NewOutgoingPackets.TryGetValue(handler.GetType(), out OutgoingHandlerPacket packet))
            {
                throw new Exception("New Outgoing packet not found: " + handler.GetType());
            }

            return handler.Handle(packet);
        }
    }
}
