using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Quests;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Quests
{
    public class GetQuestsEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(new SendQuestsComposerHandler());
        }
    }
}
