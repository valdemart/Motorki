using System.Collections.Generic;
using System.Threading;

namespace Motorki.GameClasses
{
    public class Agent
    {
        public string Name { get; protected set; }
        protected AgentController agentController;
        protected List<AgentMessage> messageList;
        /// <summary>
        /// can be used by agent to inform agent controller that agent want to be deregistered (killed)
        /// </summary>
        public bool selfDeregister { get; protected set; }

        protected bool processingMessages { get; set; }
        protected bool receivingMessage { get; set; }

        public Agent(AgentController ac, string name)
        {
            agentController = ac;
            Name = name;
            messageList = new List<AgentMessage>();
            selfDeregister = false;
            processingMessages = false;
            receivingMessage = false;
        }

        /// <summary>
        /// used by agent controller during FindSpecificAgent and FindSpecificAgents method. Should return true when this agent accepts search conditions
        /// </summary>
        /// <param name="searchData">additional data used for searching</param>
        public virtual bool SearchTest(object searchData)
        {
            return false;
        }

        /// <summary>
        /// used for sending messages to other agents
        /// </summary>
        public void SendMessage(Agent a, AgentMessage msg)
        {
            msg.sender = this;
            a.ReceiveMessage(msg);
        }

        protected void BlockMessageReceivingStart()
        {
            while (processingMessages) Thread.Sleep(10);
            receivingMessage = true;
        }

        protected void BlockMessageReceivingEnd()
        {
            receivingMessage = false;
        }

        protected void BlockMessageProcessingStart()
        {
            while (receivingMessage) Thread.Sleep(10);
            processingMessages = true;
        }

        protected void BlockMessageProcessingEnd()
        {
            processingMessages = false;
        }

        /// <summary>
        /// used to put incoming message into own message list. May be overridden to implement some filtering or special message queueing. base.ReceiveMessage(msg) inserts at the end of message queue
        /// </summary>
        protected virtual void ReceiveMessage(AgentMessage msg)
        {
            BlockMessageReceivingStart();

            messageList.Add(msg);

            BlockMessageReceivingEnd();
        }

        /// <summary>
        /// must be overridden to make agent do something useful. If not overridden, it does nothing (no base.Process() needed)
        /// </summary>
        public virtual void Process()
        {
        }
    }

    public class AgentMessage
    {
        /// <summary>
        /// value used to identify messaging threads. use if you need (no internal means assumed)
        /// </summary>
        public int threadID { get; protected set; }
        /// <summary>
        /// sender of this message
        /// </summary>
        public Agent sender { get; set; }
        /// <summary>
        /// receiver of this message
        /// </summary>
        public Agent receiver { get; protected set; }
        /// <summary>
        /// agent to send response to
        /// </summary>
        public Agent replyTo { get; protected set; }
        /// <summary>
        /// message content
        /// </summary>
        public string content { get; protected set; }

        /// <param name="threadID">set to 0 to ignore</param>
        /// <param name="sender">sender of a message</param>
        /// <param name="receiver">receiver of a message</param>
        /// <param name="replyTo">agent to send response to. set to null to ignore (answers will go to sender of this message)</param>
        /// <param name="content">message content</param>
        public AgentMessage(int threadID, Agent sender, Agent receiver, Agent replyTo, string content)
        {
            this.threadID = threadID;
            this.sender = sender;
            this.receiver = receiver;
            this.replyTo = replyTo;
            this.content = content;
        }

        public override string ToString()
        {
            return "sender: " + (sender != null ? sender.Name : "<no sender>") + "   " +
                   "receiver: " + (receiver != null ? receiver.Name : "<no receiver>") + "   " +
                   "threadID: " + threadID + "   " +
                   "content: " + (content ?? "<no content>") + "   " +
                   "replyTo: " + (replyTo != null ? replyTo.Name : "<no replyTo>");
        }

        /// <summary>
        /// creates response for this message
        /// </summary>
        /// <param name="content">response content</param>
        public AgentMessage CreateResponse(string content)
        {
            return new AgentMessage(threadID, receiver, replyTo ?? sender, receiver, content);
        }

        /// <summary>
        /// creates response for this message
        /// </summary>
        /// <param name="replyTo">agent to send response to. set to null to ignore (answers will go to sender of this message)</param>
        /// <param name="content">response content</param>
        public AgentMessage CreateResponse(Agent replyTo, string content)
        {
            return new AgentMessage(threadID, receiver, replyTo ?? sender, receiver, content);
        }
    }
}
