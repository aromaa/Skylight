using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Extanssions;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class SaveUserVolumeEventHandler : IncomingPacket
    {
        protected int VolumeSystem;
        protected int VolumeFurni;
        protected int VolumeTrax;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().GetUserSettings().Volume[0] = this.VolumeSystem.Limit(0, 100);
            session.GetHabbo().GetUserSettings().Volume[1] = this.VolumeFurni.Limit(0, 100);
            session.GetHabbo().GetUserSettings().Volume[2] = this.VolumeTrax.Limit(0, 100);
        }
    }
}
