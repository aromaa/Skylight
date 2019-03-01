using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomModel
    {
        public readonly string ID;
        public int DoorX;
        public int DoorY;
        public int DoorZ;
        public int DoorDir;
        public string Heightmap;
        public string PublicItems;
        public string ClubName;
        public int MaxX;
        public int MaxY;
        public ModelTileState[,] TileStates;
        public int[,] ModelHeights;
        public int[,] Rotation;
        public RoomModelTrigger[,] Triggers;

        private MultiRevisionServerMessage RelativeHeightmapServerMessage;
        private MultiRevisionServerMessage PublicItemsServerMessage;

        public RoomModel(string id, int doorx, int doory, int doorz, int doorDir, string heightmap, string publicItems, string clubName)
        {
            this.ID = id;
            this.DoorX = doorx;
            this.DoorY = doory;
            this.DoorZ = doorz;
            this.DoorDir = doorDir;
            this.Heightmap = heightmap.ToLower();
            this.PublicItems = publicItems;
            this.ClubName = clubName;
            
            string[] splitedHeightmap = this.Heightmap.Split(new char[]
			{
				Convert.ToChar(13)
			});

            this.MaxX = splitedHeightmap[0].Length;
            this.MaxY = splitedHeightmap.Length;

            this.TileStates = new ModelTileState[this.MaxX, this.MaxY];
            this.ModelHeights = new int[this.MaxX, this.MaxY];
            for (int i = 0; i < this.MaxY; i++)
            {
                if (i > 0)
                {
                    splitedHeightmap[i] = splitedHeightmap[i].Substring(1);
                }

                for (int j = 0; j < this.MaxX; j++)
                {
                    if (splitedHeightmap[i].Length >= j + 1)
                    {
                        string text = splitedHeightmap[i].Substring(j, 1).Trim().ToLower();
                        if (text == "x")
                        {
                            this.TileStates[j, i] = ModelTileState.BLOCKED;
                        }
                        else
                        {
                            int height;
                            if (int.TryParse(text, out height))
                            {
                                this.TileStates[j, i] = ModelTileState.OPEN;
                                this.ModelHeights[j, i] = height;
                            }
                        }
                    }
                    else
                    {
                        this.TileStates[j, i] = ModelTileState.BLOCKED;
                        Logging.WriteLine("OOPS! Room model '" + this.ID + "' got some invalid data on heightmap! Temp fixed this! :)");
                    }
                }
            }
            
            if (!string.IsNullOrWhiteSpace(this.PublicItems))
            {
                this.Rotation = new int[this.MaxX, this.MaxY];

                List<string[]> items = new List<string[]>();
                if (!this.PublicItems.Contains(Convert.ToChar(2)))
                {
                    foreach (string item in this.PublicItems.Split(Convert.ToChar(13)))
                    {
                        string[] data = item.Split(' ');

                        this.TileStates[int.Parse(data[2]), int.Parse(data[3])] = (ModelTileState)int.Parse(data[6]);
                        this.Rotation[int.Parse(data[2]), int.Parse(data[3])] = int.Parse(data[5]);

                        items.Add(data);
                    }
                }
                else
                {
                    ClientMessage message = BasicUtilies.GetRevisionClientMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message.Init(0, Skylight.GetDefaultEncoding().GetBytes(this.PublicItems));

                    int count = message.PopWiredInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string name = message.PopStringUntilBreak();
                        string item = message.PopStringUntilBreak();
                        int x = message.PopWiredInt32();
                        int y = message.PopWiredInt32();
                        int z = message.PopWiredInt32();
                        int rot = message.PopWiredInt32();

                        if (item.Contains("bench") || item.Contains("chair") || item.Contains("stool") || item.Contains("seat") || item.Contains("sofa"))
                        {
                            this.TileStates[x, y] = ModelTileState.SEAT;
                        }
                        else
                        {
                            this.TileStates[x, y] = ModelTileState.BLOCKED;
                        }

                        items.Add(new string[] { name, item, x.ToString(), y.ToString(), z.ToString(), rot.ToString() });
                    }
                }

                this.PublicItemsServerMessage = new MultiRevisionServerMessage(OutgoingPacketsEnum.PublicItems, new ValueHolder("Items", items.ToArray()));
            }

            this.TileStates[this.DoorX, this.DoorY] = ModelTileState.DOOR;

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = dbClient.ReadDataTable("SELECT * FROM room_models_triggers WHERE id = '" + this.ID + "'");
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    this.Triggers = new RoomModelTrigger[this.MaxX, this.MaxY];

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        this.Triggers[(int)dataRow["x"], (int)dataRow["y"]] = new RoomModelTrigger((int)dataRow["x"], (int)dataRow["y"], (string)dataRow["type"], (string)dataRow["data"]);
                    }
                }
            }
        }

        public byte[] GetHeightmap(Revision revision, RoomTile[,] tiles)
        {
            return BasicUtilies.GetRevisionPacketManager(revision).GetOutgoing(OutgoingPacketsEnum.Heightmap).Handle(new ValueHolder().AddValue("Heightmap", this.BuildHeightmap(tiles))).GetBytes();
        }

        public string BuildHeightmap(RoomTile[,] tiles)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //string[] splitedHeightmap = this.Heightmap.Split("\r\n".ToCharArray());
            //for (int i = 0; i < splitedHeightmap.Length; i++)
            //{
            //    string text = splitedHeightmap[i];
            //    if (!string.IsNullOrEmpty(text))
            //    {
            //        stringBuilder.Append(text);
            //        stringBuilder.Append(Convert.ToChar(13));
            //    }
            //}
            for (int i = 0; i < this.MaxY; i++)
            {
                for (int j = 0; j < this.MaxX; j++)
                {
                    if (this.DoorX == j && this.DoorY == i)
                    {
                        stringBuilder.Append(this.DoorZ);
                    }
                    else
                    {
                        if (this.TileStates[j, i] == ModelTileState.OPEN)
                        {
                            stringBuilder.Append(this.ModelHeights[j, i]);
                        }
                        else
                        {
                            stringBuilder.Append('x');
                        }
                    }
                }

                stringBuilder.Append(Convert.ToChar(13));
            }
            return stringBuilder.ToString();
        }

        public byte[] GetRelativeHeightmap(Revision revision)
        {
            if (this.RelativeHeightmapServerMessage == null)
            {
                this.RelativeHeightmapServerMessage = new MultiRevisionServerMessage(OutgoingPacketsEnum.RelativeHeightmap, new ValueHolder("RelativeHeightmap", this.BuildRelativeHeightmap()));
            }

            return this.RelativeHeightmapServerMessage.GetBytes(revision);
        }

        public byte[] GetPublicItems(Revision revision)
        {
            return this.PublicItemsServerMessage?.GetBytes(revision);
        }

        public string BuildRelativeHeightmap()
        {
            StringBuilder stringBuilder = new StringBuilder();

            //string[] splitedHeightmap = this.Heightmap.Split(new char[]
            //{
            //    Convert.ToChar(13)
            //});

            //for (int i = 0; i < this.MaxY; i++)
            //{
            //    if (i > 0)
            //    {
            //        splitedHeightmap[i] = splitedHeightmap[i].Substring(1);
            //    }

            //    for (int j = 0; j < this.MaxX; j++)
            //    {
            //        string text = splitedHeightmap[i].Substring(j, 1).Trim().ToLower();
            //        if (this.DoorX == j && this.DoorY == i)
            //        {
            //            text = string.Concat(this.DoorZ);
            //        }

            //        stringBuilder.Append(text);
            //    }

            //    stringBuilder.Append(string.Concat(Convert.ToChar(13)));
            //}

            for (int i = 0; i < this.MaxY; i++)
            {
                for (int j = 0; j < this.MaxX; j++)
                {
                    if (this.DoorX == j && this.DoorY == i)
                    {
                        stringBuilder.Append(this.DoorZ);
                    }
                    else
                    {
                        if (this.TileStates[j, i] == ModelTileState.OPEN)
                        {
                            stringBuilder.Append(this.ModelHeights[j, i]);
                        }
                        else
                        {
                            stringBuilder.Append('x');
                        }
                    }
                }

                stringBuilder.Append(Convert.ToChar(13));
            }

            return stringBuilder.ToString();
        }
    }
}
