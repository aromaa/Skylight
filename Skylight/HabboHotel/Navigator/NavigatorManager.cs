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

namespace SkylightEmulator.HabboHotel.Navigator
{
    public class NavigatorManager
    {
        private Dictionary<int, PublicItem> PublicRooms;

        public NavigatorManager()
        {
            this.PublicRooms = new Dictionary<int,PublicItem>();
        }

        public void LoadPublicRooms(DatabaseClient dbClient)
        {
            Logging.Write("Loading navigator public rooms... ");
            this.PublicRooms.Clear();
            DataTable publicRooms = dbClient.ReadDataTable("SELECT id, bannertype, caption, image, type, room_id, category_parent_id FROM navigator_publics ORDER BY ordernum ASC");
            if (publicRooms != null)
            {
                foreach(DataRow dataRow in publicRooms.Rows)
                {
                    int id = (int)dataRow["id"];
                    this.PublicRooms.Add(id, new PublicItem(id, int.Parse((string)dataRow["bannertype"]), (string)dataRow["caption"], (string)dataRow["image"], (string)dataRow["type"], (uint)dataRow["room_id"], (int)dataRow["category_parent_id"]));
                }
            }
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public ServerMessage GetPublicRooms()
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.PublicRooms);
            Message.AppendInt32(this.PublicRooms.Count);
            foreach(PublicItem item in this.PublicRooms.Values.ToList())
            {
                item.Serialize(Message);
            }
            return Message;
        }

        public ServerMessage GetMyRooms(GameClient gameClient)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.MyRooms);
            Message.AppendInt32(3); //search type
            Message.AppendStringWithBreak(""); //unknown string
            Message.AppendInt32(gameClient.GetHabbo().UserRooms.Count);
            foreach(uint roomId in gameClient.GetHabbo().UserRooms)
            {
                Skylight.GetGame().GetRoomManager().GetRoomData(roomId).Serialize(Message, false);
            }
            Message.AppendBoolean(false); //show featured room
            return Message;
        }

        public ServerMessage GetPopularRooms(GameClient gameClient, int type)
        {
            ServerMessage popularRooms = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            popularRooms.Init(r63aOutgoing.Navigator);

            List<Room> rooms = new List<Room>();
            switch(type)
            {
                case -1:
                    popularRooms.AppendInt32(1);
                    popularRooms.AppendStringWithBreak("-1");

                    rooms = Skylight.GetGame().GetRoomManager().GetLoadedRooms().Where(r => r.RoomData.Type == "private" && r.RoomData.UsersNow > 0).OrderByDescending(r => r.RoomData.UsersNow).Take(50).ToList();
                    break;

            }

            popularRooms.AppendInt32(rooms.Count);
            foreach(Room room in rooms)
            {
                room.RoomData.Serialize(popularRooms, false);
            }

            if (this.PublicRooms.Count > 0)
            {
                popularRooms.AppendBoolean(true);
                this.PublicRooms.Values.ElementAt(new Random().Next(0, this.PublicRooms.Count)).Serialize(popularRooms);
            }
            else
            {
                popularRooms.AppendBoolean(false);
            }

            return popularRooms;
        }

        public ServerMessage GetFlatCats(int rank)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.FlatCats);
            Message.AppendInt32(1); //count

            Message.AppendInt32(0); //flat cat id
            Message.AppendStringWithBreak("No category"); //name
            Message.AppendBoolean(true); //can use
            return Message;
        }
    }
}
