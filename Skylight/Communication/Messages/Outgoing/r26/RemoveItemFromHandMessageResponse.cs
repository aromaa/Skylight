using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Users.Inventory;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class RemoveItemFromHandMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            return valueHolder.GetValue<InventoryManager>("InventoryManager").OldSchoolGetHand("update");
        }
    }
}
