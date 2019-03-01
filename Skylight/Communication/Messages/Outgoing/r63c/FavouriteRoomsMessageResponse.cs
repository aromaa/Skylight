using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class FavouriteRoomsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            int limit = valueHolder.GetValue<int>("Max");
            List<uint> rooms = valueHolder.GetValue<List<uint>>("FavouriteRooms");

            ServerMessage FavouriteRooms = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            FavouriteRooms.Init(r63cOutgoing.FavouriteRooms);
            FavouriteRooms.AppendInt32(limit);
            FavouriteRooms.AppendInt32(rooms.Count);
            foreach (uint current in rooms)
            {
                FavouriteRooms.AppendUInt(current);
            }
            return FavouriteRooms;
        }
    }
}
