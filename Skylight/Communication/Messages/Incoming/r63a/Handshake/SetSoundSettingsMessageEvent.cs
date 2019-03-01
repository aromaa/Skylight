using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SetSoundSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int volume = message.PopWiredInt32();

            if (volume < 0)
            {
                volume = 0;
            }
            else if (volume > 100)
            {
                volume = 100;
            }

            session.GetHabbo().GetUserSettings().Volume = new int[] { volume, volume, volume };

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                dbClient.AddParamWithValue("volume", volume);

                dbClient.ExecuteQuery("UPDATE users SET volume = @volume WHERE id = @userId LIMIT 1");
            }
        }
    }
}
