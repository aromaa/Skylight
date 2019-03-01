using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;
using SkylightEmulator.Core;
using SkylightEmulator.Messages.MultiRevision;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class WaveMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null)
                {
                    RoomUnitUser user = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                    if (user != null)
                    {
                        user.Unidle();
                        user.AddStatus("wave", "", 5);

                        room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.Wave, new ValueHolder("VirtualID", user.VirtualID)));
                    }
                }
            }
        }
    }
}
