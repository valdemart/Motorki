using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Motorki.GameClasses
{
    public class PlayerMotor : Motorek
    {
        public enum Steering { Relative, Absolute };
        public Steering steering { get { return (iPlayerID == 0 ? GameSettings.player1Steering : (iPlayerID == 1 ? GameSettings.player2Steering : Steering.Relative)); } }
        public int iPlayerID { get; private set; }

        /// <param name="iPlayerID">0 - player1, 1 - player2, -1 - empty slot for player</param>
        /// <param name="framingRect">map limits</param>
        public PlayerMotor(MotorkiGame game, int iPlayerID, Color motorColor)
            : base(game, motorColor, new Color(255-motorColor.R, 255-motorColor.G, 255-motorColor.B))
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
                    switch (GameSettings.player1Steering)
                    {
                        case Steering.Relative:
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[0])) //rotate left
                                rotation = (rotation - motorTurnPerSecond * time) % 360;
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[1])) //rotate right
                                rotation = (rotation + motorTurnPerSecond * time) % 360;
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[3]))
                                usableSpeed /= 2.0f;
                            //unconditional forward
                            position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (usableSpeed * time);
                            break;
                        case Steering.Absolute:
                            Vector2 current_direction = new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)));
                            Vector2 new_direction = Vector2.Zero;
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[0])) //go left
                                new_direction += new Vector2(-1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[1])) //go right
                                new_direction += new Vector2(1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[2])) //go up
                                new_direction += new Vector2(0, -1);
                            if (InputEvents.IsKeyPressed(GameSettings.player1Keys[3])) //go down
                                new_direction += new Vector2(0, 1);
                            if (new_direction.Length() == 0)
                                new_direction = current_direction;
                            new_direction.Normalize();
                            new_direction = new Vector2(new_direction.Y, -new_direction.X);
                            float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product
                            float angle = MathHelper.ToDegrees((float)Math.Asin(sin_alpha));
                            rotation += Math.Sign(angle) * motorTurnPerSecond * time;
                            position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (motorSpeedPerSecond * time);
                            break;
                    }
                    break;
                case 1:
                    switch (GameSettings.player2Steering)
                    {
                        case Steering.Relative:
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[0])) //rotate left
                                rotation = (rotation - motorTurnPerSecond * time) % 360;
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[1])) //rotate right
                                rotation = (rotation + motorTurnPerSecond * time) % 360;
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[3]))
                                usableSpeed /= 2.0f;
                            //unconditional forward
                            position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (usableSpeed * time);
                            break;
                        case Steering.Absolute:
                            Vector2 current_direction = new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)));
                            Vector2 new_direction = Vector2.Zero;
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[0])) //go left
                                new_direction += new Vector2(-1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[1])) //go right
                                new_direction += new Vector2(1, 0);
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[2])) //go up
                                new_direction += new Vector2(0, -1);
                            if (InputEvents.IsKeyPressed(GameSettings.player2Keys[3])) //go down
                                new_direction += new Vector2(0, 1);
                            if (new_direction.Length() == 0)
                                new_direction = current_direction;
                            new_direction.Normalize();
                            new_direction = new Vector2(new_direction.Y, -new_direction.X);
                            float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product
                            float angle = MathHelper.ToDegrees((float)Math.Asin(sin_alpha));
                            rotation += Math.Sign(angle) * motorTurnPerSecond * time;
                            position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (motorSpeedPerSecond * time);
                            break;
                    }
                    break;
            }
        }
    }
}
