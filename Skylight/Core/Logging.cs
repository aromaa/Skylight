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
        }
    }
}
