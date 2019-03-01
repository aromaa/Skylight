using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items.Interactors
{
    public class InteractorDefault : FurniInteractor
    {
        private readonly int ModesCount;
        public InteractorDefault(int modesCount)
        {
            this.ModesCount = modesCount - 1;
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (this.ModesCount > 0)
            {
                if (userHasRights)
                {
                    int mode = 0;
                    int.TryParse(item.ExtraData, out mode); //if we fail mode stays at 0

                    if (mode <= 0)
                    {
                        mode = 1;
                    }
                    else
                    {
                        if (mode >= this.ModesCount)
                        {
                            mode = 0;
                        }
                        else
                        {
                            mode++;
                        }
                    }

                    item.ExtraData = mode.ToString();
                    item.UpdateState(true, true);
                }
            }
        }
    }
}
