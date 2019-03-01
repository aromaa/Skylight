using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Messenger
{
    public class MessengerRequest
    {
        public readonly uint ToID;
        public readonly uint FromID;
        public readonly string FromUsername;
        public readonly string FromLook;

        public MessengerRequest(uint toId, uint fromId, string fromUsername, string fromLook)
        {
            this.ToID = toId;
            this.FromID = fromId;
            this.FromUsername = fromUsername;
            this.FromLook = fromLook;
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.FromID);
            message.AppendString(this.FromUsername);
            message.AppendString(this.FromLook);
        }
    }
}
