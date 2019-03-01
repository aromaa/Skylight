using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Guide
{
    public class SendGuideRequestEventHandler : IncomingPacket
    {
        protected int Type;
        protected string Message;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                GuideRequestType type;
                switch (this.Type)
                {
                    case 1:
                        type = GuideRequestType.Help;
                        break;
                    default:
                        type = GuideRequestType.None;
                        break;
                }

                if (type != GuideRequestType.None)
                {
                    if (Skylight.GetGame().GetGuideManager().Count(type) > 0)
                    {
                        if (!Skylight.GetGame().GetGuideManager().CreateQuestion(session, type, this.Message)) //returns false if has already one
                        {
                            session.SendMessage(new GuideRequestErrorComposerHandler(GuideRequestErrorCode.SomethingWrong));
                        }
                        else
                        {
                            session.SendMessage(new GuideSessionAttachedComposerHandler(false, type, this.Message, Skylight.GetGame().GetGuideManager().GetAvarageWaitTime()));
                        }
                    }
                    else
                    {
                        session.SendMessage(new GuideRequestErrorComposerHandler(GuideRequestErrorCode.NoHelpers));
                    }
                }
                else
                {
                    session.SendMessage(new GuideRequestErrorComposerHandler(GuideRequestErrorCode.SomethingWrong));
                }
            }
        }
    }
}
