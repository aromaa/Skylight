using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class RoomMessage
    {
        public uint UserID;
        public string Username;
        public uint RoomID;
        public double Timestamp;
        public string Message;
        public int UserSessionID;

        public RoomMessage(uint userId, string username, uint roomId, double timestamp, string message, int userSessionId)
        {
            this.UserID = userId;
            this.Username = username;
            this.RoomID = roomId;
            this.Timestamp = timestamp;
            this.Message = message;
            this.UserSessionID = userSessionId;
        }

        public DateTime GetDate()
        {
            return TimeUtilies.UnixTimestampToDateTime(this.Timestamp);
        }
    }
}
