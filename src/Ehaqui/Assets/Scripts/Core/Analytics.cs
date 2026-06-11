using System.Collections;
using UnityEngine;

namespace Ehaqui.Core
{
    public class Analytics : MonoBehaviour
    {
        public static Analytics Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void TrackEvent(string eventName, params (string key, string value)[] properties)
        {
            Debug.Log($"[Analytics] {eventName}");
        }

        public void TrackTreasureFound(string treasureId, string type, int xpGained)
        {
            TrackEvent("treasure_found",
                ("treasure_id", treasureId),
                ("type", type),
                ("xp", xpGained.ToString()));
        }

        public void TrackTreasureCreated(string treasureId, string type, double lat, double lng)
        {
            TrackEvent("treasure_created",
                ("treasure_id", treasureId),
                ("type", type));
        }

        public void TrackIap(string productId, float price, string currency)
        {
            TrackEvent("iap_purchase",
                ("product_id", productId),
                ("price", price.ToString("F2")),
                ("currency", currency));
        }

        public void TrackP2PConnection(int peerCount)
        {
            TrackEvent("p2p_connected",
                ("peers", peerCount.ToString()));
        }

        public void TrackSessionStart()
        {
            TrackEvent("session_start");
        }

        public void TrackSessionEnd(int durationSeconds)
        {
            TrackEvent("session_end",
                ("duration", durationSeconds.ToString()));
        }
    }
}
