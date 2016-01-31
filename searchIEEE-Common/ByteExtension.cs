using System;

namespace searchIEEE.CustomExtensions
{
    public static class ByteExtension
    {
        public static String GetString(this Byte[] payLoad)
        {
            char[] chars = new char[payLoad.Length / sizeof(char)];
            System.Buffer.BlockCopy(payLoad, 0, chars, 0, payLoad.Length);
            return new string(chars);
        }
    }
}
