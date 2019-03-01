using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ChangeUsernameMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.FlagmeCommandUsed)
            {
                if (session.GetHabbo().HasPermission("cmd_flagme"))
                {
                    string username = TextUtilies.FilterString(message.PopFixedString());
                    if (username.Length < 3) //to short
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(2); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (username.Length > 15) // too long
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(3); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (username.Contains(" ") || !Regex.IsMatch(username, "^[-a-zA-Z0-9._:,]+$") || TextUtilies.HaveBlacklistedWords(username)) //invalid name
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(4); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (Skylight.GetGame().GetGameClientManager().UsernameExits(username)) //name already exits
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(5); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else
                    {
                        //CHANGE THE DAMN NAME
                        session.FlagmeCommandUsed = false;

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                            dbClient.AddParamWithValue("userName", session.GetHabbo().Username);
                            dbClient.AddParamWithValue("command", "flagme");
                            dbClient.AddParamWithValue("extraData", "Old username: " + session.GetHabbo().Username + " | New username: " + username);
                            dbClient.AddParamWithValue("timestamp", TimeUtilies.GetUnixTimestamp());
                            dbClient.AddParamWithValue("userSessionId", session.SessionID);

                            dbClient.ExecuteQuery("INSERT INTO cmdlogs(user_id, user_name, command, extra_data, timestamp, user_session_id) VALUES (@userId, @userName, @command, @extraData, @timestamp, @userSessionId)");

                            dbClient.AddParamWithValue("newUserName", username);
                            dbClient.ExecuteQuery("UPDATE users SET username = @newUserName WHERE id = @userId LIMIT 1");
                        }

                        foreach (Room room in Skylight.GetGame().GetRoomManager().GetLoadedRooms())
                        {
                            if (room.RoomData.OwnerID == session.GetHabbo().ID)
                            {
                                Skylight.GetGame().GetRoomManager().UnloadRoom(room);
                            }
                        }

                        session.GetHabbo().Username = username;
                        Skylight.GetGame().GetAchievementManager().AddAchievement(session, "ChangeName", 1);
                        session.Stop("Name changed");
                    }
                }
            }
            else
            {

            }
        }
    }
}
