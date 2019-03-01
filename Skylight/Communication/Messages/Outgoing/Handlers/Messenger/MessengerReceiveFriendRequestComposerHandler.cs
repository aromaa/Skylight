using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerReceiveFriendRequestComposerHandler : OutgoingHandler
    {
        public uint UserID { get; }
        public string Username { get; }
        public string Look { get; }

        public MessengerReceiveFriendRequestComposerHandler(uint userId, string username, string look)
        {
            this.UserID = userId;
            this.Username = username;
            this.Look = look;
        }
    }
}
