using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class SayMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                RoomUnit roomUser = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (roomUser != null)
                {
                    string message_ = message.PopFixedString();
                    if (message_.Length > 300)
                    {
                        message_ = message_.Substring(0, 300);
                    }

                    roomUser.Speak(TextUtilies.FilterString(message_), false);
                    roomUser.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.TypingStatus, new ValueHolder().AddValue("VirtualID", roomUser.VirtualID).AddValue("Typing", false)));
                }
            }
        }
    }
}
