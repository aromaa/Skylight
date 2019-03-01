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
        public readonly uint ID;
        public readonly uint ToID;
        public readonly uint FromID;
        public readonly string FromUsername;

        public MessengerRequest(uint id, uint to, uint from)
        {
            this.ID = id;
            this.ToID = to;
            this.FromID = from;
            this.FromUsername = Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.FromID);
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendUInt(this.ID);
            message.AppendStringWithBreak(this.FromUsername);
            message.AppendStringWithBreak(this.FromID.ToString());
        }
    }
}
