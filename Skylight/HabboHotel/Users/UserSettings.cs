using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class UserSettings
    {
        private readonly uint ID;
        public bool BlockNewFriends;
        public bool HideOnline;
        public bool HideInRoom;
        public int[] Volume;
        public bool AcceptTrading;
        public bool FriendStream;
        public bool PreferOldChat;
        public bool BlockRoomInvites;
        public bool BlockCameraFollow;
        public int ChatColor;

        public UserSettings(uint id, bool blockNewFriends, bool hideOnline, bool hideInRoom, int[] volume, bool acceptTrading, bool friendStream, bool preferOldChat, bool blockRoomInvites, bool blockCameraFollow, int chatColor)
        {
            this.ID = id;
            this.BlockNewFriends = blockNewFriends;
            this.HideOnline = hideOnline;
            this.HideInRoom = hideInRoom;
            this.Volume = volume;
            this.AcceptTrading = acceptTrading;
            this.FriendStream = friendStream;
            this.PreferOldChat = preferOldChat;
            this.BlockRoomInvites = blockRoomInvites;
            this.BlockCameraFollow = blockCameraFollow;
            this.ChatColor = chatColor;
        }
    }
}
