using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerInitEventHandler : IncomingPacket
    {
        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() == null)
            {
                session.GetHabbo().InitMessenger();

                session.SendMessage(new MessengerInitComposerHandler(10000, 10000, 10000, session.GetHabbo().GetMessenger().GetCategorys()));
                session.SendMessage(new MessengerFriendsComposerHandler(session.GetHabbo().GetMessenger().GetFriends()));
            }
        }
    }
}
