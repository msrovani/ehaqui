using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ehaqui.Offline
{
    public class OfflineCache : MonoBehaviour
    {
        public static OfflineCache Instance { get; private set; }

        private string _dbPath;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _dbPath = Path.Combine(Application.persistentDataPath, "ehaqui_cache.db");
        }

        public void CacheTreasure(string id, string json)
        {
            PlayerPrefs.SetString($"treasure_{id}", json);
        }

        public string GetCachedTreasure(string id)
        {
            return PlayerPrefs.GetString($"treasure_{id}", "");
        }

        public void CacheMapTile(int x, int y, int zoom, byte[] data)
        {
            var key = $"tile_{zoom}_{x}_{y}";
            PlayerPrefs.SetString(key, Convert.ToBase64String(data));
        }

        public byte[] GetCachedTile(int x, int y, int zoom)
        {
            var key = $"tile_{zoom}_{x}_{y}";
            var data = PlayerPrefs.GetString(key, "");
            return string.IsNullOrEmpty(data) ? null : Convert.FromBase64String(data);
        }

        public List<string> GetAllCachedTreasureIds()
        {
            var ids = new List<string>();
            foreach (var key in GetAllKeys())
            {
                if (key.StartsWith("treasure_"))
                    ids.Add(key.Replace("treasure_", ""));
            }
            return ids;
        }

        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public long GetCacheSize()
        {
            long size = 0;
            foreach (var key in GetAllKeys())
            {
                var val = PlayerPrefs.GetString(key, "");
                size += System.Text.Encoding.UTF8.GetByteCount(val);
            }
            return size;
        }

        private List<string> GetAllKeys()
        {
            var keys = new List<string>();
            foreach (var kvp in PlayerPrefs.GetKeys())
                keys.Add(kvp);
            return keys;
        }

        public int GetCachedTreasureCount()
        {
            return GetAllCachedTreasureIds().Count;
        }
    }
}
