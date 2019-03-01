using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class SetSwimsuitMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().GetRoomSession().GetRoomUser().ChangingSwimsuit)
            {
                RoomModelTrigger trigger = session.GetHabbo().GetRoomSession().GetRoom().RoomGamemapManager.Model.Triggers[session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y];
                
                session.GetHabbo().GetRoomSession().GetRoomUser().ChangingSwimsuit = false;
                session.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType &= ~RestrictMovementType.Client;
                if (trigger != null)
                {
                    session.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(int.Parse(trigger.Data[1]), int.Parse(trigger.Data[2]));

                    ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message2.Init(r26Outgoing.SpecialCast);
                    message2.AppendString(trigger.Data[0]);
                    message2.AppendString("open");
                    session.GetHabbo().GetRoomSession().GetRoom().SendToAll(message2);
                }

                session.GetHabbo().GetRoomSession().GetRoomUser().Swimsuit = message.ReadBytesAsString(message.GetRemainingLength());

                session.GetHabbo().GetRoomSession().GetRoom().SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.SetRoomUser, new ValueHolder("RoomUser", new List<RoomUnit>() { session.GetHabbo().GetRoomSession().GetRoomUser() })));
            }
        }
    }
}
