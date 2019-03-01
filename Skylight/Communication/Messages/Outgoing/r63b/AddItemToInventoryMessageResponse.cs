using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Inventory;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class AddItemToInventoryMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63bOutgoing.AddItemToHand);
            valueHolder.GetValue<InventoryItem>("Item").Serialize(message);
            return message;
        }
    }
}
