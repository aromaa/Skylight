using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetUserDiscsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            List<InventoryItem> discs = session.GetHabbo().GetInventoryManager().GetFloodItemsByInteractionType("musicdisk");

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.UserDiscs);
            message_.AppendInt32(discs.Count);
            foreach(InventoryItem disc in discs)
            {
                message_.AppendUInt(disc.ID);
                int songId;
                if (!int.TryParse(disc.ExtraData, out songId))
                {
                    songId = 0;
                }
                message_.AppendInt32(songId);
            }
            session.SendMessage(message_);
        }
    }
}
