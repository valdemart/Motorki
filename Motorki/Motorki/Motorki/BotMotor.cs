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
    public class BotMotor : Motorek
    {
        public enum BotSophistication { Easy, Normal, Hard };
        public BotSophistication sophistication;

        private int[] cmd;
        private int[] cmd_time;

        public BotMotor(Game game, Vector2 position, float rotation, Color motorColor, Color trackColor, Rectangle framingRect)
            : base(game, position, rotation, motorColor, trackColor, framingRect)
        {
            cmd = new int[2]; //0 - forward/backward, 1 - left/right
            cmd_time = new int[2];
            cmd_time[0] = 0;
            cmd_time[1] = 0;

            sophistication = BotSophistication.Easy;
        }

        protected override void MindProc(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            Random r = new Random();

            switch (sophistication)
            {
                case BotSophistication.Easy:
                    //resolve forward/backward command
                    if (cmd_time[0] > 0)
                    {
                        switch (cmd[0])
                        {
                            case 0: //go forward
                            case 2:
                                position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (motorSpeedPerSecond * time);
                                break;
                            case 3: //go backward
                                position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (-motorSpeedPerSecond * time / 2);
                                break;
                        }
                        cmd_time[0] -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        int last_cmd = cmd[0];
                        do
                        {
                            cmd[0] = r.Next(0, 3);
                        } while (cmd[0] == last_cmd);
                        cmd_time[0] = r.Next(250, 750);
                    }

                    //resolve left/right command
                    if (cmd_time[1] > 0)
                    {
                        switch (cmd[1])
                        {
                            case 1: //rotate left
                            case 4:
                                rotation = (rotation - motorTurnPerSecond * time) % 360;
                                break;
                            case 0:
                            case 3: //rotate right
                                rotation = (rotation + motorTurnPerSecond * time) % 360;
                                break;
                        }
                        cmd_time[1] -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        int last_cmd = cmd[1];
                        do
                        {
                            cmd[1] = r.Next(0, 4);
                        } while (cmd[1] == last_cmd);
                        cmd_time[1] = r.Next(100, 500);
                    }
                    break;
                case BotSophistication.Normal:
                    break;
                case BotSophistication.Hard:
                    break;
            }
        }
    }
}
