using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class GetUserTagsEventHandler : IncomingPacket
    {
        protected uint UserID;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(new SendUserTagsComposerHandler(this.UserID, session?.GetHabbo()?.Tags));
        }
    }
}
