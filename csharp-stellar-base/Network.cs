//using System.Security.Cryptography;
using System.Text;

namespace Stellar
{
    public static class Network
    {
        public static string CurrentNetwork { get; set; } = "";
        public static byte[] CurrentNetworkId
        {
            get
            {
                //SHA256 mySHA256 = SHA256Managed.Create();
				var mySHA256 = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
				byte[] bytes = Encoding.UTF8.GetBytes(CurrentNetwork);
				//return mySHA256.ComputeHash(bytes);
				return mySHA256.HashData(bytes);
            }
        }
    }
}