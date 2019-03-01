using Microsoft.Win32;
using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SkylightEmulator
{
    class Program
    {
        private static bool LicenceError = false;
        private static Platform Platform;
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
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Program.Platform = Platform.Windows;
                    break;
                case PlatformID.Unix:
                    Program.Platform = Platform.Linux;
                    break;
                case PlatformID.MacOSX:
                    Program.Platform = Platform.MacOSX;
                    break;
                default:
                    Program.Platform = Platform.Unknown;
                    break;
            }

            if (Program.Platform == Platform.Unknown)
            {
                Console.WriteLine("Unable to determite your OS!");
                return;
            }

            Console.Title = "Skylight";

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.UnhandledException);

            if (Program.Platform == Platform.Windows)
            {
                Program.ConsoleCtrlEventHandler = (Program.EventHandler)Delegate.Combine(Program.ConsoleCtrlEventHandler, new Program.EventHandler(Program.ConsoleCtrlHandler));
                Program.SetConsoleCtrlHandler(Program.ConsoleCtrlEventHandler, true);
            }

            SystemEvents.SessionEnded += Program.SessionEnded;
            SystemEvents.PowerModeChanged += Program.PowerModeChanged;

            Skylight Skylight = new Skylight();
            if (true || Licence.LicenceOK())
            {
                Skylight.Initialize();
            }
            else
            {
                Program.LicenceFailure();
            }

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Program.Destroy();
                }
            }
        }

        private static void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            Program.Destroy();
        }

        private static void SessionEnded(object sender, SessionEndedEventArgs e)
        {
            Program.Destroy();
        }

        public static void LicenceFailure()
        {
            Program.LicenceError = true;

            Task task = new Task(Program.LicenceFail);
            task.Start();
        }

        private static void LicenceFail()
        {
            Program.LicenceFail2();
        }

        private static void LicenceFail2()
        {
            Program.LicenceFail();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Program.LicenceError)
            {
                if (e.IsTerminating)
                {
                    Program.Destroy(false);

                    Logging.WriteLine("SERVER TERMINATED BECAUSE UNHANDLED RUNTIME EXCEPTION!", ConsoleColor.Red);
                    Logging.WriteLine(e.ExceptionObject.ToString(), ConsoleColor.Red);
                }
            }
            else
            {

            }
        }

        private static bool ConsoleCtrlHandler(CtrlType ctrlType)
        {
            Program.Destroy();

            return true;
        }

        public static void Destroy(bool close = true, bool takeBackup = false, bool backupCompress = false, bool restart = false)
        {
            SystemEvents.SessionEnded -= SessionEnded;
            SystemEvents.PowerModeChanged -= PowerModeChanged;

            Logging.Clear();
            Logging.WriteLine("The emulator is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!", ConsoleColor.Red);

            Skylight.Destroy(close, takeBackup, backupCompress, restart);
        }
    }
}
