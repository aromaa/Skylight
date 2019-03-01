using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SaveWarbodeMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                int slot = message.PopWiredInt32();
                if (slot > 0)
                {
                    if ((slot <= 5 && session.GetHabbo().IsHcOrVIP()) || (slot <= 10 && session.GetHabbo().IsVIP())) //hc can save up to 5 slots and vip up to 10 slots
                    {
                        string look = message.PopFixedString();
                        string gender = message.PopFixedString();

                        session.GetHabbo().GetWardrobeManager().UpdateSlot(slot, gender, look);
                    }
                }
            }
        }
    }
}
