using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
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
