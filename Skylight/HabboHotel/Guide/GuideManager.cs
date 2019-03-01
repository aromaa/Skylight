using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Guardian;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using SkylightEmulator.HabboHotel.Data.Interfaces;

namespace SkylightEmulator.HabboHotel.Guide
{
    /// <summary>
    /// Thread safe
    /// </summary>
    public class GuideManager
    {
        private const int MaxTimesStored = 20;
        private const int BullyReportTimeoutTime = 60;

        private const int BullyReportAbuseTimeoutTime = 900;
        public const int BullyReportOfferTime = 15;

        private ConcurrentDictionary<uint, GuideRequestType> Guides;
        private ConcurrentDictionary<uint, GuideRequest> BusyGuides;

        private ConcurrentDictionary<uint, GuideSession> Sessions;
        private ConcurrentQueue<int> Times;

        /// <summary>
        /// Key is reporter user id
        /// Value is BullyReport class
        /// </summary>
        private ConcurrentDictionary<uint, BullyReport> BullyReports;
        private ConcurrentDictionary<uint, double> BullyReportTimeout;
        private ConcurrentDictionary<uint, double> BullyReportAbuseTimeout;

        public Task CycleTask;

        public GuideManager()
        {
            this.Guides = new ConcurrentDictionary<uint, GuideRequestType>();
            this.BusyGuides = new ConcurrentDictionary<uint, GuideRequest>();

            this.Sessions = new ConcurrentDictionary<uint, GuideSession>();
            this.Times = new ConcurrentQueue<int>();

            this.BullyReports = new ConcurrentDictionary<uint, BullyReport>();
            this.BullyReportTimeout = new ConcurrentDictionary<uint, double>();
            this.BullyReportAbuseTimeout = new ConcurrentDictionary<uint, double>();
        }
        
        public int TourersCount => this.Guides.Values.Count(t => (t & GuideRequestType.Tour) != 0);
        public int HelpersCount => this.Guides.Values.Count(t => (t & GuideRequestType.Help) != 0);
        public int GuardiansCount => this.Guides.Values.Count(t => (t & GuideRequestType.Bully) != 0);
        public int Count(GuideRequestType type) => this.Guides.Values.Count(t => (t & type) != 0);
        public IEnumerable<uint> GuidesByType(GuideRequestType type) => this.Guides.Where(v => (v.Value & type) != 0).Select(g => g.Key);

        public void SetOnDuty(GameClient session, GuideRequestType type)
        {
            session.GetHabbo().GetUserStats().GuideOnDutyPresenceCheckStart = TimeUtilies.GetUnixTimestamp();

            if (this.Sessions.TryRemove(session.GetHabbo().ID, out GuideSession guideSession))
            {
                guideSession.Close(session, true);
            }

            this.Guides[session.GetHabbo().ID] = type;

            session.SendMessage(new GuideToolComposerHandler(true, Skylight.GetGame().GetGuideManager().TourersCount, Skylight.GetGame().GetGuideManager().HelpersCount, Skylight.GetGame().GetGuideManager().GuardiansCount));
        }

        public void RemoveFromDuty(GameClient session)
        {
            session.GetHabbo().GetUserStats().UpdateGuideOnDutyPresence();
            session.GetHabbo().GetUserStats().GuideOnDutyPresenceCheckStart = 0;

            this.Guides.TryRemove(session.GetHabbo().ID, out GuideRequestType type);

            session.SendMessage(new GuideToolComposerHandler(false, Skylight.GetGame().GetGuideManager().TourersCount, Skylight.GetGame().GetGuideManager().HelpersCount, Skylight.GetGame().GetGuideManager().GuardiansCount));
        }

        public bool CreateQuestion(GameClient requester, GuideRequestType type, string message)
        {
            if (this.Guides.TryRemove(requester.GetHabbo().ID, out GuideRequestType trash))
            {
                if (this.BusyGuides.TryRemove(requester.GetHabbo().ID, out GuideRequest guideRequest))
                {
                    (guideRequest as GuideSession)?.Close(requester, false);
                }
            }

            return this.Sessions.TryAdd(requester.GetHabbo().ID, new GuideSession(requester, type, message));
        }

