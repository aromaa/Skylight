using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Wardrobe;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetWarbodeMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.Warbode);
                message_.AppendInt32(0); //state, looks like its not used

                ICollection<WardrobeSlot> wardrobeItems = session.GetHabbo().GetWardrobeManager().GetWardrobeItems();
                message_.AppendInt32(wardrobeItems.Count); //count
                foreach(WardrobeSlot slot in wardrobeItems)
                {
                    message_.AppendInt32(slot.SlotID);
                    message_.AppendString(slot.Look);
                    message_.AppendString(slot.Gender);
                }
                session.SendMessage(message_);
            }
        }
    }
}
