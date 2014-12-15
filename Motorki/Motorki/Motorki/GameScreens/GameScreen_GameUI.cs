using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            //create game UI
            UIProgress hpbar;
            UILabel label;

            //player hp bar and name
            hpbar = new UIProgress(game);
            hpbar.Name = "hpbarMotor" + motorID;
            hpbar.Angular = true;
            hpbar.Percent = GameSettings.gameMotors[motorID].HP;
            hpbar.PositionAndSize = new Rectangle(0, 0, 100, 20);
            UIParent.UI.Add(hpbar);
            GameSettings.gameMotors[motorID].HPChanged = (Motorek_HPChanged)((m, old) => { //connect hp bar to player hp change event
                ((UIProgress)UIParent.UI["hpbarMotor" + motorID]).Percent = m.HP / 100.0f;
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
            label.fontColor = motorID >= 5 ? Color.Blue : Color.Red;
            UIParent.UI.Add(label);

            //teammates hp bars and names

            //minimap
            
            //frag/point/time counter
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
            label.fontColor = motorID >= 5 ? Color.Blue : Color.Red;
            UIParent.UI.Add(label);
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if (GameSettings.gameMotors[i] != null)
                {
                    GameSettings.gameMotors[i].FragsChanged = FragsPoints_Changed;
                    GameSettings.gameMotors[i].PointsChanged = FragsPoints_Changed;
                }
            FragsPoints_Changed(GameSettings.gameMotors[motorID], 0); //show initials

            UIParent.ESCHook += UIParent_ESCHook;

            UIParent.UI.LoadAndInitialize();
            game.mCursor.Visible = false;
            GameSettings.gamePlayScreen.gameFinished = UIParent_ESCHook;
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
            int enemyTeamFragsCount = 0; //total count of frags in enemy team
            int teamPointsCount = 0; //total count of points in team
            int enemyTeamPointsCount = 0; //total count of points in enemy team
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
                if (GameSettings.gameMotors[enemyTestingID] != null)
                {
                    enemyTeamFragsCount += GameSettings.gameMotors[enemyTestingID].FragsCount;
                    enemyTeamPointsCount += GameSettings.gameMotors[enemyTestingID].PointsCount;
                    if (GameSettings.gameMotors[enemyTestingID].HP > 0)
                        enemyTeamMembersCount++;
                }
            }
            int time_left = (int)(GameSettings.gameTimeLimit * 60 - (GameSettings.gamePlayScreen.currentTime - GameSettings.gamePlayScreen.startTime));

            //some statistics
            int leader_frags = GameSettings.gameMotors[motorID].FragsCount - GameSettings.gameMotors.Max((_) => _ != null && _ != GameSettings.gameMotors[motorID] ? _.FragsCount : 0);
            int leader_points = GameSettings.gameMotors[motorID].PointsCount - GameSettings.gameMotors.Max((_) => _ != null && _ != GameSettings.gameMotors[motorID] ? _.PointsCount : 0);
            int team_leader_frags = teamFragsCount - enemyTeamFragsCount;
            int team_leader_points = teamPointsCount - enemyTeamPointsCount;

            switch (GameSettings.gameType)
            {
                case GameType.DeathMatch: new_text = GameSettings.gameMotors[motorID].FragsCount.ToString("000") + "/" + GameSettings.gameFragLimit.ToString("000") + (leader_frags > 0 ? " (you are leading by +" + leader_frags + ")" : leader_frags == 0 ? " (tie)" : " (you are losing by " + leader_frags + ")"); break;
                case GameType.Demolition: new_text = aliveBikesCount.ToString("00") + " of " + bikesCount.ToString("00") + " alive"; break;
                case GameType.PointMatch: new_text = GameSettings.gameMotors[motorID].PointsCount.ToString("000000") + "/" + GameSettings.gamePointLimit.ToString("000000") + (leader_points > 0 ? " (you are leading by +" + leader_points + ")" : leader_points == 0 ? " (tie)" : " (you are losing by " + leader_points + ")"); break;
                case GameType.TimeMatch: new_text = (time_left / 60).ToString("00") + ":" + (time_left % 60).ToString("00") + (leader_points > 0 ? " (you are leading by +" + leader_points + " points)" : leader_points == 0 ? " (tie)" : " (you are losing by " + leader_points + " points)"); break;
                case GameType.TeamDeathMatch: new_text = teamFragsCount.ToString("000") + " [" + GameSettings.gameMotors[motorID].FragsCount.ToString("000") + " yours]/" + GameSettings.gameFragLimit.ToString("000") + (team_leader_frags > 0 ? " (your team is leading with +" + team_leader_frags + ")" : team_leader_frags == 0 ? " (tie)" : " (your team is losing with " + team_leader_frags + ")"); break;
                case GameType.TeamDemolition: new_text = teamMembersCount.ToString("0") + " alive - " + enemyTeamMembersCount.ToString("0") + " alive" + (teamMembersCount > enemyTeamMembersCount ? " (your team is leading)" : teamMembersCount == enemyTeamMembersCount ? " (tie)" : " (your team is losing)"); break;
                case GameType.TeamPointMatch: new_text = teamPointsCount.ToString("000000") + " [" + GameSettings.gameMotors[motorID].PointsCount.ToString("000000") + " yours]/" + GameSettings.gamePointLimit.ToString("000000") + (team_leader_points > 0 ? " (your team is leading by +" + team_leader_points + ")" : team_leader_points == 0 ? " (tie)" : " (your team is losing by " + team_leader_points + ")"); break;
                case GameType.TeamTimeMatch: new_text = (time_left / 60).ToString("00") + ":" + (time_left % 60).ToString("00") + (team_leader_points > 0 ? " (your team is leading by +" + team_leader_points + " points)" : team_leader_points == 0 ? " (tie)" : " (your team is losing by " + team_leader_points + " points)"); break;
            }
            UIParent.UI["playerCounterShade"].Text = new_text;
            UIParent.UI["playerCounterFront"].Text = new_text;
        }

        void UIParent_ESCHook()
        {
            UIParent.ClearESCHook();
            game.mCursor.Visible = true;
            GameSettings.gameMotors[motorID].HPChanged = null;
            GameSettings.gamePlayScreen.Destroy();

            oResult = null;
            iResult = MenuReturnCodes.MenuStartRequested;
            Call_OnExit();
        }
    }
}
