using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Exceptions
{
    public class RoomOverloadException : Exception
    {
        private uint RoomID;
        private string Reason;
        private object Because;

        public RoomOverloadException(uint roomId, string reason, object because)
        {
            this.RoomID = roomId;
            this.Reason = reason;
            this.Because = because;
        }

        public override string Message
        {
            get
            {
                return "Room cycle took over limited time! Room unloaded! Room ID: " + this.RoomID + ", Reason: " + this.Reason + ", Type: " + this.Because.GetType();
            }
        }
    }
}
