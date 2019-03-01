using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendMySettingsComposerHandler : OutgoingHandler
    {
        public int VolumeSystem { get; }
        public int VolumeFurni { get; }
        public int VolumeTrax { get; }
        public bool PreferOldChat { get; }
        public bool BlockRoomInvites { get; }
        public bool BlockCameraFollow { get; }
        public int ChatColor { get; }

        public SendMySettingsComposerHandler(int volumeSystem, int volumeFurni, int volumeTrax, bool preferOldChat, bool blockRoomInvites, bool blockCameraFollow, int chatColor)
        {
            this.VolumeSystem = volumeSystem;
            this.VolumeFurni = volumeFurni;
            this.VolumeTrax = volumeTrax;
            this.PreferOldChat = preferOldChat;
            this.BlockRoomInvites = blockRoomInvites;
            this.BlockCameraFollow = blockCameraFollow;
            this.ChatColor = chatColor;
        }
    }
}
