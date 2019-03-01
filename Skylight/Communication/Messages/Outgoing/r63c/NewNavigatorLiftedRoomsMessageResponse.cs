﻿using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class NewNavigatorLiftedRoomsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.NewNavigatorLiftedRooms);
            message.AppendInt32(1); //count

            message.AppendInt32(0); //flat id
            message.AppendInt32(0); //idk
            message.AppendString("IMAGEXCDDD"); //image
            message.AppendString("LAM"); //caption
            return message;
        }
    }
}
