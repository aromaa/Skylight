using SkylightEmulator.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerFriendRequestsComposerHandler : OutgoingHandler
    {
        public ICollection<MessengerRequest> Requests { get; }

        public MessengerFriendRequestsComposerHandler(ICollection<MessengerRequest> requests)
        {
            this.Requests = requests;
        }
    }
}
