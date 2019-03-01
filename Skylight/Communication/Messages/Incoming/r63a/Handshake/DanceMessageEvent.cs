using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class DanceMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                RoomUnitUser roomUser = session.GetHabbo().GetRoomSession().GetRoomUser();
                if (roomUser != null)
                {
                    roomUser.Unidle();

                    int danceId = message.PopWiredInt32();
                    if (danceId < 0 || danceId > 4)
                    {
                        danceId = 0;
                    }

                    if (danceId > 1 && !session.GetHabbo().IsHcOrVIP())
                    {
                        danceId = 0;
                    }

                    roomUser.SetHanditem(0);
                    roomUser.SetDance(danceId);
                }
            }
        }
    }
}
