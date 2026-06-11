using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ehaqui.Gameplay
{
    [Serializable]
    public class TreasureData
    {
        public string Id;
        public TreasureType Type;
        public string Name;
        public string Description;
        public string Hint;
        public string HintHash;
        public double Latitude;
        public double Longitude;
        public float Radius;
        public DifficultyLevel Difficulty;
        public string CreatorId;
        public string CreatorSignature;
        public long CreatedAt;
        public long ExpiresAt;
        public int PrizePool;
        public bool HasBeenFound;
        public string FoundBy;
        public long FoundAt;

        public enum TreasureType
        {
            Virtual = 0,
            Physical = 1,
            Sentimental = 2,
            HighValue = 3
        }

        public enum DifficultyLevel
        {
            Easy = 0,
            Medium = 1,
            Hard = 2,
            Legendary = 3
        }
    }

    public class TreasureManager : MonoBehaviour
    {
        public static TreasureManager Instance { get; private set; }

        public List<TreasureData> ActiveTreasures = new();
        public event Action<TreasureData> OnTreasureCreated;
        public event Action<TreasureData, string> OnTreasureFound;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public TreasureData CreateTreasure(TreasureData.TreasureType type, string name, string hint,
            double lat, double lng, float radius, TreasureData.DifficultyLevel difficulty,
            int ttlHours = 24)
        {
            var treasure = new TreasureData
            {
                Id = Guid.NewGuid().ToString("N"),
                Type = type,
                Name = name,
                Hint = hint,
                HintHash = ComputeHash(hint),
                Latitude = lat,
                Longitude = lng,
                Radius = radius,
                Difficulty = difficulty,
                CreatorId = Core.GameState.Instance.PlayerId,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (ttlHours * 3600),
                PrizePool = CalculatePrize(difficulty)
            };

            ActiveTreasures.Add(treasure);

            var sig = SignTreasure(treasure);
            treasure.CreatorSignature = sig;

            OnTreasureCreated?.Invoke(treasure);

            P2P.P2PManager.Instance?.BroadcastTreasure(lat, lng, treasure.HintHash, sig);

            return treasure;
        }

        public bool TryFindTreasure(string treasureId)
        {
            var treasure = ActiveTreasures.Find(t => t.Id == treasureId && !t.HasBeenFound);
            if (treasure == null) return false;

            var dist = GPS.GpsService.Instance.DistanceTo(treasure.Latitude, treasure.Longitude);
            if (dist > treasure.Radius) return false;

            treasure.HasBeenFound = true;
            treasure.FoundBy = Core.GameState.Instance.PlayerId;
            treasure.FoundAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var xp = CalculateXpReward(treasure);
            Core.GameState.Instance.AddXp(xp);

            OnTreasureFound?.Invoke(treasure, Core.GameState.Instance.PlayerId);
            return true;
        }

        public List<TreasureData> GetNearbyTreasures(double lat, double lng, double radiusKm = 1)
        {
            var result = new List<TreasureData>();
            foreach (var t in ActiveTreasures)
            {
                if (t.HasBeenFound) continue;
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > t.ExpiresAt) continue;
                var dist = GPS.GpsService.Haversine(lat, lng, t.Latitude, t.Longitude);
                if (dist <= radiusKm * 1000)
                    result.Add(t);
            }
            return result;
        }

        private string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        private string SignTreasure(TreasureData treasure)
        {
            var data = $"{treasure.Id}|{treasure.Latitude}|{treasure.Longitude}|{treasure.HintHash}|{treasure.CreatedAt}";
            return ComputeHash(data + treasure.CreatorId);
        }

        private int CalculatePrize(TreasureData.DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                TreasureData.DifficultyLevel.Easy => 10,
                TreasureData.DifficultyLevel.Medium => 25,
                TreasureData.DifficultyLevel.Hard => 50,
                TreasureData.DifficultyLevel.Legendary => 100,
                _ => 10
            };
        }

        private int CalculateXpReward(TreasureData treasure)
        {
            var baseXp = treasure.Difficulty switch
            {
                TreasureData.DifficultyLevel.Easy => 20,
                TreasureData.DifficultyLevel.Medium => 50,
                TreasureData.DifficultyLevel.Hard => 100,
                TreasureData.DifficultyLevel.Legendary => 250,
                _ => 20
            };

            var hoursAlive = (float)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - treasure.CreatedAt) / 3600;
            var timeBonus = Mathf.FloorToInt(hoursAlive * 2);

            return baseXp + timeBonus;
        }
    }
}
