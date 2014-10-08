using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Motorki.GameClasses
{
    public class Networking_GameServer
    {
        UdpClient udpBroadcast;
        TcpListener tcpListener;
        List<TcpClient> tcpClients;
        int lastSecond;
        IPAddress[] ipAddresses;

        public Networking_GameServer()
        {
            tcpClients = new List<TcpClient>();
        }

        public void StartServer()
        {
            ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            tcpListener = new TcpListener(ipAddresses[0], 2222);
            tcpListener.Start(10000);

            udpBroadcast = new UdpClient();
            udpBroadcast.EnableBroadcast = true;
            udpBroadcast.ExclusiveAddressUse = false;
            udpBroadcast.Connect(IPAddress.Broadcast, 2222);

            lastSecond = DateTime.Now.Second;
        }

        public void KillServer()
        {
            tcpListener.Stop();

            udpBroadcast.Close();
        }

        public void ProcessMessages()
        {
            //ping own presence
            if (lastSecond != DateTime.Now.Second)
            {
                byte[] buffer = new byte[4 + ipAddresses.Length * 4];
                Buffer.BlockCopy(Networking_Helpers.Int32ToByteArray(ipAddresses.Length), 0, buffer, 0, 4);
                for (int i = 0; i < ipAddresses.Length; i++)
                    Buffer.BlockCopy(ipAddresses[i].GetAddressBytes(), 0, buffer, 4 * (i + 1), 4);
                udpBroadcast.Send(buffer, buffer.Length);
            }

            //check for incoming connections
            while (tcpListener.Pending())
                tcpClients.Add(tcpListener.AcceptTcpClient());

            //process messages
        }
    }
}
