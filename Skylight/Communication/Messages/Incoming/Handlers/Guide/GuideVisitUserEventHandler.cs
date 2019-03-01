using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class GuideVisitUserEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Room room = Skylight.GetGame().GetGuideManager().GetGuideSessionByUserID(session.GetHabbo().ID)?.Requester?.GetHabbo()?.GetRoomSession()?.GetRoom();
                if (room != null)
                {
                    session.SendMessage(new GuideSendToRoomComposerHandler(room.ID));
                }
            }
        }
    }
}
