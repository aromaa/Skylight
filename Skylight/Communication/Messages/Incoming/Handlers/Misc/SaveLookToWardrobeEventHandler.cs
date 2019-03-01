using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class SaveLookToWardrobeEventHandler : IncomingPacket
    {
        protected int SlotID;
        protected string Look;
        protected string Gender;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                if (this.SlotID > 0 && this.SlotID <= 10)
                {
                    if ((this.SlotID <= 5 && session.GetHabbo().IsHcOrVIP()) || (this.SlotID <= 10 && session.GetHabbo().IsVIP())) //hc can save up to 5 slots and vip up to 10 slots
                    {
                        session.GetHabbo().GetWardrobeManager().UpdateSlot(this.SlotID, this.Gender, this.Look);
                    }
                }
                else
                {
                    session.SendLeetScripter();
                }
            }
        }
    }
}
