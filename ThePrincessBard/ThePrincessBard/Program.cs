using System;

namespace ThePrincessBard
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ThePrincessBard game = new ThePrincessBard())
            {
                game.Run();
            }
        }
    }
#endif
}

