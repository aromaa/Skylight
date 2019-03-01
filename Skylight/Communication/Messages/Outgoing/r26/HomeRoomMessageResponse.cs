using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class HomeRoomMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            uint roomId = valueHolder.GetValue<uint>("HomeRoom");
            if (roomId > 0)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message.Init(r26Outgoing.RoomForward); //r26 doesnt have "home room"
                message.AppendBoolean(false); //is it public room, we assume not
                message.AppendUInt(roomId);
                return message;
            }
            else
            {
                return null;
            }
        }
    }
}
