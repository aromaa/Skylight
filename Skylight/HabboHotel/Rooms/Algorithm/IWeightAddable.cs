using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Algorithm
{
    public interface IWeightAddable<T>
    {
        T WeightChange { get; set; }
    }
}
