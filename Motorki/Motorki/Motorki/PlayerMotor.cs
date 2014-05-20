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

namespace Motorki
{
    public class PlayerMotor : Motorek
    {
        public enum Streering { Relative, Absolute };
        public Streering steering;

        public PlayerMotor(Game game, Vector2 position, float rotation, Color motorColor, Color trackColor, Rectangle framingRect)
            : base(game, position, rotation, motorColor, trackColor, framingRect)
        {
            steering = Streering.Relative;
        }

        protected override void MindProc(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            switch(steering)
            {
                case Streering.Relative:
                    if (InputEvents.IsKeyPressed(Keys.Left)) //rotate left
                        rotation = (rotation - motorTurnPerSecond * time) % 360;
                    if (InputEvents.IsKeyPressed(Keys.Right)) //rotate right
                        rotation = (rotation + motorTurnPerSecond * time) % 360;
                    if (InputEvents.IsKeyPressed(Keys.Up)) //go forward
                        position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (motorSpeedPerSecond * time);
                    if (InputEvents.IsKeyPressed(Keys.Down)) //go backward
                        position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (-motorSpeedPerSecond * time / 2);
                    break;
                case Streering.Absolute:
                    Vector2 new_direction = Vector2.Zero;
                    if (InputEvents.IsKeyPressed(Keys.Left)) //go left
                        new_direction += new Vector2(-1, 0);
                    if (InputEvents.IsKeyPressed(Keys.Right)) //go right
                        new_direction += new Vector2(1, 0);
                    if (InputEvents.IsKeyPressed(Keys.Up)) //go up
                        new_direction += new Vector2(0, -1);
                    if (InputEvents.IsKeyPressed(Keys.Down)) //go down
                        new_direction += new Vector2(0, 1);
                    if (new_direction.Length() == 0)
                        break;
                    new_direction.Normalize();
                    new_direction = new Vector2(new_direction.Y, -new_direction.X);
                    Vector2 current_direction = new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)));
                    float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product
                    float angle = MathHelper.ToDegrees((float)Math.Asin(sin_alpha));
                    rotation += Math.Sign(angle) * motorTurnPerSecond * time;
                    position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (motorSpeedPerSecond * time);
                    break;
            }
        }
    }
}
