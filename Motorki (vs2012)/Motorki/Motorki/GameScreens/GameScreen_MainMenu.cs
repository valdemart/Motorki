using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Motorki.UIClasses;

namespace Motorki.GameScreens
{
    public class GameScreen_Main : GameScreen_MenuScreen
    {
        public GameScreen_Main(MotorkiGame game)
            : base(game)
        {
        }

        public override void LoadAndInitialize()
        {
            UIButton button;
            UIImage logo;

            UIParent.UI.Clear();

            logo = new UIImage(game);
            logo.Textures = game.Content.Load<Texture2D>("logo_mainmenu");
            logo.NormalTexture = new Rectangle(0, 0, 500, 75);
            logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
            UIParent.UI.Add(logo);

            button = new UIButton(game);
            button.Name = "btnNewGame";
            button.Text = "New Game";
            button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 5), 200, 75);
            button.Action += (UIButton_Action)((btn) =>
            {
                oResult = new GameScreen_NewGame(game);
                iResult = MenuReturnCodes.MenuSwitching;
                Call_OnExit();
            });
            UIParent.UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnJoinGame";
            button.Enabled = false;
            button.Text = "Join Game";
            button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 4), 200, 75);
            button.Action += (UIButton_Action)((btn) =>
            {
                oResult = new GameScreen_JoinGame(game);
                iResult = MenuReturnCodes.MenuSwitching;
                Call_OnExit();
            });
            UIParent.UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnOptions";
            button.Text = "Options";
            button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 3), 200, 75);
            button.Action += (UIButton_Action)((btn) =>
            {
                oResult = new GameScreen_Options(game);
                iResult = MenuReturnCodes.MenuSwitching;
                Call_OnExit();
            });
            UIParent.UI.Add(button);

            button = new UIButton(game);
            button.Name = "btnExit";
            button.Text = "Exit";
            button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 2), 200, 75);
            button.Action += (UIButton_Action)((btn) =>
            {
                oResult = null;
                iResult = MenuReturnCodes.Exit;
                Call_OnExit();
            });
            UIParent.UI.Add(button);

            UIParent.UI.LoadAndInitialize();
        }
    }
}
