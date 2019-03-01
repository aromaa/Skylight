using SkylightEmulator.Communication.Messages.Incoming.Handlers.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class BullyReportVoteEvent : BullyReportVoteEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            int vote = message.PopWiredInt32();

            switch(vote)
            {
                case 0:
                    this.Vote = GuardianVote.Acceptably;
                    break;
                case 1:
                    this.Vote = GuardianVote.Badly;
                    break;
                case 2:
                    this.Vote = GuardianVote.Awfully;
                    break;
                default:
                    this.Vote = GuardianVote.Waiting;
                    break;
            }

            base.Handle(session, message);
        }
    }
}
