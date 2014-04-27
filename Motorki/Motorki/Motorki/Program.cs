using System;

namespace Motorki
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MotorkiGame game = new MotorkiGame())
            {
                game.Run();
            }
        }
    }
#endif
}

