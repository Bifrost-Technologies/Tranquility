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
            IBuffer buffProtected = null;
            try
            {
                DataProtectionProvider Provider = new DataProtectionProvider("LOCAL=user");

                IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);

                buffProtected = await Provider.ProtectAsync(buffMsg);
            }
            catch
            {

            }
            return buffProtected;
        }

        public static async Task<String> UnprotectData(IBuffer buffProtected)
        {
            IBuffer buffUnprotected = null;
            String strClearText = String.Empty;
            try
            {
                DataProtectionProvider Provider = new DataProtectionProvider();

                buffUnprotected = await Provider.UnprotectAsync(buffProtected);

                strClearText = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffUnprotected);
            }
            catch
            {

            }
            return strClearText;
        }
    }
}
