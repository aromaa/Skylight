using SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MessengerSetFriendRelationEvent : MessengerSetFriendRelationEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.UserID = message.PopWiredUInt();
            this.Relation = (MessengerFriendRelation)message.PopWiredInt32();

            base.Handle(session, message);
        }
    }
}
