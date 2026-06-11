using System.Collections;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Ehaqui.Gameplay
{
    public class VibrationController : MonoBehaviour
    {
        public static VibrationController Instance { get; private set; }

        [Header("Config")]
        public float MaxDetectDistance = 50f;
        public int VibrationLevel = 0;

        public event System.Action<int> OnVibrationLevelChanged;

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _PlayHaptic(int level);
#endif

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void UpdateVibration(double distance)
        {
            var normalized = Mathf.Clamp01(1f - ((float)distance / MaxDetectDistance));
            var newLevel = normalized switch
            {
                < 0.15f => 0,
                < 0.30f => 1,
                < 0.50f => 2,
                < 0.75f => 3,
                < 0.90f => 4,
                _ => 5
            };

            if (newLevel != VibrationLevel)
            {
                VibrationLevel = newLevel;
                OnVibrationLevelChanged?.Invoke(newLevel);
                Vibrate(newLevel);
            }
        }

        public void Vibrate(int level)
        {
            if (level == 0) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrate(level);
#elif UNITY_IOS && !UNITY_EDITOR
            IOSVibrate(level);
#else
            Handheld.Vibrate();
#endif
        }

        private void AndroidVibrate(int level)
        {
            using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            using var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            using var effectClass = new AndroidJavaClass("android.os.VibrationEffect");

            long[] pattern = level switch
            {
                1 => new long[] { 0, 100, 50, 100 },
                2 => new long[] { 0, 200, 50, 200 },
                3 => new long[] { 0, 300, 50, 300, 50, 300 },
                4 => new long[] { 0, 100, 30, 100, 30, 100, 30, 100 },
                5 => new long[] { 0, 50, 20, 50, 20, 50, 20, 50, 20, 50, 20, 50 },
                _ => null
            };

            if (pattern != null)
            {
                using var effect = effectClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, -1);
                vibrator.Call("vibrate", effect);
            }
        }

        private void IOSVibrate(int level)
        {
#if UNITY_IOS
            _PlayHaptic(level);
#endif
        }

        public string GetVibrationLabel(int level)
        {
            return level switch
            {
                0 => "Frio",
                1 => "Morno",
                2 => "Quente",
                3 => "Muito Quente!",
                4 => "Pegando Fogo!",
                5 => "ACHOU!",
                _ => ""
            };
        }
    }
}
