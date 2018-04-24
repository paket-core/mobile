﻿//using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System;

namespace Stellar
{
    public class KeyPair
    {
        public byte[] PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] SeedBytes { get; private set; }
        
        public Generated.AccountID AccountId
        {
            get
            {
                return new Generated.AccountID
                {
                    InnerValue = new Generated.PublicKey
                    {
                        Discriminant = Generated.PublicKeyType.Create(Generated.PublicKeyType.PublicKeyTypeEnum.PUBLIC_KEY_TYPE_ED25519),
                        Ed25519 = new Generated.Uint256(PublicKey)
                    }
                };
            }
        }

        public string Address
        {
            get
            {
                return StrKey.EncodeCheck(VersionByte.ed25519Publickey, PublicKey);
            }
        }

        public string Seed
        {
            get
            {
                return StrKey.EncodeCheck(VersionByte.ed25519SecretSeed, SeedBytes);
            }
        }

        public byte[] SignatureHint
        {
            get
            {
                var stream = new Generated.ByteWriter();
                Generated.AccountID.Encode(stream, AccountId);
                var bytes = stream.ToArray();
                var length = bytes.Length;
                return bytes.Skip(length - 4).Take(4).ToArray();
            }
        }

        public KeyPair(string pubKey, string secretKey, string seed)
			: this(Encoding.UTF8.GetBytes(pubKey),
			       Encoding.UTF8.GetBytes(secretKey),
			       Encoding.UTF8.GetBytes(seed))
        {
        }

        public KeyPair(byte[] pubKey, byte[] secretKey, byte[] seed)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 32 bytes");
            }

            if (secretKey.Length != 64)
            {
                throw new ArgumentException("secretKey must be 64 bytes");
            }

            if (seed.Length != 32)
            {
                throw new ArgumentException("seed must be 32 bytes");
            }

            PublicKey = pubKey;
            PrivateKey = secretKey;
            SeedBytes = seed;
        }

        public KeyPair(byte[] pubKey, byte[] seed)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 32 bytes");
            }

            if (seed.Length != 32)
            {
                throw new ArgumentException("seed must be 32 bytes");
            }

            PublicKey = pubKey;
            PrivateKey = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(seed);
            SeedBytes = seed;
        }

        public KeyPair(byte[] pubKey)
        {
            if (pubKey.Length != 32)
            {
                throw new ArgumentException("pubKey must be 32 bytes");
            }

            PublicKey = pubKey;
        }

        public byte[] Sign(byte[] message)
        {
            return Chaos.NaCl.Ed25519.Sign(message, PrivateKey);
        }

        public byte[] Sign(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Chaos.NaCl.Ed25519.Sign(bytes, PrivateKey);
        }

        public Generated.DecoratedSignature SignDecorated(byte[] message)
        {
            var rawSig = Sign(message);
            return new Generated.DecoratedSignature
            {
                Hint = new Generated.SignatureHint(SignatureHint),
                Signature = new Generated.Signature(rawSig)
            };
        }

        public bool Verify(byte[] signature, byte[] message)
        {
            try
            {
                return Chaos.NaCl.Ed25519.Verify(signature, message, PublicKey);
            }
            catch
            {
                return false;
            }
        }

        public static KeyPair FromXdrPublicKey(Generated.PublicKey key)
        {
            return FromPublicKey(key.Ed25519.InnerValue);
        }

        public static KeyPair FromSeed(string seed)
        {
            var bytes = StrKey.DecodeCheck(VersionByte.ed25519SecretSeed, seed);
            return FromRawSeed(bytes);
        }

        public static KeyPair FromRawSeed(byte[] seed)
        {
            byte[] pubKey, privKey;
            Chaos.NaCl.Ed25519.KeyPairFromSeed(out pubKey, out privKey, seed);
            return new KeyPair(pubKey, privKey, seed);
        }

        public static KeyPair FromPublicKey(byte[] bytes)
        {
            return new KeyPair(bytes);
        }

        [Obsolete("Use FromPublickey instead.")]
        public static KeyPair FromAccountId(string accountId)
        {
            var bytes = StrKey.DecodeCheck(VersionByte.ed25519Publickey, accountId);
            return FromPublicKey(bytes);
        }

        [Obsolete("Use FromPublickey instead.")]
        public static KeyPair FromAddress(string accountId)
        {
            return FromAccountId(accountId);
        }

		public static KeyPair Random()
		{
			var b = new byte[32];
			//using (var rngCrypto = new RNGCryptoServiceProvider())
			//{
			var rngCrypto = PCLCrypto.NetFxCrypto.RandomNumberGenerator;
			rngCrypto.GetBytes(b);
			//}
			return KeyPair.FromRawSeed(b);
		}

        public static KeyPair FromNetworkPassphrase(string passPhrase)
        {
			var mySHA256 = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
            //SHA256 mySHA256 = SHA256Managed.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(passPhrase);
			//byte[] networkId = mySHA256.ComputeHash(bytes);
			byte[] networkId = mySHA256.HashData(bytes);
            return FromRawSeed(networkId);
        }

        public static KeyPair Master()
        {
            return FromRawSeed(Network.CurrentNetworkId);
        }
    }
}