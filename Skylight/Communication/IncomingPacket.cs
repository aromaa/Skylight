using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public interface IncomingPacket
    {
        void Handle(GameClient session, ClientMessage message);
    }
}
