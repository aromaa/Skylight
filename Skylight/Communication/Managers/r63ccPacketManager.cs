using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Incoming.r63cc;
using SkylightEmulator.Communication.Messages.Outgoing.r63cc;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Managers
{
    class r63ccPacketManager : PacketManager
    {
        public Dictionary<uint, IncomingPacket> IncomingPackets;
        public Dictionary<OutgoingPacketsEnum, OutgoingPacket> OutgoingPackets;

        public r63ccPacketManager()
        {
            this.IncomingPackets = new Dictionary<uint, IncomingPacket>();
            this.OutgoingPackets = new Dictionary<OutgoingPacketsEnum, OutgoingPacket>();
            
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ShowNotifications, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.FavouriteRooms, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AvaiblityStatus, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.ModTool, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.MOTD, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UnseenItem, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateAchievement, new EmptyMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Badges, new EmptyMessageResponse());

            this.OutgoingPackets.Add(OutgoingPacketsEnum.Fuserights, new FuserightsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.AuthOk, new AuthOkMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UserPerks, new UserPerksMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Friends, new FriendsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.Requests, new RequestsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorMetaData, new NewNavigatorMetaDataMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorLiftedRooms, new NewNavigatorLiftedRoomsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorSavedSearches, new NewNavigatorSavedSearchesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.NewNavigatorEventCategories, new NewNavigatorEventCategoriesMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateCredits, new UpdateCreditsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPoints, new UpdateActivityPointsMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.UpdateActivityPointsSilence, new UpdateActivityPointsSilenceMessageResponse());
            this.OutgoingPackets.Add(OutgoingPacketsEnum.HomeRoom, new HomeRoomMessageResponse());

            this.IncomingPackets.Add(r63ccIncoming.SSOTicket, new SSOTicketMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.GetUserData, new GetUserDataMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.InitMessenger, new InitMessengerMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.RequestRoomCategories, new RequestRoomCategoriesMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.MachineID, new MachineIDMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.UserCredits, new UserCreditsMessageEvent());
            this.IncomingPackets.Add(r63ccIncoming.GetGames, new GetGamesMessageEvent());
        }

        public override void Initialize()
        {

        }

        public override bool HandleIncoming(uint id, out IncomingPacket packet)
        {
            if (this.IncomingPackets.ContainsKey(id))
            {
                packet = this.IncomingPackets[id];
                return true;
            }
            else
            {
                packet = null;
                return false;
            }
        }

        public override bool HandleOutgoing(OutgoingPacketsEnum id, out OutgoingPacket packet)
        {
            if (this.OutgoingPackets.ContainsKey(id))
            {
                packet = this.OutgoingPackets[id];
                return true;
            }
            else
            {
                packet = null;
                return false;
            }
        }

        public override IncomingPacket GetIncoming(uint id)
        {
            return this.IncomingPackets[id];
        }

        public override OutgoingPacket GetOutgoing(OutgoingPacketsEnum id)
        {
            OutgoingPacket packet;
            if (!this.OutgoingPackets.TryGetValue(id, out packet))
            {
                throw new Exception("Outgoing packet not found: " + id);
            }
            return packet;
        }

        public override void Clear()
        {
            if (this.IncomingPackets != null)
            {
                this.IncomingPackets.Clear();
            }
            this.IncomingPackets = null;

            this.OutgoingPackets = null;
        }

        public override ServerMessage GetNewOutgoing(OutgoingHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}