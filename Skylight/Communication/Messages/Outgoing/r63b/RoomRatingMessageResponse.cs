using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class RoomRatingMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage roomRating = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            roomRating.Init(r63bOutgoing.RoomRating);
            roomRating.AppendInt32(valueHolder.GetValue<int>("Score"));
            roomRating.AppendBoolean(valueHolder.GetValue<bool>("CanVote"));
            return roomRating;
        }
    }
}
