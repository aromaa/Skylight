using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetSongInfoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            List<Soundtrack> tracks = new List<Soundtrack>();

            int count = message.PopWiredInt32();
            for (int i = 0; i < count; i++)
            {
                Soundtrack track = Skylight.GetGame().GetItemManager().TryGetSoundtrack(message.PopWiredInt32());
                if (track != null)
                {
                    tracks.Add(track);
                }
            }

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.SongInfo);
            message_.AppendInt32(count);
            foreach(Soundtrack track in tracks)
            {
                message_.AppendInt32(track.Id);
                message_.AppendString(track.Name);
                message_.AppendString(track.Track);
                message_.AppendInt32(track.LengthInMS);
                message_.AppendString(track.Author);
            }
            session.SendMessage(message_);
        }
    }
}
