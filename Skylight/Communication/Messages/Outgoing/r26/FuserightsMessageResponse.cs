using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class FuserightsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Fuserights = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Fuserights.Init(r26Outgoing.Fuserights);
            //fuserigts as string, looped, no count

            //Fuserights.AppendString("default");
            //Fuserights.AppendString("fuse_login");
            //Fuserights.AppendString("fuse_buy_credits");
            //Fuserights.AppendString("fuse_trade");
            //Fuserights.AppendString("fuse_room_queue_default");
            //Fuserights.AppendString("fuse_any_room_controller");
            return Fuserights;
        }
    }
}
