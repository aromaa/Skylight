using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Data.Enums
{
    public enum MessengerSendPrivateMessageErrorCode
    {
        ReceiverMuted = 3,
        SenderMuted = 4,
        Offline = 5,
        NotFriend = 6,
        Busy = 7,
        ReceivedHasNoChat = 8,
        SenderHasNoChat = 9,
        OfflineMessageFailed = 10,
    }
}
