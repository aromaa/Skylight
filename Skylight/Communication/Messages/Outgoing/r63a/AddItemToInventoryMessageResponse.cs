using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class AddItemToInventoryMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            InventoryItem item = valueHolder.GetValue<InventoryItem>("Item");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.AddItemToHand);
            item.Serialize(Message);
            return Message;
        }
    }
}
