using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Motorki.GameClasses;
using Motorki.UIClasses;

namespace Motorki.GameScreens
{
    public class GameScreen_GameUI : GameScreen_MenuScreen
    {
        int motorID;
        int width, height;

        static SpriteFont gameUIFont = null;
        static SpriteFont gameUIFont_Counters = null;

        /// <param name="motorID">index in gameMotors table of the leading bike for UI</param>
        public GameScreen_GameUI(MotorkiGame game, int motorID, int width, int height)
            : base(game)
        {
            this.motorID = motorID;
            this.width = width;
            this.height = height;
        }

        public override void LoadAndInitialize()
        {
            if (gameUIFont == null) gameUIFont = game.Content.Load<SpriteFont>("gameUIFont");
            if (gameUIFont_Counters == null) gameUIFont_Counters = game.Content.Load<SpriteFont>("gameUIFont_Counters");

            UIProgress hpbar;
            UILabel label;

            hpbar = new UIProgress(game);
            hpbar.Name = "hpbarMotor" + motorID;
            hpbar.Angular = true;
            hpbar.Percent = GameSettings.gameMotors[motorID].HP;
            hpbar.PositionAndSize = new Rectangle(0, 0, 100, 20);
            UIParent.UI.Add(hpbar);
            GameSettings.gameMotors[motorID].HPChanged = (Motorek_HPChanged)((m, old) => {
                ((UIProgress)UIParent.UI["hpbarMotor" + motorID]).Percent = m.HP/100.0f;
            });
            label = new UILabel(game);
            label.Font = gameUIFont;
            label.AutoSize = true;
            label.PositionAndSize = new Rectangle(2, 2, 0, 0);
            label.Text = GameSettings.gameMotors[motorID].name;
            UIParent.UI.Add(label);
            label = new UILabel(game);
            label.Font = gameUIFont;
            label.AutoSize = true;
            label.PositionAndSize = new Rectangle(0, 0, 0, 0);
            label.Text = GameSettings.gameMotors[motorID].name;
            label.fontColor = Color.Red;
            UIParent.UI.Add(label);

            label = new UILabel(game);
            label.Name = "playerCounterShade";
            label.Font = gameUIFont_Counters;
            label.AutoSize = true;
            int counter_height = (int)gameUIFont_Counters.MeasureString("Mg").Y;
            label.PositionAndSize = new Rectangle(2, height - counter_height, 0, 0);
            label.Text = "";
            UIParent.UI.Add(label);
            label = new UILabel(game);
            label.Name = "playerCounterFront";
            label.Font = gameUIFont_Counters;
            label.AutoSize = true;
            label.PositionAndSize = new Rectangle(0, height - counter_height - 2, 0, 0);
            label.Text = "";
            label.fontColor = Color.Red;
            UIParent.UI.Add(label);
            GameSettings.gameMotors[motorID].FragsChanged = FragsPoints_Changed;
            GameSettings.gameMotors[motorID].PointsChanged = FragsPoints_Changed;
            FragsPoints_Changed(GameSettings.gameMotors[motorID], 0); //show initials

            UIParent.ESCHook += UIParent_ESCHook;

            UIParent.UI.LoadAndInitialize();
            game.mCursor.Visible = false;
            GameSettings.gamePlayScreen1.gameFinished = UIParent_ESCHook;
        }

        void FragsPoints_Changed(Motorek m, int old_value)
        {
            string new_text = "";

            int bikesCount = 0; //total number of bikes for demolition
            int aliveBikesCount = 0; //number of alive bikes for demolition
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if (GameSettings.gameMotors[i] != null)
                {
                    bikesCount++;
                    if (GameSettings.gameMotors[i].HP > 0)
                        aliveBikesCount++;
                }

            int teamID = motorID / (GameSettings.gameMotors.Length / 2);

            int teamMembersCount = 0; //number of alive team members for team demolition
            int enemyTeamMembersCount = 0; //number of alive enemy team members for team demolition
            int teamFragsCount = 0; //total count of frags in team
            int teamPointsCount = 0; //total count of points in team
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
            {
                int testingID = (GameSettings.gameMotors.Length / 2) * teamID + i;
                if (GameSettings.gameMotors[testingID] != null)
                {
                    teamFragsCount += GameSettings.gameMotors[testingID].FragsCount;
                    teamPointsCount += GameSettings.gameMotors[testingID].PointsCount;
                    if (GameSettings.gameMotors[testingID].HP > 0)
                        teamMembersCount++;
                }
                int enemyTestingID = (GameSettings.gameMotors.Length / 2) * (1 - teamID) + i;
                if ((GameSettings.gameMotors[enemyTestingID] != null) && (GameSettings.gameMotors[enemyTestingID].HP > 0))
                    enemyTeamMembersCount++;
            }
            int time_left = (int)(GameSettings.gameTimeLimit * 60 - (GameSettings.gamePlayScreen1.currentTime - GameSettings.gamePlayScreen1.startTime));

            switch (GameSettings.gameType)
            {
                case GameType.DeathMatch: new_text = GameSettings.gameMotors[motorID].FragsCount.ToString("000") + "/" + GameSettings.gameFragLimit.ToString("000"); break;
                case GameType.Demolition: new_text = aliveBikesCount.ToString("00") + "/" + bikesCount.ToString("00"); break;
                case GameType.PointMatch: new_text = GameSettings.gameMotors[motorID].PointsCount.ToString("000000") + "/" + GameSettings.gamePointLimit.ToString("000000"); break;
                case GameType.TimeMatch: new_text = (time_left / 60).ToString("00") + ":" + (time_left % 60).ToString("00"); break;
                case GameType.TeamDeathMatch: new_text = teamFragsCount.ToString("000") + " [" + GameSettings.gameMotors[motorID].FragsCount.ToString("000") + "]/" + GameSettings.gameFragLimit.ToString("000"); break;
                case GameType.TeamDemolition: new_text = teamMembersCount.ToString("0") + " - " + enemyTeamMembersCount.ToString("0"); break;
                case GameType.TeamPointMatch: new_text = teamPointsCount.ToString("000000") + " [" + GameSettings.gameMotors[motorID].PointsCount.ToString("000000") + "]/" + GameSettings.gamePointLimit.ToString("000000"); break;
                case GameType.TeamTimeMatch: new_text = (time_left / 60).ToString("00") + ":" + (time_left % 60).ToString("00"); break;
            }
            UIParent.UI["playerCounterShade"].Text = new_text;
            UIParent.UI["playerCounterFront"].Text = new_text;
        }

        void UIParent_ESCHook()
        {
            UIParent.ClearESCHook();
            game.mCursor.Visible = true;
            GameSettings.gameMotors[motorID].HPChanged = null;
            GameSettings.gamePlayScreen1.Destroy();

            oResult = null;
            iResult = MenuReturnCodes.MenuStartRequested;
            Call_OnExit();
        }
    }
}
