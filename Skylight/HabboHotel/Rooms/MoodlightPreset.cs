using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class MoodlightPreset
    {
        public string ColorCode;
        public int ColorIntensity;
        public bool BackgroundOnly;

        public MoodlightPreset(string colorCode, int colorIntensity, bool backgroundOnly)
        {
            this.ColorCode = colorCode;
            this.ColorIntensity = colorIntensity;
            this.BackgroundOnly = backgroundOnly;
        }
    }
}
