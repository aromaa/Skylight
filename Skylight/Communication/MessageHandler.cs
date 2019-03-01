using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication
{
    public abstract class MessageHandler
    {
        public abstract Guid Identifier();
        public abstract bool HandleMessage(GameClient session, ClientMessage message); //return true if we shall continue, false if we should stop
    }
}
