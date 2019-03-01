using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using System.Threading;
using SkylightEmulator.Core;
using FastFoodServerAPI.Data;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class StartGameMessageEvent : IncomingPacket
    {
        public static int GameClientID = 0;

        public void Handle(GameClient session, ClientMessage message)
        {
            int gameId = message.PopWiredInt32();

            if (Skylight.GetGame().GetFastFoodManager().GetAPIConnection() != null)
            {
                FastFoodUser user;
                Skylight.GetGame().GetFastFoodManager().GetAPIConnection().AuthenicateUserAsync(user = Skylight.GetGame().GetFastFoodManager().GetOrCreate(session), (result) =>
                {
                    if (result)
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        message_.Init(r63bOutgoing.LoadGame);
                        message_.AppendInt32(gameId);
                        message_.AppendString(Interlocked.Increment(ref StartGameMessageEvent.GameClientID).ToString());
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.GameSWFUrl); //Game SWF url
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Quality); //quality
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Scale); //scale mode
                        message_.AppendInt32(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.FPS); //FPS
                        message_.AppendInt32(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.FlashMajorVersion); //flash major version min
                        message_.AppendInt32(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.FlashMinorVersion); //flash minor version min

                        message_.AppendInt32(6); //params count
                        message_.AppendString("habboHost");
                        message_.AppendString("http://habbobeta.pw");
                        message_.AppendString("accessToken");
                        message_.AppendString(user.SessionToken);

                        message_.AppendString("assetUrl");
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Params["assetUrl"]);
                        message_.AppendString("gameServerHost");
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Params["gameServerHost"]);
                        message_.AppendString("gameServerPort");
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Params["gameServerPort"]);
                        message_.AppendString("socketPolicyPort");
                        message_.AppendString(Skylight.GetGame().GetFastFoodManager().GameLoadSettings.Params["socketPolicyPort"]);

                        session.SendMessage(message_);
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        message_.Init(r63bOutgoing.LeaveGameQueue);
                        message_.AppendInt32(gameId);
                        session.SendMessage(message_);

                        ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                        message_2.Init(r63bOutgoing.GameButtonStatus);
                        message_2.AppendInt32(gameId);
                        message_2.AppendInt32(0);
                        session.SendMessage(message_2);

                        session.SendNotif("Error! Try again");
                    }
                });
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                message_.Init(r63bOutgoing.LeaveGameQueue);
                message_.AppendInt32(gameId);
                session.SendMessage(message_);

                ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                message_2.Init(r63bOutgoing.GameButtonStatus);
                message_2.AppendInt32(gameId);
                message_2.AppendInt32(0);
                session.SendMessage(message_2);

                session.SendNotif("Error! Try again");
            }
        }
    }
}
