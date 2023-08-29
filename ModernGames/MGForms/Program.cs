using System.Runtime.InteropServices;

namespace ModernGames.MyForm
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        public static Keys[] KeySet = { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.A, Keys.S };

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);
        public const int KEY_PRESSED = 0x8000;
        public static bool IsKeyDown(Keys key)
        {
            return Convert.ToBoolean(GetKeyState((int)key) & KEY_PRESSED);
        }
    }
}