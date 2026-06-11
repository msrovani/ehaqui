# Unity URP — Estudo Técnico para EHAQUI / ISHERE

> Documento de estudo aprofundado da Unity URP adaptado ao escopo do jogo.
> Plataformas: Android + iOS
> Unity 6.4 LTS | URP 17.x

---

## 1. RENDER PIPELINE — URP PARA MOBILE

### 1.1 Por que URP para este projeto

URP é a pipeline mais adequada para jogos mobile cross-platform (Android + iOS):
- Construída para ARM GPUs — mesma arquitetura de 90% dos dispositivos mobile
- SRP Batcher nativo — redução drástica de draw calls em cenas com muitos materiais compartilhados
- Suporte a Vulkan (Android) e Metal (iOS) — baixo overhead de driver
- Tamanho de build ~30-50 MB (vs 150-300 MB do HDRP/Unreal)

### 1.2 Configuração URP Asset para Mobile

```xml
URP Asset → Mobile Renderer:
  - Render Path: Forward (único compatível com SRP Batcher em mobile)
  - Main Light: Cast Shadows = OFF (usar lightmaps ou baked lighting)
  - Additional Lights: Per Object Limit = 1 (máx 2 para mobile)
  - Additional Lights: Cast Shadows = OFF
  - Shadows: Max Distance = 15m (reduzir para 10m em devices fracos)
  - Shadows: Cascade Count = 1 (2 no máx)
  - Shadows: Resolution = 512 (qualidade aceitável para mobile)
  - Post Processing: OFF no gameplay (ON apenas em menus/telas estáticas)
  - LUT Size: 16 (menor = mais rápido)
  - HDR: OFF (desnecessário para visual low-poly/stylized)
  - MSAA: 2x (4x apenas em flagships)
  - Render Scale: 1.0 (0.8 em devices fracos)
```

### 1.3 SRP Batcher — Obrigatório

O SRP (Scriptable Render Pipeline) Batcher reduz draw calls em ~90% para materiais que compartilham o mesmo shader.

**Regras para ativar o SRP Batcher:**
- Usar sempre o mesmo shader (URP/Lit ou URP/Unlit) para todos os materiais
- Evitar materiais com propriedades diferentes por instância
- Preferir `MaterialPropertyBlocks` para variações por objeto

**No EHAQUI:** mapa, baús, detector, personagens — todos com URP/Lit e `MaterialPropertyBlock` para variações de cor.

### 1.4 LOD (Level of Detail)

Sistema de LOD obrigatório para manter 30 FPS em devices médios.

| LOD | Distância | Polígonos | Detalhes |
|-----|-----------|-----------|----------|
| 0 | 0-10m | 100% | Mesh completo, textura 512px |
| 1 | 10-30m | 50% | Mesh simplificado, textura 256px |
| 2 | 30-50m | 25% | Mesh básico, textura 128px |
| Culled | >50m | 0 | Não renderiza |

### 1.5 Shaders para EHAQUI

| Elemento | Shader | Notas |
|----------|--------|-------|
| Terreno/Mapa | URP/Lit (baked) | Lightmap + textura única |
| Baús | URP/Lit | Com `_EMISSION` para brilho |
| Detector/UI | URP/Unlit | Sem luz, performance máxima |
| Personagens | URP/Lit (2 bones) | Skinned Mesh simplificado |
| Partículas | URP/Unlit Particle | Sem lighting |
| Água/Efeitos | URP/Unlit com alpha | Shader Graph custom |

### 1.6 GPU Instancing

Para objetos repetidos no mapa (árvores, pedras, postes):
```csharp
// GPU Instancing via Graphics.DrawMeshInstanced
MaterialPropertyBlock props = new MaterialPropertyBlock();
for (int i = 0; i < instances.Length; i++) {
    props.SetColor("_BaseColor", instances[i].color);
    props.SetMatrix("unity_ObjectToWorld", instances[i].matrix);
    // DrawMeshInstanced enviará tudo em 1 draw call
}
```

---

## 2. PERMISSÕES ANDROID E iOS

### 2.1 Permissões Android

```xml
<!-- AndroidManifest.xml -->
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />

<!-- GPS -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />

<!-- Geofencing (Android 12+) -->
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />

<!-- Vibração -->
<uses-permission android:name="android.permission.VIBRATE" />

<!-- Notificação -->
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_LOCATION" />

<!-- WiFi Direct / P2P -->
<uses-permission android:name="android.permission.NEARBY_WIFI_DEVICES" />
```

### 2.2 Permissões iOS (Info.plist)

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>EHAQUI precisa da sua localização para encontrar tesouros próximos.</string>
<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>EHAQUI usa localização em background para notificar sobre tesouros próximos.</string>
<key>UIBackgroundModes</key>
<array>
    <string>location</string>
    <string>fetch</string>
    <string>remote-notification</string>
</array>
<key>NSLocalNetworkUsageDescription</key>
<string>EHAQUI usa WiFi Direct para conectar com jogadores próximos.</string>
<key>NSBonjourServices</key>
<array>
    <string>_ehaqui._tcp</string>
</array>
```

### 2.3 Runtime Permission Request (Unity)

```csharp
using UnityEngine.Android;

public class PermissionManager {
    private const string LOCATION = "android.permission.ACCESS_FINE_LOCATION";
    private const string BACKGROUND_LOCATION = "android.permission.ACCESS_BACKGROUND_LOCATION";
    private const string NOTIFICATIONS = "android.permission.POST_NOTIFICATIONS";
    private const string NEARBY_WIFI = "android.permission.NEARBY_WIFI_DEVICES";

    public bool HasLocationPermission() =>
        Permission.HasUserAuthorizedPermission(LOCATION);

