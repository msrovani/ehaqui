using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ehaqui.P2P
{
    public enum MessageType : byte
    {
        Hello = 0,
        HelloAck = 1,
        Ping = 2,
        Pong = 3,
        TreasureCreate = 10,
        TreasureFound = 11,
        TreasureClaim = 12,
        TreasureExpired = 13,
        ChainStep = 20,
        ChainSolve = 21,
        ChainComplete = 22,
        ContractRequest = 30,
        ContractValidate = 31,
        ContractWitness = 32,
        ChatMessage = 40,
        Emote = 41,
        PlayerLocation = 50,
        PlayerStatus = 51,
        PlayerReputation = 52,
    }

    [Serializable]
    public class NetworkMessage
    {
        public MessageType Type;
        public string PayloadJson;
        public long Timestamp;
        public string SenderSignature;
    }

    public class PeerData
    {
        public int Id;
        public string EndPoint;
        public string Nickname;
        public float Reputation = 1f;
        public double Latitude;
        public double Longitude;
        public bool IsConnected;
    }

    public class P2PManager : MonoBehaviour
    {
        public static P2PManager Instance { get; private set; }

        [Header("Config")]
        public int Port = 9050;
        public string ConnectionKey = "EHAQUI_P2P_V1";

        [Header("State")]
        public List<PeerData> ConnectedPeers = new();
        public bool IsSearching;

        public event Action<PeerData> OnPeerConnected;
        public event Action<PeerData> OnPeerDisconnected;
        public event Action<NetworkMessage> OnMessageReceived;

        private UdpClient _udpClient;
        private bool _running;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async void StartP2P()
        {
            try
            {
                _udpClient = new UdpClient(Port);
                _udpClient.EnableBroadcast = true;
                _udpClient.Client.ReceiveTimeout = 1000;
                _running = true;
                IsSearching = true;

                _ = ListenLoop();
                _ = BroadcastPresence();
            }
            catch (Exception e)
            {
                Debug.LogError($"P2P Start failed: {e.Message}");
            }
        }

        private async Task ListenLoop()
        {
            while (_running)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    var json = Encoding.UTF8.GetString(result.Buffer);
                    var msg = JsonUtility.FromJson<NetworkMessage>(json);
                    HandleMessage(msg, result.RemoteEndPoint);
                }
                catch (SocketException) { }
                catch (Exception e)
                {
                    Debug.LogError($"P2P Receive error: {e.Message}");
                }
            }
        }

        private async Task BroadcastPresence()
        {
            while (_running)
            {
                var msg = new NetworkMessage
                {
                    Type = MessageType.Hello,
                    PayloadJson = $"{{\"nick\":\"{GameState.Core.Instance?.Nickname ?? "Hunter"}\"}}",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(msg));
                await _udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, Port));
                await Task.Delay(3000);
            }
        }

        private void HandleMessage(NetworkMessage msg, IPEndPoint sender)
        {
            switch (msg.Type)
            {
                case MessageType.Hello:
                    OnHello(sender);
                    break;
                case MessageType.TreasureCreate:
                case MessageType.TreasureFound:
                    OnMessageReceived?.Invoke(msg);
                    break;
            }
        }

        private void OnHello(IPEndPoint sender)
        {
            if (!ConnectedPeers.Exists(p => p.EndPoint == sender.ToString()))
            {
                var peer = new PeerData
                {
                    Id = ConnectedPeers.Count + 1,
                    EndPoint = sender.ToString(),
                    IsConnected = true
                };
                ConnectedPeers.Add(peer);
                OnPeerConnected?.Invoke(peer);
            }
        }

        public async void SendMessage(NetworkMessage msg)
        {
            var json = JsonUtility.ToJson(msg);
            var bytes = Encoding.UTF8.GetBytes(json);
            foreach (var peer in ConnectedPeers)
            {
                try
                {
                    var parts = peer.EndPoint.Split(':');
                    await _udpClient.SendAsync(bytes, bytes.Length,
                        new IPEndPoint(IPAddress.Parse(parts[0]), Port));
                }
                catch { }
            }
        }

        public void BroadcastTreasure(double lat, double lng, string hintHash, string signature)
        {
            SendMessage(new NetworkMessage
            {
                Type = MessageType.TreasureCreate,
                PayloadJson = $"{{\"lat\":{lat},\"lng\":{lng},\"hash\":\"{hintHash}\",\"sig\":\"{signature}\"}}",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        private void OnDisable() => StopP2P();
        private void OnApplicationQuit() => StopP2P();

        public void StopP2P()
        {
            _running = false;
            _udpClient?.Close();
            IsSearching = false;
            ConnectedPeers.Clear();
        }
    }
}
