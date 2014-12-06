using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Motorki.GameClasses;
using Motorki.GameScreens;
using Motorki.UIClasses;
using System;
using System.Collections.Generic;

namespace Motorki
{
    public enum GameLoadingStages
    {
        Load_Motors,
        Load_Map_Parameters,
        Load_Map_Points,
        Load_Map_Edges,
        Load_Map_SpawnPoints,
        Load_Map_Sectors,
        Load_Map_Rectangles,
        Load_Map_CleanUp,
        //number means percent of progress: 10=0%-10%, 20=10%-20% etc.
        Load_Map_Draw_10, Load_Map_Draw_20, Load_Map_Draw_30, Load_Map_Draw_40, Load_Map_Draw_50, Load_Map_Draw_60, Load_Map_Draw_70, Load_Map_Draw_80, Load_Map_Draw_90, Load_Map_Draw_100,
        Load_Map_Slice,
        Load_Map_MiscSettings,
        Load_Map_Bonuses_25, Load_Map_Bonuses_50, Load_Map_Bonuses_75, Load_Map_Bonuses_100,
        Load_GFX_StartCounter,
        Load_GFX_FinishImages,
        Load_GFX_BumpExplosion,
        Load_GFX_MotorExplosion,
        Load_SFX_Background,
        Load_SFX_Explosions,
    }

    public class MotorkiGame : Microsoft.Xna.Framework.Game
    {
        public static MotorkiGame game { get; private set; }
        public static Random random;

        InputEvents ieGenerator;
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BlendState LayerBlendState;
        /// <summary>
        /// layer targets list:
        ///     0 - topmost,
        ///     1 - UI,
        ///     2 - game gfx,
        ///     3 - game objects,
        ///     4 - game objects background,
        ///     5 - background
        /// </summary>
        public RenderTarget2D[] layerTargets { get; private set; }
        
        public MouseCursor mCursor;
        UIParent UI;
        GameScreen_MenuScreen menu;
        bool menu_background;
        Motorek motorek;
        Motorek[] menu_bots;
        int loading_stage;
        List<GameLoadingStages> loading_stages;
        int connecting_stage;

        GameSettings gameSettings;
        public GameTime currentTime { get; private set; }

        public MotorkiGame()
        {
            game = this;
            random = new Random();

            ieGenerator = new InputEvents(this, 500, 50, 100, 50);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            layerTargets = new RenderTarget2D[6];
            mCursor = new MouseCursor(this);
            loading_stage = -1;
            loading_stages = new List<GameLoadingStages>();
            connecting_stage = -1;
            gameSettings = new GameSettings();
            gameSettings.LoadFromFile("settings.xml");
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            gameSettings.SaveToFile("settings.xml");
        }

        public void SetGraphMode(int width, int height, bool fullscreen)
        {
            if (fullscreen)
            {
                bool found = false;
                foreach(DisplayMode dm in GraphicsDevice.Adapter.SupportedDisplayModes)
                    if ((dm.Width == width) && (dm.Height == height))
                    {
                        graphics.PreferredBackBufferWidth = width;
                        graphics.PreferredBackBufferHeight = height;
                        graphics.IsFullScreen = true;
                        found = true;
                    }
                if (!found)
                    return;
            }
            else
            {
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.IsFullScreen = false;
            }
            graphics.ApplyChanges();
        }

