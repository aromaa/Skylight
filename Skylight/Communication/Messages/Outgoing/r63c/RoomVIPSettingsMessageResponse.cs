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
    class RoomVIPSettingsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomVIPsetting = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            roomVIPsetting.Init(r63cOutgoing.RoomVIPSettings);
            roomVIPsetting.AppendBoolean(valueHolder.GetValue<bool>("Hidewalls"));
            roomVIPsetting.AppendInt32(valueHolder.GetValue<int>("Wallthick"));
            roomVIPsetting.AppendInt32(valueHolder.GetValue<int>("Floorthick"));
            return roomVIPsetting;
        }
    }
}
