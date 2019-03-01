using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Managers;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.r63a;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class BasicUtilies
    {
        public static PacketManager GetRevisionPacketManager(Revision revision)
        {
            switch(revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                    return new r63aPacketManager();
                default:
                    return null;
            }
        }

        public static ClientMessage GetRevisionClientMessage(Revision revision)
        {
            switch(revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                    return new r63aClientMessage();
                default:
                    return null;
            }
        }

        public static ServerMessage GetRevisionServerMessage(Revision revision)
        {
            switch(revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                    return new r63aServerMessage();
                default:
                    return null;
            }
        }
    }
}
