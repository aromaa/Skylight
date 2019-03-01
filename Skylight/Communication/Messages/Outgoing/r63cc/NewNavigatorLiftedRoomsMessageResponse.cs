using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class NewNavigatorLiftedRoomsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            message.Init(r63ccOutgoing.NewNavigatorLiftedRooms);
            message.AppendInt32(1); //count

            message.AppendInt32(0); //flat id
            message.AppendInt32(0); //idk
            message.AppendString("IMAGEXCDDD"); //image
            message.AppendString("LAM"); //caption
            return message;
        }
    }
}
