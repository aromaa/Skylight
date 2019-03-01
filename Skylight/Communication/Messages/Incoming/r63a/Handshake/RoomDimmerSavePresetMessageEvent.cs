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
    class RoomDimmerSavePresetMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                RoomItem dimmer = room.RoomItemManager.GetRoomDimmer();
                if (dimmer != null)
                {
                    int currentPreset = message.PopWiredInt32();
                    bool backgroundOnly = message.PopWiredInt32() == 2;
                    string colorCode = message.PopFixedString();
                    int colorIntensity = message.PopWiredInt32();
                    bool applyToRoom = message.PopWiredBoolean();
                    dimmer.MoodlightData.SetCurrentPresetSettings(currentPreset, backgroundOnly, colorCode, colorIntensity);

                    if (applyToRoom)
                    {
                        dimmer.MoodlightData.Enable();
                        dimmer.MoodlightData.CurrentPreset = currentPreset;
                        dimmer.ExtraData = dimmer.MoodlightData.GenerateExtraData();
                        dimmer.UpdateState(true, true);
                    }
                }
            }
        }
    }
}
