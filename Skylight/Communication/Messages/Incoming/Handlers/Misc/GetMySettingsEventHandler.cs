using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class GetMySettingsEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                session.SendMessage(new SendMySettingsComposerHandler(session.GetHabbo().GetUserSettings().Volume[0], session.GetHabbo().GetUserSettings().Volume[1], session.GetHabbo().GetUserSettings().Volume[2], session.GetHabbo().GetUserSettings().PreferOldChat, session.GetHabbo().GetUserSettings().BlockRoomInvites, session.GetHabbo().GetUserSettings().BlockCameraFollow, session.GetHabbo().GetUserSettings().ChatColor));
            }
        }
    }
}
