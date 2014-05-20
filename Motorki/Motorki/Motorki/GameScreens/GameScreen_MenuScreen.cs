using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Motorki.UIClasses;

namespace Motorki.GameScreens
{
    public enum MenuReturnCodes : int
    {
        Error = -1,
        Exit = 0,
        GameStartRequested = 1,
        GameJoinRequested = 2,
        MenuSwitching = 3,
    }

    public abstract class GameScreen_MenuScreen
    {
        protected Game game;
        protected UIParent UI;
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

        public GameScreen_MenuScreen(Game game, ref UIParent parent)
        {
            this.game = game;
            UI = parent;
            oResult = null;
            iResult = MenuReturnCodes.Error;
        }

        public abstract void LoadAndInitialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(ref SpriteBatch sb, GameTime gameTime);
    }
}
