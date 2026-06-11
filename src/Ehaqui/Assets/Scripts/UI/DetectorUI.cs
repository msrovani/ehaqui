using UnityEngine;
using UnityEngine.UI;

namespace Ehaqui.UI
{
    public class DetectorUI : MonoBehaviour
    {
        public static DetectorUI Instance { get; private set; }

        [Header("UI Elements")]
        public Image DetectorFill;
        public Text DistanceLabel;
        public Text LevelLabel;
        public GameObject FoundPanel;
        public Text FoundNameLabel;

        [Header("Colors")]
        public Color ColdColor = Color.blue;
        public Color WarmColor = Color.yellow;
        public Color HotColor = Color.red;
        public Color BurningColor = Color.magenta;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Gameplay.VibrationController.Instance.OnVibrationLevelChanged += OnVibrationChange;
            FoundPanel?.SetActive(false);
        }

        private void OnVibrationChange(int level)
        {
            var label = Gameplay.VibrationController.Instance.GetVibrationLabel(level);
            LevelLabel.text = label;

            DetectorFill.fillAmount = level / 5f;

            DetectorFill.color = level switch
            {
                0 => ColdColor,
                1 => WarmColor,
                2 => WarmColor,
                3 => HotColor,
                4 => BurningColor,
                5 => BurningColor,
                _ => ColdColor
            };
        }

        public void UpdateDistance(double distance)
        {
            DistanceLabel.text = distance switch
            {
                < 1 => "< 1m",
                < 1000 => $"{distance:F0}m",
                _ => $"{distance / 1000:F1}km"
            };
        }

        public void ShowFound(Gameplay.TreasureData treasure)
        {
            FoundPanel?.SetActive(true);
            if (FoundNameLabel != null)
                FoundNameLabel.text = $"ACHOU! {treasure.Name}";
        }
    }
}
