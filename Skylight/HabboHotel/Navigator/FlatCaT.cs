using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Navigator
{
    public class FlatCat
    {
        public readonly int Id;
        public readonly string Caption;
        public readonly int MinRank;
        public readonly bool CanTrade;

        public FlatCat(int Id, string Caption, int MinRank, bool CanTrade)
        {
            this.Id = Id;
            this.Caption = Caption;
            this.MinRank = MinRank;
            this.CanTrade = CanTrade;
        }
    }
}
