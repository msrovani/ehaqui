using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Ehaqui.Offline
{
    public class CryptoStorage : MonoBehaviour
    {
        public static CryptoStorage Instance { get; private set; }

        private byte[] _key;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            var deviceId = SystemInfo.deviceUniqueIdentifier;
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(deviceId + "ehaqui_aes_salt_2026"));
        }

        public string Encrypt(string plaintext)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            var result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string ciphertext)
        {
            try
            {
                var fullBytes = Convert.FromBase64String(ciphertext);
                using var aes = Aes.Create();
                aes.Key = _key;

                var iv = new byte[aes.BlockSize / 8];
                var encrypted = new byte[fullBytes.Length - iv.Length];
                Buffer.BlockCopy(fullBytes, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullBytes, iv.Length, encrypted, 0, encrypted.Length);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception e)
            {
                Debug.LogError($"Decryption failed: {e.Message}");
                return "";
            }
        }

        public string HashData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
