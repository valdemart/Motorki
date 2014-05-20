using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Motorki.UIClasses;
using System;

namespace Motorki
{
    public class MotorkiGame : Microsoft.Xna.Framework.Game
    {
        InputEvents ieGenerator;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BlendState LayerBlendState;
        
        MouseCursor mCursor;
        UIParent UI;
        RenderTarget2D UITarget;

        GameSettings gameSettings;

        Motorek motorek;
        Motorek bot;

        public MotorkiGame()
        {
            ieGenerator = new InputEvents(this, 500, 50, 100, 50);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mCursor = new MouseCursor(this);
            gameSettings = new GameSettings();
            gameSettings.LoadFromFile("settings.xml");
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            gameSettings.SaveToFile("settings.xml");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LayerBlendState = new BlendState();
            LayerBlendState.ColorSourceBlend = LayerBlendState.AlphaSourceBlend = Blend.SourceAlpha;
            LayerBlendState.ColorBlendFunction = LayerBlendState.AlphaBlendFunction = BlendFunction.Add;
            LayerBlendState.ColorDestinationBlend = LayerBlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            mCursor.LoadAndInitialize();
            mCursor.Visible = true;

            //create render targets
            UITarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            //create UI
            UI = new UIParent(this);
            //CreateTestUI();
            CreateSteeringTestUI();
            UI.LoadAndInitialize();

            motorek = new PlayerMotor(this, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 90, new Color(255, 255, 32), new Color(255, 255, 0), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            bot = new BotMotor(this, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 90, new Color(255, 32, 32), new Color(255, 255, 0), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            motorek.LoadAndInitialize();
            bot.LoadAndInitialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ieGenerator.CheckForEvents(gameTime);
            UI.Update(gameTime);

            motorek.Update(gameTime);
            bot.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //render UI
            GraphicsDevice.SetRenderTarget(UITarget);
            GraphicsDevice.Clear(Color.Transparent);
            UI.Draw(ref spriteBatch, gameTime);

            //render game
            motorek.Draw(gameTime);
            bot.Draw(gameTime);

            //gather screen layers
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, LayerBlendState);
            //spriteBatch.Draw(gameTarget, new Rectangle(0, 0, gameTarget.Width, gameTarget.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            motorek.DrawToSB(ref spriteBatch, 0, 0);
            bot.DrawToSB(ref spriteBatch, 0, 0);
            spriteBatch.Draw(UITarget, Vector2.Zero, Color.White);
            mCursor.Draw(spriteBatch);
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
                graphics.ToggleFullScreen();
                btn.Text = "Fullscreen: " + (graphics.IsFullScreen ? "On" : "Off");
                UITarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
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
                    UI[list[itemindex]].Enabled = !UI[list[itemindex]].Enabled;
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
            });
            UI.Add(scroll1);

            UICheckBox checkBox1 = new UICheckBox(this);
            checkBox1.Name = "CheckBox1";
            checkBox1.AutoSize = true;
            checkBox1.Text = "checkbox1";
            checkBox1.PositionAndSize = new Rectangle(combo1.PositionAndSize.X, combo1.PositionAndSize.Bottom + 5, 0, 0);
            UI.Add(checkBox1);
        }

        private void CreateSteeringTestUI()
        {
            UIButton btnSteering = new UIButton(this);
            btnSteering.Name = "btnSteering";
            btnSteering.Text = "Steering: Relative";
            btnSteering.PositionAndSize = new Rectangle(0, 0, 200, 30);
            btnSteering.Action += (UIButton_Action)((btn) =>
            {
                if (((PlayerMotor)motorek).steering == PlayerMotor.Streering.Absolute)
                {
                    ((PlayerMotor)motorek).steering = PlayerMotor.Streering.Relative;
                    btn.Text = "Steering: Relative";
                }
                else
                {
                    ((PlayerMotor)motorek).steering = PlayerMotor.Streering.Absolute;
                    btn.Text = "Steering: Absolute";
                }
            });
            UI.Add(btnSteering);
        }

        #endregion
    }
}
