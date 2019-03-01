using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
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
    class GetFriendStreamMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int request = message.PopWiredInt32();

            DataTable friendStream = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                friendStream = dbClient.ReadDataTable("SELECT * FROM user_friend_stream ORDER BY timestamp DESC LIMIT 15"); //check if the user is my friend xo xo
            }

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.FriendStream);
            message_.AppendInt32(friendStream.Rows.Count); //count

            foreach (DataRow dataRow in friendStream.Rows)
            {
                int type = (int)dataRow["type"];
                uint userId = (uint)dataRow["user_id"];
                string[] extraData = ((string)dataRow["extra_data"]).Split((char)9);

                message_.AppendInt32((int)dataRow["id"]);
                message_.AppendInt32(type);
                message_.AppendString(userId.ToString());
                if (type != 2) //0 = friend made, 1 = room liked, 3 = motto changed, 4 = room decorated
                {
                    string username = "Unknown";
                    string gender = "m";
                    string look = "";

                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                    if (target != null)
                    {
                        username = target.GetHabbo().Username;
                        gender = target.GetHabbo().Gender;
                        look = target.GetHabbo().Look;
                    }
                    else
                    {
                        DataRow userData = null;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", userId);
                            userData = dbClient.ReadDataRow("SELECT username, gender, look FROM users WHERE id = @userId LIMIT 1");
                        }

                        if (userData != null)
                        {
                            username = (string)userData["username"];
                            gender = (string)userData["gender"];
                            look = (string)userData["look"];
                        }
                    }

                    message_.AppendString(username); //user name
                    message_.AppendString(gender.ToLower()); //gender
                    message_.AppendString("http://localhost:7977/habbo-imaging/head.gif?figure=" + look); //user head
                }
                else //2 = achievement earned
                {
                    message_.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId)); //user name
                    message_.AppendString(""); //gender
                    message_.AppendString("http://25.76.128.47/habway/c_images/album1584/" + extraData[0] + ".gif"); //image
                }

                message_.AppendInt32((int)Math.Ceiling((TimeUtilies.GetUnixTimestamp() - (double)dataRow["timestamp"]) / 60.0)); //time as minutes
                if (type != 0) //1 = room liked, 2 = achievement earned, 3 = motto changed, 4 = room decorated
                {
                    if (type == 4)
                    {
                        message_.AppendInt32(2); //link action
                    }
                    else
                    {
                        message_.AppendInt32(1 + type); //link action
                    }
                }
                else //0 = friend made
                {
                    if (session.GetHabbo().GetMessenger().IsFriendWith(uint.Parse(extraData[0])))
                    {
                        message_.AppendInt32(1); //link action
                    }
                    else
                    {
                        message_.AppendInt32(5); //link action
                    }
                }
                message_.AppendInt32(0); //likes
                message_.AppendBoolean(false); //can like?

                if (type == 0) //friend made
                {
                    message_.AppendString(extraData[0]); //friend id
                    message_.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(extraData[0]))); //friend name
                }
                else if (type == 1) //room liked
                {
                    RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetRoomData(uint.Parse(extraData[0]));
                    if (roomData != null)
                    {
                        message_.AppendString(roomData.ID.ToString()); //room id
                        message_.AppendString(roomData.Name); //room name
                    }
                    else
                    {
                        message_.AppendString(extraData[0]); //room id
                        message_.AppendString("Room not found"); //room name
                    }
                }
                else if (type == 2) //achievement earned
                {
                    message_.AppendString(extraData[0]); //badge code
                }
                else if (type == 3) //motto changed
                {
                    message_.AppendString(extraData[0]); //new motto
                }
                else if (type == 4) //room decorated
                {
                    RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetRoomData(uint.Parse(extraData[0]));
                    if (roomData != null)
                    {
                        message_.AppendString(roomData.ID.ToString()); //room id
                        message_.AppendString(roomData.Name); //room name
                    }
                    else
                    {
                        message_.AppendString(extraData[0]); //room id
                        message_.AppendString("Room not found"); //room name
                    }
                }
            }

            session.SendMessage(message_);
        }
    }
}
