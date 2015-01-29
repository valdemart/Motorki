using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Motorki.GameClasses
{
    public class AgentController
    {
        internal enum ACRequests
        {
            /// <summary>
            /// empty (or corrupted request) - will be ignored
            /// </summary>
            Dummy,
            /// <summary>
            /// used only for testing. causes MessageBox.Show(requestParams[n].ToString())
            /// </summary>
            TestRequest,
            /// <summary>
            /// deregisters all agents and stops agent controller therad
            /// </summary>
            KillAgentController,
            /// <summary>
            /// causes agent controller thread to finish browsing agent list so that agent controller may change list content. When all changes are done agent controller must post this request again
            /// </summary>
            SynchronizeAgentList,
        }

        static Thread acThread;
        List<Agent> agentRegister;
        ACRequests acInternalRequest;

        /// <summary>
        /// creates and starts agent controller
        /// </summary>
        public AgentController()
        {
            if (acThread != null)
                throw new Exception("It's not allowed to create more than one agent controller");
            agentRegister = new List<Agent>();
            acThread = new Thread(acMain);
            acInternalRequest = ACRequests.Dummy;
            acThread.Start();
        }

        /// <summary>
        /// agent controller thread main function
        /// </summary>
        void acMain()
        {
            while (true)
            {
                if (acInternalRequest == ACRequests.KillAgentController) break;
                if (acInternalRequest == ACRequests.SynchronizeAgentList)
                {
                    acInternalRequest = ACRequests.Dummy;
                    while (acInternalRequest != ACRequests.SynchronizeAgentList)  Thread.Sleep(100);
                    acInternalRequest = ACRequests.Dummy;
                }

                // agent jobs
                for (int i = 0; i < agentRegister.Count; i++)
                {
                    agentRegister[i].Process();
                    if (agentRegister[i].selfDeregister)
                    {
                        agentRegister.RemoveAt(i);
                        i--;
                    }
                    if (acInternalRequest == ACRequests.KillAgentController) break;
                }

                Thread.Sleep(20);
            }
            acInternalRequest = ACRequests.Dummy;
            acThread = null;
            Thread.CurrentThread.Abort();
        }

        #region requests for agent controller made by GamePlay objects

        /// <summary>
        /// stops agent controller thread and deregisters all agents
        /// </summary>
        public void KillAgentController()
        {
            acInternalRequest = ACRequests.KillAgentController;
            while (acInternalRequest == ACRequests.KillAgentController) Thread.Sleep(20);
            agentRegister.Clear();
        }

        /// <summary>
        /// registers agent
        /// </summary>
        public void RegisterAgent(Agent a)
        {
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
            agentRegister.Add(a);
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
        }

        /// <summary>
        /// deregisters agent
        /// </summary>
        public void DeregisterAgent(Agent a)
        {
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
            agentRegister.Remove(a);
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
        }

        /// <summary>
        /// deregisters all agents which names correspond to specified pattern
        /// </summary>
        public void DeregisterAgents(string namePattern)
        {
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
            for (int i = 0; i < agentRegister.Count; )
                if (Regex.IsMatch(agentRegister[i].Name, namePattern))
                    agentRegister.RemoveAt(i);
                else
                    i++;
            acInternalRequest = ACRequests.SynchronizeAgentList;
            while (acInternalRequest == ACRequests.SynchronizeAgentList) Thread.Sleep(20);
        }

        /// <summary>
        /// posts a message to all agents
        /// </summary>
        public void SendToAllAgents(AgentMessage msg)
        {
            for (int i = 0; i < agentRegister.Count; i++)
                agentRegister[i].SendMessage(agentRegister[i], msg);
        }

        #endregion

        #region agent controller request possible to make by agents

        /// <summary>
        /// searches for agent with name corresponding to specified regular expression
        /// </summary>
        public Agent FindAgent(string namePattern)
        {
            for (int i = 0; i < agentRegister.Count; i++)
                if (Regex.IsMatch(agentRegister[i].Name, namePattern))
                    return agentRegister[i];
            return null;
        }

        /// <summary>
        /// searches for all agents with names corresponding to specified regular expression
        /// </summary>
        public List<Agent> FindAgents(string namePattern)
        {
            List<Agent> results = new List<Agent>();
            for (int i = 0; i < agentRegister.Count; i++)
                if (Regex.IsMatch(agentRegister[i].Name, namePattern))
                    results.Add(agentRegister[i]);
            return results;
        }

        /// <summary>
        /// searches for agent, which responses true on search request
        /// </summary>
        public Agent FindSpecificAgent(object searchData)
        {
            for (int i = 0; i < agentRegister.Count; i++)
                if (agentRegister[i].SearchTest(searchData))
                    return agentRegister[i];
            return null;
        }

        /// <summary>
        /// searches for all agents, which response true on search request
        /// </summary>
        public List<Agent> FindSpecificAgents(object searchData)
        {
            List<Agent> results = new List<Agent>();
            for (int i = 0; i < agentRegister.Count; i++)
                if (agentRegister[i].SearchTest(searchData))
                    results.Add(agentRegister[i]);
            return results;
        }

        #endregion
    }
}
