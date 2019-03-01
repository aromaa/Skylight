using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class AvaiblityStatusMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage AvaiblityStatus = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            AvaiblityStatus.Init(r63cOutgoing.AvaiblityStatus);
            AvaiblityStatus.AppendBoolean(true); //is open, unused
            AvaiblityStatus.AppendBoolean(false); //trading disabled
            return AvaiblityStatus;
        }
    }
}
