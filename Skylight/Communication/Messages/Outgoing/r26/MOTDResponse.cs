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
    class MOTDResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null) //r26 dosent have 
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.SendNotifFromAdmin);
            message.AppendString(string.Join("\r\n", valueHolder.GetValue<List<string>>("Message")));
            return message;
        }
    }
}