    public void RequestAllPermissions() {
        // Android 13+ (API 33) — notificações separadas
        if (Application.platform == RuntimePlatform.Android) {
            var androidVersion = GetAndroidApiLevel();
            if (androidVersion >= 33)
                Permission.RequestUserPermission(NOTIFICATIONS);
            // Android 12+ (API 31) — background location separada
            if (androidVersion >= 31)
                Permission.RequestUserPermission(BACKGROUND_LOCATION);
            // Android 13+ — nearby devices
            if (androidVersion >= 33)
                Permission.RequestUserPermission(NEARBY_WIFI);
        }
        Permission.RequestUserPermission(LOCATION);
    }

    private int GetAndroidApiLevel() {
        // Usa AndroidJavaClass para obter Build.VERSION.SdkInt
        using var version = new AndroidJavaClass("android.os.Build$VERSION");
        return version.GetStatic<int>("SDK_INT");
    }
}
```

**iOS:** No iOS, as permissões são solicitadas automaticamente ao acessar o serviço pela primeira vez. Usar `Input.location` no Unity.

---

## 3. GPS E GEOFENCING (Android + iOS)

### 3.1 Unity Location Service (nativo, ambos OS)

```csharp
public class GpsService : MonoBehaviour {
    private float _updateInterval = 1f;   // 1 segundo em foreground
    private float _bgInterval = 60f;      // 60 segundos em background
    private bool _isBackground;

    public IEnumerator StartGps() {
        // Verificar permissões primeiro
        if (!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION")) {
            Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");
            yield return new WaitForSeconds(0.5f);
        }

        Input.location.Start(_updateInterval, 0.1f); // 0.1m de distância mínima

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.LogError("GPS falhou — verificar permissões");
            yield break;
        }

        // Iniciar foreground service (Android)
        if (Application.platform == RuntimePlatform.Android)
            StartForegroundService();

        // Loop de updates
        while (enabled) {
            if (Input.location.status == LocationServiceStatus.Running) {
                var data = Input.location.lastData;
                OnLocationUpdated(data.latitude, data.longitude, data.altitude,
                                  data.horizontalAccuracy, data.timestamp);
            }
            yield return new WaitForSeconds(_isBackground ? _bgInterval : _updateInterval);
        }
    }

    private void StartForegroundService() {
        // Android 8+ precisa de foreground service para localização em background
        using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        using var intent = new AndroidJavaObject("android.content.Intent", activity,
            new AndroidJavaClass("com.ehaqui.GpsForegroundService"));
        activity.Call("startForegroundService", intent);
    }
}
```

### 3.2 Android Native GPS Plugin (alternativa superior)

Para maior precisão e controle, usar um Android Native Plugin com FusedLocationProviderClient:

```kotlin
// EhaquiGpsPlugin.kt (Android Native)
class EhaquiGpsPlugin {
    private var fusedLocationClient: FusedLocationProviderClient? = null
    private var locationCallback: LocationCallback? = null

    fun startLocationUpdates(context: Context, intervalMs: Long) {
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(context)
        val request = LocationRequest.Builder(intervalMs)
            .setPriority(Priority.PRIORITY_HIGH_ACCURACY)
            .setMinUpdateDistanceMeters(5f)  // 5m mínimo
            .build()

        locationCallback = object : LocationCallback() {
            override fun onLocationResult(result: LocationResult) {
                result.lastLocation?.let { loc ->
                    UnityPlayer.UnitySendMessage("GpsReceiver", "OnLocation",
                        "${loc.latitude},${loc.longitude},${loc.accuracy},${loc.time}")
                }
            }
        }

        fusedLocationClient?.requestLocationUpdates(request, locationCallback!!, null)
    }

    fun stopLocationUpdates() {
        locationCallback?.let { fusedLocationClient?.removeLocationUpdates(it) }
    }
}
```

### 3.3 Geofencing (Android)

Geofencing nativo do Android para notificação por passagem:

```kotlin
// GeofencePlugin.kt
class GeofencePlugin {
    private var geofencingClient: GeofencingClient? = null

    fun addTreasureGeofence(context: Context, id: String, lat: Double, lng: Double,
                            radius: Float, expirationMs: Long) {
        geofencingClient = LocationServices.getGeofencingClient(context)

        val geofence = Geofence.Builder()
            .setRequestId(id)
            .setCircularRegion(lat, lng, radius)
            .setExpirationDuration(expirationMs)
            .setTransitionTypes(Geofence.GEOFENCE_TRANSITION_ENTER)
            .build()

        val request = GeofencingRequest.Builder()
            .addGeofence(geofence)
            .setInitialTrigger(GeofencingRequest.INITIAL_TRIGGER_ENTER)
            .build()

        val intent = Intent(context, GeofenceBroadcastReceiver::class.java)
        val pendingIntent = PendingIntent.getBroadcast(context, 0, intent,
            PendingIntent.FLAG_UPDATE_CURRENT or PendingIntent.FLAG_IMMUTABLE)

        geofencingClient?.addGeofences(request, pendingIntent)
    }
}
```

### 3.4 iOS Core Location

```swift
// EhaquiLocationManager.swift
import CoreLocation

class EhaquiLocationManager: NSObject, CLLocationManagerDelegate {
    let manager = CLLocationManager()

    func start() {
        manager.delegate = self
        manager.desiredAccuracy = kCLLocationAccuracyBest
        manager.distanceFilter = 5 // 5m
        manager.allowsBackgroundLocationUpdates = true
        manager.pausesLocationUpdatesAutomatically = false
        manager.startUpdatingLocation()

        // Region monitoring para geofencing
        let region = CLCircularRegion(center: CLLocationCoordinate2D(...),
            radius: 50, identifier: "treasure_1")
        region.notifyOnEntry = true
        manager.startMonitoring(for: region)
    }
}
```

---

## 4. P2P NETWORKING — LiteNetLib

### 4.1 Arquitetura P2P para EHAQUI

O jogo usa uma arquitetura **P2P híbrida** com fallback:

1. **WiFi Direct** (Android) — descoberta local + socket UDP direto
2. **Bluetooth** (Android/iOS) — fallback quando WiFi Direct não disponível
3. **LAN** (mesma rede WiFi) — via broadcast UDP
4. **Servidor relay** (fallback) — via Cloud Run apenas quando P2P falha

### 4.2 LiteNetLib — Setup Unity

```csharp
using LiteNetLib;
using LiteNetLib.Utils;

