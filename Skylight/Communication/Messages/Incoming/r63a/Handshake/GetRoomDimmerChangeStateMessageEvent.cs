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
    class GetRoomDimmerChangeStateMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                RoomItem dimmer = room.RoomItemManager.GetRoomDimmer();
                if (dimmer != null)
                {
                    if (dimmer.MoodlightData.Enabled)
                    {
                        dimmer.MoodlightData.Disable();
                    }
                    else
                    {
                        dimmer.MoodlightData.Enable();
                    }

                    dimmer.ExtraData = dimmer.MoodlightData.GenerateExtraData();
                    dimmer.UpdateState(true, true);
                }
            }
        }
    }
}
