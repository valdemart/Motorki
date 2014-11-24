using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Motorki.GameClasses;
using Motorki.UIClasses;
using System;

namespace Motorki.GameScreens
{
    public class GameScreen_NewGame : GameScreen_MenuScreen
    {
        int subscreen;

        /// <param name="subscreen">0 - game settings, 1 - game slots settings</param>
        public GameScreen_NewGame(MotorkiGame game, int subscreen = 0)
            : base(game)
        {
            this.subscreen = subscreen;
        }

        public override void LoadAndInitialize()
        {
            UIParent.UI.Clear();

            switch (subscreen)
            {
                case 0: //common game settings
                    {
                        UIButton button;
                        UILabel label;
                        UITextBox textbox;
                        UIComboBox combo;
                        UICheckBox check;
                        UIImage logo;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_newgame");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, logo.PositionAndSize.Bottom + 10, 0, 0);
                        label.Text = "Game name: ";
                        UIParent.UI.Add(label);

                        textbox = new UITextBox(game);
                        textbox.Name = "tboxGameName";
                        textbox.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 200, 0);
                        textbox.TextLenghtLimit = 15;
                        textbox.Text = GameSettings.gameName;
                        UIParent.UI.Add(textbox);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, textbox.PositionAndSize.Bottom + 15, 0, 0);
                        label.Text = "Game type: ";
                        UIParent.UI.Add(label);

                        combo = new UIComboBox(game);
                        combo.Name = "cboxGameType";
                        combo.Edible = false;
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 200, 0);
                        combo.Values.Add(new UITaggedValue("deathmatch", GameType.DeathMatch));
                        combo.Values.Add(new UITaggedValue("demolition", GameType.Demolition));
                        combo.Values.Add(new UITaggedValue("point match", GameType.PointMatch));
                        combo.Values.Add(new UITaggedValue("time match", GameType.TimeMatch));
                        combo.Values.Add(new UITaggedValue("team deathmatch", GameType.TeamDeathMatch));
                        combo.Values.Add(new UITaggedValue("team demolition", GameType.TeamDemolition));
                        combo.Values.Add(new UITaggedValue("team point match", GameType.TeamPointMatch));
                        combo.Values.Add(new UITaggedValue("team time match", GameType.TeamTimeMatch));
                        combo.SelectedIndex = -1;
                        combo.SelectionChanged += (UIComboBox_SelectionChanged)((cbox, old) => {
                            switch ((GameType)cbox.SelectedItem.Tag)
                            {
                                case GameType.DeathMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = true;
                                    UIParent.UI["labelGameDescription"].Text = "Destroy specified number of enemy bikes";
                                    UIParent.UI["tboxFragLimit"].Enabled = true;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.Demolition:
                                    UIParent.UI["btnProceedGame"].Enabled = true;
                                    UIParent.UI["labelGameDescription"].Text = "Destroy all enemy bikes";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.PointMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = true;
                                    UIParent.UI["labelGameDescription"].Text = "Gather specified amount of points by destroying enemy bikes";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = true;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.TimeMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = true;
                                    UIParent.UI["labelGameDescription"].Text = "Gather as much points as possible in specified amount of time";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = true;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.TeamDeathMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = true;
                                    UIParent.UI["labelGameDescription"].Text = "Destroy specified number of bikes from opposite team";
                                    UIParent.UI["tboxFragLimit"].Enabled = true;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.TeamDemolition:
                                    UIParent.UI["btnProceedGame"].Enabled = false;
                                    UIParent.UI["labelGameDescription"].Text = "(locked) Destroy all bikes from opposite team";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.TeamPointMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = false;
                                    UIParent.UI["labelGameDescription"].Text = "(locked) Help your team gather specified amount of points by destroying bikes from opposite team";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = true;
                                    UIParent.UI["tboxTimeLimit"].Enabled = false;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                                case GameType.TeamTimeMatch:
                                    UIParent.UI["btnProceedGame"].Enabled = false;
                                    UIParent.UI["labelGameDescription"].Text = "(locked) Help your team gather as much points as possible in specified amount of time";
                                    UIParent.UI["tboxFragLimit"].Enabled = false;
                                    UIParent.UI["tboxPointLimit"].Enabled = false;
                                    UIParent.UI["tboxTimeLimit"].Enabled = true;
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Clear();
                                    foreach (UITaggedValue map in Map.EnumerateMaps((GameType)cbox.SelectedItem.Tag))
                                        ((UIComboBox)UIParent.UI["cboxMapName"]).Values.Add(map);
                                    ((UIComboBox)UIParent.UI["cboxMapName"]).SelectedIndex = 0;
                                    break;
                            }
                        });
                        UIParent.UI.Add(combo);