public class P2PManager : MonoBehaviour, INetEventListener {
    private NetManager _netManager;
    private NetDataWriter _writer;
    public event System.Action<PeerData> OnPeerConnected;
    public event System.Action<string> OnTreasureFound;

    private void Awake() {
        _netManager = new NetManager(this) {
            UnconnectedMessagesEnabled = true,  // Necessário para broadcast
            BroadcastReceiveEnabled = true,     // Descoberta local
            UpdateTime = 15,                     // ~60 updates/s
            DisconnectTimeout = 30000,          // 30s timeout
            PingInterval = 1000,                // Ping a cada 1s
        };
        _writer = new NetDataWriter();
    }

    public void StartP2P(int port = 9050) {
        _netManager.Start(port);

        // WiFi Direct discovery (Android nativo via plugin)
        if (Application.platform == RuntimePlatform.Android)
            StartWiFiDirectDiscovery();

        // LAN discovery
        _netManager.SendBroadcast(_writer, port);
    }

    // Chamado pelo Android Plugin quando WiFi Direct peer encontrado
    public void OnWiFiDirectPeerFound(string ipAddress) {
        _netManager.Connect(ipAddress, 9050, "EHAQUI_P2P_KEY");
    }

    public void SendTreasure(TreasureData treasure) {
        _writer.Reset();
        _writer.Put((byte)MessageType.TreasureCreate);
        _writer.Put(treasure.Id);
        _writer.Put(treasure.Latitude);
        _writer.Put(treasure.Longitude);
        _writer.Put(treasure.HintHash);
        _writer.Put(treasure.Signature);  // P2P Contract
        _netManager.SendToAll(_writer, DeliveryMethod.ReliableOrdered);
    }

    // INetEventListener
    public void OnPeerConnected(NetPeer peer) {
        OnPeerConnected?.Invoke(new PeerData {
            Id = peer.Id,
            EndPoint = peer.EndPoint
        });
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader,
                                  byte channel, DeliveryMethod method) {
        var type = (MessageType)reader.GetByte();
        switch (type) {
            case MessageType.TreasureCreate:
                HandleTreasureCreated(reader);
                break;
            case MessageType.TreasureFound:
                OnTreasureFound?.Invoke(reader.GetString());
                break;
            case MessageType.ContractValidate:
                HandleContractValidation(reader);
                break;
        }
        reader.Recycle();
    }
}
```

### 4.3 WiFi Direct Android Plugin

```kotlin
// WiFiDirectPlugin.kt
class WiFiDirectPlugin {
    private var manager: WifiP2pManager? = null
    private var channel: WifiP2pManager.Channel? = null
    private var receiver: BroadcastReceiver? = null

    fun initialize(context: Context) {
        manager = context.getSystemService(Context.WIFI_P2P_SERVICE) as WifiP2pManager
        channel = manager?.initialize(context, context.mainLooper, null)
        registerReceiver(context)
    }

    fun startDiscovery() {
        manager?.discoverPeers(channel, object : WifiP2pManager.ActionListener {
            override fun onSuccess() { /* discovery started */ }
            override fun onFailure(reason: Int) { /* fallback para Bluetooth */ }
        })
    }

    fun connectToPeer(device: WifiP2pDevice) {
        val config = WifiP2pConfig().apply {
            deviceAddress = device.deviceAddress
            groupOwnerIntent = 15  // Preferir ser Group Owner
        }
        manager?.connect(channel, config, object : WifiP2pManager.ActionListener {
            override fun onSuccess() { /* connected — socket info via requestGroupInfo */ }
            override fun onFailure(reason: Int) { /* fallback */ }
        })
    }

    private fun registerReceiver(context: Context) {
        val intentFilter = IntentFilter().apply {
            addAction(WifiP2pManager.WIFI_P2P_STATE_CHANGED_ACTION)
            addAction(WifiP2pManager.WIFI_P2P_PEERS_CHANGED_ACTION)
            addAction(WifiP2pManager.WIFI_P2P_CONNECTION_CHANGED_ACTION)
            addAction(WifiP2pManager.WIFI_P2P_THIS_DEVICE_CHANGED_ACTION)
        }
        receiver = object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                when (intent.action) {
                    WifiP2pManager.WIFI_P2P_PEERS_CHANGED_ACTION -> {
                        manager?.requestPeers(channel) { peers ->
                            val peerList = (0 until peers.count).map { peers[it] }
                            // Enviar para Unity
                            UnityPlayer.UnitySendMessage("P2PManager", "OnPeersFound",
                                peerList.joinToString(";") { it.deviceAddress })
                        }
                    }
                    WifiP2pManager.WIFI_P2P_CONNECTION_CHANGED_ACTION -> {
                        // Obter IP do Group Owner
                        manager?.requestGroupInfo(channel) { group ->
                            val ownerIp = group.ownerIp  // Socket address
                            UnityPlayer.UnitySendMessage("P2PManager",
                                "OnWiFiDirectPeerFound", ownerIp ?: "")
                        }
                    }
                }
            }
        }
        context.registerReceiver(receiver, intentFilter,
            Context.RECEIVER_EXPORTED or Context.RECEIVER_NOT_EXPORTED)
    }
}
```

### 4.4 Protocolo de Mensagens P2P

```csharp
public enum MessageType : byte {
    // Handshake
    Hello = 0,
    HelloAck = 1,
    Ping = 2,
    Pong = 3,

    // Tesouros
    TreasureCreate = 10,
    TreasureFound = 11,
    TreasureClaim = 12,
    TreasureExpired = 13,

