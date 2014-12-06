using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if ((bam.msgType == BotAgentMessages.GameStarted) || (bam.msgType == BotAgentMessages.GameFinished) ||
                (bam.msgType == BotAgentMessages.PauseForControlApplying) || (bam.msgType == BotAgentMessages.ControlsApplied))
                messageList.Insert(0, bam);

            //ordinary message processing - insert at the end
            messageList.Add(bam);
        }

        public override void Process()
        {
            //!!!to get last Update time check MotorkiGame.game.currentTime
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
