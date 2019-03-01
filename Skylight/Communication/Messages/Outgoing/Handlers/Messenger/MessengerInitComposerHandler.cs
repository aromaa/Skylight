using SkylightEmulator.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerInitComposerHandler : OutgoingHandler
    {
        public int Limit { get; }
        public int HCLimit { get; }
        public int VIPLimit { get; }
        public ICollection<MessengerCategory> Categorys { get; }

        public MessengerInitComposerHandler(int limit, int hcLimit, int vipLimit, ICollection<MessengerCategory> categorys)
        {
            this.Limit = limit;
            this.HCLimit = hcLimit;
            this.VIPLimit = vipLimit;
            this.Categorys = categorys;
        }
    }
}
