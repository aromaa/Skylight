using SkylightEmulator.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerFriendsComposerHandler : OutgoingHandler
    {
        public ICollection<MessengerFriend> Friends { get; }

        public MessengerFriendsComposerHandler(ICollection<MessengerFriend> friends)
        {
            this.Friends = friends;
        }
    }
}
