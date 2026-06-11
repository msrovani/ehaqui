using UnityEngine;
using UnityEngine.UI;

namespace Ehaqui.UI
{
    public class HUD : MonoBehaviour
    {
        public static HUD Instance { get; private set; }

        [Header("Status")]
        public Text NicknameLabel;
        public Text LevelLabel;
        public Text CoinLabel;
        public Text XpLabel;
        public Text NearbyCountLabel;
        public Text SyncLabel;

        [Header("Buttons")]
        public Button CreateTreasureButton;
        public Button ScanQrButton;
        public Button ShopButton;
        public Button SettingsButton;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Core.GameState.Instance.OnCoinsChanged += coins => CoinLabel.text = coins.ToString();
            Core.GameState.Instance.OnXpChanged += xp => XpLabel.text = xp.ToString();
            Core.GameState.Instance.OnLevelUp += level => LevelLabel.text = $"Nv {level}";

            CreateTreasureButton.onClick.AddListener(OnCreateTreasure);
            ScanQrButton.onClick.AddListener(OnScanQR);
            ShopButton.onClick.AddListener(OnShop);
            SettingsButton.onClick.AddListener(OnSettings);
        }

        private void Update()
        {
            if (Core.GameState.Instance != null)
            {
                if (NicknameLabel != null)
                    NicknameLabel.text = Core.GameState.Instance.Nickname;

                if (SyncLabel != null)
                {
                    var pending = Core.GameState.Instance.PendingSyncCount;
                    SyncLabel.enabled = pending > 0;
                    if (pending > 0)
                        SyncLabel.text = $"{pending} pendentes";
                }
            }

            if (NearbyCountLabel != null && Gameplay.TreasureManager.Instance != null)
            {
                var count = Gameplay.TreasureManager.Instance.GetNearbyTreasures(
                    GPS.GpsService.Instance.Latitude,
                    GPS.GpsService.Instance.Longitude).Count;
                NearbyCountLabel.text = $"{count} tesouros próximos";
            }
        }

        private void OnCreateTreasure()
        {
            // Abrir tela de criação
        }

        private void OnScanQR()
        {
            // Abrir câmera para QR
        }

        private void OnShop()
        {
            // Abrir loja
        }

        private void OnSettings()
        {
            // Abrir configurações
        }
    }
}
