using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RedeemVoucherMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string voucher = message.PopFixedString();

            DataRow voucherData = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("code", voucher);
                voucherData = dbClient.ReadDataRow("SELECT credits, activity_points_type, activity_points FROM vouchers WHERE code = @code LIMIT 1");
                if (voucherData != null)
                {
                    dbClient.ExecuteQuery("DELETE FROM vouchers WHERE code = @code LIMIT 1");
                }
            }

            if (voucherData != null)
            {
                session.GetHabbo().Credits += (int)voucherData["credits"];
                session.GetHabbo().UpdateCredits(true);

                session.GetHabbo().AddActivityPoints((int)voucherData["activity_points_type"], (int)voucherData["activity_points"]);
                session.GetHabbo().UpdateActivityPoints((int)voucherData["activity_points_type"], true);

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.RedeemVoucher);
                message_.AppendString(""); //product name
                message_.AppendString(""); //product desc
                session.SendMessage(message_);
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.RedeemVoucherError);
                message_.AppendString("0");
                session.SendMessage(message_);
            }
        }
    }
}
