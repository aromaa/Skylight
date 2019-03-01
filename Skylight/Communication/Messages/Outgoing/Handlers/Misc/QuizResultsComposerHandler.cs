using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class QuizResultsComposerHandler : OutgoingHandler
    {
        public string Name { get; }
        public ICollection<int> WrongAnswers { get; }

        public QuizResultsComposerHandler(string name, ICollection<int> wrongAnswers)
        {
            this.Name = name;
            this.WrongAnswers = wrongAnswers;
        }
    }
}
