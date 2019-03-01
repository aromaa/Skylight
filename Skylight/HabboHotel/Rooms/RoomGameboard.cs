using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomGameboard
    {
        private static int NextID = 0;

        private Dictionary<int, RoomGameboardUser> Players;
        private string Type;
        private string Turn;
        private bool Ended;

        public int ID { get; private set; }
        public string[,] Gameboard = new string[25, 25];

        public RoomGameboard(string type)
        {
            this.Players = new Dictionary<int, RoomGameboardUser>(2);
            this.Type = type;
        }

        public void Join(RoomUnitUser user)
        {
            if (this.Players.Count < 2)
            {
                if (this.Players.Count <= 0)
                {
                    this.ID = Interlocked.Increment(ref RoomGameboard.NextID);
                }

                RoomGameboardUser user_ = new RoomGameboardUser(user);
                this.Players.Add(user.VirtualID, user_);

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                message.Init(r26Outgoing.OpenGameBoard);
                message.AppendString(this.ID.ToString(), 9);
                message.AppendString(this.Type, 9);
                user.Session.SendMessage(message);

                if (this.Type == "TicTacToe")
                {
                    RoomGameboardUser side = this.Players.Values.FirstOrDefault(u => !string.IsNullOrEmpty(u.Side));
                    if (side != null && !string.IsNullOrEmpty(side.Side))
                    {
                        if (side.Side == "O")
                        {
                            user_.Side = "X";
                        }
                        else
                        {
                            user_.Side = "O";
                        }

                        ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                        message2.Init(r26Outgoing.GameBoardData);
                        message2.AppendString(this.ID.ToString(), 13);
                        message2.AppendString("SELECTTYPE " + user_.Side, 13);
                        user.Session.SendMessage(message2);
                    }

                    ServerMessage message3 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message3.Init(r26Outgoing.GameBoardData);
                    message3.AppendString(this.ID.ToString(), 13);
                    message3.AppendString("OPPONENTS", 13);

                    RoomGameboardUser O = this.Players.Values.FirstOrDefault(u => u.Side == "O");
                    if (O != null)
                    {
                        message3.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
                    }
                    else
                    {
                        message3.AppendString("", 13);
                    }

                    RoomGameboardUser X = this.Players.Values.FirstOrDefault(u => u.Side == "X");
                    if (X != null)
                    {
                        message3.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
                    }
                    else
                    {
                        message3.AppendString("", 13);
                    }

                    foreach (RoomGameboardUser player in this.Players.Values)
                    {
                        player.User.Session.SendMessage(message3);
                    }

                    ServerMessage message4 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    message4.Init(r26Outgoing.GameBoardData);
                    message4.AppendString(this.ID.ToString(), 13);
                    message4.AppendString("BOARDDATA", 13);
                    
                    if (O != null)
                    {
                        message4.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
                    }
                    else
                    {
                        message4.AppendString("", 13);
                    }
                    
                    if (X != null)
                    {
                        message4.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
                    }
                    else
                    {
                        message4.AppendString("", 13);
                    }

                    for (int y_ = 0; y_ < 25; y_++)
                    {
                        for (int x_ = 0; x_ < 25; x_++)
                        {
                            message4.AppendString(this.Gameboard[x_, y_] ?? " ", null);
                        }
                    }
                    message4.AppendString("", 13);
                    user.Session.SendMessage(message4);
                }
            }
        }

        public void Leave(RoomUnitUser user)
        {
            this.Players.Remove(user.VirtualID);

            ServerMessage message3 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message3.Init(r26Outgoing.GameBoardData);
            message3.AppendString(this.ID.ToString(), 13);
            message3.AppendString("OPPONENTS", 13);

            RoomGameboardUser O = this.Players.Values.FirstOrDefault(u => u.Side == "O");
            if (O != null)
            {
                message3.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
            }
            else
            {
                message3.AppendString("", 13);
            }

            RoomGameboardUser X = this.Players.Values.FirstOrDefault(u => u.Side == "X");
            if (X != null)
            {
                message3.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
            }
            else
            {
                message3.AppendString("", 13);
            }

            foreach (RoomGameboardUser player in this.Players.Values)
            {
                player.User.Session.SendMessage(message3);
            }
        }

        public bool InGame(RoomUnitUser user)
        {
            return this.Players.ContainsKey(user.VirtualID);
        }

        public void DoAction(RoomUnitUser user, string[] data)
        {
            if (this.Type == "TicTacToe")
            {
                RoomGameboardUser user_ = this.Players[user.VirtualID];

                switch (data[0])
                {
                    case "CHOOSETYPE":
                        {

                            if (string.IsNullOrEmpty(user_.Side))
                            {
                                if (data[1] == "O" || data[1] == "X")
                                {
                                    this.Turn = data[1];
                                    user_.Side = data[1];

                                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                                    message.Init(r26Outgoing.GameBoardData);
                                    message.AppendString(this.ID.ToString(), 13);
                                    message.AppendString("SELECTTYPE " + data[1], 13);
                                    user.Session.SendMessage(message);

                                    RoomGameboardUser side = this.Players.Values.FirstOrDefault(u => string.IsNullOrEmpty(u.Side));
                                    if (side != null && string.IsNullOrEmpty(side.Side))
                                    {
                                        if (user_.Side == "O")
                                        {
                                            side.Side = "X";
                                        }
                                        else
                                        {
                                            side.Side = "O";
                                        }

                                        ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                                        message2.Init(r26Outgoing.GameBoardData);
                                        message2.AppendString(this.ID.ToString(), 13);
                                        message2.AppendString("SELECTTYPE " + side.Side, 13);
                                        side.User.Session.SendMessage(message2);
                                    }
                                    
                                    ServerMessage message3 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                                    message3.Init(r26Outgoing.GameBoardData);
                                    message3.AppendString(this.ID.ToString(), 13);
                                    message3.AppendString("OPPONENTS", 13);

                                    RoomGameboardUser O = this.Players.Values.FirstOrDefault(u => u.Side == "O");
                                    if (O != null)
                                    {
                                        message3.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
                                    }
                                    else
                                    {
                                        message3.AppendString("", 13);
                                    }

                                    RoomGameboardUser X = this.Players.Values.FirstOrDefault(u => u.Side == "X");
                                    if (X != null)
                                    {
                                        message3.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
                                    }
                                    else
                                    {
                                        message3.AppendString("", 13);
                                    }

                                    foreach (RoomGameboardUser player in this.Players.Values)
                                    {
                                        player.User.Session.SendMessage(message3);
                                    }
                                }
                            }
                        }
                        break;
                    case "CLOSE":
                        {
                            this.Leave(user);
                        }
                        break;
                    case "SETSECTOR":
                        {
                            if (!this.Ended && this.Turn == user_.Side)
                            {
                                int x = int.Parse(data[2]);
                                int y = int.Parse(data[3]);

                                if (x >= 0 && y >= 0 && x <= 23 && y <= 22 && this.Gameboard[x, y] == null)
                                {
                                    this.Gameboard[x, y] = user_.Side;

                                    if (!(this.Ended = this.CheckIfWin(1, 0, user_.Side)))
                                    {
                                        if (!(this.Ended = this.CheckIfWin(0, 1, user_.Side)))
                                        {
                                            if (!(this.Ended = this.CheckIfWin(1, 1, user_.Side)))
                                            {
                                                this.Ended = this.CheckIfWin(1, -1, user_.Side);
                                            }
                                        }
                                    }

                                    ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                                    message2.Init(r26Outgoing.GameBoardData);
                                    message2.AppendString(this.ID.ToString(), 13);
                                    message2.AppendString("BOARDDATA", 13);

                                    RoomGameboardUser O = this.Players.Values.FirstOrDefault(u => u.Side == "O");
                                    if (O != null)
                                    {
                                        message2.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
                                    }
                                    else
                                    {
                                        message2.AppendString("", 13);
                                    }

                                    RoomGameboardUser X = this.Players.Values.FirstOrDefault(u => u.Side == "X");
                                    if (X != null)
                                    {
                                        message2.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
                                    }
                                    else
                                    {
                                        message2.AppendString("", 13);
                                    }

                                    for (int y_ = 0; y_ < 25; y_++)
                                    {
                                        for (int x_ = 0; x_ < 25; x_++)
                                        {
                                            message2.AppendString(this.Gameboard[x_, y_] ?? " ", null);
                                        }
                                    }
                                    message2.AppendString("", 13);

                                    foreach (RoomGameboardUser player in this.Players.Values)
                                    {
                                        player.User.Session.SendMessage(message2);
                                    }

                                    if (user_.Side == "X")
                                    {
                                        this.Turn = "O";
                                    }
                                    else
                                    {
                                        this.Turn = "X";
                                    }
                                }
                            }
                        }
                        break;
                    case "RESTART":
                        {
                            this.Ended = false;
                            this.Gameboard = new string[25, 25];
                            this.Turn = user_.Side;

                            ServerMessage message2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                            message2.Init(r26Outgoing.GameBoardData);
                            message2.AppendString(this.ID.ToString(), 13);
                            message2.AppendString("BOARDDATA", 13);

                            RoomGameboardUser O = this.Players.Values.FirstOrDefault(u => u.Side == "O");
                            if (O != null)
                            {
                                message2.AppendString("O: " + O.User.Session.GetHabbo().Username, 13);
                            }
                            else
                            {
                                message2.AppendString("", 13);
                            }

                            RoomGameboardUser X = this.Players.Values.FirstOrDefault(u => u.Side == "X");
                            if (X != null)
                            {
                                message2.AppendString("X: " + X.User.Session.GetHabbo().Username, 13);
                            }
                            else
                            {
                                message2.AppendString("", 13);
                            }

                            for (int y_ = 0; y_ < 25; y_++)
                            {
                                for (int x_ = 0; x_ < 25; x_++)
                                {
                                    message2.AppendString(this.Gameboard[x_, y_] ?? " ", null);
                                }
                            }
                            message2.AppendString("", 13);

                            foreach (RoomGameboardUser player in this.Players.Values)
                            {
                                player.User.Session.SendMessage(message2);
                            }
                        }
                        break;
                }
            }
        }

        public bool CheckIfWin(int x, int y, string string_)
        {
            for(int x_ = 0;  x_ < 25; x_++)
            {
                for(int y_ = 0; y_ < 25; y_++)
                {
                    string original = this.Gameboard[x_, y_];

                    if (original == string_)
                    {
                        List<Point> points = new List<Point>();

                        for (int i = 0; i < 25; i++)
                        {
                            int nextX = x_ + (x * i);
                            int nextY = y_ + (y * i);
                            if (nextX >= 0 && nextY >= 0)
                            {
                                if (this.Gameboard[nextX, nextY] == original)
                                {
                                    points.Add(new Point(nextX, nextY));
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (points.Count >= 5)
                        {
                            foreach (Point point in points)
                            {
                                this.Gameboard[point.X, point.Y] = string_ == "X" ? "+" : "q";
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
