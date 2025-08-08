namespace Shared.Extensions
{
    public static class ByteExtensions
    {
        public static bool GetBitBigEndian(this byte b, int bitNumber)
        {
            return (b & 1 << bitNumber) != 0;
        }

        public static bool GetBitLittleEndian(this byte b, int bitNumber)
        {
            return (b & 1 << 7 - bitNumber) != 0;
        }
    }
}
