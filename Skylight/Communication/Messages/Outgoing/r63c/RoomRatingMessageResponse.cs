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
    class RoomRatingMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomRating = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            roomRating.Init(r63cOutgoing.RoomRating);
            roomRating.AppendInt32(valueHolder.GetValue<int>("Score"));
            roomRating.AppendBoolean(valueHolder.GetValue<bool>("CanVote"));
            return roomRating;
        }
    }
}
