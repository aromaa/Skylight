using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerSendFriendRequestErrorComposerHandler : OutgoingHandler
    {
        public readonly MessengerSendFriendRequestErrorCode ErrorCode;

        public MessengerSendFriendRequestErrorComposerHandler(MessengerSendFriendRequestErrorCode errorCode)
        {
            this.ErrorCode = errorCode;
        }
    }
}
