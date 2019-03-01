using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Bots
{
    public class RoomBotData
    {
        public readonly int ID;
        public readonly uint RoomID;
        public string Name;
        public string Look;
        public string Motto;
        public int X;
        public int Y;
        public double Z;
        public int Rotation;
        public int WalkMode;
        public int MinX;
        public int MinY;
        public int MaxX;
        public int MaxY;
        public int Effect;
        public List<BotResponse> BotResponses;
        public List<BotSpeech> BotSpeechs;

        public RoomBotData(int id, uint roomId, string name, string look, string motto, int x, int y, double z, int rotation, int walkMode, int minX, int minY, int maxX, int maxY, int effect, List<BotResponse> responses, List<BotSpeech> speechs)
        {
            this.ID = id;
            this.RoomID = roomId;
            this.Name = name;
            this.Look = look;
            this.Motto = motto;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Rotation = rotation;
            this.WalkMode = walkMode;
            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
            this.Effect = effect;
            this.BotResponses = responses;
            this.BotSpeechs = speechs;
        }
    }
}
