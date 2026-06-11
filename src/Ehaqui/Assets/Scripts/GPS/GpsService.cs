using System;
using System.Collections;
using UnityEngine;

namespace Ehaqui.GPS
{
    public class GpsService : MonoBehaviour
    {
        public static GpsService Instance { get; private set; }

        [Header("Config")]
        public float ForegroundInterval = 1f;
        public float BackgroundInterval = 60f;

        [Header("State")]
        public double Latitude;
        public double Longitude;
        public double Altitude;
        public float Accuracy;
        public bool IsRunning;

        public event Action<double, double, float> OnLocationUpdated;

        private bool _isBackground;
        private Coroutine _gpsRoutine;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartGps()
        {
            if (_gpsRoutine != null) StopCoroutine(_gpsRoutine);
            _gpsRoutine = StartCoroutine(GpsRoutine());
        }

        private IEnumerator GpsRoutine()
        {
#if !UNITY_EDITOR
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogWarning("GPS disabled by user");
                yield break;
            }
#endif
            Input.location.Start(1f, 1f);

            var maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogError("GPS failed to start");
                yield break;
            }

            IsRunning = true;

            while (IsRunning)
            {
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    var data = Input.location.lastData;
                    Latitude = data.latitude;
                    Longitude = data.longitude;
                    Altitude = data.altitude;
                    Accuracy = data.horizontalAccuracy;

                    OnLocationUpdated?.Invoke(Latitude, Longitude, Accuracy);
                }

                yield return new WaitForSeconds(_isBackground ? BackgroundInterval : ForegroundInterval);
            }

            Input.location.Stop();
        }

        public double DistanceTo(double lat, double lng)
        {
            return Haversine(Latitude, Longitude, lat, lng);
        }

        public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371000;
            var dLat = (lat2 - lat1) * Mathf.Deg2Rad;
            var dLon = (lon2 - lon1) * Mathf.Deg2Rad;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Mathf.Deg2Rad) * Math.Cos(lat2 * Mathf.Deg2Rad) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        public void SetBackground(bool bg)
        {
            _isBackground = bg;
        }

        private void OnDisable()
        {
            IsRunning = false;
            if (_gpsRoutine != null) StopCoroutine(_gpsRoutine);
        }
    }
}
