using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class PrivateMessage
    {
        public uint SenderID;
        public string SenderUsername;
        public uint ReceiverID;
        public string ReceiverUsername;
        public double Timestamp;
        public string Message;
        public int SenderSessionID;
        public int ReceiverSessionID;
        public string ExtraData;

        public PrivateMessage(uint userId, string username, uint receiverId, string receiverUsername, double timestamp, string message, int senderSessionId, int receiverSessionId, string extraData = "")
        {
            this.SenderID = userId;
            this.SenderUsername = username;
            this.ReceiverID = receiverId;
            this.ReceiverUsername = receiverUsername;
            this.Timestamp = timestamp;
            this.Message = message;
            this.SenderSessionID = senderSessionId;
            this.ReceiverSessionID = receiverSessionId;
            this.ExtraData = extraData;
        }

        public DateTime GetDate()
        {
            return TimeUtilies.UnixTimestampToDateTime(this.Timestamp);
        }
    }
}
