using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Inventory;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class AddItemToInventoryMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            return valueHolder.GetValue<InventoryManager>("InventoryManager").OldSchoolGetHand("last");
        }
    }
}
