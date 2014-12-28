using System;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Motorki.GameClasses
{
    public class BotMotor : Motorek
    {
        public enum BotSophistication { Easy, Normal, Hard };
        public BotSophistication sophistication { get; set; }

        private int[] cmd;
        private int[] cmd_time;

        public BotMotor(MotorkiGame game, Color motorColor, BotSophistication sophistication = BotSophistication.Easy)
            : base(game, motorColor, new Color(255 - motorColor.R, 255 - motorColor.G, 255 - motorColor.B))
        {
            //do some randomization
            int a = MotorkiGame.random.Next(1000);
            for (int i = 0; i < a * 1000; i++)
                a = (a + a - a) * a / a;

            cmd = new int[2]; //0 - brakes active/inactive, 1 - forward/left/right
            cmd_time = new int[2];
            cmd_time[0] = 0;
            cmd_time[1] = 0;

            this.sophistication = sophistication;
        }

        public override void LoadAndInitialize(Rectangle framingRect)
        {
            base.LoadAndInitialize(framingRect);

            //check is there need to start agent
            if (sophistication != BotSophistication.Easy)
            {
                if (GameSettings.agentController != null)
                {
                    int motorID;
                    for (motorID = 0; motorID < GameSettings.gameMotors.Length; motorID++)
                        if (GameSettings.gameMotors[motorID] == this)
                            break;
                    GameSettings.agentController.RegisterAgent(new BotAgent(GameSettings.agentController, "bot" + name, motorID));
                }
            }
        }

        protected override void MindProc(GameTime gameTime)
        {
            switch (sophistication)
            {
                case BotSophistication.Easy:
                    //generate brakes active/inactive command
                    if (cmd_time[0] <= 0)
                    {
                        //0.2 of chance for activating brakes
                        cmd[0] = MotorkiGame.random.Next(0, 15) % 5 > 0 ? 0 : 1;
                        cmd_time[0] = MotorkiGame.random.Next(250, 750);
                    }
                    //resolve brakes active/inactive command
                    ctrlBrakes = cmd[0] == 1;
                    cmd_time[0] -= gameTime.ElapsedGameTime.Milliseconds;

                    //generate forward/left/right command
                    if (cmd_time[1] <= 0)
                    {
                        //(1/3) of chance for going forward, left or right
                        cmd[1] = MotorkiGame.random.Next(0, 12) % 3 - 1;
                        cmd_time[1] = MotorkiGame.random.Next(100, 500);
                    }
                    //resolve forward/left/right command
                    ctrlDirection = cmd[1];
                    cmd_time[1] -= gameTime.ElapsedGameTime.Milliseconds;
                    break;
                case BotSophistication.Normal:
                    break;
                case BotSophistication.Hard:
                    break;
            }
        }
    }
}
