using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RollerMovementMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomItemRollerMovement[] movement = valueHolder.GetValueOrDefault<RoomItemRollerMovement[]>("Movement");
            RoomUserRollerMovement user = valueHolder.GetValueOrDefault<RoomUserRollerMovement>("User");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message.Init(r63cOutgoing.RollerMovement);
            if (movement != null)
            {
                RoomItemRollerMovement single = movement[0];

                message.AppendInt32(single.CurrentXY.X);
                message.AppendInt32(single.CurrentXY.Y);
                message.AppendInt32(single.NextXY.X);
                message.AppendInt32(single.NextXY.Y);

                message.AppendInt32(movement.Length); //items count
                foreach (RoomItemRollerMovement item in movement)
                {
                    message.AppendUInt(item.ItemId);
                    message.AppendString(TextUtilies.DoubleWithDotDecimal(item.CurrentZ));
                    message.AppendString(TextUtilies.DoubleWithDotDecimal(item.NextZ));
                }

                message.AppendUInt(single.SourceID); //source of action
            }
            else
            {
                message.AppendInt32(user.CurrentX);
                message.AppendInt32(user.CurrentY);
                message.AppendInt32(user.NextX);
                message.AppendInt32(user.NextY);

                message.AppendInt32(0); //items count

                message.AppendUInt(user.SourceID); //source of action
            }

            if (user != null)
            {
                message.AppendInt32(2); //user movement type
                message.AppendInt32(user.VirtualID);
                message.AppendString(TextUtilies.DoubleWithDotDecimal(user.CurrentZ));
                message.AppendString(TextUtilies.DoubleWithDotDecimal(user.NextZ));
            }
            else
            {
                message.AppendInt32(0);
            }
            return message;
        }
    }
}
