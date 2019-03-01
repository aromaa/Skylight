using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class UserMoveMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                RoomUnitUser user = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (user != null)
                {
                    if (user.Riding != null)
                    {
                        int x = message.PopBase64Int32();
                        int y = message.PopBase64Int32();
                        user.Riding.MoveTo(x, y);
                    }
                    else
                    {
                        if ((user.RestrictMovementType & RestrictMovementType.Client) == 0)
                        {
                            int x = message.PopBase64Int32();
                            int y = message.PopBase64Int32();
                            if (user.X != x || user.Y != y)
                            {
                                if (user.Teleport)
                                {
                                    user.StopMoving();
                                    user.SetLocation(x, y, user.Z);
                                }
                                else
                                {
                                    user.MoveTo(x, y);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
