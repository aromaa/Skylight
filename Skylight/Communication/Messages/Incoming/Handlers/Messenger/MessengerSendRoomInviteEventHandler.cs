using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerSendRoomInviteEventHandler : IncomingPacket
    {
        protected uint[] SendTo;
        protected string Message;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                string filteredMessage = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(this.Message));

                List<string> receiverUsernames = new List<string>();
                List<int> receiverSessionIds = new List<int>();
                foreach (uint userId in this.SendTo)
                {
                    if (userId > 0) //real user
                    {
                        if (session.GetHabbo().GetMessenger().IsFriendWith(userId))
                        {
                            Skylight.GetGame().GetGameClientManager().GetGameClientById(userId)?.SendMessage(new MessengerReceivedRoomInviteComposerHandler(session.GetHabbo().ID, filteredMessage));
                        }
                        else
                        {
                            receiverUsernames.Add(Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId));
                            receiverSessionIds.Add(-1);
                        }
                    }
                }

                Skylight.GetGame().GetChatlogManager().LogRoomInvite(session, this.SendTo.Where(u => u > 0).ToList(), receiverUsernames, receiverSessionIds, this.Message);
            }
        }
    }
}
