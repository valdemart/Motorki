using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Motorki.GameClasses
{
    public class Networking_UDPMultiIn
    {
        UdpClient client;
        IPEndPoint localEp;

        public event UDP_Received Received;

        public Networking_UDPMultiIn(int port)
        {
            Received = null;
            client = new UdpClient();
            localEp = new IPEndPoint(IPAddress.Any, port);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(localEp);

            IPAddress multicastaddress = IPAddress.Parse("238.0.0.222");
            client.JoinMulticastGroup(multicastaddress);
        }

        public void StartReceive()
        {
            client.BeginReceive(Receive_Callback, null);
        }

        void Receive_Callback(IAsyncResult ar)
        {
            byte[] receiveBytes = client.EndReceive(ar, ref localEp);
            if (Received != null)
                Received(receiveBytes);
        }
    }

    public class Networking_UDPMultiOut
    {
        UdpClient client;
        IPEndPoint remoteep;

        public Networking_UDPMultiOut(int port)
        {
            client = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse("238.0.0.222");
            client.JoinMulticastGroup(multicastaddress);
            remoteep = new IPEndPoint(multicastaddress, port);
        }

        public void Send(byte[] bytes)
        {
            client.Send(bytes, bytes.Length, remoteep);
        }
    }
}
