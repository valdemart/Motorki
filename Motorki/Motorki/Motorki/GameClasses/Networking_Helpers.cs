
namespace Motorki.GameClasses
{
    public delegate void UDP_Received(byte[] bytes);

    public class Networking_Helpers
    {
        public static byte[] Int32ToByteArray(int i)
        {
            return new byte[] { (byte)(i & 0xff), (byte)((i >> 8) & 0xff), (byte)((i >> 16) & 0xff), (byte)((i >> 24) & 0xff) };
        }

        public static int ByteArrayToInt32(byte[] ba)
        {
            return ba[0] + (((int)ba[1]) << 8) + (((int)ba[2]) << 16) + (((int)ba[3]) << 24);
        }
    }
}
