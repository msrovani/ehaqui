using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ehaqui.Gameplay
{
    [Serializable]
    public class ChainData
    {
        public string Id;
        public string Name;
        public List<ClueStep> Clues = new();
        public int CurrentStep;
        public bool IsComplete;
        public string CreatorId;
        public long CreatedAt;

        [Serializable]
        public class ClueStep
        {
            public int StepNumber;
            public string Riddle;
            public string AnswerHash;
            public double Latitude;
            public double Longitude;
            public float Radius;
            public bool IsSolved;
        }
    }

    public class ChainManager : MonoBehaviour
    {
        public static ChainManager Instance { get; private set; }

        public ChainData ActiveChain;
        public event Action<ChainData> OnChainStarted;
        public event Action<int> OnStepSolved;
        public event Action<ChainData> OnChainComplete;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public ChainData CreateChain(string name, List<(string riddle, string answer, double lat, double lng, float radius)> clues)
        {
            if (clues.Count < 3 || clues.Count > 10)
            {
                Debug.LogError("Chain must have 3-10 clues");
                return null;
            }

            var chain = new ChainData
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                CreatorId = Core.GameState.Instance.PlayerId,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            for (int i = 0; i < clues.Count; i++)
            {
                var c = clues[i];
                chain.Clues.Add(new ChainData.ClueStep
                {
                    StepNumber = i + 1,
                    Riddle = c.riddle,
                    AnswerHash = ComputeHash(c.answer.ToLower().Trim()),
                    Latitude = c.lat,
                    Longitude = c.lng,
                    Radius = c.radius,
                    IsSolved = false
                });
            }

            chain.CurrentStep = 0;
            chain.IsComplete = false;
            ActiveChain = chain;
            OnChainStarted?.Invoke(chain);
            return chain;
        }

        public bool TrySolveStep(string answer)
        {
            if (ActiveChain == null || ActiveChain.IsComplete) return false;

            var currentClue = ActiveChain.Clues[ActiveChain.CurrentStep];
            var answerHash = ComputeHash(answer.ToLower().Trim());

            if (answerHash != currentClue.AnswerHash) return false;

            currentClue.IsSolved = true;
            ActiveChain.CurrentStep++;

            if (ActiveChain.CurrentStep >= ActiveChain.Clues.Count)
            {
                ActiveChain.IsComplete = true;
                OnChainComplete?.Invoke(ActiveChain);

                var bonus = ActiveChain.Clues.Count * 50;
                Core.GameState.Instance.AddXp(bonus);
            }
            else
            {
                OnStepSolved?.Invoke(ActiveChain.CurrentStep);
            }

            return true;
        }

        public ChainData.ClueStep GetCurrentClue()
        {
            if (ActiveChain == null || ActiveChain.IsComplete) return null;
            return ActiveChain.Clues[ActiveChain.CurrentStep];
        }

        private string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
