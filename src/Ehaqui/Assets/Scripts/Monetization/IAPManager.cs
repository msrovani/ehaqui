using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.Monetization
{
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        public const string COINS_100 = "com.ehaqui.coins.100";
        public const string COINS_500 = "com.ehaqui.coins.500";
        public const string COINS_1200 = "com.ehaqui.coins.1200";
        public const string NO_ADS = "com.ehaqui.noads";
        public const string HUNTER_PASS = "com.ehaqui.hunterpass";
        public const string SEASON_PASS = "com.ehaqui.seasonpass";

        public event Action<string> OnPurchaseSuccess;
        public event Action<string, string> OnPurchaseFailed;

        private Dictionary<string, Action<bool>> _callbacks = new();

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void BuyProduct(string productId, Action<bool> callback = null)
        {
            Debug.Log($"IAP: Buying {productId}");
            _callbacks[productId] = callback;

            if (callback != null)
                callback(true);
        }

        public bool IsPassActive()
        {
            var savedDate = PlayerPrefs.GetString("hunter_pass_expiry", "");
            if (string.IsNullOrEmpty(savedDate)) return false;

            if (DateTime.TryParse(savedDate, out var expiry))
                return DateTime.UtcNow < expiry;

            return false;
        }

        public void RestorePurchases()
        {
            Debug.Log("IAP: Restoring purchases");
        }

        private void HandlePurchaseSuccess(string productId)
        {
            switch (productId)
            {
                case COINS_100:
                    Core.GameState.Instance.AddCoins(100);
                    break;
                case COINS_500:
                    Core.GameState.Instance.AddCoins(500);
                    break;
                case COINS_1200:
                    Core.GameState.Instance.AddCoins(1200);
                    break;
                case NO_ADS:
                    Core.GameState.Instance.NoAds = true;
                    break;
                case HUNTER_PASS:
                    Core.GameState.Instance.HasHunterPass = true;
                    PlayerPrefs.SetString("hunter_pass_expiry",
                        DateTime.UtcNow.AddMonths(1).ToString("O"));
                    break;
            }

            if (_callbacks.TryGetValue(productId, out var cb))
            {
                cb?.Invoke(true);
                _callbacks.Remove(productId);
            }

            OnPurchaseSuccess?.Invoke(productId);
            Core.GameState.Instance.SaveToDisk();
        }

        private void HandlePurchaseFailed(string productId, string reason)
        {
            if (_callbacks.TryGetValue(productId, out var cb))
            {
                cb?.Invoke(false);
                _callbacks.Remove(productId);
            }
            OnPurchaseFailed?.Invoke(productId, reason);
        }
    }
}
