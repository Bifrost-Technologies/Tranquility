using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace Tranquility.Security
{
    public class DataProtection
    {
        public static async Task<string> ProtectTest(String data)
        {
            IBuffer buffProtected = await ProtectAsync(data);

            String strDecrypted = await UnprotectData(buffProtected);
            return strDecrypted;
        }

        public static async Task<IBuffer> ProtectAsync(String data)
        {
            DataProtectionProvider Provider = new DataProtectionProvider("LOCAL=user");

            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);

            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);
            return buffProtected;
        }

        public static async Task<String> UnprotectData(IBuffer buffProtected)
        {
            DataProtectionProvider Provider = new DataProtectionProvider();

            IBuffer buffUnprotected = await Provider.UnprotectAsync(buffProtected);

            String strClearText = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffUnprotected);
            return strClearText;
        }
    }
}
