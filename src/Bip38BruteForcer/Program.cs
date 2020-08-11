using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Bip38BruteForcer
{
    public static class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;
        private static bool _exitSystem = false;
        private static App _app;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown.");

            _app.SaveCurrentProgress();

            _exitSystem = true;
            Environment.Exit(-1);
            return true;
        }

        public static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            _app = new App();
            _app.Run();

            while (!_exitSystem)
            {
                Thread.Sleep(500);
            }
        }
    }
}