using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ehaqui.P2P
{
    public class PeerReputation : MonoBehaviour
    {
        public static PeerReputation Instance { get; private set; }

        private Dictionary<string, float> _reputations = new();
        private Dictionary<string, int> _validationCounts = new();
        private Dictionary<string, long> _lastSeen = new();

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public float GetReputation(string peerId)
        {
            return _reputations.TryGetValue(peerId, out var rep) ? rep : 1f;
        }

        public void ReportValidation(string peerId, bool isValid)
        {
            if (!_validationCounts.ContainsKey(peerId))
                _validationCounts[peerId] = 0;

            _validationCounts[peerId]++;

            if (!_reputations.ContainsKey(peerId))
                _reputations[peerId] = 1f;

            if (isValid)
                _reputations[peerId] = Mathf.Min(1f, _reputations[peerId] + 0.1f);
            else
                _reputations[peerId] = Mathf.Max(0f, _reputations[peerId] - 0.3f);

            _lastSeen[peerId] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public bool IsTrusted(string peerId)
        {
            return GetReputation(peerId) >= 0.5f;
        }

        public void CleanupOldEntries(int maxAgeHours = 72)
        {
            var cutoff = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (maxAgeHours * 3600);
            var toRemove = new List<string>();
            foreach (var kvp in _lastSeen)
                if (kvp.Value < cutoff) toRemove.Add(kvp.Key);
            foreach (var id in toRemove)
            {
                _reputations.Remove(id);
                _validationCounts.Remove(id);
                _lastSeen.Remove(id);
            }
        }
    }
}
