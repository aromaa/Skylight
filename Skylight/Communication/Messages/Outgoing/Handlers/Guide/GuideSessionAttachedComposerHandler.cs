using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide
{
    public class GuideSessionAttachedComposerHandler : OutgoingHandler
    {
        public bool IsHelper { get; }
        public GuideRequestType Type { get; }
        public string Message { get; }
        public int TimeLeft { get; }

        public GuideSessionAttachedComposerHandler(bool isHelper, GuideRequestType type, string message, int timeLeft)
        {
            this.IsHelper = isHelper;
            this.Type = type;
            this.Message = message;
            this.TimeLeft = timeLeft;
        }
    }
}
