using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class AvaiblityStatusMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage AvaiblityStatus = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            AvaiblityStatus.Init(r63aOutgoing.AvaiblityStatus);
            AvaiblityStatus.AppendInt32(1); //is open, unused
            AvaiblityStatus.AppendInt32(0); //trading disabled
            return AvaiblityStatus;
        }
    }
}
