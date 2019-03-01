using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Navigator;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetPublicRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.PublicRooms);

            Dictionary<int, PublicItem>.ValueCollection items = Skylight.GetGame().GetNavigatorManager().GetPublicRoomItems();
            message_.AppendInt32(items.Count);
            foreach(PublicItem item in items)
            {
                if (item.Type == PublicItemType.CATEGORY)
                {
                    item.Serialize(message_);
                    foreach (PublicItem item_ in items)
                    {
                        if (item_.ParentCategoryID == item.ID)
                        {
                            item_.Serialize(message_);
                        }
                    }
                }
                else
                {
                    if (item.ParentCategoryID == 0 || item.ParentCategoryID == -1)
                    {
                        item.Serialize(message_);
                    }
                }
            }
            message_.AppendInt32(0);
            message_.AppendInt32(0);
            session.SendMessage(message_);
        }
    }
}
