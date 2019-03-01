using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Managers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.OldCrypto;
using SkylightEmulator.Messages.NewCrypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class BasicUtilies
    {
        private static Dictionary<Revision, PacketManager> PacketManagers;

        static BasicUtilies()
        {
            BasicUtilies.PacketManagers = new Dictionary<Revision, PacketManager>();
        }

        public static PacketManager GetRevisionPacketManager(Revision revision)
        {
            PacketManager packetManager = null;
            if (!BasicUtilies.PacketManagers.TryGetValue(revision, out packetManager))
            {
                packetManager = BasicUtilies.GetRevisionPacketManager_(revision);
                packetManager.Initialize();
                BasicUtilies.PacketManagers.Add(revision, packetManager);
            }
            return packetManager;
        }

        private static PacketManager GetRevisionPacketManager_(Revision revision)
        {
            switch (revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                    return new r63aPacketManager();
                case Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169:
                    return new r26PacketManager();
                case Revision.RELEASE63_201211141113_913728051:
                    return new r63bPacketManager();
                case Revision.PRODUCTION_201601012205_226667486:
                    return new r63cPacketManager();
                case Revision.PRODUCTION_201611291003_338511768:
                    return new r63ccPacketManager();
                default:
                    return null;
            }
        }

        public static ClientMessage GetRevisionClientMessage(Revision revision)
        {
            switch(revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                case Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169:
                    return new OldCryptoClientMessage();
                case Revision.RELEASE63_201211141113_913728051:
                case Revision.PRODUCTION_201601012205_226667486:
                case Revision.PRODUCTION_201611291003_338511768:
                    return new NewCryptoClientMessage();
                default:
                    return null;
            }
        }

        public static ServerMessage GetRevisionServerMessage(Revision revision)
        {
            switch(revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                case Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169:
                    return new OldCryptoServerMessage(revision);
                case Revision.RELEASE63_201211141113_913728051:
                case Revision.PRODUCTION_201601012205_226667486:
                case Revision.PRODUCTION_201611291003_338511768:
                    return new NewCryptoServerMessage(revision);
                default:
                    return null;
            }
        }

        public static ServerMessage GetRevisionServerMessage(Revision revision, uint header)
        {
            ServerMessage message = null;
            switch (revision)
            {
                case Revision.RELEASE63_35255_34886_201108111108:
                case Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169:
                    message = new OldCryptoServerMessage(revision);
                    break;
                case Revision.RELEASE63_201211141113_913728051:
                case Revision.PRODUCTION_201601012205_226667486:
                case Revision.PRODUCTION_201611291003_338511768:
                    message = new NewCryptoServerMessage(revision);
                    break;
                default:
                    return null;
            }

            message?.Init(header);
            return message;
        }
    }
}
