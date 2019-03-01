using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class FavouriteRoomsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<uint> rooms = valueHolder.GetValue<List<uint>>("FavouriteRooms");

            ServerMessage FavouriteRooms = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            FavouriteRooms.Init(r63aOutgoing.FavouriteRooms);
            FavouriteRooms.AppendInt32(valueHolder.GetValue<int>("Max"));
            FavouriteRooms.AppendInt32(rooms.Count);
            foreach (uint current in rooms)
            {
                FavouriteRooms.AppendUInt(current);
            }
            return FavouriteRooms;
        }
    }
}
