using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.Core
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }

        public bool IsLoggedIn { get; private set; }
        public string Token { get; private set; }

        public event Action OnLogin;
        public event Action OnLogout;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> LoginAnonymously()
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("ehaqui_device_id", deviceId);

            try
            {
                using var request = new WWW($"https://api.ehaqui.com/auth/anonymous?device={deviceId}");
                while (!request.isDone) await Task.Yield();

                if (string.IsNullOrEmpty(request.error))
                {
                    Token = request.text;
                    IsLoggedIn = true;
                    OnLogin?.Invoke();
                    return true;
                }
            }
            catch { }

            IsLoggedIn = true;
            OnLogin?.Invoke();
            return true;
        }

        public void Logout()
        {
            Token = null;
            IsLoggedIn = false;
            OnLogout?.Invoke();
        }
    }
}
