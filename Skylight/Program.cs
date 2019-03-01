using SkylightEmulator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator
{
    class Program
    {
        private static bool Booted = false;
        private enum CtrlType
        {
            CTRL_BREAK_EVENT = 1,
            CTRL_C_EVENT = 0,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        //only windows support these
        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler ConsoleCtrlEventHandler;
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(Program.EventHandler handler, bool add);
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            Console.Title = "Skylight";

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.UnhandledException);

            Program.ConsoleCtrlEventHandler = (Program.EventHandler)Delegate.Combine(Program.ConsoleCtrlEventHandler, new Program.EventHandler(Program.ConsoleCtrlHandler));
            Program.SetConsoleCtrlHandler(Program.ConsoleCtrlEventHandler, true);

            Skylight Skylight = new Skylight();
            Skylight.Initialize();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    ConsoleCtrlHandler(CtrlType.CTRL_CLOSE_EVENT);
                }
            }
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }

        private static bool ConsoleCtrlHandler(CtrlType ctrlType)
        {
            if (Program.Booted)
            {
                Console.Clear();
                Console.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                Skylight.Destroy();
            }
            return true;
        }
    }
}
