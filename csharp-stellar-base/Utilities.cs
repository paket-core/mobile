using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stellar
{
    public static class Utilities
    {
        public static byte[] Hash(byte[] data)
        {
			var mySHA256 = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
			//SHA256 mySHA256 = SHA256Managed.Create();
			//byte[] hash = mySHA256.ComputeHash(data);
			byte[] hash = mySHA256.HashData(data);
            return hash;
        }
    }
}
