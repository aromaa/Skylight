using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorActionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            message.PopWiredInt32(); //allways 1
            int type = message.PopWiredInt32(); //0 = just cauction, 1 = cauction + kick everyone, 2 = not used, 3 = message, 4 = message + kick everyone
            string message_ = message.PopFixedString();

            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
            if (room != null)
            {
                foreach (RoomUnitUser user in room.RoomUserManager.GetRealUsers())
                {
                    user.Session.SendNotif(message_);

                    if (type == 0 || type == 1) //give cauction
                    {
                        if (user.Session.GetHabbo().Rank < session.GetHabbo().Rank)
                        {
                            Skylight.GetGame().GetCautionManager().GiveCauction(session, user.Session, message_);
                        }
                    }
                }
            }
        }
    }
}
