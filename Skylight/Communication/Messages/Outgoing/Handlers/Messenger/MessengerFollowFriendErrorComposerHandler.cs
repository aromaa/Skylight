using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerFollowFriendErrorComposerHandler : OutgoingHandler
    {
        public MessengerFollowFriendErrorCode ErrorCode { get; }

        public MessengerFollowFriendErrorComposerHandler(MessengerFollowFriendErrorCode errorCode)
        {
            this.ErrorCode = errorCode;
        }
    }
}
