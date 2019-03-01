using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class ShoutMessageEvent : IncomingPacket
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

                    roomUser.Speak(TextUtilies.FilterString(message_), true, message.PopWiredInt32());
                }
            }
        }
    }
}
