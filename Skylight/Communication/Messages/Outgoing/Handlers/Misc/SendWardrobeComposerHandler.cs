using SkylightEmulator.HabboHotel.Users.Wardrobe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendWardrobeComposerHandler : OutgoingHandler
    {
        public ICollection<WardrobeSlot> Slots { get; }

        public SendWardrobeComposerHandler(ICollection<WardrobeSlot> slots)
        {
            this.Slots = slots;
        }
    }
}