    // Correntes
    ChainStep = 20,
    ChainSolve = 21,
    ChainComplete = 22,

    // P2P Contract
    ContractRequest = 30,
    ContractValidate = 31,
    ContractWitness = 32,

    // Chat
    ChatMessage = 40,
    Emote = 41,

    // Jogador
    PlayerLocation = 50,
    PlayerStatus = 51,
    PlayerReputation = 52,
}

public struct NetworkMessage {
    public MessageType Type;
    public string PayloadJson;  // JSON serializado
    public long Timestamp;
    public string SenderSignature;  // ECDSA
}
```

### 4.5 Bluetooth Fallback (Android + iOS)

Quando WiFi Direct não está disponível:
- **Android:** BluetoothSocket via RFCOMM
- **iOS:** Multipeer Connectivity Framework (MCBrowserViewController)

```csharp
public class BluetoothP2P : MonoBehaviour {
    // Android — BluetoothSocket RFCOMM
    // iOS — MultipeerConnectivity (via Native Plugin)
    public void FallbackToBluetooth() {
        if (Application.platform == RuntimePlatform.Android)
            StartBluetoothAndroid();
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            StartBluetoothIOS();
    }
}
```

---

## 5. VIBRAÇÃO — DETECTOR PROGRESSIVO

### 5.1 Vibrator API (Android + iOS)

```csharp
public class VibrationController : MonoBehaviour {
    private const long SHORT = 50;     // ms
    private const long MEDIUM = 150;
    private const long LONG = 300;

    // Android: VibrationEffect (API 26+) ou Vibrator legacy (API < 26)
    // iOS: Native plugin (UIKit não expõe vibração diretamente no Unity)

    public void VibrateProgressive(float distance, float maxDistance) {
        float intensity = 1f - (distance / maxDistance);
        intensity = Mathf.Clamp01(intensity);

        int level = intensity switch {
            < 0.25f => 1,  // Frio
            < 0.50f => 2,  // Morno
            < 0.75f => 3,  // Quente
            < 0.90f => 4,  // Muito quente
            _ => 5          // Pegando fogo!
        };

        if (Application.platform == RuntimePlatform.Android)
            VibrateAndroid(level);
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            VibrateIOS(level);
    }

    private void VibrateAndroid(int level) {
        using var vibrator = new AndroidJavaClass("android.os.Vibrator");
        using var activity = GetUnityActivity();
        using var systemService = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
        using var vibratorManagerClass = new AndroidJavaClass("android.os.VibratorManager");
        using var vibratorManager = context.Call<AndroidJavaObject>("getSystemService", "vibrator_manager");
        using var vibrator2 = vibratorManager.Call<AndroidJavaObject>("getDefaultVibrator");

        long[] pattern = level switch {
            1 => new long[] { 0, 100, 50, 100 },
            2 => new long[] { 0, 200, 50, 200 },
            3 => new long[] { 0, 300, 50, 300, 50, 300 },
            4 => new long[] { 0, 100, 30, 100, 30, 100, 30, 100 },
            5 => new long[] { 0, 50, 20, 50, 20, 50, 20, 50, 20, 50, 20, 50 },
        };

        using var effectClass = new AndroidJavaClass("android.os.VibrationEffect");
        using var effect = effectClass.CallStatic<AndroidJavaObject>(
            "createWaveform", pattern, 0);  // 0 = não repetir
        vibrator2.Call("vibrate", effect);
    }

    private void VibrateIOS(int level) {
        // iOS: usará Native Plugin com AudioToolbox.AudioServicesPlaySystemSound
        // para vibração tátil (kSystemSoundID_Vibrate)
        // iPhone 7+ tem haptic feedback via CoreHaptics:
        //   level 1-2: UIImpactFeedbackGenerator(style: .light)
        //   level 3-4: UIImpactFeedbackGenerator(style: .medium)
        //   level 5:   UIImpactFeedbackGenerator(style: .heavy)
    }
}
```

### 5.2 iOS Native Plugin para Haptics

```csharp
// iOSNativeHaptics.mm
#import <UIKit/UIKit.h>

