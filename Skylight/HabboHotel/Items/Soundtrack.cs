using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class Soundtrack
    {
        public int Id;
        public string Name;
        public string Author;
        public string Track;
        public int Length;
        public Soundtrack(int Id, string name, string author, string track, int length)
        {
            this.Id = Id;
            this.Name = name;
            this.Author = author;
            this.Track = track;
            this.Length = length;
        }

        public int LengthInMS
        {
            get
            {
                return this.Length * 1000;
            }
        }
    }
}