        public void OnCycle()
        {
            foreach(KeyValuePair<uint, GuideSession> request in this.Sessions)
            {
                if (request.Value.FindHelper)
                {
                    if (request.Value.OfferedTo != null && this.BusyGuides.TryRemove(request.Value.OfferedTo.GetHabbo().ID, out GuideRequest trash))
                    {
                        request.Value.OfferedTo.SendMessage(new GuideSessionDetachedComposerHandler());

                        this.RemoveFromDuty(request.Value.OfferedTo);
                    }

                    bool hasHelpers = false;
                    foreach(uint helper_ in this.GuidesByType(request.Value.Type))
                    {
                        if (!request.Value.RejectedUsers.Contains(helper_))
                        {
                            hasHelpers = true;
                            
                            if (this.BusyGuides.TryAdd(helper_, request.Value))
                            {
                                request.Value.OfferTo(Skylight.GetGame().GetGameClientManager().GetGameClientById(helper_));
                            }

                            break;
                        }
                    }

                    if (!hasHelpers)
                    {
                        if (this.Sessions.TryRemove(request.Key, out GuideSession guideSession))
                        {
                            guideSession.Requester.SendMessage(new GuideRequestErrorComposerHandler(GuideRequestErrorCode.Rejected));
                        }
                    }
                }
            }

            foreach(KeyValuePair<uint, BullyReport> report in this.BullyReports)
            {
                if (report.Value.TryFinish())
                {
                    this.BullyReports.TryRemove(report.Key, out BullyReport trash);
                }

                if (report.Value.FindingGuardians)
                {
                    foreach(KeyValuePair<uint, double> guardian in report.Value.Guardians)
                    {
                        if (TimeUtilies.GetUnixTimestamp() - guardian.Value >= GuideManager.BullyReportOfferTime && this.BusyGuides.TryRemove(guardian.Key, out GuideRequest trash))
                        {
                            report.Value.Guardians.TryRemove(guardian.Key, out double trash_);

                            GameClient session = Skylight.GetGame().GetGameClientManager().GetGameClientById(guardian.Key);
                            if (session != null)
                            {
                                session.SendMessage(new BullyReportDetachedComposerHandler());

                                this.RemoveFromDuty(session);
                            }
                        }
                    }

                    foreach(uint helper in this.GuidesByType(GuideRequestType.Bully))
                    {
                        if (helper != report.Value.ReportedID && helper != report.Value.ReporterID)
                        {
                            if (!report.Value.DeclinedUsers.Contains(helper) && !report.Value.GuardianVotes.ContainsKey(helper) && this.BusyGuides.TryAdd(helper, report.Value))
                            {
                                report.Value.OfferTo(Skylight.GetGame().GetGameClientManager().GetGameClientById(helper));
                            }
                        }
                    }
                }
            }

            foreach(KeyValuePair<uint, double> timeout in this.BullyReportTimeout)
            {
                if (TimeUtilies.GetUnixTimestamp() - timeout.Value >= GuideManager.BullyReportTimeoutTime)
                {
                    this.BullyReportTimeout.TryRemove(timeout.Key, out double trash);
                }
            }

            foreach (KeyValuePair<uint, double> timeout in this.BullyReportAbuseTimeout)
            {
                if (TimeUtilies.GetUnixTimestamp() - timeout.Value >= GuideManager.BullyReportAbuseTimeoutTime)
                {
                    this.BullyReportAbuseTimeout.TryRemove(timeout.Key, out double trash);
                }
            }
        }

        public int GetAvarageWaitTime()
        {
            return this.Times.Count > 0 ? (this.Times.Sum(t => t) / this.Times.Count) : 0;
        }

        public void CancelRequest(GameClient session)
        {
            if (this.Sessions.TryRemove(session.GetHabbo().ID, out GuideSession guideSession))
            {
                if (guideSession.OfferedTo != null && this.BusyGuides.TryRemove(guideSession.OfferedTo.GetHabbo().ID, out GuideRequest trash))
                {
                    guideSession.OfferedTo.SendMessage(new GuideSessionDetachedComposerHandler());
                }

                guideSession.Requester.SendMessage(new GuideSessionDetachedComposerHandler());
            }
        }

