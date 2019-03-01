using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class ActionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int action = message.PopWiredInt32();

            session.GetHabbo().GetRoomSession().GetRoomUser().Unidle();

            if (action == 5)
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().SetIdleStatus(true);
            }
            else
            {
                session.GetHabbo().GetRoomSession().GetRoom().SendToAll(OutgoingPacketsEnum.Action, new ValueHolder("VirtualID", session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID, "Action", action));
            }
        }
    }
}