extern "C" void _PlayHaptic(int level) {
    if (@available(iOS 13.0, *)) {
        UIImpactFeedbackGenerator *gen;
        switch (level) {
            case 1: gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight]; break;
            case 2: gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium]; break;
            case 3: gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy]; break;
            case 4: gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleRigid]; break;
            case 5: gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleSoft]; break;
        }
        [gen prepare];
        [gen impactOccurred];
    } else {
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
    }
}
```

---

## 6. MONETIZAÇÃO — GOOGLE PLAY BILLING + APP STORE IAP

### 6.1 Unity IAP 5.0+ (Google Play Billing Library v8)

```csharp
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : IDetailedStoreListener {
    private static IAPManager _instance;
    private IStoreController _controller;
    private IExtensionProvider _extensions;

    // Product IDs (devem corresponder ao Google Play Console e App Store Connect)
    public const string NO_ADS = "com.ehaqui.noads";
    public const string HUNTER_PASS = "com.ehaqui.hunterpass";
    public const string COINS_100 = "com.ehaqui.coins.100";
    public const string COINS_500 = "com.ehaqui.coins.500";
    public const string COINS_1200 = "com.ehaqui.coins.1200";

    public void Initialize() {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(NO_ADS, ProductType.NonConsumable);
        builder.AddProduct(HUNTER_PASS, ProductType.Subscription,
            new IDs { { HUNTER_PASS, GooglePlay.Name }, { HUNTER_PASS, AppleAppStore.Name } });
        builder.AddProduct(COINS_100, ProductType.Consumable);
        builder.AddProduct(COINS_500, ProductType.Consumable);
        builder.AddProduct(COINS_1200, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        _controller = controller;
        _extensions = extensions;
        Debug.Log("IAP initialized");

        // Verificar se assinatura está ativa
        var sub = controller.products.WithID(HUNTER_PASS);
        if (sub.hasReceipt)
            GameState.Instance.HasHunterPass = sub.isPurchased;
    }

    public void BuyProduct(string productId) {
        _controller.InitiatePurchase(productId);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
        var product = args.purchasedProduct;

        // Validação local do receipt
        if (!ValidateReceipt(product)) {
            // Reembolso forçado
            _extensions.GetExtension<IGooglePlayExtensions>()
                .FinishAdditionalTransaction(product, "confirmPriceChange");
            return PurchaseProcessingResult.Complete;
        }

        switch (product.definition.id) {
            case COINS_100:
                GameState.Instance.AddCoins(100);
                break;
            case HUNTER_PASS:
                GameState.Instance.HasHunterPass = true;
                break;
            case NO_ADS:
                GameState.Instance.NoAds = true;
                break;
        }

        // Enviar receipt para servidor para validação server-side
        StartCoroutine(ValidateServerSide(product.receipt, product.definition.id));

        return PurchaseProcessingResult.Complete;
    }

    private bool ValidateReceipt(Product product) {
        // Validação cross-platform do receipt
        var validator = new Unity.Purchasing.Security.CrossPlatformValidator(
            GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        try {
            var result = validator.Validate(product.receipt);
            return result.Any();  // Receipt válido
        } catch {
            return false;  // Receipt inválido
        }
    }
}
```

### 6.2 Produtos do EHAQUI

| Produto | Tipo | Preço BR | Preço US | Descrição |
|---------|------|----------|----------|-----------|
| `coins.100` | Consumable | R$ 4,90 | $0.99 | 100 moedas |
| `coins.500` | Consumable | R$ 19,90 | $3.99 | 500 moedas (+bônus) |
| `coins.1200` | Consumable | R$ 39,90 | $7.99 | 1200 moedas (+super) |
| `noads` | NonConsumable | R$ 9,90 | $2.99 | Remove anúncios |
| `hunterpass` | Subscription | R$ 14,90/mês | $4.99/mês | Passe do Caçador |
| `seasonpass` | NonConsumable | R$ 29,90 | $9.99 | Passe de temporada |

### 6.3 Anúncio Recompensado (AdMob + AdUnit)

```csharp
using GoogleMobileAds.Api;

public class RewardedAdManager : MonoBehaviour {
    private RewardedAd _rewardedAd;
    private string _adUnitId;

    // Android e iOS usam AdUnit IDs diferentes
    private void Awake() {
        #if UNITY_ANDROID
            _adUnitId = "ca-app-pub-xxx/yyy";
        #elif UNITY_IOS
            _adUnitId = "ca-app-pub-xxx/zzz";
        #endif
    }

    public void LoadAndShowAd(System.Action onRewarded) {
        var request = new AdRequest();
        RewardedAd.Load(_adUnitId, request, (ad, error) => {
            if (error != null) return;
            _rewardedAd = ad;
            _rewardedAd.Show((reward) => {
                onRewarded?.Invoke();
                // Turbo detector 5 min ou +1 pá
            });
        });
    }
}
```

---

## 7. ADDRESSABLE ASSETS — SISTEMA DE PLUGINS

### 7.1 Arquitetura de Plugins (.themepack / .questpack / .soundpack)

O sistema de plugins do EHAQUI usa Addressables + Cloud Content Delivery (CCD):

```csharp
// PluginManager.cs
public class PluginManager : MonoBehaviour {
    // Cada pacote é um Addressable Group remoto
    // - themepack_001: skins de baú, detector, mapa
    // - questpack_001: quests oficiais com NPCs
    // - soundpack_001: trilha sonora temática

    public async Task<PluginPackage> LoadPlugin(string pluginId) {
        var handle = Addressables.LoadAssetAsync<PluginPackage>(
            $"plugins/{pluginId}/manifest.json");
        await handle.Task;

        var manifest = handle.Result;
        // Baixar assets do pacote
        foreach (var assetKey in manifest.AssetKeys) {
            var assetHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetKey);
            await assetHandle.Task;
            ApplyAsset(assetKey, assetHandle.Result);
        }
        return manifest;
    }

    // Aplicar tema (.themepack)
    private void ApplyTheme(ThemePack theme) {
        // TROCAR:
        // - Material do baú → theme.ChestMaterial
        // - Cor do detector → theme.DetectorColor
        // - Textura do mapa → theme.MapTexture
        // - Música de fundo → theme.BackgroundMusic
        // Tudo via ResourceLoader assíncrono
    }
}
```

### 7.2 Estrutura de um .themepack

```json
{
    "id": "themepack_disney_princess",
    "name": "Disney Princess",
    "version": "1.0.0",
    "studio": "Disney",
    "bundle": "themepack_disney_princess",
    "assets": [
        "themepack_disney/chest_material.asset",
        "themepack_disney/detector_skin.asset",
        "themepack_disney/map_border.asset",
        "themepack_disney/background_music.ogg",
        "themepack_disney/particle_fx.asset"
    ],
    "price": 4990,  // R$ 49,90
    "commission": 0.30  // 70% criador, 30% plataforma
}
```

### 7.3 Validação de Integridade

```csharp
public bool ValidatePlugin(PluginPackage plugin) {
    // 1. Verificar hash SHA256 de cada asset
    foreach (var asset in plugin.Assets) {
        if (!VerifyHash(asset.Path, asset.ExpectedHash))
            return false;
    }
    // 2. Verificar assinatura do criador (ECDSA)
    if (!VerifySignature(plugin.ManifestJson, plugin.CreatorSignature))
        return false;
    // 3. Verificar tamanho máximo (50MB por pacote)
    if (plugin.TotalSize > 50 * 1024 * 1024)
        return false;
    return true;
}
```

---

## 8. LOCALIZAÇÃO (i18n) — Unity Localization Package

### 8.1 Unity Localization Package Setup

```csharp
// Instalar: Package Manager → Localization (Unity Official)
// Versão: 1.5+ (Unity 2022.3 LTS / Unity 6)

// Setup via código:
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocaleManager : MonoBehaviour {
    public static LocaleManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLanguage(string localeCode) {
        // localeCode: "pt-BR" (EHAQUI) ou "en" (ISHERE)
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;
    }

    public string GetLocalizedString(string tableName, string key) {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, key);
        return op.IsDone ? op.Result : key;
    }

    // Smart Strings para texto dinâmico: "Encontre {0} tesouros!"
    public string FormatString(string key, params object[] args) {
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("UI", key);
        return string.Format(op.IsDone ? op.Result : key, args);
    }
}
```

### 8.2 Integração com JSON Externo

O Unity Localization Package pode importar dos nossos arquivos `i18n/pt-BR.json` e `i18n/en.json`:

```
Window → Localization Table → Create → String Table Collection
Import → CSV / JSON → Selecionar i18n/pt-BR.json e i18n/en.json
```

Os arquivos JSON do projeto servem como **source of truth**. O Unity Localization Package gera os assets binários para runtime.

### 8.3 Fallback Manual (caso não use o Package)

```csharp
// LocaleReader.cs — fallback manual caso o Unity Localization Package
// não atenda necessidades específicas do P2P/offline
public class LocaleReader {
    private Dictionary<string, Dictionary<string, string>> _tables;