        public void Decline(GameClient session)
        {
            if (this.BusyGuides.TryRemove(session.GetHabbo().ID, out GuideRequest guideRequest))
            {
                (guideRequest as GuideSession)?.Decline(session);
            }
        }

        public void Accept(GameClient session)
        {
            if (this.BusyGuides.TryGetValue(session.GetHabbo().ID, out GuideRequest guideRequest))
            {
                if ((guideRequest as GuideSession).Accept(session, out int waitTime))
                {
                    this.Times.Enqueue(waitTime);

                    while (this.Times.Count > GuideManager.MaxTimesStored)
                    {
                        this.Times.TryDequeue(out int trash);
                    }
                }
            }
        }

        public void Recommend(GameClient session, bool recommend)
        {
            if (this.BusyGuides.TryRemove(session.GetHabbo().ID, out GuideRequest guideRequest))
            {
                (guideRequest as GuideSession).Recommend(session, recommend);
            }
            else if (this.Sessions.TryRemove(session.GetHabbo().ID, out GuideSession guideSession))
            {
                guideSession.Recommend(session, recommend);
            }
        }

        public GuideSession GetGuideSessionByUserID(uint userId)
        {
            if (this.BusyGuides.TryGetValue(userId, out GuideRequest guideRequest))
            {
                return (guideRequest as GuideSession);
            }
            else if (this.Sessions.TryGetValue(userId, out GuideSession guideSession))
            {
                return guideSession;
            }
            else
            {
                return null;
            }
        }

        public void Disconnect(GameClient session)
        {
            if (this.Guides.TryRemove(session.GetHabbo().ID, out GuideRequestType trash))
            {
                if (this.BusyGuides.TryRemove(session.GetHabbo().ID, out GuideRequest guideRequest))
                {
                    if (guideRequest is GuideSession guideSession)
                    {
                        guideSession.Close(session, true);
                    }
                    else if (guideRequest is BullyReport bullyReport)
                    {
                        bullyReport.Close(session);
                    }
                }
            }
            else if (this.Sessions.TryRemove(session.GetHabbo().ID, out GuideSession guideSession))
            {
                guideSession.Close(session, true);
            }
        }

        public BullyReport GetBullyReportByReporterID(uint userId)
        {
            this.BullyReports.TryGetValue(userId, out BullyReport report);
            return report;
        }

        public BullyReport GetBullyReportByReportedID(uint userId)
        {
            return this.BullyReports.Values.FirstOrDefault(r => r.ReportedID == userId);
        }

        public bool GetHasBullyReportTimeout(uint userId)
        {
            return this.BullyReportTimeout.ContainsKey(userId);
        }

        public bool GetHasBullyReportAbuseTimeout(uint userId)
        {
            return this.BullyReportAbuseTimeout.ContainsKey(userId);
        }

        public void ActivateBullyReportAbuseTimeout(uint userId)
        {
            this.BullyReportAbuseTimeout.TryAdd(userId, TimeUtilies.GetUnixTimestamp());
        }

        public void SendBullyReport(GameClient session, uint userID, uint roomID, List<RoomMessage> chatlog)
        {
            this.BullyReports.TryAdd(session.GetHabbo().ID, new BullyReport(session.GetHabbo().ID, userID, roomID, chatlog));
        }

        public void AcceptBully(GameClient session)
        {
            if (this.BusyGuides.TryGetValue(session.GetHabbo().ID, out GuideRequest report))
            {
                (report as BullyReport)?.Accept(session);
            }
        }

        public void DeclineBully(GameClient session)
        {
            if (this.BusyGuides.TryRemove(session.GetHabbo().ID, out GuideRequest report))
            {
                (report as BullyReport)?.Decline(session);
            }
        }

        public void SkipBullyReport(GameClient session)
        {
            if (this.BusyGuides.TryRemove(session.GetHabbo().ID, out GuideRequest report))
            {
                (report as BullyReport)?.Skip(session);
            }
        }

        public void Vote(GameClient session, GuardianVote type)
        {
            if (this.BusyGuides.TryGetValue(session.GetHabbo().ID, out GuideRequest report))
            {
                (report as BullyReport)?.Vote(session, type);
            }
        }
    }
}
