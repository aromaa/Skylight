using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Core
{
    public class Logging
    {
        public static ConsoleColor DefaultColor = ConsoleColor.White;

        static Logging()
        {
            Console.ForegroundColor = DefaultColor;
        }

        public static void WriteBlank()
        {
            WriteLine("");
        }

        public static void WriteLine(string message)
        {
            WriteLine(message, DefaultColor);
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = DefaultColor;
        }

        public static void Write(string message)
        {
            Write(message, DefaultColor);
        }

        public static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = DefaultColor;
        }

        public static void LogException(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("Exception has been saved", ConsoleColor.Red);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerChatMessage);
            Message.AppendUInt(0);
            Message.AppendString("!!AUTOMATIC EXCEPTION REPORT!!\nException has been saved!");
            foreach (GameClient sesion_ in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (sesion_ != null && sesion_.GetHabbo() != null)
                    {
                        if (sesion_.GetHabbo().HasPermission("acc_staffchat"))
                        {
                            sesion_.SendMessage(Message);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static void LogRoomException(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions_room.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("Room exception has been saved", ConsoleColor.Red);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerChatMessage);
            Message.AppendUInt(0);
            Message.AppendString("!!AUTOMATIC EXCEPTION REPORT!!\nRoom exception has been saved!");
            foreach (GameClient sesion_ in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (sesion_ != null && sesion_.GetHabbo() != null)
                    {
                        if (sesion_.GetHabbo().HasPermission("acc_staffchat"))
                        {
                            sesion_.SendMessage(Message);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static void LogRoomOverload(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions_room_overload.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("Room overload exception has been saved", ConsoleColor.Red);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerChatMessage);
            Message.AppendUInt(0);
            Message.AppendString("!!AUTOMATIC OVERLOAD REPORT!!\nSomeone tried to lag hotel and make room to overload!");
            foreach (GameClient sesion_ in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (sesion_ != null && sesion_.GetHabbo() != null)
                    {
                        if (sesion_.GetHabbo().HasPermission("acc_staffchat"))
                        {
                            sesion_.SendMessage(Message);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static void LogUserException(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions_user.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("User exception has been saved", ConsoleColor.Red);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerChatMessage);
            Message.AppendUInt(0);
            Message.AppendString("!!AUTOMATIC EXCEPTION REPORT!!\nUser exception has been saved!");
            foreach (GameClient sesion_ in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (sesion_ != null && sesion_.GetHabbo() != null)
                    {
                        if (sesion_.GetHabbo().HasPermission("acc_staffchat"))
                        {
                            sesion_.SendMessage(Message);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static void LogCommandException(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions_command.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("Command exception has been saved", ConsoleColor.Red);
        }

        public static void LogDDoS(string ip)
        {
            try
            {
                FileStream fileStream = new FileStream("ddos_protection.txt", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					"DDoS protection! IP " + ip + " has been detected from DDoS! Actions have been made!",
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {

            }

            Logging.WriteLine("DDoS protection! IP " + ip + " has been detected from DDoS! Actions have been made!", ConsoleColor.Red);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerChatMessage);
            Message.AppendUInt(0);
            Message.AppendString("!!AUTOMATIC DDOS REPORT!!\nWe are under attack!");
            foreach (GameClient sesion_ in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (sesion_ != null && sesion_.GetHabbo() != null)
                    {
                        if (sesion_.GetHabbo().HasPermission("acc_staffchat"))
                        {
                            sesion_.SendMessage(Message);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static void LogPacketException(string logText)
        {
            try
            {
                FileStream fileStream = new FileStream("exceptions_packet.err", FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(new object[]
				{
					DateTime.Now,
					": ",
					logText,
					"\r\n\r\n"
				}));
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (Exception)
            {
                Logging.WriteLine(DateTime.Now + ": " + logText);
            }

            Logging.WriteLine("Packet exception has been saved", ConsoleColor.Red);
        }

        public static void Clear()
        {
            Console.Clear();
        }
    }
}
