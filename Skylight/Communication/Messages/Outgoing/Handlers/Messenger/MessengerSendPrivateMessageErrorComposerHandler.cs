using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerSendPrivateMessageErrorComposerHandler : OutgoingHandler
    {
        public readonly uint UserID;
        public readonly MessengerSendPrivateMessageErrorCode ErrorCode;

        public MessengerSendPrivateMessageErrorComposerHandler(uint userId, MessengerSendPrivateMessageErrorCode errorCode)
        {
            this.UserID = userId;
            this.ErrorCode = errorCode;
        }
    }
}
