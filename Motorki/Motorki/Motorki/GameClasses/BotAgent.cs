using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Motorki.GameClasses
{
    public class BotAgent : Agent
    {
        int motorID;
        BotMotor ownMotor;
        /// <summary>
        /// key=id in gameMotors, value=Motor
        /// </summary>
        Hashtable teammates, enemies;
        /// <summary>
        /// key=id in gameMotors, value=BotAgent
        /// </summary>
        Hashtable friendlyAgents;

        //internal state variables
        public BotAgentProcessingStates state { get; private set; }
        /// <summary>
        /// id on enemies list of a target ("primary objective: kill enemies[target]")
        /// </summary>
        int target;
        bool steeringLocked;
        FoolAroundData faData;

        /// <summary>
        /// warning: motor should be created before creating an agent. if not, use AttachBike to assign motor correctly
        /// </summary>
        public BotAgent(AgentController ac, string name, int motorID)
            : base(ac, name)
        {
            this.motorID = motorID;
            ownMotor = (BotMotor)GameSettings.gameMotors[motorID];
            teammates = new Hashtable();
            enemies = new Hashtable();
            friendlyAgents = new Hashtable();
            steeringLocked = false;

            faData = new FoolAroundData();
        }

        public void AttachBike(int motorID)
        {
            this.motorID = motorID;
            ownMotor = (BotMotor)GameSettings.gameMotors[motorID];
            teammates = new Hashtable();
            enemies = new Hashtable();
            friendlyAgents = new Hashtable();
        }

        protected override void ReceiveMessage(AgentMessage msg)
        {
            BotAgentMessage bam = msg as BotAgentMessage;
            if (bam == null)
                return;

            //always ignore Dummy messages
            if (bam.msgType == BotAgentMessages.Dummy)
                return;
            //always insert messages from GamePlay level at the beginning
            BlockMessageReceivingStart();

            if ((bam.msgType == BotAgentMessages.GameStarted) || (bam.msgType == BotAgentMessages.GameFinished) ||
                (bam.msgType == BotAgentMessages.PauseForControlApplying) || (bam.msgType == BotAgentMessages.ControlsApplied))
                messageList.Insert(0, bam);

            //ordinary message processing - insert at the end
            messageList.Add(bam);

            BlockMessageReceivingEnd();
        }

        private class MotorDataContext
        {
            public int ctrlDirection;
            public bool ctrlBrakes;
            /// <summary>
            /// -1/0/1 - normal speed steering, -2/3/2 - steering with brakes, other values - no steering needed
            /// </summary>
            public int newSteering;
            public float width, height;
            public Vector2 position;
            public Vector2 dirVec;
            public Vector2 dirVecPerp;
            public float turningRange;
            public float turningRangeBrakes;

            public MotorDataContext() { }
        }

        public override void Process()
        {
            if(ownMotor==null) return;
            if (ownMotor.sophistication == BotMotor.BotSophistication.Easy) return;

            //!!!to get last Update time check MotorkiGame.game.currentTime

            MotorDataContext mdc = new MotorDataContext()
            {
                ctrlDirection = ownMotor.ctrlDirection,
                ctrlBrakes = ownMotor.ctrlBrakes,
                newSteering = 0,
                width = ownMotor.width,
                height = ownMotor.height,
                position = ownMotor.position,
                dirVec = Utils.CalculateDirectionVector(ownMotor.rotation.ToRadians()).Normalized(),
                dirVecPerp = Utils.CalculateDirectionVector(ownMotor.rotation.ToRadians()).Perpendicular().Normalized(),
                turningRange = (float)(180.0 * ownMotor.motorSpeedPerSecond / (Math.PI * ownMotor.motorTurnPerSecond)),
                turningRangeBrakes = (float)(90.0 * ownMotor.motorSpeedPerSecond / (Math.PI * ownMotor.motorTurnPerSecond))
            };

            //process messages
            BlockMessageProcessingStart();
            while (messageList.Count > 0)
            {
                BotAgentMessage msg = (BotAgentMessage)messageList[0];
                messageList.RemoveAt(0);
                switch (msg.msgType)
                {
                    case BotAgentMessages.PauseForControlApplying: steeringLocked = true; break;
                    case BotAgentMessages.ControlsApplied: steeringLocked = false; break;
                }
            }
            BlockMessageProcessingEnd();
            
            //redetect enemies and friends

            //do current action
            //1st priority - own safety (traces)
            if (false)
            {
                //logic note: percentage for avoiding obstacle
                //logic note: if happend to not avoid then penalty time which forces no changes in direction and braking
                //logic note: force avoiding traces over avoiding walls (when forced to hit wall or trace choose wall)
                return; //lock lower priority processing
            }

            //2nd priority - own safety (walls)
            int obstacleSteering;
            int obstacleImportance;
            if ((obstacleImportance = CheckObstacles(ref mdc)) == 2) //when need steering on brakes
            {
                ApplySteering(mdc.newSteering);
                return; //lock lower priority processing
            }
            obstacleSteering = mdc.newSteering;

            if (ownMotor.sophistication == BotMotor.BotSophistication.Hard)
            {
                //3rd priority - select target
                if (false)
                {
                    //logic note: when to change target/select it again: monitor targets HP. when new value is greater than old value then HP has been reset - target died and has been respawned
                    return; //lock lower priority processing
                }

                //4th priority - get closer to the target
                if (false)
                {
                    return; //lock lower priority processing
                }

                //5th priority - consult strategy
                if (false)
                {
                    return; //lock lower priority processing
                }
            }

            //other priorities didn't respond causing return - apply obstacle detection or fool around if no obstacles
            if (obstacleImportance == 1)
                ApplySteering(obstacleSteering);
            else if (obstacleImportance == 0)
            {
                FoolAround(ref mdc, faData);
                ApplySteering(mdc.newSteering);
            }
        }

        private void ApplySteering(int steering)
        {
            if (steeringLocked) return;

            switch (steering)
            {
                case 0: ownMotor.ctrlDirection = 0; ownMotor.ctrlBrakes = false; break;
                case -1: ownMotor.ctrlDirection = -1; ownMotor.ctrlBrakes = false; break;
                case 1: ownMotor.ctrlDirection = 1; ownMotor.ctrlBrakes = false; break;
                case 3: ownMotor.ctrlDirection = 0; ownMotor.ctrlBrakes = true; break;
                case -2: ownMotor.ctrlDirection = -1; ownMotor.ctrlBrakes = true; break;
                case 2: ownMotor.ctrlDirection = 1; ownMotor.ctrlBrakes = true; break;
            }
        }

        /// <summary>
        /// determines whether there's a need to avoid walls. returns 0 when tests didn't detect any danger, 1 when danger is acceptable, 2 when danger is not acceptable
        /// </summary>
        private int CheckObstacles(ref MotorDataContext mdc)
        {
            float range = mdc.turningRange + mdc.width / 2;
            float rangeBrakes = mdc.turningRangeBrakes + mdc.width / 2;
            float safetyCoef = 2.5f; //safety margin for turns (multiplier for turn ranges, should be safetyCoef < 1.0f). greater value means turning further to the edge

            //sequence: normal speed turn left, normal speed turn right, brakes turn left, brakes turn right
            Vector2[] turnCenters = new Vector2[] { mdc.position - mdc.turningRange * mdc.dirVecPerp, mdc.position + mdc.turningRange * mdc.dirVecPerp,
                                                    mdc.position - mdc.turningRangeBrakes * mdc.dirVecPerp, mdc.position + mdc.turningRangeBrakes * mdc.dirVecPerp };
            //sequence: normal speed range, brakes range
            float[] turnRanges = new float[] { (mdc.turningRange + mdc.width / 2) * safetyCoef, (mdc.turningRangeBrakes + mdc.width / 2) * safetyCoef };

            float margin = float.PositiveInfinity;
            int result = 0;
            foreach (Map.MapEdge me in GameSettings.gameMap.Edges)
            {
                //check is this edge any threat at all
                if (me.facing.Dot(mdc.dirVec) >= 0.0f) continue;

                //check distance for avoiding an edge
                int turnSign = mdc.dirVecPerp.Dot(me.facing).Sign();
                if (turnSign <= 0) //test left turns
                {
                    float dist;

                    //no brakes
                    dist = Utils.DistanceFromLineSegment(me.start, me.end, me.facing, turnCenters[0]);
                    if ((dist < turnRanges[0]) && (margin > dist))
                    {
                        margin = dist;
                        mdc.newSteering = -1;
                        result = 1;
                        if ((mdc.ctrlDirection == mdc.newSteering) && !mdc.ctrlBrakes)
                        {
                            //amplify current steering
                            margin *= 0.5f;
                        }
                    }

                    //brakes
                    dist = Utils.DistanceFromLineSegment(me.start, me.end, me.facing, turnCenters[2]);
                    if ((dist < turnRanges[1]) && (margin > dist))
                    {
                        margin = dist;
                        mdc.newSteering = -2;
                        result = 2;
                        if ((mdc.ctrlDirection == mdc.newSteering / 2) && mdc.ctrlBrakes)
                        {
                            //amplify current steering
                            margin *= 0.5f;
                        }
                    }
                }
                if (turnSign >= 0) //test right turns
                {
                    float dist;

                    //no brakes
                    dist = Utils.DistanceFromLineSegment(me.start, me.end, me.facing, turnCenters[1]);
                    if ((dist < turnRanges[0]) && (margin > dist))
                    {
                        margin = dist;
                        mdc.newSteering = 1;
                        result = 1;
                        if ((mdc.ctrlDirection == mdc.newSteering) && !mdc.ctrlBrakes)
                        {
                            //amplify current steering
                            margin *= 0.5f;
                        }
                    }

                    //brakes
                    dist = Utils.DistanceFromLineSegment(me.start, me.end, me.facing, turnCenters[3]);
                    if ((dist < turnRanges[1]) && (margin > dist))
                    {
                        margin = dist;
                        mdc.newSteering = 2;
                        result = 2;
                        if ((mdc.ctrlDirection == mdc.newSteering / 2) && mdc.ctrlBrakes)
                        {
                            //amplify current steering
                            margin *= 0.5f;
                        }
                    }
                }
            }
            return result;
        }

        private class FoolAroundData
        {
            public int steering;
            public bool brakes;
            public int steering_time;
            public int brakes_time;

            public FoolAroundData()
            {
                steering = 0;
                steering_time = 0;
                brakes = false;
                brakes_time = 0;
            }
        }

        private void FoolAround(ref MotorDataContext mdc, FoolAroundData fad)
        {
            //generate brakes active/inactive command
            if (fad.brakes_time <= 0)
            {
                //0.2 of chance for activating brakes
                fad.brakes = MotorkiGame.random.Next(0, 15) % 5 > 0 ? false : true;
                fad.brakes_time = MotorkiGame.random.Next(250, 750);
            }
            fad.brakes_time -= MotorkiGame.game.currentTime.ElapsedGameTime.Milliseconds;

            //generate forward/left/right command
            if (fad.steering_time <= 0)
            {
                //(1/3) of chance for going forward, left or right
                fad.steering = MotorkiGame.random.Next(0, 12) % 3 - 1;
                fad.steering_time = MotorkiGame.random.Next(100, 500);
            }
            fad.steering_time -= MotorkiGame.game.currentTime.ElapsedGameTime.Milliseconds;

            //encode steering signal
            mdc.newSteering = fad.steering * (fad.brakes ? 2 : 1) + ((fad.steering == 0) && fad.brakes ? 3 : 0);
        }
    }

    public enum BotAgentProcessingStates
    {
        NoTask,
        /// <summary>
        /// searching for a possible targets and teammates. done once per some time
        /// </summary>
        DetectingEnemiesAndFriends,
        /// <summary>
        /// reading inbox
        /// </summary>
        MessageChecking,
        /// <summary>
        /// sending pending messages
        /// </summary>
        MessageSending,
        /// <summary>
        /// detecting map edges, trace spots, etc.
        /// </summary>
        ObstaclesDetectionAndProcessing,
        /// <summary>
        /// selecting a target
        /// </summary>
        EnemyTargeting,
        /// <summary>
        /// maintaining strategy progress
        /// </summary>
        StrategyRelization,
    }

    public enum BotAgentMessages
    {
        /// <summary>
        /// empty message
        /// </summary>
        Dummy,
        /// <summary>
        /// sent by GamePlay to notify bot agents that game started
        /// </summary>
        GameStarted,
        /// <summary>
        /// sent by GamePlay to notify bot agents that game finished
        /// </summary>
        GameFinished,
        /// <summary>
        /// notifies bot agents that they should wait because GamePlay applies controls
        /// </summary>
        PauseForControlApplying,
        /// <summary>
        /// notifies bot agents that they can continue processing (controls are applied)
        /// </summary>
        ControlsApplied,
        /// <summary>
        /// bot agent suggests a strategy to another bot agent against specified motor
        /// </summary>
        StrategySuggestion,
        /// <summary>
        /// bot agent accepts joining suggested strategy
        /// </summary>
        StrategyAccept,
        /// <summary>
        /// bot agent denies joining suggested strategy
        /// </summary>
        StrategyDeny,
        /// <summary>
        /// bot agent notifies that it must abort strategy due to own safety
        /// </summary>
        StrategyAbort,
    }

    public class BotAgentMessage : AgentMessage
    {
        public BotAgentMessages msgType;

        public BotAgentMessage(Agent sender, Agent receiver, BotAgentMessages msgType, string content)
            : base(0, sender, receiver, sender, content)
        {
            this.msgType = msgType;
        }

        public override string ToString()
        {
            return msgType + " " + content;
        }
    }
}
