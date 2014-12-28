using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motorki.GameClasses
{
    /// <summary>
    /// local game controller for both players (currently only single player supported)
    /// </summary>
    public class GamePlay
    {
        private delegate bool GameFinishConditionCheck();
        public delegate void GameFinished();

        public bool gameStarted { get; private set; }
        private int cameraX { get; set; }
        private int cameraY { get; set; }
        private int cameraStickBikeID { get; set; }
        /// <summary>
        /// total number of seconds on a time of starting GamePlay
        /// </summary>
        public double startTime { get; private set; }
        /// <summary>
        /// current total number of seconds
        /// </summary>
        public double currentTime { get; private set; }

        private GameFinishConditionCheck winningFinish;
        private GameFinishConditionCheck losingFinish;
        public GameFinished gameFinished;

        public GamePlay()
        {
            gameFinished = null;
            winningFinish = null;
            losingFinish = null;
        }

        private Color GetBotBikeColor()
        {
            int r = MotorkiGame.random.Next() % 7;
            switch (r)
            {
                case 0: return Color.White;
                case 1: return Color.Red;
                case 2: return Color.Green;
                case 3: return Color.Blue;
                case 4: return Color.Cyan;
                case 5: return Color.Magenta;
                case 6: return Color.Yellow;
            }
            return Color.Black;
        }

        /// <summary>
        /// starts game depending on current GameSettings properties
        /// </summary>
        public void LoadAndInitialize(GameTime gameTime, int cameraStickBikeID)
        {
            //setup camera
            cameraX = cameraY = 0;
            this.cameraStickBikeID = cameraStickBikeID;

            //do some research about game
            bool team_game = false;
            switch(GameSettings.gameType)
            {
                case GameType.DeathMatch:
                    winningFinish = DeathMatchWinning;
                    losingFinish = DeathMatchLosing;
                    break;
                case GameType.Demolition:
                    winningFinish = DemolitionWinning;
                    losingFinish = DemolitionLosing;
                    break;
                case GameType.PointMatch:
                    winningFinish = PointMatchWinning;
                    losingFinish = PointMatchLosing;
                    break;
                case GameType.TimeMatch:
                    winningFinish = TimeMatchWinning;
                    losingFinish = TimeMatchLosing;
                    break;
                case GameType.TeamDeathMatch:
                    team_game = true;
                    winningFinish = TeamDeathMatchWinning;
                    losingFinish = TeamDeathMatchLosing;
                    break;
                case GameType.TeamDemolition:
                    team_game = true;
                    winningFinish = TeamDemolitionWinning;
                    losingFinish = TeamDemolitionLosing;
                    break;
                case GameType.TeamPointMatch:
                    team_game = true;
                    winningFinish = TeamPointMatchWinning;
                    losingFinish = TeamPointMatchLosing;
                    break;
                case GameType.TeamTimeMatch:
                    team_game = true;
                    winningFinish = TeamTimeMatchWinning;
                    losingFinish = TeamTimeMatchLosing;
                    break;
            }

            //map parameters already loaded during new game/join game setup

            //setup bikes
            bool need_agentcontroller = false;
            for (int i = 0; i < GameSettings.gameSlots.Length; i++)
                if (GameSettings.gameSlots[i] != null)
                {
                    if (GameSettings.gameSlots[i].type == typeof(BotMotor))
                    {
                        GameSettings.gameMotors[i] = new BotMotor(MotorkiGame.game, (team_game ? (i / 5 == 0 ? Color.Red : Color.Blue) : GetBotBikeColor()), (BotMotor.BotSophistication)GameSettings.gameSlots[i].playerID);
                        GameSettings.gameMotors[i].name = GameSettings.gameSlots[i].name;
                        if ((BotMotor.BotSophistication)GameSettings.gameSlots[i].playerID != BotMotor.BotSophistication.Easy)
                        {
                            need_agentcontroller = true;
                        }
                    }
                    else if ((GameSettings.gameSlots[i].type == typeof(PlayerMotor)) && (GameSettings.gameSlots[i].playerID != -1))
                    {
                        GameSettings.gameMotors[i] = new PlayerMotor(MotorkiGame.game, GameSettings.gameSlots[i].playerID, (team_game ? (i / 5 == 0 ? Color.Red : Color.Blue) : (GameSettings.gameSlots[i].playerID == 0 ? GameSettings.playerColor : GetBotBikeColor())));
                        GameSettings.gameMotors[i].name = (GameSettings.gameSlots[i].playerID == 0 ? GameSettings.playerName : "<network>");
                    }
                }
                else
                    GameSettings.gameMotors[i] = null;
            if (need_agentcontroller)
            {
                GameSettings.agentController = new AgentController();
            }

            //load map data and initialize it
            GameSettings.gameMap.LoadAndInitialize();
            //load and initialize bikes
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if (GameSettings.gameMotors[i] != null)
                {
                    Vector2 pos;
                    float rot;
                    if (!GameSettings.gameMap.GetBikeSpawnData((team_game ? i / 5 : -1), out pos, out rot))
                        throw new Exception("No spawn point found!!");
                    GameSettings.gameMotors[i].LoadAndInitialize(new Rectangle(0, 0, (int)GameSettings.gameMap.Parameters.Size.X, (int)GameSettings.gameMap.Parameters.Size.Y));
                    GameSettings.gameMotors[i].Spawn(pos, rot);
                }

            //confirm game started
            currentTime = startTime = gameTime.TotalGameTime.TotalSeconds;
            gameStarted = true;
        }

        public void Destroy()
        {
            gameStarted = false;
            if (GameSettings.agentController != null)
            {
                GameSettings.agentController.KillAgentController();
                GameSettings.agentController = null;
            }
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if (GameSettings.gameMotors[i] != null)
                {
                    GameSettings.gameMotors[i].Destroy();
                    GameSettings.gameMotors[i] = null;
                }
        }

        #region game winning/losing condition tests

        private bool DeathMatchWinning()
        {
            //check stick bike for winning conditions
            if (GameSettings.gameMotors[cameraStickBikeID].FragsCount >= GameSettings.gameFragLimit)
                return true;
            return false;
        }

        private bool DeathMatchLosing()
        {
            //check stick bike for losing conditions
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if ((GameSettings.gameMotors[i] != null) && (i != cameraStickBikeID) && (GameSettings.gameMotors[i].FragsCount >= GameSettings.gameFragLimit))
                    return true;
            return false;
        }

        private bool DemolitionWinning()
        {
            //check stick bike for game continue conditions
            if (GameSettings.gameMotors[cameraStickBikeID].HP > 0)
            {
                for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                    if ((GameSettings.gameMotors[i] != null) && (i != cameraStickBikeID) && (GameSettings.gameMotors[i].HP > 0))
                        return false;
                return true;
            }
            else
                return false;
        }

        private bool DemolitionLosing()
        {
            //check stick bike for losing conditions
            if (GameSettings.gameMotors[cameraStickBikeID].HP <= 0)
                return true;
            return false;
        }

        private bool PointMatchWinning()
        {
            //check stick bike for winning conditions
            if (GameSettings.gameMotors[cameraStickBikeID].PointsCount >= GameSettings.gamePointLimit)
                return true;
            return false;
        }

        private bool PointMatchLosing()
        {
            //check stick bike for losing conditions
            for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                if ((GameSettings.gameMotors[i] != null) && (i != cameraStickBikeID) && (GameSettings.gameMotors[i].PointsCount >= GameSettings.gamePointLimit))
                    return true;
            return false;
        }

        private bool TimeMatchWinning()
        {
            //check is time out
            if ((currentTime - startTime) / 60.0 >= GameSettings.gameTimeLimit)
            {
                int stickbikePoints = GameSettings.gameMotors[cameraStickBikeID].PointsCount;
                for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                    if ((GameSettings.gameMotors[i] != null) && (i != cameraStickBikeID) && (GameSettings.gameMotors[i].PointsCount > stickbikePoints))
                        return false;
                return true;
            }
            else
                return false;
        }

        private bool TimeMatchLosing()
        {
            //check is time out
            if ((currentTime - startTime) / 60.0 >= GameSettings.gameTimeLimit)
            {
                int stickbikePoints = GameSettings.gameMotors[cameraStickBikeID].PointsCount;
                for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                    if ((GameSettings.gameMotors[i] != null) && (i != cameraStickBikeID) && (GameSettings.gameMotors[i].PointsCount >= stickbikePoints))
                        return true;
                return false;
            }
            else
                return false;
        }

        private bool TeamDeathMatchWinning()
        {
            //check stick bike for winning conditions
            int team = cameraStickBikeID / (GameSettings.gameMotors.Length / 2);
            int sum = 0;
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null)
                    sum += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].FragsCount;
            if (sum >= GameSettings.gameFragLimit)
                return true;
            return false;
        }

        private bool TeamDeathMatchLosing()
        {
            //check stick bike for losing conditions
            int team = 1 - (cameraStickBikeID / (GameSettings.gameMotors.Length / 2));
            int sum = 0;
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null)
                    sum += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].FragsCount;
            if (sum >= GameSettings.gameFragLimit)
                return true;
            return false;
        }

        private bool TeamDemolitionWinning()
        {
            //check stick bike for game continue conditions
            int team = 1 - (cameraStickBikeID / (GameSettings.gameMotors.Length / 2));
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if ((GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null) && (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].HP > 0))
                    return false;
            return true;
        }

        private bool TeamDemolitionLosing()
        {
            //check stick bike for losing conditions
            int team = cameraStickBikeID / (GameSettings.gameMotors.Length / 2);
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if ((GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null) && (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].HP > 0))
                    return false;
            return true;
        }

        private bool TeamPointMatchWinning()
        {
            //check stick bike for winning conditions
            int team = cameraStickBikeID / (GameSettings.gameMotors.Length / 2);
            int sum = 0;
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null)
                    sum += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].PointsCount;
            if (sum >= GameSettings.gamePointLimit)
                return true;
            return false;
        }

        private bool TeamPointMatchLosing()
        {
            //check stick bike for losing conditions
            int team = 1 - (cameraStickBikeID / (GameSettings.gameMotors.Length / 2));
            int sum = 0;
            for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i] != null)
                    sum += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 * team + i].PointsCount;
            if (sum >= GameSettings.gamePointLimit)
                return true;
            return false;
        }

        private bool TeamTimeMatchWinning()
        {
            //check is time out
            if ((currentTime - startTime) / 60.0 >= GameSettings.gameTimeLimit)
            {
                int team = cameraStickBikeID / (GameSettings.gameMotors.Length / 2);
                int sum1 = 0, sum2 = 0;
                for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                {
                    if (GameSettings.gameMotors[i] != null)
                        sum1 += GameSettings.gameMotors[i].PointsCount;
                    if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 + i] != null)
                        sum2 += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 + i].PointsCount;
                }
                if ((team == 0 && sum1 >= sum2) || (team == 1 && sum1 <= sum2))
                    return true;
                return false;
            }
            else
                return false;
        }

        private bool TeamTimeMatchLosing()
        {
            //check is time out
            if ((currentTime - startTime) / 60.0 >= GameSettings.gameTimeLimit)
            {
                int team = cameraStickBikeID / (GameSettings.gameMotors.Length / 2);
                int sum1 = 0, sum2 = 0;
                for (int i = 0; i < GameSettings.gameMotors.Length / 2; i++)
                {
                    if (GameSettings.gameMotors[i] != null)
                        sum1 += GameSettings.gameMotors[i].PointsCount;
                    if (GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 + i] != null)
                        sum2 += GameSettings.gameMotors[GameSettings.gameMotors.Length / 2 + i].PointsCount;
                }
                if ((team == 0 && sum1 <= sum2) || (team == 1 && sum1 >= sum2))
                    return true;
                return false;
            }
            else
                return false;
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (gameStarted)
            {
                //update motors
                if (GameSettings.agentController != null)
                    GameSettings.agentController.SendToAllAgents(new BotAgentMessage(null, null, BotAgentMessages.PauseForControlApplying, ""));
                for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                    if (GameSettings.gameMotors[i] != null)
                        GameSettings.gameMotors[i].Update(gameTime);
                if (GameSettings.agentController != null)
                    GameSettings.agentController.SendToAllAgents(new BotAgentMessage(null, null, BotAgentMessages.ControlsApplied, ""));

                //do some game type research
                bool team_game = (GameSettings.gameType == GameType.TeamDeathMatch) || (GameSettings.gameType == GameType.TeamDemolition) || (GameSettings.gameType == GameType.TeamPointMatch) || (GameSettings.gameType == GameType.TeamTimeMatch);
                bool respawn_allowed = !((GameSettings.gameType == GameType.Demolition) || (GameSettings.gameType == GameType.TeamDemolition));
                bool timed_game = (GameSettings.gameType == GameType.TimeMatch) || (GameSettings.gameType == GameType.TeamTimeMatch);

                //respawn something if allowed
                if (respawn_allowed)
                {
                    for(int i=0; i<GameSettings.gameMotors.Length; i++)
                        if ((GameSettings.gameMotors[i] != null) && (GameSettings.gameMotors[i].HP == 0))
                        {
                            Vector2 pos;
                            float rot;

                            GameSettings.gameMap.GetBikeSpawnData((team_game ? i / 5 : -1), out pos, out rot);
                            GameSettings.gameMotors[i].Spawn(pos, rot);
                        }
                }

                //detect collisions
                for(int i=0; i<GameSettings.gameMotors.Length; i++)
                    if ((GameSettings.gameMotors[i] != null) && (GameSettings.gameMotors[i].HP > 0))
                    {
                        //collisions with map
                        {
                            Vector2 dispVec;
                            float dmg;
                            if (GameSettings.gameMap.TestCollisions(ref GameSettings.gameMotors[i], out dispVec, out dmg))
                            {
                                GameSettings.gameMotors[i].HP -= dmg;
                                GameSettings.gameMotors[i].position += dispVec;
                            }
                        }
                        GameSettings.gameMotors[i].RefreshBoundingPoints();

                        //collisions with enemy bike
                        {
                            //xxxA variables for A->B collisions, xxxB variables for B->A collisions
                            Vector2 dispVecA, dispVecB;
                            float rotA, rotB;
                            float dmgA, dmgB;
                            bool resA, resB;
                            for (int j = (team_game && (i < 5) ? 5 : (team_game ? GameSettings.gameMotors.Length : i + 1)); j < GameSettings.gameMotors.Length; j++)
                                if ((GameSettings.gameMotors[j] != null) && (GameSettings.gameMotors[j].HP > 0))
                                {
                                    resA = GameSettings.gameMotors[i].TestCollisions(ref GameSettings.gameMotors[j], out dispVecA, out rotA, out dmgA);
                                    resB = GameSettings.gameMotors[j].TestCollisions(ref GameSettings.gameMotors[i], out dispVecB, out rotB, out dmgB);
                                    if (resA)
                                    {
                                        GameSettings.gameMotors[j].HP -= dmgA;
                                        GameSettings.gameMotors[j].position += dispVecA;
                                        GameSettings.gameMotors[j].rotation = rotA;
                                        //add points and frags
                                        if (GameSettings.gameMotors[j].HP == 0)
                                            GameSettings.gameMotors[i].FragsCount += 1;
                                        GameSettings.gameMotors[i].PointsCount += (int)(dmgA * 10);
                                    }
                                    if (resB)
                                    {
                                        GameSettings.gameMotors[i].HP -= dmgB;
                                        GameSettings.gameMotors[i].position += dispVecB;
                                        GameSettings.gameMotors[i].rotation = rotB;
                                        //add points and frags
                                        if (GameSettings.gameMotors[i].HP == 0)
                                            GameSettings.gameMotors[j].FragsCount += 1;
                                        GameSettings.gameMotors[j].PointsCount += (int)(dmgB * 10);
                                    }
                                    GameSettings.gameMotors[i].RefreshBoundingPoints();
                                    GameSettings.gameMotors[j].RefreshBoundingPoints();
                                }
                        }

                        //collisions with enemy trace
                        {
                            Vector2 dispVec, dispVecA, dispVecB;
                            float rot, rotA, rotB;
                            float dmg, dmgA, dmgB;
                            bool resA, resB;

                            for (int j = (team_game && (i < 5) ? 5 : (team_game ? GameSettings.gameMotors.Length : i + 1)); j < GameSettings.gameMotors.Length; j++)
                                if ((GameSettings.gameMotors[j] != null) && (GameSettings.gameMotors[j].HP > 0))
                                {
                                    resA = resB = false;
                                    dispVecA = dispVecB = Vector2.Zero;
                                    dmgA = dmgB = 0;
                                    rotA = GameSettings.gameMotors[i].rotation;
                                    rotB = GameSettings.gameMotors[j].rotation;

                                    foreach (MotorTraceSpot mts in GameSettings.gameMotors[j].trace)
                                        if (mts.TestCollisions(ref GameSettings.gameMotors[i], out dispVec, out rot, out dmg))
                                        {
                                            resA = true;
                                            dmgA += dmg;
                                            dispVecA += dispVec;
                                            rotA = rot;
                                        }
                                    foreach (MotorTraceSpot mts in GameSettings.gameMotors[i].trace)
                                        if (mts.TestCollisions(ref GameSettings.gameMotors[j], out dispVec, out rot, out dmg))
                                        {
                                            resB = true;
                                            dmgB += dmg;
                                            dispVecB += dispVec;
                                            rotB = rot;
                                        }
                                    if (resA)
                                    {
                                        GameSettings.gameMotors[i].HP -= dmgA;
                                        GameSettings.gameMotors[i].position += dispVecA;
                                        GameSettings.gameMotors[i].rotation = rotA;
                                        //add points and frags
                                        if (GameSettings.gameMotors[i].HP == 0)
                                            GameSettings.gameMotors[j].FragsCount += 1;
                                        GameSettings.gameMotors[j].PointsCount += (int)(dmgA * 10);
                                    }
                                    if (resB)
                                    {
                                        GameSettings.gameMotors[j].HP -= dmgB;
                                        GameSettings.gameMotors[j].position += dispVecB;
                                        GameSettings.gameMotors[j].rotation = rotB;
                                        //add points and frags
                                        if (GameSettings.gameMotors[j].HP == 0)
                                            GameSettings.gameMotors[i].FragsCount += 1;
                                        GameSettings.gameMotors[i].PointsCount += (int)(dmgB * 10);
                                    }
                                    GameSettings.gameMotors[i].RefreshBoundingPoints();
                                    GameSettings.gameMotors[j].RefreshBoundingPoints();
                                }
                        }

                        //clean up a bit
                        GameSettings.gameMotors[i].oldCollided.Clear();
                        GameSettings.gameMotors[i].oldCollided.AddRange(GameSettings.gameMotors[i].newCollided);
                        GameSettings.gameMotors[i].newCollided.Clear();
                    }

                //set new camera position depending on stick bike position
                cameraX = (int)GameSettings.gameMotors[cameraStickBikeID].position.X;
                cameraY = (int)GameSettings.gameMotors[cameraStickBikeID].position.Y;

                if (GameSettings.gameMap.Parameters.Size.X <= 800)
                    cameraX = (int)GameSettings.gameMap.Parameters.Size.X / 2;
                else if (cameraX < 400)
                    cameraX = 400;
                else if (cameraX > GameSettings.gameMap.Parameters.Size.X - 400)
                    cameraX = (int)GameSettings.gameMap.Parameters.Size.X - 400;

                if (GameSettings.gameMap.Parameters.Size.Y <= 600)
                    cameraY = (int)GameSettings.gameMap.Parameters.Size.Y / 2;
                else if (cameraY < 300)
                    cameraY = 300;
                else if (cameraY > GameSettings.gameMap.Parameters.Size.Y - 300)
                    cameraY = (int)GameSettings.gameMap.Parameters.Size.Y - 300;

                //update time counter
                currentTime = gameTime.TotalGameTime.TotalSeconds;
                if (timed_game)
                    GameSettings.gameMotors[cameraStickBikeID].FragsChanged(null, 0);

                //check game finish conditions
                bool finish = false;
                bool winning, losing;
                finish |= winning = winningFinish();
                finish |= losing = losingFinish();

                if (finish)
                {
                    //do game finish processing
                    if (winning && losing)
                    {
                        System.Windows.Forms.MessageBox.Show("tied");
                        gameStarted = false;
                        if (gameFinished != null)
                            gameFinished();
                        return;
                    }
                    if (winning)
                    {
                        System.Windows.Forms.MessageBox.Show("won");
                        gameStarted = false;
                        if (gameFinished != null)
                            gameFinished();
                        return;
                    }
                    if (losing)
                    {
                        System.Windows.Forms.MessageBox.Show("lost");
                        gameStarted = false;
                        if (gameFinished != null)
                            gameFinished();
                    }
                }
            }
        }

        public void Draw(ref SpriteBatch sb, GameTime gameTime)
        {
            if (gameStarted)
            {
                //draw map
                GameSettings.gameMap.Draw(ref sb, cameraX, cameraY);

                //draw bikes
                for (int i = 0; i < GameSettings.gameMotors.Length; i++)
                    if (GameSettings.gameMotors[i] != null)
                        GameSettings.gameMotors[i].Draw(ref sb, gameTime, cameraX, cameraY);
            }
        }
    }
}
