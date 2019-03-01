using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
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

        private ServerMessage HeightmapServerMessage;
        private ServerMessage RelativeHeightmapServerMessage;

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
                            this.TileStates[j, i] = ModelTileState.HOLE;
                        }
                        else
                        {
                            int height;
                            if (int.TryParse(text, out height))
                            {
                                this.TileStates[j, i] = ModelTileState.FLOOR;
                                this.ModelHeights[j, i] = height;
                            }
                        }
                    }
                    else
                    {
                        this.TileStates[j, i] = ModelTileState.HOLE;
                        Logging.WriteLine("OOPS! Room model '" + this.ID + "' got some invalid data on heightmap! Temp fixed this! :)");
                    }
                }
            }
            this.TileStates[this.DoorX, this.DoorY] = ModelTileState.DOOR;

            this.GetHeightmap(); //load them to cache
            this.GetRelativeHeightmap(); //load them to cache
        }

        public ServerMessage GetHeightmap()
        {
            if (this.HeightmapServerMessage == null)
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.Heightmap);
                Message.AppendStringWithBreak(this.BuildHeightmap());
                this.HeightmapServerMessage = Message;
            }

            return this.HeightmapServerMessage;
        }

        public string BuildHeightmap()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] splitedHeightmap = this.Heightmap.Split("\r\n".ToCharArray());
            for (int i = 0; i < splitedHeightmap.Length; i++)
            {
                string text = splitedHeightmap[i];
                if (!string.IsNullOrEmpty(text))
                {
                    stringBuilder.Append(text);
                    stringBuilder.Append(Convert.ToChar(13));
                }
            }
            return stringBuilder.ToString();
        }

        public ServerMessage GetRelativeHeightmap()
        {
            if (this.RelativeHeightmapServerMessage == null)
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.RelativeHeightmap);
                string[] splitedHeightmap = this.Heightmap.Split(new char[]
			    {
				    Convert.ToChar(13)
			    });

                for (int i = 0; i < this.MaxY; i++)
                {
                    if (i > 0)
                    {
                        splitedHeightmap[i] = splitedHeightmap[i].Substring(1);
                    }

                    for (int j = 0; j < this.MaxX; j++)
                    {
                        string text = splitedHeightmap[i].Substring(j, 1).Trim().ToLower();
                        if (this.DoorX == j && this.DoorY == i)
                        {
                            text = string.Concat(this.DoorZ);
                        }
                        Message.AppendString(text);
                    }
                    Message.AppendString(string.Concat(Convert.ToChar(13)));
                }

                this.RelativeHeightmapServerMessage = Message;
            }

            return this.RelativeHeightmapServerMessage;
        }
    }
}
