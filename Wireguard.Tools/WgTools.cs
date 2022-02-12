using System;
using System.Linq;
using NSec.Cryptography;

namespace ArkProjects.Wireguard.Tools
{
    public static class WgTools
    {
        /// <summary>
        /// Generate private key
        /// </summary>
        /// <remarks>See: <c>wireguard-tools/src/genkey.c</c></remarks>
        /// <returns>byte[32] array</returns>
        public static byte[] GenPrivateKey()
        {
            using var key = Key.Create(KeyAgreementAlgorithm.X25519, new KeyCreationParameters()
            {
                ExportPolicy = KeyExportPolicies.AllowPlaintextExport
            });
            var privKey = key.Export(KeyBlobFormat.RawPrivateKey);
            return privKey;
        }

        /// <summary>
        /// Generate pre-shared key
        /// </summary>
        /// <remarks>See: <c>wireguard-tools/src/curve25519.h</c></remarks>
        /// <returns>byte[32] array</returns>
        public static byte[] GenPreSharedKey()
        {
            var secret = GenPrivateKey();
            secret[0] &= 248;
            secret[31] = (byte)((secret[31] & 127) | 64);

            return secret;
        }

        /// <summary>
        /// Generate public key
        /// </summary>
        /// <remarks>See: <c>wireguard-tools/src/pubkey.c</c></remarks>
        /// <returns>byte[32] array</returns>
        public static byte[] GenPublicKey(byte[] privateKey)
        {
            if (privateKey == null) 
                throw new ArgumentNullException(nameof(privateKey));

            using var key = Key.Import(KeyAgreementAlgorithm.X25519, privateKey, KeyBlobFormat.RawPrivateKey);
            var privKey = key.Export(KeyBlobFormat.RawPublicKey);
            return privKey;
        }

        /// <summary>
        /// Validate key pair
        /// </summary>
        /// <param name="privKey">32 byte key</param>
        /// <param name="pubKey">32 byte key</param>
        public static bool KeyPairValid(byte[] privKey, byte[] pubKey)
        {
            if (privKey is not { Length: 32 } || pubKey is not { Length: 32 })
                return false;
            try
            {
                using var key = Key.Import(KeyAgreementAlgorithm.X25519, privKey, KeyBlobFormat.RawPrivateKey);
                return key.Export(KeyBlobFormat.RawPublicKey).SequenceEqual(pubKey);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate public key
        /// </summary>
        /// <param name="keyBytes">32 byte key</param>
        public static bool PublicKeyValid(byte[] keyBytes)
        {
            return keyBytes is { Length: 32 };
        }

        /// <summary>
        /// Validate private key
        /// </summary>
        /// <param name="keyBytes">32 byte key</param>
        public static bool PrivateKeyValid(byte[] keyBytes)
        {
            if (keyBytes is not { Length: 32 })
                return false;
            try
            {
                using var key = Key.Import(KeyAgreementAlgorithm.X25519, keyBytes, KeyBlobFormat.RawPrivateKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate pre-shared key
        /// </summary>
        /// <param name="key">32 byte key</param>
        public static bool PreSharedKeyValid(byte[] key)
        {
            return key is { Length: 32 };
        }

        /// <summary>
        /// Validate key pair
        /// </summary>
        /// <param name="privKey">44 char B64 string</param>
        /// <param name="pubKey">44 char B64 string</param>
        public static bool KeyPairValid(string privKey, string pubKey) => WrapCheck(privKey, pubKey, KeyPairValid);

        /// <summary>
        /// Validate public key
        /// </summary>
        /// <param name="key">44 char B64 string</param>
        public static bool PublicKeyValid(string key) => WrapCheck(key, PublicKeyValid);

        /// <summary>
        /// Validate private key
        /// </summary>
        /// <param name="key">44 char B64 string</param>
        public static bool PrivateKeyValid(string key) => WrapCheck(key, PrivateKeyValid);

        /// <summary>
        /// Validate pre-shared key
        /// </summary>
        /// <param name="key">44 char B64 string</param>
        public static bool PreSharedKeyValid(string key) => WrapCheck(key, PreSharedKeyValid);

        private static bool WrapCheck(string key, Func<byte[], bool> checkFunc)
        {
            if (key is not { Length: 44 })
                return false;

            try
            {
                return checkFunc(Convert.FromBase64String(key));
            }
            catch
            {
                return false;
            }
        }

        private static bool WrapCheck(string key1, string key2, Func<byte[], byte[], bool> checkFunc)
        {
            if (key1 is not { Length: 44 } || key2 is not { Length: 44 })
                return false;

            try
            {
                return checkFunc(Convert.FromBase64String(key1), Convert.FromBase64String(key2));
            }
            catch
            {
                return false;
            }
        }
    }
}