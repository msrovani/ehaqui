using UnityEngine;

namespace Ehaqui.Gameplay
{
    public class MiniGameController : MonoBehaviour
    {
        public static MiniGameController Instance { get; private set; }

        public enum MiniGameType
        {
            SwipeDig = 0,
            PatternTrace = 1,
            TapBurst = 2,
            ShakeReveal = 3,
            RhythmTap = 4
        }

        public event System.Action<MiniGameType, bool> OnMiniGameComplete;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void StartMiniGame(MiniGameType type, float difficulty)
        {
            switch (type)
            {
                case MiniGameType.SwipeDig:
                    StartSwipeDig(difficulty);
                    break;
                case MiniGameType.TapBurst:
                    StartTapBurst(difficulty);
                    break;
            }
        }

        private void StartSwipeDig(float difficulty)
        {
            // UI: Swipe rápido para cavar
            // Quantidade de swipes = 3 + (difficulty * 5)
            // Tempo limite = 10s - (difficulty * 2)
            var requiredSwipes = Mathf.RoundToInt(3 + difficulty * 5);
            var timeLimit = Mathf.Max(3f, 10f - difficulty * 2);

            Debug.Log($"SwipeDig: {requiredSwipes} swipes in {timeLimit}s");
        }

        private void StartTapBurst(float difficulty)
        {
            // UI: Tocar no baú N vezes
            var requiredTaps = Mathf.RoundToInt(5 + difficulty * 10);
            Debug.Log($"TapBurst: {requiredTaps} taps");
        }

        public void CompleteMiniGame(bool success)
        {
            // Calcular hash do mini-game para anti-cheat
            var hash = ComputeMiniGameHash(success);
            Debug.Log($"MiniGame {success} | Hash: {hash}");
        }

        private string ComputeMiniGameHash(bool success)
        {
            var data = $"{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()}|{success}|{Core.GameState.Instance.PlayerId}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            var hash = sha256.ComputeHash(bytes);
            return System.Convert.ToHexString(hash).ToLower();
        }
    }
}
