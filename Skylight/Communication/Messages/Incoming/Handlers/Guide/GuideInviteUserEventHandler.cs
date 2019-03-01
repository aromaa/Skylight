using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Guide;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class GuideInviteUserEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                Room room = Skylight.GetGame().GetGuideManager().GetGuideSessionByUserID(session.GetHabbo().ID)?.Helper?.GetHabbo()?.GetRoomSession()?.GetRoom();
                if (room != null)
                {
                    Skylight.GetGame().GetGuideManager().GetGuideSessionByUserID(session.GetHabbo().ID)?.Requester.SendMessage(new GuideSendInviteComposerHandler(room.ID, room.RoomData.Name));
                }
            }
        }
    }
}
