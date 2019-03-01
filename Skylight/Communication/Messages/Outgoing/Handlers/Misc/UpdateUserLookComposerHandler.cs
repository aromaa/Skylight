using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class UpdateUserLookComposerHandler : OutgoingHandler
    {
        public string Look { get; }
        public string Gender { get; }

        public UpdateUserLookComposerHandler(string look, string gender)
        {
            this.Look = look;
            this.Gender = gender;
        }
    }
}
