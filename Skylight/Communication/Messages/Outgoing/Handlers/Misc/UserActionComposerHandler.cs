using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class UserActionComposerHandler : OutgoingHandler
    {
        public uint UserID { get; }
        public ActionType Type { get; }

        public UserActionComposerHandler(uint userId, ActionType type)
        {
            this.UserID = userId;
            this.Type = type;
        }
    }
}
