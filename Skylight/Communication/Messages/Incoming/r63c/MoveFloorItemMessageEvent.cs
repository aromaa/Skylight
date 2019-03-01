using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MoveFloorItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = session.GetHabbo().GetRoomSession().GetRoom();
                if (room != null)
                {
                    RoomItem item = room.RoomItemManager.TryGetFloorItem(message.PopWiredUInt());
                    if (room.GaveRoomRights(session))
                    {
                        if (item != null)
                        {
                            int x = message.PopWiredInt32();
                            int y = message.PopWiredInt32();
                            int rot = message.PopWiredInt32();

                            if (room.RoomItemManager.MoveFloorItemOnRoom(session, item, x, y, rot))
                            {
                                return;
                            }
                        }
                    }

                    //ServerMessage msg = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                    //msg.Init(2419);
                    //msg.AppendString("furni_placement_error");
                    //msg.AppendInt32(1);
                    //msg.AppendString("message");
                    //msg.AppendString("${room.error.cant_set_item}");
                    //session.SendMessage(msg);
                    session.SendMessage(BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.UpdateFloorItem).Handle(new ValueHolder("Item", item)));
                }
            }
        }
    }
}
