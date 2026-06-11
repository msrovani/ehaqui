using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.Core
{
    public class LocaleManager : MonoBehaviour
    {
        public static LocaleManager Instance { get; private set; }

        public string CurrentLocale { get; private set; } = "pt-BR";
        public string AppName => CurrentLocale == "pt-BR" ? "EHAQUI" : "ISHERE";

        private Dictionary<string, Dictionary<string, string>> _tables;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            var saved = PlayerPrefs.GetString("ehaqui_locale", "");
            if (!string.IsNullOrEmpty(saved))
                CurrentLocale = saved;
            await LoadLocale(CurrentLocale);
        }

        public async Task LoadLocale(string localeCode)
        {
            CurrentLocale = localeCode;
            PlayerPrefs.SetString("ehaqui_locale", localeCode);

            var path = Path.Combine(Application.streamingAssetsPath, $"i18n/{localeCode}.json");

#if UNITY_ANDROID && !UNITY_EDITOR
            using var www = new WWW(path);
            while (!www.isDone) await Task.Yield();
            var json = www.text;
#else
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Locale not found: {path}. Loading fallback.");
                return;
            }
            var json = await File.ReadAllTextAsync(path);
#endif

            var raw = JsonUtility.FromJson<LocaleData>(json);
            _tables = Flatten(raw, "");
        }

        public string Get(string key)
        {
            if (_tables == null) return key;
            if (_tables.TryGetValue(key, out var value))
                return value;
            return key;
        }

        public string Format(string key, params object[] args)
        {
            var template = Get(key);
            return string.Format(template, args);
        }

        private Dictionary<string, string> Flatten(LocaleData data, string prefix)
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in data.ToDictionary())
            {
                result[prefix + kvp.Key] = kvp.Value;
            }
            return result;
        }

        private class LocaleData
        {
            public List<LocaleEntry> entries;

            public Dictionary<string, string> ToDictionary()
            {
                var dict = new Dictionary<string, string>();
                if (entries == null) return dict;
                foreach (var e in entries)
                    dict[e.key] = e.value;
                return dict;
            }
        }

        [System.Serializable]
        private class LocaleEntry
        {
            public string key;
            public string value;
        }
    }
}
