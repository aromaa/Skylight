using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System.Data;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerSearchUserResultsComposerHandler : OutgoingHandler
    {
        private static readonly List<DataRow> Empty = new List<DataRow>(0);

        public readonly List<DataRow> Friends;
        public readonly List<DataRow> Users;

        public MessengerSearchUserResultsComposerHandler(List<DataRow> friends = null, List<DataRow> users = null)
        {
            this.Friends = friends ?? MessengerSearchUserResultsComposerHandler.Empty;
            this.Users = users ?? MessengerSearchUserResultsComposerHandler.Empty;
        }
    }
}
