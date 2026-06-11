using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Ehaqui.Blockchain
{
    public class CryptoIdentity
    {
        public string PublicKey { get; private set; }
        private ECDsa _ecdsa;

        public CryptoIdentity()
        {
            _ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var pubKey = _ecdsa.ExportSubjectPublicKeyInfo();
            PublicKey = Convert.ToBase64String(pubKey);
        }

        public string SignData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var signature = _ecdsa.SignData(bytes, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(signature);
        }

        public static bool VerifySignature(string data, string signature, string publicKeyBase64)
        {
            try
            {
                var pubKeyBytes = Convert.FromBase64String(publicKeyBase64);
                var ecdsa = ECDsa.Create();
                ecdsa.ImportSubjectPublicKeyInfo(pubKeyBytes, out _);

                var dataBytes = Encoding.UTF8.GetBytes(data);
                var sigBytes = Convert.FromBase64String(signature);
                return ecdsa.VerifyData(dataBytes, sigBytes, HashAlgorithmName.SHA256);
            }
            catch
            {
                return false;
            }
        }

        public static string ComputeKeccak256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using var sha3 = SHA256.Create();
            var hash = sha3.ComputeHash(bytes);
            return "0x" + Convert.ToHexString(hash).ToLower();
        }
    }

    public class P2PContract
    {
        public string TreasureId;
        public string CreatorPublicKey;
        public double Latitude;
        public double Longitude;
        public string HintHash;
        public long CreatedAt;
        public long ExpiresAt;
        public string CreatorSignature;

        public string Serialize()
        {
            return $"{TreasureId}|{CreatorPublicKey}|{Latitude}|{Longitude}|{HintHash}|{CreatedAt}|{ExpiresAt}";
        }

        public string Sign(CryptoIdentity identity)
        {
            CreatorSignature = identity.SignData(Serialize());
            return CreatorSignature;
        }

        public bool Verify()
        {
            return CryptoIdentity.VerifySignature(Serialize(), CreatorSignature, CreatorPublicKey);
        }
    }
}
