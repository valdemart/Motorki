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
    public class GameScreen_Main : GameScreen_MenuScreen
    {
        public GameScreen_Main(Game game, ref UIParent parent)
            : base(game, ref parent)
        {
        }

        public override void LoadAndInitialize()
        {
            Rectangle screen = game.GraphicsDevice.PresentationParameters.Bounds;
            UIButton button;

            UI.Clear();

            throw new NotImplementedException();
            //push game logo into UI

            button = new UIButton(game);
            button.Name = "btnNewGame";
            button.Text = "Nowa Gra";
            button.PositionAndSize = new Rectangle(screen.Width / 2 - 50, 100 + (screen.Height - 85 * 4), 100, 75);
            button.Action += (UIButton_Action)((btn) => {
                oResult = new GameScreen_NewGame(game, ref UI);
                iResult = MenuReturnCodes.MenuSwitching;
            });
            UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnJoinGame";
            button.Text = "Do³¹cz do gry";
            button.PositionAndSize = new Rectangle(screen.Width / 2 - 50, 100 + (screen.Height - 85 * 3), 100, 75);
            button.Action += (UIButton_Action)((btn) => {
                oResult = new GameScreen_JoinGame(game, ref UI);
                iResult = MenuReturnCodes.MenuSwitching;
            });
            UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnOptions";
            button.Text = "Opcje";
            button.PositionAndSize = new Rectangle(screen.Width / 2 - 50, 100 + (screen.Height - 85 * 2), 100, 75);
            button.Action += (UIButton_Action)((btn) => {
                oResult = new GameScreen_Options(game, ref UI);
                iResult = MenuReturnCodes.MenuSwitching;
            });
            UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnExit";
            button.Text = "WyjdŸ";
            button.PositionAndSize = new Rectangle(screen.Width / 2 - 50, 100 + (screen.Height - 85 * 1), 100, 75);
            button.Action += (UIButton_Action)((btn) => {
                oResult = null;
                iResult = MenuReturnCodes.Exit;
            });
            UI.Add(button);

            UI.LoadAndInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            UI.Update(gameTime);
        }

        public override void Draw(ref SpriteBatch sb, GameTime gameTime)
        {
            UI.Draw(ref sb, gameTime);
        }
    }
}
