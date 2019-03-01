using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Guide
{
    /// <summary>
    /// Thread safe
    /// </summary>
    public class GuideSession : GuideRequest
    {
        public const int OfferTime = 15;

        public GameClient Requester { get; }
        public GuideRequestType Type { get; }
        public string Message { get; }
        
        public GuideSessionStatus Status { get; private set; }
        public double StatusChangedTimestamp { get; private set; }

        private volatile GameClient helper;
        public GameClient Helper => this.helper;

        public GameClient offeredTo;
        public GameClient OfferedTo => this.offeredTo;

        public double OfferedToTimestamp { get; private set; }
        public bool FindHelper => this.Status == GuideSessionStatus.WaitingForHelper ? this.OfferedTo == null ? true : TimeUtilies.GetUnixTimestamp() - this.OfferedToTimestamp >= GuideSession.OfferTime : false;

        public HashSet<uint> RejectedUsers { get; }

        public GuideSession(GameClient requester, GuideRequestType type, string message)
        {
            this.RejectedUsers = new HashSet<uint>();

            this.Requester = requester;
            this.Type = type;
            this.Message = message;

            this.SetStatus(GuideSessionStatus.WaitingForHelper);
        }

        public void SetStatus(GuideSessionStatus status)
        {
            this.Status = status;
            this.StatusChangedTimestamp = TimeUtilies.GetUnixTimestamp();
        }

        public void OfferTo(GameClient helper)
        {
            if (this.Status == GuideSessionStatus.WaitingForHelper)
            {
                (this.offeredTo = helper).SendMessage(new GuideSessionAttachedComposerHandler(true, this.Type, this.Message, GuideSession.OfferTime));

                this.OfferedToTimestamp = TimeUtilies.GetUnixTimestamp();
            }
        }
        
        public bool Accept(GameClient session, out int waitTime)
        {
            waitTime = (int)(TimeUtilies.GetUnixTimestamp() - this.StatusChangedTimestamp);

            if (this.Status == GuideSessionStatus.WaitingForHelper)
            {
                if (this.OfferedTo?.GetHabbo().ID == session.GetHabbo().ID)
                {
                    if (Interlocked.CompareExchange(ref this.helper, this.OfferedTo, null) == null)
                    {
                        this.SetStatus(GuideSessionStatus.OnGoing);

                        this.offeredTo = null;
                        this.SendToBoth(new GuideStartSessionComposerHandler(this.Requester.GetHabbo().ID, this.Requester.GetHabbo().Username, this.Requester.GetHabbo().Look, this.Helper.GetHabbo().ID, this.Helper.GetHabbo().Username, this.Helper.GetHabbo().Look));
                        
                        return true;
                    }
                }
            }
            
            return false;
        }

        public void Decline(GameClient session)
        {
            if (this.Status == GuideSessionStatus.WaitingForHelper)
            {
                if (Interlocked.CompareExchange(ref this.offeredTo, null, session) != null)
                {
                    session.SendMessage(new GuideSessionDetachedComposerHandler());

                    this.RejectedUsers.Add(session.GetHabbo().ID);
                }
            }
        }

        public void SendToBoth(OutgoingHandler handler)
        {
            this.Requester.SendMessage(handler);
            this.Helper.SendMessage(handler);
        }

        public void Close(GameClient session, bool disconnect = false)
        {
            if (this.Status == GuideSessionStatus.OnGoing)
            {
                this.SetStatus(GuideSessionStatus.Ended);

                this.Requester.SendMessage(new GuideCloseReasonCodeComposerHandler(this.Requester.GetHabbo().ID == session.GetHabbo().ID ? GuideCloseReasonCode.MeClosed : disconnect ? GuideCloseReasonCode.OtherDisconnected : GuideCloseReasonCode.OtherClosed));
                this.Helper.SendMessage(new GuideCloseReasonCodeComposerHandler(this.Helper.GetHabbo().ID == session.GetHabbo().ID ? GuideCloseReasonCode.MeClosed : disconnect ? GuideCloseReasonCode.OtherDisconnected : GuideCloseReasonCode.OtherClosed));

                this.Requester.GetHabbo().GetUserStats().GuideRequester++;
                this.Requester.GetHabbo().GetUserAchievements().CheckAchievement("GuideRequester");

                this.Helper.GetHabbo().GetUserStats().GuideRequestsHandled++;
                this.Helper.GetHabbo().GetUserAchievements().CheckAchievement("GuideRequestHandler");
            }
            else if (this.Status == GuideSessionStatus.WaitingForHelper)
            {
                this.offeredTo = null;
            }
        }
        
        public void Recommend(GameClient session, bool recommend)
        {
            session.SendMessage(new GuideSessionDetachedComposerHandler());

            if (this.Status == GuideSessionStatus.Ended)
            {
                if (this.Requester.GetHabbo().ID == session.GetHabbo().ID)
                {
                    this.SetStatus(GuideSessionStatus.Recommended);

                    this.Requester.GetHabbo().GetUserStats().GuideFeedbackGiver++;
                    this.Requester.GetHabbo().GetUserAchievements().CheckAchievement("GuideFeedbackGiver");

                    if (recommend)
                    {
                        this.Helper.GetHabbo().GetUserStats().GuideRecommendations++;
                        this.Helper.GetHabbo().GetUserAchievements().CheckAchievement("GuideRecommendation");
                    }
                }
            }
        }

        public void SetTyping(GameClient session, bool typing)
        {
            if (this.Status == GuideSessionStatus.OnGoing)
            {
                if (this.Requester.GetHabbo().ID == session.GetHabbo().ID)
                {
                    this.Helper.SendMessage(new GuideSessionPartnerTypingComposerHandler(typing));
                }
                else
                {
                    this.Requester.SendMessage(new GuideSessionPartnerTypingComposerHandler(typing));
                }
            }
        }

        public void SendMessage(uint userId, string message)
        {
            if (this.Status == GuideSessionStatus.OnGoing)
            {
                this.SendToBoth(new GuideSessionReceiveMessageComposerHandler(userId, message));
            }
        }
    }
}
