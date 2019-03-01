using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetRoomDimmerPresetsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                RoomItem dimmer = room.RoomItemManager.GetRoomDimmer();
                if (dimmer != null)
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.RoomDimmerPresets);
                    message_.AppendInt32(dimmer.MoodlightData.Presets.Count);
                    message_.AppendInt32(dimmer.MoodlightData.CurrentPreset);

                    int i = 0;
                    foreach(MoodlightPreset preset in dimmer.MoodlightData.Presets)
                    {
                        i++;
                        message_.AppendInt32(i);
                        message_.AppendInt32(preset.BackgroundOnly ? 2 : 1);
                        message_.AppendString(preset.ColorCode);
                        message_.AppendInt32(preset.ColorIntensity);
                    }

                    session.SendMessage(message_);
                }
            }
        }
    }
}
