using System;
using Microsoft.Xna.Framework;

namespace Motorki.GameClasses
{
    public class PlayerMotor : Motorek
    {
        public enum Steering { Relative, Absolute, Network };
        public Steering steering { get { return (iPlayerID == 0 ? GameSettings.playerSteering : Steering.Network); } }
        public int iPlayerID { get; private set; }

        /// <param name="iPlayerID">0 - player, -1 - empty slot for network player</param>
        /// <param name="framingRect">map limits</param>
        public PlayerMotor(MotorkiGame game, int iPlayerID, Color motorColor)
            : base(game, motorColor, new Color(255 - motorColor.R, 255 - motorColor.G, 255 - motorColor.B))
        {
            this.iPlayerID = iPlayerID;
        }

        protected override void MindProc(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float usableSpeed = motorSpeedPerSecond;

            switch (iPlayerID)
            {
                case 0:
                    switch (GameSettings.playerSteering)
                    {
                        case Steering.Relative:
                            ctrlDirection = 0;
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[0])) //rotate left
                                ctrlDirection += -1;
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[1])) //rotate right
                                ctrlDirection += 1;
                            ctrlBrakes = false;
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[3]))
                                ctrlBrakes = true;
                            break;
                        case Steering.Absolute:
                            Vector2 current_direction = new Vector2((float)Math.Sin(rotation.ToRadians()), -(float)Math.Cos(rotation.ToRadians()));
                            Vector2 new_direction = Vector2.Zero;
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[0])) //go left
                                new_direction += new Vector2(-1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[1])) //go right
                                new_direction += new Vector2(1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[2])) //go up
                                new_direction += new Vector2(0, -1);
                            if (InputEvents.IsKeyPressed(GameSettings.playerKeys[3])) //go down
                                new_direction += new Vector2(0, 1);
                            if (new_direction.Length() == 0)
                                new_direction = current_direction;
                            new_direction.Normalize();
                            new_direction = new Vector2(new_direction.Y, -new_direction.X);
                            float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product
                            float angle = ((float)Math.Asin(sin_alpha)).ToDegrees();
                            ctrlBrakes = false;
                            ctrlDirection = (angle > 0 ? 1 : (angle < 0 ? -1 : 0));
                            break;
                    }
                    break;
            }
        }
    }
}