    public async Task LoadLocale(string localeCode) {
        var path = Path.Combine(Application.streamingAssetsPath,
            $"i18n/{localeCode}.json");
        var json = await File.ReadAllTextAsync(path);
        _tables = JsonUtility.FromJson<LocaleData>(json).ToDict();
    }

    public string Get(string key) {
        var keys = key.Split('.');
        if (_tables.TryGetValue(keys[0], out var table)
            && table.TryGetValue(keys[1], out var value))
            return value;
        return key;  // Fallback: mostra a chave
    }
}
```

---

## 9. OFFLINE MODE — CACHE LOCAL

### 9.1 SQLite no Unity

```csharp
// Usar: https://github.com/robertohuertasm/SQLite4Unity3d
// Ou: Mono.Data.Sqlite (built-in no Unity)

public class OfflineCache : MonoBehaviour {
    private string _dbPath;
    private IDbConnection _db;

    private void Awake() {
        _dbPath = Path.Combine(Application.persistentDataPath, "ehaqui_cache.db");
        _db = new SqliteConnection($"URI=file:{_dbPath}");
        _db.Open();
        InitializeDatabase();
    }

    private void InitializeDatabase() {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS treasures (
                id TEXT PRIMARY KEY,
                type INTEGER,
                latitude REAL,
                longitude REAL,
                hint_hash TEXT,
                contract_json TEXT,
                creator_id TEXT,
                created_at INTEGER,
                ttl INTEGER,
                expires_at INTEGER,
                status INTEGER DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS pending_actions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                action_type INTEGER,
                payload_json TEXT,
                created_at INTEGER,
                retry_count INTEGER DEFAULT 0,
                next_retry_at INTEGER
            );
            CREATE TABLE IF NOT EXISTS map_tiles (
                x INTEGER,
                y INTEGER,
                zoom INTEGER,
                tile_data BLOB,
                cached_at INTEGER,
                PRIMARY KEY (x, y, zoom)
            );
            CREATE TABLE IF NOT EXISTS peer_reputation (
                peer_id TEXT PRIMARY KEY,
                score REAL DEFAULT 1.0,
                total_validations INTEGER DEFAULT 0,
                last_seen INTEGER
            );
        ";
        cmd.ExecuteNonQuery();
    }

    public void CacheTreasure(TreasureData treasure) {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"INSERT OR REPLACE INTO treasures
            (id, type, latitude, longitude, hint_hash, contract_json,
             creator_id, created_at, ttl, expires_at, status)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        cmd.Parameters.Add(treasure.ToDbParams());
        cmd.ExecuteNonQuery();
    }

    public void EnqueueSync(SyncAction action) {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"INSERT INTO pending_actions
            (action_type, payload_json, created_at)
            VALUES (?, ?, ?)";
        cmd.Parameters.Add(action.ToDbParams());
        cmd.ExecuteNonQuery();
    }

    public List<TreasureData> GetNearbyTreasures(double lat, double lng, double radiusKm) {
        // Haversine em SQL
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"
            SELECT *, (6371 * 2 * ASIN(SQRT(
                POWER(SIN(RADIANS(latitude - ?) / 2), 2) +
                COS(RADIANS(?)) * COS(RADIANS(latitude)) *
                POWER(SIN(RADIANS(longitude - ?) / 2), 2)
            ))) AS distance
            FROM treasures
            WHERE status = 0 AND expires_at > strftime('%s','now')
            HAVING distance < ?
            ORDER BY distance";
        return cmd.ExecuteQuery<TreasureData>();
    }
}
```

### 9.2 Fila de Sincronização Criptografada (AES-256)

```csharp
public class SyncQueue {
    private const string ENCRYPTION_KEY = "ehaqui-sync-key-256"; // Em produção: derivada de chave do usuário

    public async Task SyncPendingActions() {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return; // Offline — tentar depois

        var pending = _cache.GetPendingActions();
        foreach (var action in pending) {
            try {
                var success = await SendToServer(action);
                if (success)
                    _cache.RemoveAction(action.Id);
                else
                    _cache.IncrementRetry(action.Id);
            } catch {
                _cache.ScheduleRetry(action.Id,
                    CalculateBackoff(action.RetryCount));
            }
        }
    }

    private int CalculateBackoff(int retryCount) {
        // Exponential backoff: 30s, 1min, 2min, 4min, 8min... max 1h
        return Math.Min(30 * (int)Math.Pow(2, retryCount), 3600);
    }

