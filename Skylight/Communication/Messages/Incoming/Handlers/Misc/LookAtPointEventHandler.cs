using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Pathfinders;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class LookAtPointEventHandler : IncomingPacket
    {
        protected int X;
        protected int Y;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            RoomUnitUser user = session?.GetHabbo()?.GetRoomSession()?.CurrentRoomRoomUser;
            if (user != null)
            {
                user.Unidle();
                    
                if (this.X != user.X || this.Y != user.Y)
                {
                    user.SetRotation(WalkRotation.Walk(user.X, user.Y, this.X, this.Y), false);
                }
            }
        }
    }
}
