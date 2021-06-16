using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Universe.CryptoLogic;
using Universe.Utility;

namespace Universe.Coin.TradeLogic
{
    public interface ITradeOptions
    {
        string AssemblyFile { get; set; }
        string ClientFullName { get; set; }
        string AccessKey { get; set; }
        string SecretKey { get; set; }

        string GetAccessKey() => Decode(AccessKey);
        string GetSecretKey() => Decode(SecretKey);

        private static string Decode(string hexa) => Crypto.Decode(getKey(), hexa.FromHexa()).ToUtf8String();
        private static string Encode(string plain) => Crypto.Encode(getKey(), plain, false).ToHexa();

        private static readonly int[] _key =
        {
            172 ^ 0xFF, 158 ^ 0xFF, 186 ^ 0xFF, 29 ^ 0xFF, 129 ^ 0xFF, 117 ^ 0xFF, 73 ^ 0xFF, 069 ^ 0xFF,
            145 ^ 0xFF, 240 ^ 0xFF, 030 ^ 0xFF, 72 ^ 0xFF, 113 ^ 0xFF, 062 ^ 0xFF, 81 ^ 0xFF, 205 ^ 0xFF
        };
        private static string getKey() => new Guid(_key.Select(x => (byte)(x ^ 0xFF)).ToArray()).ToString();


        #region ---- TEST ----

        public static void SaveEncrptedKey(string accessKey, string secretKey, string filePath)
        {
            var ak = Encode(accessKey);
            var sk = Encode(secretKey);
            File.WriteAllText(filePath, $"{ak}\n{sk}");
        }
        #endregion

    }//class
}
