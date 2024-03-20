using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Tranquility.Security
{
    public static class SHA512
    {
        public static string Hash(string input)
        {
            String strHash = String.Empty;
            try
            {
                HashAlgorithmProvider SHA512_Provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
                CryptographicHash Hash = SHA512_Provider.CreateHash();

                IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf16BE);
                Hash.Append(buffMsg);
                IBuffer buffHash = Hash.GetValueAndReset();

                strHash = CryptographicBuffer.EncodeToBase64String(buffHash);
            }
            catch
            {

            }
                return strHash;
        }
    }
}
