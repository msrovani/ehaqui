using UnityEngine;
using UnityEngine.UI;

namespace Ehaqui.UI
{
    public class ShopUI : MonoBehaviour
    {
        public static ShopUI Instance { get; private set; }

        [Header("Panels")]
        public GameObject ShopPanel;

        [Header("Buttons")]
        public Button BuyCoins100;
        public Button BuyCoins500;
        public Button BuyCoins1200;
        public Button BuyNoAds;
        public Button BuyHunterPass;
        public Button CloseButton;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ShopPanel?.SetActive(false);

            BuyCoins100?.onClick.AddListener(() => Buy("com.ehaqui.coins.100"));
            BuyCoins500?.onClick.AddListener(() => Buy("com.ehaqui.coins.500"));
            BuyCoins1200?.onClick.AddListener(() => Buy("com.ehaqui.coins.1200"));
            BuyNoAds?.onClick.AddListener(() => Buy("com.ehaqui.noads"));
            BuyHunterPass?.onClick.AddListener(() => Buy("com.ehaqui.hunterpass"));
            CloseButton?.onClick.AddListener(() => ShopPanel.SetActive(false));
        }

        public void Open()
        {
            ShopPanel?.SetActive(true);
        }

        private void Buy(string productId)
        {
            Debug.Log($"Buy: {productId}");
        }
    }
}
