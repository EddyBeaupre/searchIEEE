using System;
using System.Text.RegularExpressions;

namespace searchOUIDB.CustomExtensions
{
    public static class StringExtension
    {
        public static Byte[] GetBytes(this String payLoad)
        {
            byte[] bytes = new byte[payLoad.Length * sizeof(char)];
            Buffer.BlockCopy(payLoad.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static String GetOid(this String payLoad)
        {
            try
            {
                Regex rgx = new Regex("[^a-fA-F0-9]");
                return (rgx.Replace(payLoad, "").ToUpper().PadRight(12, '0'));
            }
            catch
            {
                return (String.Empty);
            }

        }

        public static UInt64 GetOid64(this String payLoad)
        {
            try
            {
                return (Convert.ToUInt64("0x" + GetOid(payLoad), 16));
            }
            catch
            {
                return (UInt64.MaxValue);
            }
        }
    }

    public static class ByteExtension
    {
        public static String GetString(this Byte[] payLoad)
        {
            char[] chars = new char[payLoad.Length / sizeof(char)];
            Buffer.BlockCopy(payLoad, 0, chars, 0, payLoad.Length);
            return new string(chars);
        }
    }
}
