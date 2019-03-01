using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class RoomSearchMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomData> rooms = valueHolder.GetValue<List<RoomData>>("Rooms");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            if (rooms.Count > 0)
            {
                message.Init(r26Outgoing.RoomSearchResults);

                foreach(RoomData data in rooms)
                {
                    message.AppendString(data.ID.ToString(), 9);
                    message.AppendString(data.Name, 9);
                    message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(data.OwnerID), 9);
                    message.AppendString("locked", 9);
                    message.AppendString("x", 9);
                    message.AppendString(data.UsersNow.ToString(), 9);
                    message.AppendString(data.UsersMax.ToString(), 9);
                    message.AppendString("null", 9);
                    message.AppendString(data.Description, 9);
                    message.AppendString("", 13);
                }
            }
            else
            {
                message.Init(r26Outgoing.RoomSearchNoResults);
            }
            return message;
        }
    }
}