        public void SetGraphMode(int dm_index, bool fullscreen)
        {
            foreach (DisplayMode dm in GraphicsDevice.Adapter.SupportedDisplayModes)
                if (dm_index == 0)
                {
                    SetGraphMode(dm.Width, dm.Height, fullscreen);
                    break;
                }
                else
                    dm_index--;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LayerBlendState = new BlendState();
            LayerBlendState.ColorSourceBlend = LayerBlendState.AlphaSourceBlend = Blend.SourceAlpha;
            LayerBlendState.ColorBlendFunction = LayerBlendState.AlphaBlendFunction = BlendFunction.Add;
            LayerBlendState.ColorDestinationBlend = LayerBlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            SetGraphMode(GameSettings.videoGraphMode, GameSettings.videoFullscreen);
            for (int i = 0; i < layerTargets.Length; i++)
            {
                if (layerTargets[i] != null)
                    layerTargets[i].Dispose();
                layerTargets[i] = new RenderTarget2D(GraphicsDevice, 800, 600, false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                GraphicsDevice.SetRenderTarget(layerTargets[i]);
                GraphicsDevice.Clear(Color.Transparent);
            }
            GraphicsDevice.SetRenderTarget(null);

            mCursor.LoadAndInitialize();
            mCursor.Visible = true;

            //create UI
            UI = new UIParent(this);
            //CreateTestUI();
            //CreateSteeringTestUI();
            //UI.LoadAndInitialize();

            menu = new GameScreen_Main(this);
            menu.OnExit += menu_OnExit;
            menu.LoadAndInitialize();
            menu_InitBackground();
        }

        /// <summary>
        /// menu processor
        /// </summary>
        void menu_OnExit(GameScreen_MenuScreen menu)
        {
            switch (menu.iResult)
            {
                case MenuReturnCodes.Error:
                case MenuReturnCodes.Exit:
                    Exit();
                    break;
                case MenuReturnCodes.GameStartRequested:
                    /*switch (loading_stage)
                    {
                        case -1: //show loading UI and find items to load
                            menu_DestroyBackground();
                            this.menu = menu.oResult as GameScreen_MenuScreen;
                            this.menu.OnExit += menu_OnExit;
                            this.menu.LoadAndInitialize();

                            loading_stages.Clear();
                            //obligatory loadings
                            loading_stages.Add(GameLoadingStages.Load_Motors);
                            loading_stages.Add(GameLoadingStages.Load_Map_Parameters);
                            loading_stages.Add(GameLoadingStages.Load_Map_Points);
                            loading_stages.Add(GameLoadingStages.Load_Map_Edges);
                            loading_stages.Add(GameLoadingStages.Load_Map_SpawnPoints);
                            loading_stages.Add(GameLoadingStages.Load_Map_Sectors);
                            loading_stages.Add(GameLoadingStages.Load_Map_Rectangles);
                            loading_stages.Add(GameLoadingStages.Load_Map_CleanUp);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_10);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_20);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_30);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_40);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_50);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_60);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_70);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_80);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_90);
                            loading_stages.Add(GameLoadingStages.Load_Map_Draw_100);
                            loading_stages.Add(GameLoadingStages.Load_Map_Slice);
                            loading_stages.Add(GameLoadingStages.Load_Map_MiscSettings);
                            //non-obligatory but map dependent loadings (need to use though not need to execute)
                            loading_stages.Add(GameLoadingStages.Load_Map_Bonuses_25);
                            loading_stages.Add(GameLoadingStages.Load_Map_Bonuses_50);
                            loading_stages.Add(GameLoadingStages.Load_Map_Bonuses_75);
                            loading_stages.Add(GameLoadingStages.Load_Map_Bonuses_100);
                            //obligatory
                            loading_stages.Add(GameLoadingStages.Load_GFX_StartCounter);
                            loading_stages.Add(GameLoadingStages.Load_GFX_FinishImages);
                            loading_stages.Add(GameLoadingStages.Load_GFX_BumpExplosion);
                            loading_stages.Add(GameLoadingStages.Load_GFX_MotorExplosion);
                            if (GameSettings.audioEnabled)
                            {
                                loading_stages.Add(GameLoadingStages.Load_SFX_Background);
                                loading_stages.Add(GameLoadingStages.Load_SFX_Explosions);
                            }

                            loading_stage++;
                            break;
                        default:
                            switch (loading_stages[loading_stage])
                            {
                                case GameLoadingStages.Load_Motors:
                                    break;
                            }
                            loading_stage++;
                            break;
                    }*/
                    menu_DestroyBackground();
                    UIParent.UI.Clear();
                    int cameraStickBikeID = -1;
                    int playerBikeID = -1;
                    for (int i = 0; i < GameSettings.gameSlots.Length; i++)
                        if (GameSettings.gameSlots[i] != null)
                        {
                            if ((GameSettings.gameSlots[i].type == typeof(PlayerMotor)) && (GameSettings.gameSlots[i].playerID != -1))
                            {
                                playerBikeID = i;
                                break;
                            }
                            if ((cameraStickBikeID == -1) && (GameSettings.gameSlots[i].type == typeof(BotMotor)))
                                cameraStickBikeID = i;
                        }
                    GameSettings.gamePlayScreen1 = new GamePlay();
                    GameSettings.gamePlayScreen1.LoadAndInitialize(currentTime, (playerBikeID != -1 ? playerBikeID : cameraStickBikeID));
                    this.menu = new GameScreen_GameUI(game, (playerBikeID != -1 ? playerBikeID : cameraStickBikeID), 800, 600);
                    this.menu.OnExit += menu_OnExit;
                    this.menu.LoadAndInitialize();
                    break;
                case MenuReturnCodes.GameJoinRequested:
                    //menu_DestroyBackground();
                    break;
                case MenuReturnCodes.MenuStartRequested:
                    this.menu = new GameScreen_Main(this);
                    this.menu.OnExit += menu_OnExit;
                    this.menu.LoadAndInitialize();
                    menu_InitBackground();
                    break;
                case MenuReturnCodes.MenuSwitching:
                    this.menu = menu.oResult as GameScreen_MenuScreen;
                    this.menu.OnExit += menu_OnExit;
                    this.menu.LoadAndInitialize();
                    break;
            }
        }

        void menu_InitBackground()
        {
            if (menu_background)
                menu_DestroyBackground();

            motorek = new PlayerMotor(this, 0, new Color(255, 255, 255));
            motorek.name = "";
            menu_bots = new BotMotor[3];
            menu_bots[0] = new BotMotor(this, Color.Red);
            menu_bots[0].name = "";
            menu_bots[1] = new BotMotor(this, Color.Green);
            menu_bots[1].name = "";
            menu_bots[2] = new BotMotor(this, Color.Blue);
            menu_bots[2].name = "";
            motorek.LoadAndInitialize(new Rectangle(0, 0, 800, 600));
            motorek.Spawn(new Vector2(400, 300), 90);
            menu_bots[0].LoadAndInitialize(new Rectangle(0, 0, 900, 700));
            menu_bots[0].Spawn(new Vector2(10, 10), 135);
            menu_bots[1].LoadAndInitialize(new Rectangle(0, 0, 900, 700));
            menu_bots[1].Spawn(new Vector2(10, 600 - 10), 45);
            menu_bots[2].LoadAndInitialize(new Rectangle(0, 0, 900, 700));
            menu_bots[2].Spawn(new Vector2(800 - 10, 600 - 10), -45);
            menu_background = true;
        }

        void menu_DestroyBackground()
        {
            if (!menu_background)
                return;

            motorek.Destroy();
            motorek = null;
            for (int i = 0; i < 3; i++)
            {
                menu_bots[i].Destroy();
                menu_bots[i] = null;
            }
            menu_background = false;
        }

        protected override void Update(GameTime gameTime)
        {
            currentTime = gameTime;

            base.Update(gameTime);

            ieGenerator.CheckForEvents(gameTime);
            UI.Update(gameTime);

            if (menu_background)
            {
                motorek.Update(gameTime);
                for(int i=0; i<3; i++)
                    menu_bots[i].Update(gameTime);
            }

            if (GameSettings.gamePlayScreen1 != null)
                GameSettings.gamePlayScreen1.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //clear render targets
            for (int i = 0; i < layerTargets.Length; i++)
            {
                GraphicsDevice.SetRenderTarget(layerTargets[i]);
                GraphicsDevice.Clear(Color.Transparent);
            }
            GraphicsDevice.SetRenderTarget(null);

            //render UI
            UI.Draw(ref spriteBatch, gameTime);
            mCursor.Draw(ref spriteBatch, gameTime);
            if (menu_background)
            {
                motorek.Draw(ref spriteBatch, gameTime, 400, 300);
                for (int i = 0; i < 3; i++)
                    menu_bots[i].Draw(ref spriteBatch, gameTime, 450, 350);
            }

            //render game
            if (GameSettings.gamePlayScreen1 != null)
                GameSettings.gamePlayScreen1.Draw(ref spriteBatch, gameTime);

            //gather screen layers
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, LayerBlendState);
            for (int i = layerTargets.Length - 1; i >= 0; i--)
                spriteBatch.Draw(layerTargets[i], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), layerTargets[i].Bounds, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region testing shit...

        private void CreateTestUI()
        {
            UIButton button;
            button = new UIButton(this);
            button.Name = "Button1";
            button.Text = "Fullscreen: " + (graphics.IsFullScreen ? "On" : "Off");
            button.PositionAndSize = new Rectangle(10, 10, 150, 30);
            button.Action += (UIButton_Action)((btn) =>
            {
                SetGraphMode(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, !graphics.IsFullScreen);
                btn.Text = "Fullscreen: " + (graphics.IsFullScreen ? "On" : "Off");
                UI["TextBox1"].Enabled = !UI["TextBox1"].Enabled;
                UI["Combo1"].Enabled = !UI["Combo1"].Enabled;
            });
            UI.Add(button);

            button = new UIButton(this);
            button.Name = "Button2";
            button.Text = "taaa...";
            button.PositionAndSize = new Rectangle(165, 10, 100, 50);
            button.Action += (UIButton_Action)((btn) =>
            {
                UI["Button1"].Enabled = !UI["Button1"].Enabled;
                UI["Label1"].Enabled = !UI["Label1"].Enabled;
                ((UIComboBox)UI["Combo1"]).Edible = !((UIComboBox)UI["Combo1"]).Edible;
                if (((UIComboBox)UI["Combo1"]).Edible)
                    UI["Combo1"].PositionAndSize = new Rectangle(UI["Combo1"].PositionAndSize.X, UI["Combo1"].PositionAndSize.Y, 200, 50);
                else
                    UI["Combo1"].PositionAndSize = new Rectangle(UI["Combo1"].PositionAndSize.X, UI["Combo1"].PositionAndSize.Y, 100, 0);
                switch (((UITextBox)UI["TextBox1"]).PasswordChar)
                {
                    case '\0': ((UITextBox)UI["TextBox1"]).PasswordChar = '*'; break;
                    case '*': ((UITextBox)UI["TextBox1"]).PasswordChar = '\0'; break;
                }
                ((UIListBox)UI["ListBox1"]).Add(UI["TextBox1"].Text);
            });
            UI.Add(button);

            UITextBox textbox = new UITextBox(this);
            textbox.Name = "TextBox1";
            textbox.Text = "kupa";
            textbox.TextLenghtLimit = 10;
            textbox.PositionAndSize = new Rectangle(270, 10, 150, 30);
            textbox.PasswordChar = '*';
            UI.Add(textbox);

            UILabel label = new UILabel(this);
            label.Name = "Label1";
            label.Text = "jakis tam...\ntekst";
            label.Enabled = false;
            label.PositionAndSize = new Rectangle(UI["Button1"].PositionAndSize.X, UI["Button1"].PositionAndSize.Y + UI["Button1"].PositionAndSize.Height + 5, 0, 0);
            UI.Add(label);

            UIComboBox combo1 = new UIComboBox(this);
            combo1.Name = "Combo1";
            combo1.Edible = false;
            combo1.PositionAndSize = new Rectangle(UI["Button2"].PositionAndSize.X, UI["Button2"].PositionAndSize.Y + UI["Button2"].PositionAndSize.Height + 5, 100, 0);
            combo1.Values.Add("pos1");
            combo1.Values.Add("pos2");
            combo1.Values.Add("pos3");
            combo1.Values.Add("pos4");
            combo1.MaxDisplayedItems = 3;
            combo1.SelectionChanged += (UIComboBox_SelectionChanged)((combo, oldindex) =>
            {
                UI["TextBox1"].Text = combo.SelectedValue;
            });
            UI.Add(combo1);

            UIListBox listBox1 = new UIListBox(this);
            listBox1.Name = "ListBox1";
            listBox1.PositionAndSize = new Rectangle(UI["TextBox1"].PositionAndSize.X + UI["TextBox1"].PositionAndSize.Width + 5, UI["TextBox1"].PositionAndSize.Y, 150, 0);
            listBox1.AutoSizeHeight = true;
            listBox1.MaxVisibleValues = 5;
            listBox1.Add("Button1");
            listBox1.Add("Button2");
            listBox1.Add("TextBox1");
            listBox1.Add("Label1");
            listBox1.Add("Combo1");
            listBox1.AutoSizeHeight = false;
            listBox1.PositionAndSize = new Rectangle(listBox1.PositionAndSize.X, listBox1.PositionAndSize.Y, listBox1.PositionAndSize.Width, listBox1.PositionAndSize.Height + 50);
            listBox1.ItemHover += (UIListBox_ItemEvent)((list, itemindex) =>
            {
                list.SelectedIndex = itemindex;
            });
            listBox1.ItemClicked += (UIListBox_ItemEvent)((list, itemindex) =>
            {
                if (itemindex < 5)
                    UI[list[itemindex].Text].Enabled = !UI[list[itemindex].Text].Enabled;
                else
                    ((UIListBox)UI["ListBox1"]).RemoveAt(itemindex);
            });
            UI.Add(listBox1);

            UIScrollBar scroll1 = new UIScrollBar(this, false);
            scroll1.Name = "Scroll1";
            scroll1.MinimalValue = 0;
            scroll1.MaximalValue = 200;
            scroll1.PositionAndSize = new Rectangle(label.PositionAndSize.X, label.PositionAndSize.Y + label.PositionAndSize.Height + 100, 150, 150);
            scroll1.Value = 0;
            scroll1.ValuesOnScreen = 3;
            scroll1.ValueChanged += (UIScrollBar_ValueChanged)((scroll, oldval) =>
            {
                UI["Label1"].Text = (oldval < scroll.Value ? "zwiekszone na:\n" : "zmniejszone na:\n") + scroll.Value;
                ((UIProgress)UI["ProgressBar1"]).Percent = scroll.Value / 200.0;
            });
            UI.Add(scroll1);

            UICheckBox checkBox1 = new UICheckBox(this);
            checkBox1.Name = "CheckBox1";
            checkBox1.AutoSize = true;
            checkBox1.Text = "checkbox1";
            checkBox1.PositionAndSize = new Rectangle(combo1.PositionAndSize.X, combo1.PositionAndSize.Bottom + 5, 0, 0);
            UI.Add(checkBox1);

            UIKeyPicker keypicker1 = new UIKeyPicker(this);
            keypicker1.Name = "KeyPicker1";
            keypicker1.PositionAndSize = new Rectangle(scroll1.PositionAndSize.X, scroll1.PositionAndSize.Bottom + 5, 100, 30);
            keypicker1.SelectedKey = Keys.A;
            UI.Add(keypicker1);

            UIProgress progress1 = new UIProgress(this);
            progress1.Name = "ProgressBar1";
            progress1.PositionAndSize = new Rectangle(keypicker1.PositionAndSize.X, keypicker1.PositionAndSize.Bottom + 5, 100, 20);
            progress1.Angular = true;
            progress1.color = Color.Blue; //relevant when Angular==true
            progress1.Percent = 0.8;
            UI.Add(progress1);
        }

        private void CreateSteeringTestUI()
        {
            UIButton btnSteering = new UIButton(this);
            btnSteering.Name = "btnSteering";
            btnSteering.Text = "Steering: Relative";
            btnSteering.PositionAndSize = new Rectangle(0, 0, 200, 30);
            btnSteering.Action += (UIButton_Action)((btn) =>
            {
                if (GameSettings.player1Steering == PlayerMotor.Steering.Absolute)
                {
                    GameSettings.player1Steering = PlayerMotor.Steering.Relative;
                    btn.Text = "Steering: Relative";
                }
                else
                {
                    GameSettings.player1Steering = PlayerMotor.Steering.Absolute;
                    btn.Text = "Steering: Absolute";
                }
            });
            UI.Add(btnSteering);
        }

        #endregion
    }
}
