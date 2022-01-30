using Pokedex.PkdxDatabase.Context;
using System;
using System.Runtime.InteropServices;

namespace Pokedex.PkdxDatabase
{
    class Program
    {
        const int SWP_NOSIZE = 0x0001;


        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        static void Main(string[] args)
        {
            int xpos = -1300;
            int ypos = 300;
            SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);

            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            PopulateDB.PopulateDatabase(context);
        }
    }
}
