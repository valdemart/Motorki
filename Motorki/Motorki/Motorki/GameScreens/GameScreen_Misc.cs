using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class GameScreen_Misc : GameScreen_MenuScreen
    {
        int subscreen;

        /// <param name="subscreen">0 - loading, 1 - connecting</param>
        public GameScreen_Misc(MotorkiGame game, int subscreen = 0)
            : base(game)
        {
            this.subscreen = subscreen;
        }

        public override void LoadAndInitialize()
        {
            UIParent.UI.Clear();

            switch (subscreen)
            {
                case 0: //loading
                    {
                        UIProgress progress;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Name = "imgLoading";
                        logo.Textures = game.Content.Load<Texture2D>("logo_loading");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 125, 500, 75);
                        UIParent.UI.Add(logo);

                        progress = new UIProgress(game);
                        progress.Name = "pbarLoading";
                        progress.Angular = false;
                        progress.color = null;
                        progress.Percent = 0;
                        progress.PositionAndSize = new Rectangle(400 - 300, 400, 600, 50);
                        UIParent.UI.Add(progress);
                    }
                    break;
                case 1: //connecting
                    {
                        UIProgress progress;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Name = "imgConnecting";
                        logo.Textures = game.Content.Load<Texture2D>("logo_connecting");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 125, 500, 75);
                        UIParent.UI.Add(logo);

                        progress = new UIProgress(game);
                        progress.Name = "pbarConnecting";
                        progress.Angular = false;
                        progress.color = null;
                        progress.Percent = null;
                        progress.PositionAndSize = new Rectangle(400 - 300, 400, 600, 50);
                        UIParent.UI.Add(progress);

                        UIButton btnBack = new UIButton(game);
                        btnBack.Name = "btnBack";
                        btnBack.Text = "<-- Back";
                        btnBack.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        btnBack.Action += (UIButton_Action)((btn) =>
                        {
                            //cancel connecting

                            UIParent.ESCHook -= UIParent_ESCHook;
                            oResult = new GameScreen_Main(game);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(btnBack);

                        UIParent.ESCHook += UIParent_ESCHook;
                    }
                    break;
            }

            UIParent.UI.LoadAndInitialize();
        }

        void UIParent_ESCHook()
        {
            UIParent.ESCHook -= UIParent_ESCHook;
            oResult = new GameScreen_Main(game);
            iResult = MenuReturnCodes.MenuSwitching;
            Call_OnExit();
        }
    }
}