    public string EncryptPayload(string json) {
        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(ENCRYPTION_KEY));
        aes.IV = new byte[16];  // IV fixo para determinismo (cache local)
        using var encryptor = aes.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
    }
}
```

### 9.3 Controle de Estado Offline

```csharp
public class OfflineState : MonoBehaviour {
    public bool IsOffline =>
        Application.internetReachability == NetworkReachability.NotReachable;

    public int PendingSyncCount { get; private set; }

    private void Update() {
        PendingSyncCount = _cache.GetPendingCount();

        if (!IsOffline && PendingSyncCount > 0) {
            StartCoroutine(SyncCoroutine());
        }
    }

    private IEnumerator SyncCoroutine() {
        yield return new WaitForSeconds(2); // Delay para evitar flood
        var queue = new SyncQueue();
        yield return queue.SyncPendingActions();
    }
}
```

---

## 10. P2P CONTRACT — VALIDAÇÃO CRIPTOGRÁFICA

### 10.1 Geração de Par de Chaves ECDSA

```csharp
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

public class CryptoIdentity {
    public AsymmetricCipherKeyPair KeyPair { get; private set; }
    public string PublicKeyBase64 { get; private set; }

    public CryptoIdentity() {
        var gen = new ECKeyPairGenerator("ECDSA");
        var keyGenParam = new ECKeyGenerationParameters(
            SecObjectIdentifiers.SecP256k1, new SecureRandom());
        gen.Init(keyGenParam);
        KeyPair = gen.GenerateKeyPair();

        var pubKey = (ECPublicKeyParameters)KeyPair.Public;
        PublicKeyBase64 = Convert.ToBase64String(pubKey.Q.GetEncoded(true));
    }
}
```

### 10.2 Contrato de Tesouro (P2P)

```csharp
public class TreasureContract {
    public string TreasureId;
    public string CreatorPublicKey;
    public double Latitude;
    public double Longitude;
    public string HintHash;        // keccak256 da dica
    public long CreatedAt;
    public long ExpiresAt;
    public string CreatorSignature;

    public byte[] Serialize() {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(TreasureId);
        writer.Write(CreatorPublicKey);
        writer.Write(Latitude);
        writer.Write(Longitude);
        writer.Write(HintHash);
        writer.Write(CreatedAt);
        writer.Write(ExpiresAt);
        return ms.ToArray();
    }

    public string Sign(ECPrivateKeyParameters privateKey) {
        var data = Serialize();
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(data, 0, data.Length);
        var signature = signer.GenerateSignature();
        return Convert.ToBase64String(signature);
    }

    public bool Verify(string signatureBase64) {
        var sigBytes = Convert.FromBase64String(signatureBase64);
        var pubKeyBytes = Convert.FromBase64String(CreatorPublicKey);
        var pubKey = PublicKeyFactory.CreateKey(pubKeyBytes);

        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(false, pubKey);
        signer.BlockUpdate(Serialize(), 0, Serialize().Length);
        return signer.VerifySignature(sigBytes);
    }
}
```

---

## 11. BUILD CONFIG — ANDROID + iOS

### 11.1 Android Build Settings

```
Player Settings → Android:
  - Package Name: com.ehaqui.app / com.ishere.app
  - Target API Level: 35 (Android 15)
  - Minimum API Level: 26 (Android 8.0)
  - Scripting Backend: IL2CPP
  - ARM64: ✅ (desativar ARMv7)
  - Multithreaded Rendering: ❌ (causa problemas em alguns devices)
  - Vulkan: ✅ (OpenGLES3 como fallback)
  - Split APK: ❌ (usar App Bundle)
  - Custom Gradle Template: ✅ (para configurar foreground service, etc.)

Publishing Settings:
  - Build App Bundle (Google Play): ✅
  - Keystore: criar com Unity ou Android Studio
```

### 11.2 iOS Build Settings

```
Player Settings → iOS:
  - Bundle Identifier: com.ehaqui.app / com.ishere.app
  - Target SDK: Device SDK
  - Target minimum iOS Version: 15.0
  - Architecture: ARM64
  - Scripting Backend: IL2CPP
  - Metal: ✅

Other Settings:
  - Accelerometer Frequency: Desligado
  - 32-bit Deprecation: ❌ (desativar)
