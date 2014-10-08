
namespace Motorki.GameScreens
{
    public enum MenuReturnCodes : int
    {
        Error = -1,
        Exit = 0,
        GameStartRequested = 1,
        GameJoinRequested = 2,
        MenuStartRequested = 3,
        MenuSwitching = 4,
    }

    public delegate void MenuScreen_MenuExit(GameScreen_MenuScreen menu);

    public abstract class GameScreen_MenuScreen
    {
        protected MotorkiGame game;
        /// <summary>
        /// values:
        ///     -1 - error (oResult contains error text string)
        ///      0 - exit
        ///      1 - game start requested (game params in GameSettings)
        ///      2 - game join requested (game params in GameSettings)
        ///      3 - menu switching (new menu screen in oResult, only created - not initiated)
        /// </summary>
        public MenuReturnCodes iResult { get; protected set; }
        public object oResult { get; protected set; }
        public event MenuScreen_MenuExit OnExit;

        public GameScreen_MenuScreen(MotorkiGame game)
        {
            this.game = game;
            oResult = null;
            iResult = MenuReturnCodes.Error;
            OnExit = null;
        }

        public abstract void LoadAndInitialize();

        protected void Call_OnExit()
        {
            if (OnExit != null)
                OnExit(this);
        }
    }
}
