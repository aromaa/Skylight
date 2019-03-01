using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendQuizComposerHandler : OutgoingHandler
    {
        public string Name;
        public ICollection<int> Questions;

        public SendQuizComposerHandler(string name, ICollection<int> questions)
        {
            this.Name = name;
            this.Questions = questions;
        }
    }
}