```

### 11.3 Build Script Automatizado

```csharp
// Assets/Editor/BuildScript.cs
using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildScript {
    [MenuItem("Build/EHAQUI Android")]
    public static void BuildAndroid() {
        var options = new BuildPlayerOptions {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = "build/ehaqui-android.aab",
            target = BuildTarget.Android,
            options = BuildOptions.CompressWithLz4
        };
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/ISHERE iOS")]
    public static void BuildIOS() {
        var options = new BuildPlayerOptions {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = "build/ishere-ios",
            target = BuildTarget.iOS,
            options = BuildOptions.CompressWithLz4
        };
        BuildPipeline.BuildPlayer(options);
    }
}
```

---

## 12. ESTRUTURA DE DIRETÓRIOS UNITY

```
src/Ehaqui/
├── Assets/
│   ├── Scenes/
│   │   ├── Boot.unity          # Loading + initialization
│   │   ├── Main.unity          # Game world (mapa + detector)
│   │   ├── Shop.unity           # Loja
│   │   └── Settings.unity      # Configurações
│   │
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameState.cs       # Estado global (singleton)
│   │   │   ├── SessionManager.cs  # Login + auth
│   │   │   └── Analytics.cs       # Event tracking
│   │   │
│   │   ├── P2P/
│   │   │   ├── P2PManager.cs      # LiteNetLib wrapper
│   │   │   ├── WiFiDirectPlugin.cs # Android WiFi Direct
│   │   │   ├── BluetoothP2P.cs    # Fallback Bluetooth
│   │   │   ├── MessageProtocol.cs  # Tipos de mensagem
│   │   │   └── PeerReputation.cs  # Anti-cheat
│   │   │
│   │   ├── GPS/
│   │   │   ├── GpsService.cs       # Location updates
│   │   │   ├── GeofenceManager.cs  # Geofencing
│   │   │   └── MapController.cs    # Mapa + tiles
│   │   │
│   │   ├── Gameplay/
│   │   │   ├── TreasureManager.cs  # Criar/achar tesouros
│   │   │   ├── ChainManager.cs     # Correntes de pistas
│   │   │   ├── VibrationController.cs # Detector progressivo
│   │   │   └── MiniGameController.cs # Mini-games
│   │   │
│   │   ├── UI/
│   │   │   ├── HUD.cs              # Interface principal
│   │   │   ├── DetectorUI.cs       # Tela do detector
│   │   │   ├── ShopUI.cs           # Loja
│   │   │   ├── ChainUI.cs          # Corrente de pistas
│   │   │   └── FeedbackUI.cs       # Feedback sentimental
│   │   │
│   │   ├── Offline/
│   │   │   ├── OfflineCache.cs     # SQLite cache
│   │   │   ├── SyncQueue.cs        # Fila de sincronização
│   │   │   └── CryptoStorage.cs    # AES-256
│   │   │
│   │   ├── B2B/
│   │   │   ├── PluginManager.cs    # .themepack / .questpack
│   │   │   ├── AssetBundleLoader.cs # Download + validação
│   │   │   └── StorePortal.cs      # Integração loja parceira
│   │   │
│   │   ├── Blockchain/
│   │   │   ├── CryptoIdentity.cs   # ECDSA key pair
│   │   │   ├── P2PContract.cs      # Contrato de tesouro
│   │   │   └── Web3Integration.cs  # Polygon L2 (opcional)
│   │   │
│   │   └── Monetization/
│   │       ├── IAPManager.cs       # Google Play + App Store
│   │       ├── RewardedAdManager.cs # AdMob
│   │       └── SeasonPass.cs       # Passe do Caçador
│   │
│   ├── Prefabs/
│   │   ├── TreasureChest.prefab
│   │   ├── Detector.prefab
│   │   ├── PlayerCharacter.prefab
│   │   ├── MapTile.prefab
│   │   └── UIPrefabs/
│   │
│   ├── Art/
│   │   ├── Materials/
│   │   ├── Textures/
│   │   ├── Models/
│   │   ├── Animations/
│   │   └── Shaders/
│   │
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   │
│   ├── Resources/
│   │   └── IAPProductCatalog.asset
│   │
│   └── AddressableAssets/
│       ├── Local/
│       └── Remote/
│           ├── themepacks/
│           ├── questpacks/
│           └── soundpacks/
│
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
│
├── ProjectSettings/
│   ├── ProjectSettings.asset
│   └── ProjectVersion.txt
│
└── Plugins/
    ├── Android/
    │   ├── WiFiDirectPlugin.aar
    │   ├── GeofencePlugin.aar
    │   └── GpsForegroundService.aar
    └── iOS/
        ├── EhaquiLocationManager.mm
        └── EhaquiHaptics.mm
```

---

## 13. DECISÕES TÉCNICAS PARA EHAQUI / ISHERE

### 13.1 Mobile-First, Cross-Platform

| Área | Decisão | Motivo |
|------|---------|--------|
| Render Pipeline | URP Forward | SRP Batcher, mobile-first, Vulkan + Metal |
| Linguagem | C# | LiteNetLib, BouncyCastle, SQLite — tudo C# nativo |
| P2P | LiteNetLib + WiFi Direct | Maturidade, performance, exemplo Unity incluso |
| GPS | Android Native Plugin + iOS Core Location | FusedLocationProvider > Unity Input.location |
| Cache | SQLite (Mono.Data) | Leve, embarcado, sem dependência externa |
| IAP | Unity IAP 5.0+ | Google Billing v8 + StoreKit 2, receipt validation |
| Anúncio | AdMob (recompensado) | SDK oficial Unity, cross-platform |
| Plugins | Addressables + CCD | AssetBundle remoto com catálogo, validação hash |
| i18n | Unity Localization Package + JSON source | Import automático, Smart Strings, runtime switching |
| Cripto | BouncyCastle (ECDSA) | secp256k1, keccak256, compatível com Ethereum |
| Anti-cheat | Hash + GPS + Timestamp + Reputação | Validação P2P + testemunhas + cache de reputação |

### 13.2 Performance Targets

| Métrica | Alvo | Notas |
|---------|------|-------|
| FPS | 30 (mínimo 20 em devices fracos) | URP otimizado + LOD + GPU Instancing |
| APK size | < 50 MB (Android) / < 100 MB (iOS) | Compressão LZ4, ARM64 only |
| RAM | < 200 MB em devices médios | Pooling de objetos, Addressables sob demanda |
| GPS atualização | 1s (foreground) / 60s (background) | Balance entre precisão e bateria |
| P2P latência | < 100ms (WiFi Direct) / < 200ms (LAN) | LiteNetLib com QoS |
| Download inicial | < 5s (4G) / < 30s (3G) | Initial scene com progresso |
| Cache offline | < 50 MB SQLite + tiles | Compactação AES-256 |

### 13.3 Build Variants

| Variant | App Name | Bundle ID | Store | i18n |
|---------|----------|-----------|-------|------|
| EHAQUI BR | EHAQUI | `com.ehaqui.app` | Google Play | pt-BR default |
| ISHERE Global | ISHERE | `com.ishere.app` | App Store | en default |

Ambos os builds compartilham 99% do código. A diferença é:
- Nome do app e bundle ID
- Idioma padrão (pt-BR vs en)
- Preços na loja (R$ vs $)
- Dados do servidor (regionais)

---

*Documento gerado em 11/06/2026 — Unity 6.4 LTS | URP 17.x | LiteNetLib 2.1.4*
