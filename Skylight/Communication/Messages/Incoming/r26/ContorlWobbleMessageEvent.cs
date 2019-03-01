using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms.Games;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class ContorlWobbleMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetRoomSession()?.GetRoom()?.RoomGameManager?.RoomWobbleSquabbleManager?.Status == WSStatus.Running)
            {
                if (!session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.NeedUpdate)
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.NeedUpdate = true;

                    string action = session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Action = message.ReadBytesAsString(1);
                    switch (action)
                    {
                        case "0":
                            {
                                if (!session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.HasUsedResetLean)
                                {
                                    session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.HasUsedResetLean = true;
                                    session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Lean = 0;
                                }
                            }
                            break;
                        case "A":
                            {
                                session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Lean -= 35 + RandomUtilies.GetRandom().Next(0, 10);
                            }
                            break;
                        case "D":
                            {
                                session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Lean += 35 + RandomUtilies.GetRandom().Next(0, 10);
                            }
                            break;
                        case "S":
                            {
                                if (session.GetHabbo().GetRoomSession().GetRoom().RoomGameManager.RoomWobbleSquabbleManager.IsOpen(session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Location + 1))
                                {
                                    session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Location++;
                                }
                            }
                            break;
                        case "X":
                            {
                                if (session.GetHabbo().GetRoomSession().GetRoom().RoomGameManager.RoomWobbleSquabbleManager.IsOpen(session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Location - 1))
                                {
                                    session.GetHabbo().GetRoomSession().GetRoomUser().WSPlayer.Location--;
                                }
                            }
                            break;
                        case "E":
                            {
                                session.GetHabbo().GetRoomSession().GetRoom().RoomGameManager.RoomWobbleSquabbleManager.Hit(session.GetHabbo().GetRoomSession().GetRoomUser(), false);
                            }
                            break;
                        case "W":
                            {
                                session.GetHabbo().GetRoomSession().GetRoom().RoomGameManager.RoomWobbleSquabbleManager.Hit(session.GetHabbo().GetRoomSession().GetRoomUser(), true);
                            }
                            break;
                    }
                }
            }
        }
    }
}
