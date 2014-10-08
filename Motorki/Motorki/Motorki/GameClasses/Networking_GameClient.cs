using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Motorki.GameClasses
{
    public enum Networking_GameSummaryProps
    {
        gameType, connectedCount, maxCount, fragLimit, pointLimit, timeLimit, botSkills
    }

    public class Networking_GameSummary
    {
        public string gameServerIP { get; set; }
        public string gameName { get; set; }
        public string gameMapName { get; set; }
        public GameType gameType { get; set; }
        public int gamePointLimit { get; set; }
        public int gameFragLimit { get; set; }
        /// <summary>
        /// in minutes
        /// </summary>
        public int gameTimeLimit { get; set; }
    }

    public delegate void NetGameClient_ServersDetected(List<Networking_GameSummary> list);

    public class Networking_GameClient
    {
        TcpClient tcpClient;
        Networking_UDPBroadIn udpBroad;
        Networking_UDPMultiIn udpMulti;

        public event NetGameClient_ServersDetected ServersDetected;

        public Networking_GameClient()
        {
            ServersDetected = null;
        }

        public void Connect(string serverIP)
        {
        }

        public void Disconnect()
        {
        }

        public void ProcessMessages()
        {

        }

        /// <summary>
        /// data received through server enumeration:
        /// - game name
        /// - game type
        /// - map name
        /// - connected players count
        /// - max players count
        /// - game limits
        /// - bot sophistication
        /// </summary>
        public List<Networking_GameSummary> EnumerateServers()
        {
            List<Networking_GameSummary> ret = new List<Networking_GameSummary>();

            return ret;
        }

        public void Join(Networking_GameSummary gs)
        {
        }

        public void Leave(Networking_GameSummary gs)
        {
        }
    }
}
