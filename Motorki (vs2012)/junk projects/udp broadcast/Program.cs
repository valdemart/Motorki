using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_broadcast
{
    class Program
    {
        public static byte[] Int32ToByteArray(int i)
        {
            return new byte[] { (byte)(i & 0xff), (byte)((i >> 8) & 0xff), (byte)((i >> 16) & 0xff), (byte)((i >> 24) & 0xff) };
        }

        public static int ByteArrayToInt32(byte[] ba, int offset)
        {
            return ba[0 + offset] + (((int)ba[1 + offset]) << 8) + (((int)ba[2 + offset]) << 16) + (((int)ba[3 + offset]) << 24);
        }

        static void Main(string[] args)
        {
            Console.Write("Start broadcast(b) or multicast(m)? "); char mode = Console.ReadKey(false).KeyChar; Console.WriteLine();
            Console.Write("Start server (s) or client(c)? "); char side = Console.ReadKey(false).KeyChar; Console.WriteLine();
            switch (mode)
            {
                case 'b':
                    {
                        switch (side)
                        {
                            case 's':
                                {
                                    //broadcast udp out
                                    Console.WriteLine("Press any key to send a message"); Console.ReadKey(true);

                                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                    IPAddress[] addrs = Dns.GetHostAddresses(Dns.GetHostName());
                                    IPAddress broadcast = null;
                                    for(int i=0; i<addrs.Length; i++)
                                        if(addrs[i].AddressFamily==AddressFamily.InterNetwork)
                                            broadcast = addrs[i];
                                    broadcast = IPAddress.Parse(broadcast.ToString().Substring(0, broadcast.ToString().LastIndexOf('.')) + ".255");
                                    s.ExclusiveAddressUse = false;

                                    IPEndPoint ep = new IPEndPoint(broadcast, 11000);

                                    for (int i = 0; i <= 8000; i++)
                                    {
                                        byte[] buffer = Int32ToByteArray(i);
                                        s.SendTo(buffer, ep);
                                        Console.WriteLine("Sent " + i);
                                    }

                                    Console.WriteLine("Message sent to the broadcast address");
                                }
                                break;
                            case 'c':
                                {
                                    //broadcast udp in
                                    UdpClient listener = new UdpClient();
                                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 11000);
                                    listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                    listener.ExclusiveAddressUse = false;
                                    listener.Client.Bind(groupEP);

                                    try
                                    {
                                        Console.WriteLine("Waiting for broadcast");

                                        int check = -1;
                                        while (true)
                                        {
                                            Byte[] data = listener.Receive(ref groupEP);
                                            for (int n = 0; n < data.Length / 4; n++)
                                            {
                                                int i = ByteArrayToInt32(data, 4 * n);
                                                if (check == -1)
                                                    check = i;
                                                if (check == i)
                                                    check++;
                                                Console.WriteLine(i + (check - 1 == i ? " OK" : " ERROR"));
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                    finally
                                    {
                                        listener.Close();
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case 'm':
                    {
                        switch (side)
                        {
                            case 's':
                                {
                                    //multicast udp out
                                    UdpClient udpclient = new UdpClient();

                                    IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                                    udpclient.JoinMulticastGroup(multicastaddress);
                                    IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

                                    Byte[] buffer = null;

                                    Console.WriteLine("Press ENTER to start sending messages");
                                    Console.ReadLine();

                                    for (int i = 0; i <= 8000; i++)
                                    {
                                        buffer = Int32ToByteArray(i);
                                        udpclient.Send(buffer, buffer.Length, remoteep);
                                        Console.WriteLine("Sent " + i);
                                    }
                                }
                                break;
                            case 'c':
                                {
                                    //multicast udp in
                                    UdpClient client = new UdpClient();

                                    client.ExclusiveAddressUse = false;
                                    IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2222);

                                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                    client.ExclusiveAddressUse = false;

                                    client.Client.Bind(localEp);

                                    IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                                    client.JoinMulticastGroup(multicastaddress);

                                    Console.WriteLine("Listening this will quit on key press");

                                    int check = -1;
                                    while (true)
                                    {
                                        Byte[] data = client.Receive(ref localEp);
                                        for (int n = 0; n < data.Length / 4; n++)
                                        {
                                            int i = ByteArrayToInt32(data, 4 * n);
                                            if (check == -1)
                                                check = i;
                                            if (check == i)
                                                check++;
                                            Console.WriteLine(i + (check - 1 == i ? " OK" : " ERROR"));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            
            Console.WriteLine("\nExiting..."); Console.ReadKey(true);
        }
    }
}
