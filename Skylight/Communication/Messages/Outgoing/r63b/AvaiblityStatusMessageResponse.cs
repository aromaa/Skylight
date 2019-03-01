using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class AvaiblityStatusMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage AvaiblityStatus = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            AvaiblityStatus.Init(r63bOutgoing.AvaiblityStatus);
            AvaiblityStatus.AppendBoolean(true); //is open, unused
            AvaiblityStatus.AppendBoolean(false); //trading disabled
            return AvaiblityStatus;
        }
    }
}
