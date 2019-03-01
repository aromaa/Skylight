using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class SupportTicket
    {
        public readonly uint ID;
        public int Score;
        public int Type;
        public SupportTicketStatus Status;
        public uint SenderID;
        public uint ReportedID;
        public uint PickerID;
        public string Message;
        public uint RoomID;
        public string RoomName;
        public double Timestmap;
        public string SenderUsername;
        public string ReportedUsername;
        public string PickerUsername;

        public int State
        {
            get
            {
                int result;
                if (this.Status == SupportTicketStatus.Open)
                {
                    result = 1;
                }
                else
                {
                    if (this.Status == SupportTicketStatus.Picked)
                    {
                        result = 2;
                    }
                    else
                    {
                        result = 0;
                    }
                }

                return result;
            }
        }

        public SupportTicket(uint id, int score, int type, SupportTicketStatus status, uint senderId, uint reportedId, uint pickerId, string message, uint roomId, string roomName, double timestamp)
        {
            this.ID = id;
            this.Score = score;
            this.Type = type;
            this.Status = status;
            this.SenderID = senderId;
            this.ReportedID = reportedId;
            this.PickerID = pickerId;
            this.Message = message;
            this.RoomID = roomId;
            this.RoomName = roomName;
            this.Timestmap = timestamp;
            this.SenderUsername = Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.SenderID);
            this.ReportedUsername = Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.ReportedID);
            this.PickerUsername = Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.PickerID);
        }

        public void Pick(GameClient picker, bool updateToDB)
        {
            this.Status = SupportTicketStatus.Picked;
            this.PickerID = picker.GetHabbo().ID;
            this.PickerUsername = picker.GetHabbo().Username;

            if (updateToDB)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("ticketId", this.ID);
                    dbClient.AddParamWithValue("pickerId", this.PickerID);
                    dbClient.AddParamWithValue("pickerUsername", this.PickerUsername);
                    dbClient.ExecuteQuery("UPDATE moderation_tickets SET status = 'picked', picker_id = @pickerId WHERE id = @ticketId LIMIT 1");
                }
            }
        }

        public void Close(SupportTicketStatus closeStatus, bool updateToDB)
        {
            this.Status = closeStatus;

            if (updateToDB)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("ticketId", this.ID);
                    dbClient.AddParamWithValue("status", closeStatus == SupportTicketStatus.Resolved ? "resolved" : closeStatus == SupportTicketStatus.Abusive ? "abusive" : "invalid");
                    dbClient.ExecuteQuery("UPDATE moderation_tickets SET status = @status WHERE id = @ticketId LIMIT 1");
                }
            }
        }

        public void Release(bool updateToDB)
        {
            this.Status = SupportTicketStatus.Open;

            if (updateToDB)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("ticketId", this.ID);
                    dbClient.ExecuteQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = @ticketId LIMIT 1");
                }
            }
        }

        public void Delete(bool updateToDB)
        {
            this.Status = SupportTicketStatus.Deleted;

            if (updateToDB)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("ticketId", this.ID);
                    dbClient.ExecuteQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = @ticketId LIMIT 1");
                }
            }
        }

        public ServerMessage Serialize(ServerMessage message)
        {
            message.AppendUInt(this.ID);
            message.AppendInt32(this.State);
            message.AppendInt32(this.Type);
            message.AppendInt32(this.Type);
            message.AppendInt32((int)(TimeUtilies.GetUnixTimestamp() - this.Timestmap) * 1000); //time since sended as ms
            message.AppendInt32(this.Score);
            message.AppendUInt(this.SenderID);
            message.AppendString(this.SenderUsername);
            message.AppendUInt(this.ReportedID);
            message.AppendString(this.ReportedUsername);
            message.AppendUInt(this.PickerID);
            message.AppendString(this.PickerUsername);
            message.AppendString(this.Message);
            message.AppendUInt(this.RoomID);
            message.AppendString(this.RoomName);
            message.AppendInt32(2); //room type, 2 makes skip the conditions, not really needed
            if (Skylight.Revision >= Revision.RELEASE63_201211141113_913728051)
            {
                message.AppendInt32(0); //chat history count?
            }

            //message.AppendInt32(0); //room type, 0 = public, 1 private (?)
            //if (false) //public room
            //{
            //    message.AppendString("PUBLIC ROOM TEST :D"); //not even used so idk
            //    message.AppendUInt(this.RoomID); //guess
            //    message.AppendInt32(0); //some kind boolean, 0 = false, 1 = true
            //}
            //else //private
            //{
            //    message.AppendString("PRIVATE ROOM TEST :D #1"); //not even used so idk
            //    message.AppendUInt(this.RoomID); //guess
            //    message.AppendString("PRIVATE ROOM TEST :D #2");  //not even used so idk
            //}
            return message;
        }

        public ServerMessage Serialize()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.SupportTicker);
            this.Serialize(message);
            return message;
        }
    }
}
