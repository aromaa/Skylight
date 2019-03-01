using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerReceivePrivateMessageComposerHandler : OutgoingHandler
    {
        public readonly uint UserID;
        public readonly string Username;
        public readonly double SendedOn;

        public readonly string SenderUsername;
        public readonly string SenderLook;
        public readonly uint SenderID;

        public MessengerReceivePrivateMessageComposerHandler(uint userId, string username, double sendedOn = 0, string senderUsername = null, string senderLook = null, uint senderId = 0)
        {
            this.UserID = userId;
            this.Username = username;
            this.SendedOn = sendedOn;

            this.SenderUsername = senderUsername;
            this.SenderLook = senderLook;
            this.SenderID = senderId;
        }

        public bool IsGroupMessage
        {
            get
            {
                return this.SenderUsername != null && this.SenderLook != null && this.SenderID > 0;
            }
        }

        public string GroupMessageData
        {
            get
            {
                return this.SenderUsername + "/" + this.SenderLook + "/" + this.SenderID;
            }
        }
    }
}
