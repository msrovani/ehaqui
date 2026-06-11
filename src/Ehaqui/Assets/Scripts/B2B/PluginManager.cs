using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.B2B
{
    [Serializable]
    public class PluginPackage
    {
        public string Id;
        public string Name;
        public string Version;
        public string Studio;
        public PluginType Type;
        public List<string> AssetKeys = new();
        public int PriceCents;
        public float Commission;
        public string ManifestHash;
        public string CreatorSignature;

        public enum PluginType
        {
            ThemePack,
            QuestPack,
            SoundPack
        }
    }

    public class PluginManager : MonoBehaviour
    {
        public static PluginManager Instance { get; private set; }

        private List<PluginPackage> _installedPlugins = new();
        public event Action<PluginPackage> OnPluginInstalled;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public async Task<PluginPackage> LoadPlugin(string pluginId, string manifestJson)
        {
            var manifest = JsonUtility.FromJson<PluginPackage>(manifestJson);

            if (!VerifySignature(manifest))
            {
                Debug.LogError($"Plugin signature invalid: {pluginId}");
                return null;
            }

            var pluginPath = Path.Combine(Application.persistentDataPath, "plugins", pluginId);
            Directory.CreateDirectory(pluginPath);

            var manifestPath = Path.Combine(pluginPath, "manifest.json");
            await File.WriteAllTextAsync(manifestPath, manifestJson);

            _installedPlugins.Add(manifest);
            OnPluginInstalled?.Invoke(manifest);

            return manifest;
        }

        public void ApplyTheme(PluginPackage theme)
        {
            var path = Path.Combine(Application.persistentDataPath, "plugins", theme.Id);
            foreach (var assetKey in theme.AssetKeys)
            {
                var assetPath = Path.Combine(path, assetKey);
                if (File.Exists(assetPath))
                {
                    var bytes = File.ReadAllBytes(assetPath);
                    ApplyAsset(assetKey, bytes);
                }
            }
        }

        private void ApplyAsset(string key, byte[] data)
        {
            Debug.Log($"Applying asset: {key} ({data.Length} bytes)");
        }

        private bool VerifySignature(PluginPackage plugin)
        {
            var data = $"{plugin.Id}|{plugin.Name}|{plugin.Version}|{plugin.Studio}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            var computedHash = System.Convert.ToHexString(hash).ToLower();
            return computedHash == plugin.ManifestHash;
        }

        public List<PluginPackage> GetInstalledThemes()
        {
            return _installedPlugins.FindAll(p => p.Type == PluginPackage.PluginType.ThemePack);
        }

        public List<PluginPackage> GetInstalledQuests()
        {
            return _installedPlugins.FindAll(p => p.Type == PluginPackage.PluginType.QuestPack);
        }

        public void RemovePlugin(string pluginId)
        {
            _installedPlugins.RemoveAll(p => p.Id == pluginId);
            var path = Path.Combine(Application.persistentDataPath, "plugins", pluginId);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