                        label = new UILabel(game);
                        label.Name = "labelGameDescription";
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(30, combo.PositionAndSize.Bottom + 5, 0, 0);
                        label.Text = "<game description>";
                        UIParent.UI.Add(label);
                        Rectangle last_pos = new Rectangle(10, label.PositionAndSize.Bottom + 10, 0, 0);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, last_pos.Top + 5, 0, 0);
                        label.Text = "Map: ";
                        UIParent.UI.Add(label);

                        combo = new UIComboBox(game);
                        combo.Name = "cboxMapName";
                        combo.Edible = false;
                        combo.MaxDisplayedItems = 10;
                        combo.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, last_pos.Top, 200, 0);
                        combo.SelectedIndex = -1;
                        UIParent.UI.Add(combo);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, combo.PositionAndSize.Bottom + 10, 0, 0);
                        label.Text = "Destroy limit: ";
                        UIParent.UI.Add(label);

                        textbox = new UITextBox(game);
                        textbox.Name = "tboxFragLimit";
                        textbox.Enabled = false;
                        textbox.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 100, 0);
                        textbox.TextLenghtLimit = 3;
                        textbox.CharacterFilter = (UITextBox.CharacterFilterFunction)((ch) =>
                        {
                            return (ch >= '0') && (ch <= '9');
                        });
                        textbox.Text = "" + GameSettings.gameFragLimit;
                        UIParent.UI.Add(textbox);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, textbox.PositionAndSize.Bottom + 10, 0, 0);
                        label.Text = "Point limit: ";
                        UIParent.UI.Add(label);

                        textbox = new UITextBox(game);
                        textbox.Name = "tboxPointLimit";
                        textbox.Enabled = false;
                        textbox.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 100, 0);
                        textbox.TextLenghtLimit = 6;
                        textbox.CharacterFilter = (UITextBox.CharacterFilterFunction)((ch) =>
                        {
                            return (ch >= '0') && (ch <= '9');
                        });
                        textbox.Text = "" + GameSettings.gamePointLimit;
                        UIParent.UI.Add(textbox);

                        label = new UILabel(game);
                        label.AutoSize = true;
                        label.PositionAndSize = new Rectangle(10, textbox.PositionAndSize.Bottom + 10, 0, 0);
                        label.Text = "Time limit: ";
                        UIParent.UI.Add(label);

                        textbox = new UITextBox(game);
                        textbox.Name = "tboxTimeLimit";
                        textbox.Enabled = false;
                        textbox.PositionAndSize = new Rectangle(label.PositionAndSize.Right + 5, label.PositionAndSize.Top - 5, 100, 0);
                        textbox.TextLenghtLimit = 3;
                        textbox.CharacterFilter = (UITextBox.CharacterFilterFunction)((ch) =>
                        {
                            return (ch >= '0') && (ch <= '9');
                        });
                        textbox.Text = "" + GameSettings.gameTimeLimit;
                        UIParent.UI.Add(textbox);

                        check = new UICheckBox(game);
                        check.Name = "cbTwoPlayer";
                        check.Enabled = false; //!!!
                        check.AutoSize = true;
                        check.PositionAndSize = new Rectangle(label.PositionAndSize.Left, textbox.PositionAndSize.Bottom + 5, 0, 0);
                        check.Text = "Start two-player game";
                        UIParent.UI.Add(check);

                        button = new UIButton(game);
                        button.Name = "btnProceedGame";
                        button.Text = "Next -->";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            GameSettings.gameServerIP = "127.0.0.1";
                            GameSettings.gameName = UIParent.UI["tboxGameName"].Text;
                            GameSettings.gameType = (GameType)((UIComboBox)UIParent.UI["cboxGameType"]).SelectedItem.Tag;
                            GameSettings.SelectMap((string)((UIComboBox)UIParent.UI["cboxMapName"]).SelectedItem.Tag);
                            try { GameSettings.gameFragLimit = int.Parse(UIParent.UI["tboxFragLimit"].Text); }
                            catch (Exception) { GameSettings.gameFragLimit = 20; }
                            try { GameSettings.gamePointLimit = int.Parse(UIParent.UI["tboxPointLimit"].Text); }
                            catch (Exception) { GameSettings.gamePointLimit = 1000; }
                            try { GameSettings.gameTimeLimit = int.Parse(UIParent.UI["tboxTimeLimit"].Text); }
                            catch (Exception) { GameSettings.gameTimeLimit = 120; }
                            GameSettings.gameTwoPlayer = ((UICheckBox)UIParent.UI["cbTwoPlayer"]).Checked;

                            UIParent.ClearESCHook();
                            oResult = new GameScreen_NewGame(game, 1);
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

                        //select game type
                        for (int i = 0; i < ((UIComboBox)UIParent.UI["cboxGameType"]).Values.Count; i++)
                            if ((GameType)((UIComboBox)UIParent.UI["cboxGameType"]).Values[i].Tag == GameSettings.gameType)
                            {
                                ((UIComboBox)UIParent.UI["cboxGameType"]).SelectedIndex = i;
                                break;
                            }
                    }
                    break;
                case 1: //game slots settings
                    {
                        UIButton button;
                        UILabel label;
                        UIComboBox combo;
                        UIImage logo;

                        bool team_game = (GameSettings.gameType == GameType.TeamDeathMatch) || (GameSettings.gameType == GameType.TeamDemolition) || (GameSettings.gameType == GameType.TeamPointMatch) || (GameSettings.gameType == GameType.TeamTimeMatch);
                        int enabledSlotsCount = 0;
                        int enabledSlotsLimit = GameSettings.gameMap.Parameters.MaxPlayers;
                        int enabledTeamSlotsLimit = GameSettings.gameMap.Parameters.MaxPlayers / 2;

                        logo = new UIImage(game);
                        logo.Textures = game.Content.Load<Texture2D>("logo_newgame");
                        logo.NormalTexture = new Rectangle(0, 0, 500, 75);
                        logo.PositionAndSize = new Rectangle(400 - 250, 20, 500, 75);
                        UIParent.UI.Add(logo);

                        for (int _t = 0; _t < 2; _t++)
                        {
                            if (team_game)
                                enabledSlotsCount = 0;

                            Rectangle last_pos = new Rectangle(10 + 400 * _t, logo.PositionAndSize.Bottom + 5, 40 + 400 * _t, 195 + 400 * _t);

                            if (team_game)
                            {
                                label = new UILabel(game);
                                label.AutoSize = true;
                                label.PositionAndSize = new Rectangle(last_pos.Left, logo.PositionAndSize.Bottom + 5, 0, 0);
                                label.Text = (_t == 0 ? "Red team" : "Blue team");
                                UIParent.UI.Add(label);
                                last_pos = new Rectangle(last_pos.Left + 10, label.PositionAndSize.Bottom + 5, last_pos.Width, last_pos.Height);
                            }

                            for (int _m = 0; _m < 5; _m++)
                            {
                                label = new UILabel(game);
                                label.AutoSize = true;
                                label.PositionAndSize = new Rectangle(last_pos.Left, last_pos.Top + 5, 0, 0);
                                if (team_game)
                                    label.Text = (_m + 1) + ": ";
                                else
                                    label.Text = (_t * 5 + _m + 1) + ": ";
                                UIParent.UI.Add(label);
                                combo = new UIComboBox(game);
                                combo.Name = "t" + (_t + 1) + "m" + (_m + 1) + "Name";
                                combo.Edible = false;
                                combo.Enabled = false;
                                combo.MaxDisplayedItems = 5;
                                combo.PositionAndSize = new Rectangle(last_pos.Width, last_pos.Top, last_pos.Height - last_pos.Width - 5, 0);
                                UIParent.UI.Add(combo);
                                button = new UIButton(game);
                                button.Name = "t" + (_t + 1) + "m" + (_m + 1) + "Type";
                                button.Enabled = (enabledSlotsCount < (team_game ? enabledTeamSlotsLimit : enabledSlotsLimit));
                                enabledSlotsCount += (button.Enabled ? 1 : 0);
                                button.PositionAndSize = new Rectangle(last_pos.Height, last_pos.Top, 80, combo.PositionAndSize.Height);
                                button.Text = "None";
                                button.Action += (UIButton_Action)((btn) =>
                                {
                                    switch (btn.Text)
                                    {
                                        case "None":
                                            btn.Text = "Player";
                                            UIParent.UI["btnProceedGame"].Enabled = true;
                                            UIParent.UI[btn.Name.Substring(0, 4) + "Name"].Enabled = true;
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Clear();
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Add(new UITaggedValue("<empty>", -1));
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Add(new UITaggedValue(GameSettings.player1Name, 0));
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Add(new UITaggedValue(GameSettings.player2Name, 1));
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).SelectedIndex = 0;
                                            break;
                                        case "Player":
                                            btn.Text = "Bot";
                                            UIParent.UI["btnProceedGame"].Enabled = true;
                                            UIParent.UI[btn.Name.Substring(0, 4) + "Name"].Enabled = true;
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Clear();
                                            for (int i = 0; i < Enum.GetNames(typeof(Default_BotNames)).Length; i++)
                                                ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Add(new UITaggedValue(Enum.GetNames(typeof(Default_BotNames))[i]));
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).SelectedIndex = 0;
                                            break;
                                        case "Bot":
                                            btn.Text = "None";
                                            int count = 0;
                                            for (int t = 1; t <= 2; t++)
                                                for (int m = 1; m <= 5; m++)
                                                    if (UIParent.UI["t" + t + "m" + m + "Type"].Text == "None")
                                                        count++;
                                            if (count == 10)
                                                UIParent.UI["btnProceedGame"].Enabled = false;
                                            UIParent.UI[btn.Name.Substring(0, 4) + "Name"].Enabled = false;
                                            ((UIComboBox)UIParent.UI[btn.Name.Substring(0, 4) + "Name"]).Values.Clear();
                                            break;
                                    }
                                });
                                UIParent.UI.Add(button);
                                last_pos = new Rectangle(last_pos.Left, button.PositionAndSize.Bottom + 5, last_pos.Width, last_pos.Height);
                            }
                        }

                        button = new UIButton(game);
                        button.Name = "btnProceedGame";
                        button.Enabled = false;
                        button.Text = "Start";
                        button.PositionAndSize = new Rectangle(800 - 105, 600 - 41, 100, 37);
                        button.Action += (UIButton_Action)((btn) =>
                        {
                            //save changes
                            for(int i=0; i<5; i++)
                            {
                                switch (UIParent.UI["t1m" + (i + 1) + "Type"].Text)
                                {
                                    case "None":
                                        GameSettings.gameSlots[i] = null;
                                        break;
                                    case "Player":
                                        GameSettings.gameSlots[i] = new MotorShortDescription(typeof(PlayerMotor), ((UIComboBox)UIParent.UI["t1m" + (i + 1) + "Name"]).SelectedItem.Text, (int)((UIComboBox)UIParent.UI["t1m" + (i + 1) + "Name"]).SelectedItem.Tag);
                                        break;
                                    case "Bot":
                                        GameSettings.gameSlots[i] = new MotorShortDescription(typeof(BotMotor), ((UIComboBox)UIParent.UI["t1m" + (i + 1) + "Name"]).SelectedItem.Text, -1);
                                        break;
                                }
                                switch (UIParent.UI["t2m" + (i + 1) + "Type"].Text)
                                {
                                    case "None":
                                        GameSettings.gameSlots[5 + i] = null;
                                        break;
                                    case "Player":
                                        GameSettings.gameSlots[5 + i] = new MotorShortDescription(typeof(PlayerMotor), ((UIComboBox)UIParent.UI["t2m" + (i + 1) + "Name"]).SelectedItem.Text, (int)((UIComboBox)UIParent.UI["t2m" + (i + 1) + "Name"]).SelectedItem.Tag);
                                        break;
                                    case "Bot":
                                        GameSettings.gameSlots[5 + i] = new MotorShortDescription(typeof(BotMotor), ((UIComboBox)UIParent.UI["t2m" + (i + 1) + "Name"]).SelectedItem.Text, -1);
                                        break;
                                }
                            }

                            UIParent.ClearESCHook();
                            oResult = null;/*new GameScreen_Misc(game, 0);*/
                            iResult = MenuReturnCodes.GameStartRequested;
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
                            oResult = new GameScreen_NewGame(game, 0);
                            iResult = MenuReturnCodes.MenuSwitching;
                            Call_OnExit();
                        });
                        UIParent.UI.Add(btnBack);

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
            oResult = new GameScreen_NewGame(game, 0);
            iResult = MenuReturnCodes.MenuSwitching;
            Call_OnExit();
        }
    }
}
