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
    class DoorbellMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage doorbellUser = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            doorbellUser.Init(r63aOutgoing.Doorbell);
            doorbellUser.AppendString(valueHolder == null ? "" : valueHolder.GetValueOrDefault<string>("Username"));
            return doorbellUser;
        }
    }
}
