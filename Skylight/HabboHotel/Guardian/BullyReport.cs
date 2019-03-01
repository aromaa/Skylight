using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using SkylightEmulator.HabboHotel.Guide;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;

namespace SkylightEmulator.HabboHotel.Guardian
{
    /// <summary>
    /// Thread safe
    /// </summary>
    public class BullyReport : GuideRequest
    {
        public const int VoteTime = 120;
        private const int MinVotes = 5;
        private const int MaxVotes = 10;
        private const int MuteTime = 1800;

        public ConcurrentDictionary<uint, double> Guardians { get; }
        public ConcurrentDictionary<uint, GuardianVote> GuardianVotes { get; }

        public HashSet<uint> DeclinedUsers { get; }

        public uint ReporterID { get; }
        public uint ReportedID { get; }
        public uint RoomID { get; }
        public List<RoomMessage> Chatlog { get; }
        public double Timestamp { get; }
        public int Time => (int)(TimeUtilies.GetUnixTimestamp() - this.Timestamp);

        public bool FindingGuardians => this.GuardianVotes.Count < BullyReport.MaxVotes;

        public BullyReport(uint reporterId, uint reportedId, uint roomId, List<RoomMessage> chatlog)
        {
            this.Guardians = new ConcurrentDictionary<uint, double>();
            this.GuardianVotes = new ConcurrentDictionary<uint, GuardianVote>();

            this.DeclinedUsers = new HashSet<uint>();

            this.ReporterID = reporterId;
            this.ReportedID = reportedId;
            this.RoomID = roomId;
            this.Chatlog = chatlog;
            this.Timestamp = TimeUtilies.GetUnixTimestamp();
        }

        public void OfferTo(GameClient session)
        {
            this.Guardians.TryAdd(session.GetHabbo().ID, TimeUtilies.GetUnixTimestamp());

            session.SendMessage(new NewBullyReportComposerHandler(GuideManager.BullyReportOfferTime));
        }

        public void Accept(GameClient session)
        {
            if (this.GuardianVotes.TryAdd(session.GetHabbo().ID, GuardianVote.Waiting))
            {
                this.Guardians[session.GetHabbo().ID] = TimeUtilies.GetUnixTimestamp() + BullyReport.VoteTime;

                session.SendMessage(new BullyReportAttachedComposerHandler(this));
            }

            this.UpdateVotes();
        }

        public void Decline(GameClient session)
        {
            this.DeclinedUsers.Add(session.GetHabbo().ID);

            this.Guardians.TryRemove(session.GetHabbo().ID, out double trash);

            session.SendMessage(new BullyReportDetachedComposerHandler());

            this.UpdateVotes();
        }

        public void Skip(GameClient session)
        {
            this.DeclinedUsers.Add(session.GetHabbo().ID);

            if (this.Guardians.TryRemove(session.GetHabbo().ID, out double trash))
            {
                this.GuardianVotes.TryUpdate(session.GetHabbo().ID, GuardianVote.Skip, GuardianVote.Waiting);

                this.UpdateVotes();
            }
        }

        public void Vote(GameClient session, GuardianVote type)
        {
            if (this.GuardianVotes.TryUpdate(session.GetHabbo().ID, type, GuardianVote.Waiting))
            {
                this.Guardians[session.GetHabbo().ID] = double.MaxValue;

                this.UpdateVotes();
            }
        }

