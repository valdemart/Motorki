using System;
using System.Net;
using System.Net.Sockets;

namespace Motorki.GameClasses
{
    public class Networking_UDPBroadIn
    {
        UdpClient client;
        IPEndPoint groupEP;
        public event UDP_Received Received;

        public Networking_UDPBroadIn()
        {
            Received = null;
            client = new UdpClient();
            groupEP = new IPEndPoint(IPAddress.Any, 2222);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(groupEP);
        }

        public void StartReceive()
        {
            client.BeginReceive(ReceiveCallback, this);
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            byte[] receiveBytes = client.EndReceive(ar, ref groupEP);
            if (Received != null)
                Received(receiveBytes);
        }
    }

    public class Networking_UDPBroadOut
    {
        Socket s;
        IPEndPoint ep;

        public Networking_UDPBroadOut()
        {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress[] addrs = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress broadcast = null;
            for (int i = 0; i < addrs.Length; i++)
                if (addrs[i].AddressFamily == AddressFamily.InterNetwork)
                    broadcast = addrs[i];
            if (broadcast == null)
                throw new SocketException();
            broadcast = IPAddress.Parse(broadcast.ToString().Substring(0, broadcast.ToString().LastIndexOf('.')) + ".255");
            s.ExclusiveAddressUse = false;
            ep = new IPEndPoint(broadcast, 11000);
        }

        public void Send(byte[] bytes)
        {
            s.SendTo(bytes, ep);
        }
    }
}
