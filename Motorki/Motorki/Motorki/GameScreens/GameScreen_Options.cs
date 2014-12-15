using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Motorki.GameClasses;
using Motorki.UIClasses;

namespace Motorki.GameScreens
{
    public class GameScreen_Options : GameScreen_MenuScreen
    {
        private int subscreen;

        /// <param name="subscreen">values: -1 - main, 0 - player, 1 - video, 2 - audio, 3 - common</param>
        public GameScreen_Options(MotorkiGame game, int subscreen = -1)
            : base(game)
        {
            this.subscreen = subscreen;
        }

        public override void LoadAndInitialize()
        {
            UIParent.UI.Clear();

            switch (subscreen)
            {
                case -1: //main
                    {
                        UIButton button;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_options");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        button = new UIButton(game);
                        button.Name = "btnPlayerOptions";
                        button.Text = "Players";
                        button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 5), 200, 75);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, 0);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnVideoOptions";
                        button.Text = "Video";
                        button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 4), 200, 75);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, 1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnAudioOptions";
                        button.Text = "Audio";
                        button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 3), 200, 75);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, 2);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnCommon";
                        button.Text = "Other settings";
                        button.PositionAndSize = new Rectangle(400 - 100, (600 - 85 * 2), 200, 75);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, 3);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        UIButton btnBack = new UIButton(game);
                        btnBack.Name = "btnBack";
                        btnBack.Text = "<-- Back";
                        btnBack.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        btnBack.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Main(game);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(btnBack);

                        UIParent.ESCHook += UIParent_ESCHook1;
                    }
                    break;
                case 0: //player
                    {
                        UIButton button;
                        UIComboBox combo;
                        UITextBox textbox;
                        UIKeyPicker kpicker;
                        UILabel label;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_playersoptions");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Player name: ";
                        label.PositionAndSize = new Rectangle(10, 95 + 5, 0, 0);
                        UIParent.UI.Add(label);

                        textbox = new UITextBox(game);
                        textbox.Name = "tboxName";
                        textbox.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 200, 0);
                        textbox.TextLenghtLimit = 15;
                        textbox.Text = "";
                        UIParent.UI.Add(textbox);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Color: ";
                        label.PositionAndSize = new Rectangle(textbox.PositionAndSize.Right + 20, textbox.PositionAndSize.Top + 5, 0, 0);
                        UIParent.UI.Add(label);

                        combo = new UIComboBox(game);
                        combo.Name = "cboxColor";
                        combo.Edible = false;
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, textbox.PositionAndSize.Top, 100, 0);
                        combo.Values.Add(new UITaggedValue("white", Color.White));
                        combo.Values.Add(new UITaggedValue("black", Color.Black));
                        combo.Values.Add(new UITaggedValue("red", Color.Red));
                        combo.Values.Add(new UITaggedValue("green", Color.Green));
                        combo.Values.Add(new UITaggedValue("blue", Color.Blue));
                        combo.Values.Add(new UITaggedValue("yellow", Color.Yellow));
                        combo.Values.Add(new UITaggedValue("magenta", Color.Magenta));
                        combo.Values.Add(new UITaggedValue("cyan", Color.Cyan));
                        combo.MaxDisplayedItems = 6;
                        combo.SelectedIndex = 0;
                        UIParent.UI.Add(combo);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Keys:";
                        label.PositionAndSize = new Rectangle(10, textbox.PositionAndSize.Bottom + 10, 0, 0);
                        UIParent.UI.Add(label);
                        Rectangle last_pos = new Rectangle(label.PositionAndSize.Left + 10, label.PositionAndSize.Bottom + 5, 100, textbox.PositionAndSize.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Turn left/Go left: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpGoLeft";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Turn right/Go right: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpGoRight";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "---/Go up: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpGoUp";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Brake/Go down: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpGoDown";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Chat: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpOpenChat";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Bot menu: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpBotMenu";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Speed up bonus: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpSpeedUp";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Trace widen bonus: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpTraceWiden";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Fear bomb bonus: ";
                        label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        kpicker = new UIKeyPicker(game);
                        kpicker.Name = "kpRandomizationBomb";
                        kpicker.PositionAndSize = new Rectangle(300, last_pos.Top, last_pos.Width, last_pos.Height);
                        kpicker.SelectedKey = Keys.None;
                        UIParent.UI.Add(kpicker);
                        last_pos = new Rectangle(last_pos.Left, kpicker.PositionAndSize.Bottom, last_pos.Width, last_pos.Height);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.Text = "Steering: ";
                        label.PositionAndSize = new Rectangle(10, last_pos.Top + 10, 0, 0);
                        UIParent.UI.Add(label);

                        combo = new UIComboBox(game);
                        combo.Name = "cboxSteering";
                        combo.Edible = false;
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, last_pos.Top + 5, 100, 0);
                        combo.Values.Add(new UITaggedValue("relative", PlayerMotor.Steering.Relative));
                        combo.Values.Add(new UITaggedValue("absolute", PlayerMotor.Steering.Absolute));
                        combo.SelectedIndex = 0;
                        UIParent.UI.Add(combo);

                        //load player data
                        UIParent.UI["tboxName"].Text = GameSettings.playerName;
                        for (int i = 0; i < Enum.GetNames(typeof(GameKeyNames)).Length; i++)
                            ((UIKeyPicker)UIParent.UI["kp" + Enum.GetNames(typeof(GameKeyNames))[i]]).SelectedKey = GameSettings.playerKeys[i];
                        for (int i = 0; i < ((UIComboBox)UIParent.UI["cboxColor"]).Values.Count; i++)
                            if ((Color)((UIComboBox)UIParent.UI["cboxColor"]).Values[i].Tag == GameSettings.playerColor)
                            {
                                ((UIComboBox)UIParent.UI["cboxColor"]).SelectedIndex = i;
                                break;
                            }
                        for (int i = 0; i < ((UIComboBox)UIParent.UI["cboxSteering"]).Values.Count; i++)
                            if ((PlayerMotor.Steering)((UIComboBox)UIParent.UI["cboxSteering"]).Values[i].Tag == GameSettings.playerSteering)
                            {
                                ((UIComboBox)UIParent.UI["cboxSteering"]).SelectedIndex = i;
                                break;
                            }

                        button = new UIButton(game);
                        button.Name = "btnAcceptPlayerChanges";
                        button.Text = "Save";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            GameSettings.playerName = UIParent.UI["tboxName"].Text;
                            for (int i = 0; i < Enum.GetNames(typeof(GameKeyNames)).Length; i++)
                                GameSettings.playerKeys[i] = ((UIKeyPicker)UIParent.UI["kp" + Enum.GetNames(typeof(GameKeyNames))[i]]).SelectedKey;
                            GameSettings.playerColor = (Color)((UIComboBox)UIParent.UI["cboxColor"]).SelectedItem.Tag;
                            GameSettings.playerSteering = (PlayerMotor.Steering)((UIComboBox)UIParent.UI["cboxSteering"]).SelectedItem.Tag;

                            //don't exit - user might want to change something more
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnBack";
                        button.Text = "<-- Back";
                        button.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        UIParent.ESCHook += UIParent_ESCHook2;
                    }
                    break;
                case 1: //video
                    {
                        UIButton button;
                        UICheckBox checkbox;
                        UIComboBox combo;
                        UILabel label;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_videooptions");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        label = new UILabel(game);
                        label.Name = "label1";
                        label.Text = "Display mode:";
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, logo.PositionAndSize.Bottom + 10, 0, 0);
                        UIParent.UI.Add(label);

                        combo = new UIComboBox(game);
                        combo.Name = "comboDisplayMode";
                        combo.Edible = false;
                        combo.MaxDisplayedItems = 5;
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top, 200, 0);
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top + (label.PositionAndSize.Height / 2 - combo.PositionAndSize.Height / 2), 200, 0);
                        foreach (DisplayMode dm in game.GraphicsDevice.Adapter.SupportedDisplayModes)
                            combo.Values.Add(new UITaggedValue(dm.Width + "x" + dm.Height + " " + dm.Format, dm));
                        combo.SelectedIndex = GameSettings.videoGraphMode;
                        combo.SelectionChanged += (UIComboBox_SelectionChanged)((c, old) => {
                            game.SetGraphMode(c.SelectedIndex, ((UICheckBox)UIParent.UI["cboxFullscreen"]).Checked);
                        });
                        UIParent.UI.Add(combo);

                        checkbox = new UICheckBox(game);
                        checkbox.Name = "cboxFullscreen";
                        checkbox.Text = "Fullscreen mode";
                        checkbox.AutoSize = true;
                        checkbox.PositionAndSize = new Rectangle(label.PositionAndSize.Left, combo.PositionAndSize.Bottom + 5, 0, 0);
                        checkbox.Checked = GameSettings.videoFullscreen;
                        checkbox.CheckChanged += (UICheckBox_CheckChanged)((cbox) => {
                            game.SetGraphMode(game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight, ((UICheckBox)UIParent.UI["cboxFullscreen"]).Checked);
                        });
                        UIParent.UI.Add(checkbox);

                        button = new UIButton(game);
                        button.Name = "btnAcceptVideoChanges";
                        button.Text = "Save";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            GameSettings.videoGraphMode = ((UIComboBox)UIParent.UI["comboDisplayMode"]).SelectedIndex;
                            GameSettings.videoFullscreen = ((UICheckBox)UIParent.UI["cboxFullscreen"]).Checked;

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnBack";
                        button.Text = "<-- Back";
                        button.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            game.SetGraphMode(GameSettings.videoGraphMode, GameSettings.videoFullscreen);

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        UIParent.ESCHook += UIParent_ESCHook2;
                    }
                    break;
                case 2: //audio
                    {
                        UIButton button;
                        UICheckBox checkbox;
                        UILabel label;
                        UIScrollBar scroll;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_audiooptions");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        checkbox = new UICheckBox(game);
                        checkbox.Name = "cbEnable";
                        checkbox.AutoSize = true;
                        checkbox.Checked = GameSettings.audioEnabled;
                        checkbox.Enabled = false; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        checkbox.PositionAndSize = new Rectangle(10, logo.PositionAndSize.Bottom + 10, 0, 0);
                        checkbox.Text = "Enable sound";
                        checkbox.CheckChanged += (UICheckBox_CheckChanged)((cb) => {
                            UIParent.UI["scrollVolume"].Enabled = cb.Checked;
                            UIParent.UI["scrollMusic"].Enabled = cb.Checked;
                            UIParent.UI["scrollSFX"].Enabled = cb.Checked;
                            UIParent.UI["scrollUI"].Enabled = cb.Checked;
                        });
                        UIParent.UI.Add(checkbox);
                        Rectangle last_pos = new Rectangle(checkbox.PositionAndSize.Left + 10, checkbox.PositionAndSize.Bottom + 5, 300, 0);

                        scroll = new UIScrollBar(game);
                        scroll.Name = "scrollVolume";
                        scroll.Enabled = GameSettings.audioEnabled;
                        scroll.IsHorizontal = true;
                        scroll.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, last_pos.Width, 0);
                        scroll.MinimalValue = 0;
                        scroll.MaximalValue = 100;
                        scroll.ValuesOnScreen = 1;
                        scroll.Value = GameSettings.audioVolumePercent;
                        scroll.ValueChanged += (UIScrollBar_ValueChanged)((scrl, old) => {
                            //game.SetupAudio(((UICheckBox)UIParent.UI["cbEnable"]).Checked, ((UIScrollBar)UIParent.UI["scrollVolume"]).Value, ((UIScrollBar)UIParent.UI["scrollMusic"]).Value, ((UIScrollBar)UIParent.UI["scrollSFX"]).Value, ((UIScrollBar)UIParent.UI["scrollUI"]).Value);
                        });
                        UIParent.UI.Add(scroll);
                        label = new UILabel(game);
                        label.Name = "labelVolume";
                        label.AutoSize = true;
                        label.Text = scroll.Value + "%";
                        label.PositionAndSize = new Rectangle(scroll.PositionAndSize.Right + 5, scroll.PositionAndSize.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        last_pos = new Rectangle(scroll.PositionAndSize.Left, scroll.PositionAndSize.Bottom + 5, last_pos.Width, 0);

                        scroll = new UIScrollBar(game);
                        scroll.Name = "scrollMusic";
                        scroll.Enabled = GameSettings.audioEnabled;
                        scroll.IsHorizontal = true;
                        scroll.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, last_pos.Width, 0);
                        scroll.MinimalValue = 0;
                        scroll.MaximalValue = 100;
                        scroll.ValuesOnScreen = 1;
                        scroll.Value = GameSettings.audioMusicVolumePercent;
                        scroll.ValueChanged += (UIScrollBar_ValueChanged)((scrl, old) => {
                            //game.SetupAudio(((UICheckBox)UIParent.UI["cbEnable"]).Checked, ((UIScrollBar)UIParent.UI["scrollVolume"]).Value, ((UIScrollBar)UIParent.UI["scrollMusic"]).Value, ((UIScrollBar)UIParent.UI["scrollSFX"]).Value, ((UIScrollBar)UIParent.UI["scrollUI"]).Value);
                        });
                        UIParent.UI.Add(scroll);
                        label = new UILabel(game);
                        label.Name = "labelMusic";
                        label.AutoSize = true;
                        label.Text = scroll.Value + "%";
                        label.PositionAndSize = new Rectangle(scroll.PositionAndSize.Right + 5, scroll.PositionAndSize.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        last_pos = new Rectangle(scroll.PositionAndSize.Left, scroll.PositionAndSize.Bottom + 5, last_pos.Width, 0);

                        scroll = new UIScrollBar(game);
                        scroll.Name = "scrollSFX";
                        scroll.Enabled = GameSettings.audioEnabled;
                        scroll.IsHorizontal = true;
                        scroll.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, last_pos.Width, 0);
                        scroll.MinimalValue = 0;
                        scroll.MaximalValue = 100;
                        scroll.ValuesOnScreen = 1;
                        scroll.Value = GameSettings.audioSFXVolumePercent;
                        scroll.ValueChanged += (UIScrollBar_ValueChanged)((scrl, old) => {
                            //game.SetupAudio(((UICheckBox)UIParent.UI["cbEnable"]).Checked, ((UIScrollBar)UIParent.UI["scrollVolume"]).Value, ((UIScrollBar)UIParent.UI["scrollMusic"]).Value, ((UIScrollBar)UIParent.UI["scrollSFX"]).Value, ((UIScrollBar)UIParent.UI["scrollUI"]).Value);
                        });
                        UIParent.UI.Add(scroll);
                        label = new UILabel(game);
                        label.Name = "labelSFX";
                        label.AutoSize = true;
                        label.Text = scroll.Value + "%";
                        label.PositionAndSize = new Rectangle(scroll.PositionAndSize.Right + 5, scroll.PositionAndSize.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        last_pos = new Rectangle(scroll.PositionAndSize.Left, scroll.PositionAndSize.Bottom + 5, last_pos.Width, 0);

                        scroll = new UIScrollBar(game);
                        scroll.Name = "scrollUI";
                        scroll.Enabled = GameSettings.audioEnabled;
                        scroll.IsHorizontal = true;
                        scroll.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, last_pos.Width, 0);
                        scroll.MinimalValue = 0;
                        scroll.MaximalValue = 100;
                        scroll.ValuesOnScreen = 1;
                        scroll.Value = GameSettings.audioUIVolumePercent;
                        scroll.ValueChanged += (UIScrollBar_ValueChanged)((scrl, old) => {
                            //game.SetupAudio(((UICheckBox)UIParent.UI["cbEnable"]).Checked, ((UIScrollBar)UIParent.UI["scrollVolume"]).Value, ((UIScrollBar)UIParent.UI["scrollMusic"]).Value, ((UIScrollBar)UIParent.UI["scrollSFX"]).Value, ((UIScrollBar)UIParent.UI["scrollUI"]).Value);
                        });
                        UIParent.UI.Add(scroll);
                        label = new UILabel(game);
                        label.Name = "labelUI";
                        label.AutoSize = true;
                        label.Text = scroll.Value + "%";
                        label.PositionAndSize = new Rectangle(scroll.PositionAndSize.Right + 5, scroll.PositionAndSize.Top + 5, 0, 0);
                        UIParent.UI.Add(label);
                        last_pos = new Rectangle(scroll.PositionAndSize.Left, scroll.PositionAndSize.Bottom + 5, last_pos.Width, 0);

                        button = new UIButton(game);
                        button.Name = "btnAcceptAudioChanges";
                        button.Text = "Save";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            GameSettings.audioEnabled = ((UICheckBox)UIParent.UI["cbEnable"]).Checked;
                            GameSettings.audioVolumePercent = ((UIScrollBar)UIParent.UI["scrollVolume"]).Value;
                            GameSettings.audioMusicVolumePercent = ((UIScrollBar)UIParent.UI["scrollMusic"]).Value;
                            GameSettings.audioSFXVolumePercent = ((UIScrollBar)UIParent.UI["scrollSFX"]).Value;
                            GameSettings.audioUIVolumePercent = ((UIScrollBar)UIParent.UI["scrollUI"]).Value;

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnBack";
                        button.Text = "<-- Back";
                        button.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //game.SetupAudio(GameSettings.audioEnabled, GameSettings.audioVolumePercent, GameSettings.audioMusicVolumePercent, GameSettings.audioSFXVolumePercent, GameSettings.audioUIVolumePercent);

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        UIParent.ESCHook += UIParent_ESCHook2;
                    }
                    break;
                case 3: //common
                    {
                        UIButton button;
                        UICheckBox checkbox;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_otheroptions");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);
                        Rectangle last_pos = new Rectangle(10, logo.PositionAndSize.Bottom + 5, 0, 0);

                        checkbox = new UICheckBox(game);
                        checkbox.Name = "cbMinimap";
                        checkbox.AutoSize = true;
                        checkbox.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, 0, 0);
                        checkbox.Checked = GameSettings.gameoptMinimap;
                        checkbox.Text = "Show minimap";
                        UIParent.UI.Add(checkbox);
                        last_pos = new Rectangle(last_pos.Left, checkbox.PositionAndSize.Bottom + 5, 0, 0);

                        checkbox = new UICheckBox(game);
                        checkbox.Name = "cbNames";
                        checkbox.AutoSize = true;
                        checkbox.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, 0, 0);
                        checkbox.Checked = GameSettings.gameoptNames;
                        checkbox.Text = "Show names";
                        UIParent.UI.Add(checkbox);
                        last_pos = new Rectangle(last_pos.Left, checkbox.PositionAndSize.Bottom + 5, 0, 0);

                        checkbox = new UICheckBox(game);
                        checkbox.Name = "cbPointers";
                        checkbox.AutoSize = true;
                        checkbox.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top, 0, 0);
                        checkbox.Checked = GameSettings.gameoptPointers;
                        checkbox.Text = "Show motor pointers";
                        UIParent.UI.Add(checkbox);

                        button = new UIButton(game);
                        button.Name = "btnAcceptCommonChanges";
                        button.Text = "Save";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            GameSettings.gameoptMinimap = ((UICheckBox)UIParent.UI["cbMinimap"]).Checked;
                            GameSettings.gameoptNames = ((UICheckBox)UIParent.UI["cbNames"]).Checked;
                            GameSettings.gameoptPointers = ((UICheckBox)UIParent.UI["cbPointers"]).Checked;

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        button = new UIButton(game);
                        button.Name = "btnBack";
                        button.Text = "<-- Back";
                        button.PositionAndSize = new Rectangle(5, (600 - 41), 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            UIParent.ClearESCHook();
                            oResult = new GameScreen_Options(game, -1);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(button);

                        UIParent.ESCHook += UIParent_ESCHook2;
                    }
                    break;
            }

            UIParent.UI.LoadAndInitialize();
        }

        void UIParent_ESCHook1()
        {
            UIParent.ClearESCHook();
            oResult = new GameScreen_Main(game);
            iResult = MenuReturnCodes.MenuSwitching;
            Call_OnExit();
        }

        void UIParent_ESCHook2()
        {
            UIParent.ClearESCHook();
            oResult = new GameScreen_Options(game, -1);
            iResult = MenuReturnCodes.MenuSwitching;
            Call_OnExit();
        }
    }
}
