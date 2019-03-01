using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Pets;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class PlacePetMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && (room.RoomData.AllowPets || room.HaveOwnerRights(session)))
            {
                uint petId = message.PopWiredUInt();
                Pet pet = session.GetHabbo().GetInventoryManager().TryGetPet(petId);
                if (pet != null)
                {
                    int x = message.PopWiredInt32();
                    int y = message.PopWiredInt32();
                    if (x == 0 && y == 0)
                    {
                        RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();
                        if (user.HeadRotation == 0)
                        {
                            x = user.X;
                            y = user.Y - 1;
                        }
                        else if (user.HeadRotation == 2)
                        {
                            x = user.X + 1;
                            y = user.Y;
                        }
                        else if (user.HeadRotation == 4)
                        {
                            x = user.X;
                            y = user.Y + 1;
                        }
                        else if (user.HeadRotation == 6)
                        {
                            x = user.X - 1;
                            y = user.Y;
                        }
                        else
                        {
                            x = user.X;
                            y = user.Y + 1;
                        }
                    }

                    if (room.RoomGamemapManager.CoordsInsideRoom(x, y) && room.RoomGamemapManager.GetTile(x, y).CanUserMoveToTile)
                    {
                        //pet limit

                        double z = room.RoomGamemapManager.GetTile(x, y).GetZ(true);
                        room.RoomUserManager.AddPet(pet, x, y, z, 0);
                        session.GetHabbo().GetInventoryManager().RemovePet(pet);

                        if (room.HaveOwnerRights(session)) //only the room owner can put their pets permamently in the room
                        {
                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("petId", pet.ID);
                                dbClient.AddParamWithValue("roomId", room.ID);
                                dbClient.AddParamWithValue("x", x);
                                dbClient.AddParamWithValue("y", y);
                                dbClient.AddParamWithValue("z", z);
                                dbClient.ExecuteQuery("UPDATE user_pets SET room_id = @roomId, x = @x, y = @y, z = @z WHERE id = @petId LIMIT 1");
                            }
                        }             
                    }
                }
            }
        }
    }
}
