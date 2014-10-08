using Microsoft.Xna.Framework;
using System;

namespace Motorki.GameClasses
{
    public class BotMotor : Motorek
    {
        public enum BotSophistication { Easy, Normal, Hard };
        public BotSophistication sophistication { get; set; }

        private int[] cmd;
        private int[] cmd_time;

        public BotMotor(MotorkiGame game, Color motorColor)
            : base(game, motorColor, new Color(255 - motorColor.R, 255 - motorColor.G, 255 - motorColor.B))
        {
            int a = MotorkiGame.random.Next(1000);
            for (int i = 0; i < a * 1000; i++)
                a = (a + a - a) * a / a;

            cmd = new int[2]; //0 - forward/backward, 1 - left/right
            cmd_time = new int[2];
            cmd_time[0] = 0;
            cmd_time[1] = 0;

            sophistication = BotSophistication.Easy;
        }

        protected override void MindProc(GameTime gameTime)
        {
            float time = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float usableSpeed = motorSpeedPerSecond;
            
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
                                usableSpeed = motorSpeedPerSecond;
                                break;
                            case 3: //go backward
                                usableSpeed /= 2.0f;
                                break;
                        }
                        position += (new Vector2((float)Math.Sin(MathHelper.ToRadians(rotation)), -(float)Math.Cos(MathHelper.ToRadians(rotation)))) * (usableSpeed * time);
                        cmd_time[0] -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        int last_cmd = cmd[0];
                        do
                        {
                            cmd[0] = MotorkiGame.random.Next(0, 3);
                        } while (cmd[0] == last_cmd);
                        cmd_time[0] = MotorkiGame.random.Next(250, 750);
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
                            cmd[1] = MotorkiGame.random.Next(0, 4);
                        } while (cmd[1] == last_cmd);
                        cmd_time[1] = MotorkiGame.random.Next(100, 500);
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