        public bool TryFinish()
        {
            if (this.Time >= BullyReport.VoteTime)
            {
                if (this.GuardianVotes.Count >= BullyReport.MinVotes)
                {
                    GuardianVote verdict = this.VerdictCalc();
                    GuardianVote verdictDisplay = GuardianVote.Forwarded;
                    
                    if (verdict == GuardianVote.Acceptably)
                    {
                        verdictDisplay = GuardianVote.Acceptably;

                        Skylight.GetGame().GetGuideManager().ActivateBullyReportAbuseTimeout(this.ReporterID);
                    }
                    else if (verdict == GuardianVote.Badly)
                    {
                        verdictDisplay = GuardianVote.Badly;

                        double expires = TimeUtilies.GetUnixTimestamp() + BullyReport.MuteTime;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("muteExpires", expires);
                            dbClient.AddParamWithValue("targetId", this.ReportedID);
                            dbClient.ExecuteQuery("UPDATE users SET mute_expires = @muteExpires WHERE id = @targetId LIMIT 1");
                        }

                        GameClient session = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ReportedID);
                        if (session != null)
                        {
                            session.GetHabbo().MuteExpires = expires;
                            session.SendMessage(OutgoingPacketsEnum.Flood, new ValueHolder("TimeLeft", session.GetHabbo().MuteTimeLeft()));
                        }
                    }
                    else
                    {
                        verdictDisplay = GuardianVote.Forwarded;

                        //send to mod
                    }

                    foreach (KeyValuePair<uint, GuardianVote> vote in this.GuardianVotes)
                    {
                        if (this.Guardians.ContainsKey(vote.Key))
                        {
                            GameClient session = Skylight.GetGame().GetGameClientManager().GetGameClientById(vote.Key);

                            if (session != null)
                            {
                                (List<GuardianVote> votes, GuardianVote myVote) = this.GetVotes(session);

                                session.SendMessage(new BullyReportResultsComposerHandler(verdictDisplay, myVote, votes));
                            }
                        }
                    }

                    Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ReporterID)?.SendMessage(new BullyReportCloseComposerHandler(verdict != GuardianVote.Acceptably));
                }
                else
                {
                    //send to mod

                    foreach (KeyValuePair<uint, GuardianVote> vote in this.GuardianVotes)
                    {
                        if (this.Guardians.ContainsKey(vote.Key))
                        {
                            GameClient session = Skylight.GetGame().GetGameClientManager().GetGameClientById(vote.Key);

                            if (session != null)
                            {
                                (List<GuardianVote> votes, GuardianVote myVote) = this.GetVotes(session);

                                session.SendMessage(new BullyReportResultsComposerHandler(GuardianVote.Forwarded, myVote, votes));
                            }
                        }
                    }

                    Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ReporterID)?.SendMessage(new BullyReportCloseComposerHandler(true));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public GuardianVote VerdictCalc()
        {
            List<IGrouping<GuardianVote, KeyValuePair<uint, GuardianVote>>> votes = this.GuardianVotes.GroupBy(k => k.Value).OrderBy(k => k.Count()).ToList();
            if (votes[0].Count() != votes[1].Count())
            {
                return votes[0].Key;
            }
            else
            {
                return GuardianVote.Forwarded;
            }
        }

        public void UpdateVotes()
        {
            foreach(KeyValuePair<uint, GuardianVote> vote in this.GuardianVotes)
            {
                if (this.Guardians.ContainsKey(vote.Key))
                {
                    GameClient session = Skylight.GetGame().GetGameClientManager().GetGameClientById(vote.Key);

                    if (session != null)
                    {
                        session.SendMessage(new BullyVoteResultsComposerHandler(this.GetVotes(session).Votes));
                    }
                }
            }
        }

        public (List<GuardianVote> Votes, GuardianVote MyVote) GetVotes(GameClient sesison)
        {
            List<GuardianVote> votes = new List<GuardianVote>();
            GuardianVote myVote = GuardianVote.Waiting;

            IEnumerator<KeyValuePair<uint, GuardianVote>> votes_  = this.GuardianVotes.GetEnumerator();
            for(int i = 0; i < Math.Max(BullyReport.MinVotes, this.GuardianVotes.Count); i++)
            {
                if (votes_.MoveNext())
                {
                    if (votes_.Current.Key != sesison.GetHabbo().ID)
                    {
                        votes.Add(votes_.Current.Value);
                    }
                    else
                    {
                        myVote = votes_.Current.Value;
                    }
                }
                else
                {
                    votes.Add(GuardianVote.Searching);
                }
            }

            return (votes, myVote);
        }

        public void Close(GameClient session)
        {
            this.Guardians.TryRemove(session.GetHabbo().ID, out double trash);
            if (this.GuardianVotes.TryGetValue(session.GetHabbo().ID, out GuardianVote vote))
            {
                if (vote == GuardianVote.Waiting)
                {
                    this.GuardianVotes.TryRemove(session.GetHabbo().ID, out GuardianVote trash_);
                }
            }
        }
    }
}
