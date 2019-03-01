using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.HabboHotel.Talent;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class RequestTalentTrackEventHandler : IncomingPacket
    {
        protected string Name;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                TalentTrack track = Skylight.GetGame().GetTalentManager().GetTalentTrack(this.Name);

                if (track != null)
                {
                    session.SendMessage(new SendTalentTrackComposerHandler(track, session.GetHabbo()));
                }
            }
        }
    }
}
