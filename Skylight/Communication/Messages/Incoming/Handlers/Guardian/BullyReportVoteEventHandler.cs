using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guardian
{
    public class BullyReportVoteEventHandler : IncomingPacket
    {
        protected GuardianVote Vote;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Skylight.GetGame().GetGuideManager().Vote(session, this.Vote);
            }
        }
    }
}
