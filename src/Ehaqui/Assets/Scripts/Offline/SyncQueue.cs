using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Ehaqui.Offline
{
    public class SyncQueue : MonoBehaviour
    {
        public static SyncQueue Instance { get; private set; }

        [Serializable]
        public class SyncAction
        {
            public string Id;
            public string ActionType;
            public string PayloadJson;
            public long CreatedAt;
            public int RetryCount;
            public long NextRetryAt;
        }

        private List<SyncAction> _pendingActions = new();
        private const string SYNC_KEY_PREFIX = "sync_action_";
        private const int MAX_RETRIES = 10;
        private const int BASE_DELAY = 30;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            LoadPending();
        }

        public void Enqueue(string actionType, string payloadJson)
        {
            var action = new SyncAction
            {
                Id = Guid.NewGuid().ToString("N"),
                ActionType = actionType,
                PayloadJson = payloadJson,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                RetryCount = 0,
                NextRetryAt = 0
            };
            _pendingActions.Add(action);
            SaveAction(action);

            Core.GameState.Instance.PendingSyncCount = _pendingActions.Count;
        }

        private async void Update()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Core.GameState.Instance.SetOnline(false);
                Core.GameState.Instance.IsOfflineMode = true;
                return;
            }

            Core.GameState.Instance.SetOnline(true);
            Core.GameState.Instance.IsOfflineMode = false;

            if (_pendingActions.Count > 0)
                await ProcessQueue();
        }

        private async Task ProcessQueue()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var toProcess = _pendingActions.FindAll(a => a.NextRetryAt <= now);

            foreach (var action in toProcess)
            {
                var success = await TrySync(action);
                if (success)
                {
                    _pendingActions.Remove(action);
                    RemoveSavedAction(action);
                }
                else
                {
                    action.RetryCount++;
                    var delay = CalculateBackoff(action.RetryCount);
                    action.NextRetryAt = now + delay;
                    SaveAction(action);

                    if (action.RetryCount >= MAX_RETRIES)
                    {
                        _pendingActions.Remove(action);
                        RemoveSavedAction(action);
                    }
                }

                Core.GameState.Instance.PendingSyncCount = _pendingActions.Count;
            }
        }

        private async Task<bool> TrySync(SyncAction action)
        {
            try
            {
                using var request = new UnityWebRequest($"https://api.ehaqui.com/sync/{action.ActionType}", "POST");
                var body = System.Text.Encoding.UTF8.GetBytes(action.PayloadJson);
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var task = request.SendWebRequest();
                while (!task.isDone) await Task.Yield();

                return request.result == UnityWebRequest.Result.Success;
            }
            catch
            {
                return false;
            }
        }

        private int CalculateBackoff(int retryCount)
        {
            return Math.Min(BASE_DELAY * (int)Math.Pow(2, retryCount), 3600);
        }

        private void SaveAction(SyncAction action)
        {
            var json = JsonUtility.ToJson(action);
            var encrypted = Encrypt(json);
            PlayerPrefs.SetString(SYNC_KEY_PREFIX + action.Id, encrypted);
        }

        private void RemoveSavedAction(SyncAction action)
        {
            PlayerPrefs.DeleteKey(SYNC_KEY_PREFIX + action.Id);
        }

        private void LoadPending()
        {
            var keys = PlayerPrefs.GetKeys();
            foreach (var key in keys)
            {
                if (key.StartsWith(SYNC_KEY_PREFIX))
                {
                    var encrypted = PlayerPrefs.GetString(key, "");
                    var json = Decrypt(encrypted);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var action = JsonUtility.FromJson<SyncAction>(json);
                        if (action != null)
                            _pendingActions.Add(action);
                    }
                }
            }
            Core.GameState.Instance.PendingSyncCount = _pendingActions.Count;
        }

        private string Encrypt(string plaintext)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(bytes);
        }

        private string Decrypt(string ciphertext)
        {
            try
            {
                var bytes = Convert.FromBase64String(ciphertext);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch { return ""; }
        }
    }
}
