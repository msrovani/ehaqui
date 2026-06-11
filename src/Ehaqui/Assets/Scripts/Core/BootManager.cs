using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.Core
{
    public class BootManager : MonoBehaviour
    {
        [Header("Boot Sequence")]
        public float MinLoadTime = 2f;

        private void Start()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var startTime = Time.realtimeSinceStartup;

            var tasks = new List<Task>
            {
                InitLocale(),
                InitSession(),
                InitGps(),
                InitP2P(),
                InitOffline()
            };

            await Task.WhenAll(tasks);

            // Garantir tempo mínimo de loading (UX)
            var elapsed = Time.realtimeSinceStartup - startTime;
            if (elapsed < MinLoadTime)
                await Task.Delay((int)((MinLoadTime - elapsed) * 1000));

            // Ativar cena principal
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }

        private async Task InitLocale()
        {
            var locale = FindObjectOfType<LocaleManager>();
            if (locale != null)
                await locale.LoadLocale(PlayerPrefs.GetString("ehaqui_locale", "pt-BR"));
        }

        private async Task InitSession()
        {
            var session = FindObjectOfType<SessionManager>();
            if (session != null)
                await session.LoginAnonymously();
        }

        private async Task InitGps()
        {
            var gps = FindObjectOfType<GPS.GpsService>();
            if (gps != null)
            {
                gps.StartGps();
                await Task.Delay(100);
            }
        }

        private async Task InitP2P()
        {
            var p2p = FindObjectOfType<P2P.P2PManager>();
            if (p2p != null)
            {
                p2p.StartP2P();
                await Task.Delay(100);
            }
        }

        private async Task InitOffline()
        {
            var sync = FindObjectOfType<Offline.SyncQueue>();
            if (sync != null)
                await Task.Delay(50);
        }
    }
}
