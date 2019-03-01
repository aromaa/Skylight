using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class SetDisableRoomUnitsEventHandler : IncomingPacket
    {
        protected bool Block;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().GetUserSettings().BlockRoomInvites = this.Block;
        }
    }
}
