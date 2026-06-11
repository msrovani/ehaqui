using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ehaqui.Core
{
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        [Header("Player")]
        public string PlayerId;
        public string Nickname;
        public int Level = 1;
        public int XP;
        public int Coins;
        public bool NoAds;
        public bool HasHunterPass;

        [Header("State")]
        public bool IsOnline;
        public bool IsOfflineMode;
        public int PendingSyncCount;

        public event Action<int> OnCoinsChanged;
        public event Action<int> OnXpChanged;
        public event Action<int> OnLevelUp;
        public event Action<bool> OnOnlineStatusChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            LoadFromDisk();
        }

        public void AddCoins(int amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }

        public bool SpendCoins(int amount)
        {
            if (Coins < amount) return false;
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public void AddXp(int amount)
        {
            XP += amount;
            OnXpChanged?.Invoke(XP);
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            int required = Level * 100;
            while (XP >= required)
            {
                XP -= required;
                Level++;
                required = Level * 100;
                OnLevelUp?.Invoke(Level);
            }
        }

        public void SetOnline(bool online)
        {
            IsOnline = online;
            OnOnlineStatusChanged?.Invoke(online);
        }

        public void SaveToDisk()
        {
            var data = new SaveData
            {
                PlayerId = PlayerId,
                Nickname = Nickname,
                Level = Level,
                XP = XP,
                Coins = Coins,
                NoAds = NoAds,
                HasHunterPass = HasHunterPass
            };
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("ehaqui_save", json);
            PlayerPrefs.Save();
        }

        public void LoadFromDisk()
        {
            var json = PlayerPrefs.GetString("ehaqui_save", "");
            if (string.IsNullOrEmpty(json)) return;
            var data = JsonUtility.FromJson<SaveData>(json);
            PlayerId = data.PlayerId;
            Nickname = data.Nickname;
            Level = data.Level;
            XP = data.XP;
            Coins = data.Coins;
            NoAds = data.NoAds;
            HasHunterPass = data.HasHunterPass;
        }

        [Serializable]
        private class SaveData
        {
            public string PlayerId;
            public string Nickname;
            public int Level;
            public int XP;
            public int Coins;
            public bool NoAds;
            public bool HasHunterPass;
        }
    }
}
