using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ehaqui.GPS
{
    public class GeofenceManager : MonoBehaviour
    {
        public static GeofenceManager Instance { get; private set; }

        public class GeofenceRegion
        {
            public string Id;
            public double Latitude;
            public double Longitude;
            public float Radius;
            public long ExpiresAt;
            public bool HasEntered;
            public event Action OnEnter;
            public event Action OnExit;
        }

        private List<GeofenceRegion> _regions = new();
        private double _lastCheckLat;
        private double _lastCheckLng;
        private float _checkInterval = 5f;
        private float _timer;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _checkInterval) return;
            _timer = 0;

            if (!GpsService.Instance.IsRunning) return;

            var lat = GpsService.Instance.Latitude;
            var lng = GpsService.Instance.Longitude;

            foreach (var region in _regions)
            {
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > region.ExpiresAt)
                    continue;

                var dist = GpsService.Haversine(lat, lng, region.Latitude, region.Longitude);
                var inside = dist <= region.Radius;

                if (inside && !region.HasEntered)
                {
                    region.HasEntered = true;
                    region.OnEnter?.Invoke();
                }
                else if (!inside && region.HasEntered)
                {
                    region.HasEntered = false;
                    region.OnExit?.Invoke();
                }
            }
        }

        public GeofenceRegion AddRegion(string id, double lat, double lng, float radius, float durationHours = 24)
        {
            var region = new GeofenceRegion
            {
                Id = id,
                Latitude = lat,
                Longitude = lng,
                Radius = radius,
                ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)(durationHours * 3600),
                HasEntered = false
            };
            _regions.Add(region);
            return region;
        }

        public void RemoveRegion(string id)
        {
            _regions.RemoveAll(r => r.Id == id);
        }

        public bool IsInsideAny()
        {
            return _regions.Exists(r => r.HasEntered);
        }
    }
}
