using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Data.Enums
{
    public enum GuideRequestType
    {
        None = 0,
        Tour = 1,
        Help = 2,
        Bully = 4
    }

    public static class GuideRequestTypeUtils
    {
        public static int GuideRequestTypeToInt(GuideRequestType type)
        {
            switch (type)
            {
                case GuideRequestType.Tour:
                    return 0;
                case GuideRequestType.Help:
                    return 1;
                case GuideRequestType.Bully:
                    return 2;
                default:
                    return -1;
            }
        }
    }
}
