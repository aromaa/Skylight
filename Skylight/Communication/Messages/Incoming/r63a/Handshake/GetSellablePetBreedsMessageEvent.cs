using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetSellablePetBreedsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string pet = message.PopFixedString();
            if (pet.ToLower().StartsWith("a0 pet"))
            {
                int petId = int.Parse(pet.Substring(6));

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.PetBreeds);
                message_.AppendString(pet);

                List<PetRace> races = Skylight.GetGame().GetCatalogManager().GetPetRaces(petId);
                message_.AppendInt32(races.Count); //count
                foreach(PetRace race in races)
                {
                    message_.AppendInt32(race.RaceID);
                    message_.AppendInt32(race.Color1); //color id
                    message_.AppendBoolean(true); //sellable
                    message_.AppendBoolean(false); //idk
                }
                session.SendMessage(message_);
            }
        }
    }
}
