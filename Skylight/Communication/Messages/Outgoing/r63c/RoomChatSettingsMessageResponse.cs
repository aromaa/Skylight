using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RoomChatSettingsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            roomData.Init(r63cOutgoing.RoomChatSettings);
            roomData.AppendInt32(valueHolder.GetValue<int>("ChatMod"));
            roomData.AppendInt32(valueHolder.GetValue<int>("ChatWeight"));
            roomData.AppendInt32(valueHolder.GetValue<int>("ChatSpeed"));
            roomData.AppendInt32(valueHolder.GetValue<int>("ChatDistance"));
            roomData.AppendInt32(valueHolder.GetValue<int>("ChatProtection"));
            return roomData;
        }
    }
}
