# ⚙️ Fios Lógicos — Caça ao Tesouro P2P
## Máquinas de estado, regras, fluxos e sistemas

---

## ÍNDICE

1. [Ciclo de Vida do Tesouro](#1-ciclo-de-vida-do-tesouro)
2. [Ciclo de Vida da Corrente de Pistas](#2-ciclo-de-vida-da-corrente)
3. [Máquina de Estado do Jogador](#3-máquina-de-estado-do-jogador)
4. [Sistema de Detecção (GPS + Background)](#4-sistema-de-detecção)
5. [Sistema de Notificação por Passagem](#5-notificação-por-passagem)
6. [Sistema de Prêmio Crescente](#6-premio-crescente)
7. [Sistema de Peer Discovery (P2P)](#7-peer-discovery)
8. [Sistema de Anti-Cheat](#8-anti-cheat)
9. [Sistema de Monetização](#9-sistema-de-monetização)
10. [Sistema de Plugins (Pacotes de Estúdio)](#10-plugins)
11. [Economia do Jogo](#11-economia)
12. [Árvore de Decisão do Detector](#12-arvore-detector)
13. [Regras de Negócio (Regras Fixas)](#13-regras-de-negocio)
14. [Sistema de Feedback Sentimental](#14-feedback-sentimental)
15. [Sistema de Tesouro de Alto Valor](#15-tesouro-alto-valor)
16. [Sistema Offline](#16-sistema-offline)

---

## 1. CICLO DE VIDA DO TESOURO

### 1.1 Tesouro Virtual

```
         ┌─────────────────────────────────────────────────────┐
         │                   ENTERRAR                          │
         │  Jogador define: local + TTL (opcional) + raridade │
         └─────────┬───────────────────────────────────────────┘
                   │
                   ▼
         ┌──────────────────┐
         │   SEM ENTERRADO  │  ← salvo no dispositivo do criador
         │   (não visível)  │     broadcast P2P: "novo tesouro em (lat,lng)"
         └─────────┬────────┘
                   │
                   ▼
         ┌──────────────────┐
         │      ATIVO       │  ← visível para peers na área
         │                  │     detector aponta, prêmio começa a crescer
         └─────────┬────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
 ┌──────────────┐   ┌──────────────────┐
 │   ACHADO     │   │  EXPIRADO (TTL)  │
 │ jogador      │   │ TTL estourou sem │
 │ descobriu →  │   │ ninguém achar    │
 │ valida P2P   │   └────────┬─────────┘
 └────────┬─────┘            │
          │                  ▼
          ▼          ┌──────────────────┐
 ┌──────────────┐    │   REVERTIDO      │
 │  COLETADO    │    │ criador recebe   │
 │ premio pago  │    │ bônus de volta   │
 │ criador 50%  │    │ (+ juros se      │
 │ XP           │    │  apostou moedas) │
 └──────────────┘    └──────────────────┘
```

### 1.2 Tesouro Físico (Loja)

```
 ┌──────────────────┐
 │  LOJA CRIA QR    │  ← assinante B2B gera QR pelo app/web
 │  define: premio  │
 └─────────┬────────┘
           │
           ▼
 ┌──────────────────┐
 │     ATIVO        │  ← QR impresso colado na loja
 │                  │     visível no mapa como "baú dourado"
 └─────────┬────────┘
           │
           ▼
 ┌──────────────────┐
 │   JOGADOR CHEGA  │  ← abre app → escaneia QR
 │   valida GPS +   │     loja confirma presencialmente
 │   QR + timestamp │
 └─────────┬────────┘
           │
           ▼
 ┌──────────────────┐
 │    COLETADO      │  ← jogador ganha brinde
 │                  │     loja paga assinatura
 │                  │     QR pode ser reusado
 └──────────────────┘
```

---

## 2. CICLO DE VIDA DA CORRENTE

```
 ┌──────────────────────────────────────────────────┐
 │                  CRIAR CORRENTE                  │
 │ Jogador: define nº de pistas (3-7), locais,     │
 │ charadas, premio final (virtual ou físico)       │
 └──────────────────────┬───────────────────────────┘
                        │
                        ▼
 ┌──────────────────────────────────────────────────┐
 │                  NÃO INICIADA                     │
 │ Criador ainda não ativou a corrente              │
 │ (pode editar, salvar rascunho)                   │
 └──────────────────────┬───────────────────────────┘
                        │ [criador ativa]
                        ▼
 ┌──────────────────────────────────────────────────┐
 │                  EM CURSO                         │
 │ Primeira pista visível no mapa                   │
 │ Apenas a pista atual de cada jogador é revelada  │
 └──────────────────────┬───────────────────────────┘
                        │
                        ├────────────────────────────┐
                        ▼                            ▼
 ┌────────────────────────────┐        ┌──────────────────────────┐
 │     PISTA N DESBLOQUEADA   │        │    CORRENTE EXPIRADA     │
 │ Jogador chegou ao local,   │        │ (TTL da corrente estourou│
 │ escaneou QR / validou GPS  │        │  sem ninguém completar)  │
 │ → próxima pista revelada   │        └──────────┬───────────────┘
 └─────────────┬──────────────┘                   │
               │                                  ▼
               │ (última pista?)         ┌────────────────────────┐
               ├── sim ────►             │   ARQUIVADA            │
               │                         │ Criador pode reativar  │
               ▼                         │ ou receber bônus       │
 ┌────────────────────────────┐          └────────────────────────┘
 │         COMPLETADA         │
 │ Jogador resolveu a última  │
 │ pista → ganha prêmio final │
 │ Criador ganha XP bônus     │
 │ Corrente entra no hall     │
 └────────────────────────────┘
```

### Regras da Corrente

| Regra | Valor |
|---|---|
| Pistas mínimas | 3 |
| Pistas máximas (grátis) | 5 |
| Pistas máximas (premium) | 10 |
| Distância mínima entre pistas | 20m |
| Distância máxima entre pistas | 5km |
| TTL da corrente (opcional) | 24h a 30 dias |
| Nº de tentativas por pista | ilimitado |
| Tempo máximo por pista | sem limite |
| Jogadores simultâneos na mesma corrente | ilimitado |

---

## 3. MÁQUINA DE ESTADO DO JOGADOR

```
                      ┌──────────┐
                      │  OFFLINE │
                      │ (menu)   │
                      └────┬─────┘
                           │ [entrar no mapa]
                           ▼
 ┌────────────────────────────────────────────────────────────┐
 │                      LIVRE                                 │
 │  - Vê mapa com tesouros ativos na região                  │
 │  - Pode escolher um tesouro e navegar até ele             │
 │  - Pode criar/enterrar tesouro                           │
 │  - Pode criar corrente                                   │
 │  - Recebe notificações passivas (background)              │
 └──┬───────────────┬───────────────┬───────────────┬────────┘
    │               │               │               │
    ▼               ▼               ▼               ▼
 ┌──────┐     ┌──────────┐    ┌──────────┐    ┌────────────┐
 │NAVEG.│     │CAÇANDO   │    │ENTERrando│    │CRIANDO     │
 │(rota)│     │(detector │    │tesouro   │    │CORRENTE    │
 │      │     │ ligado)  │    │virtual   │    │(editor)    │
 └──┬───┘     └────┬─────┘    └────┬─────┘    └─────┬──────┘
    │              │               │                │
    └──────┬───────┘               │                │
           ▼                       │                │
    ┌───────────┐                  │                │
    │VALIDANDO  │                  │                │
    │(mini-game │                  │                │
    │ desenterro│                  │                │
    │ ou escanei│                  │                │
    │ QR)       │                  │                │
    └─────┬─────┘                  │                │
          │                        │                │
          ▼                        ▼                ▼
    ┌───────────┐            ┌──────────┐     ┌──────────┐
    │ COLETADO  │            │ENTERADO  │     │PUBLICADO │
    │ +XP +moeda│            │(salvo no │     │(ativa)   │
    │ +animação │            │ device)  │     │          │
    └─────┬─────┘            └──────────┘     └──────────┘
          │
          ▼
    ┌───────────┐
    │  LIVRE    │  ← retorna ao estado livre
    └───────────┘
```

---

## 4. SISTEMA DE DETECÇÃO

### 4.1 Fluxo do Detector

```
 [Jogador em estado CAÇANDO]
         │
         ▼
 ┌──────────────────────┐
 │   LÊ GPS (1s ciclo)  │
 │   posição atual (lat,│
 │   lng, precisão)     │
 └──────────┬───────────┘
            │
            ▼
 ┌──────────────────────┐
 │  CALCULA DISTÂNCIA   │
 │  para cada tesouro   │
 │  ativo nos peers     │
 └──────────┬───────────┘
            │
            ▼
 ┌──────────────────────────────────────────────────┐
 │            ÁRVORE DE DISTÂNCIA                    │
 │                                                  │
 │  d > 200m  → sem sinal, tela normal              │
 │  50-200m   → vibração fraca (intervalo 2s)      │
 │                     + borda pulsando fraca       │
 │  20-50m    → vibração média (intervalo 1s)      │
 │                     + som de aproximação         │
 │  5-20m     → vibração forte (intervalo 0.3s)    │
 │                     + seta direcional            │
 │  0-5m      → vibração contínua                  │
 │                     + "CAV AQUI!" + mini-game   │
 └──────────────────────────────────────────────────┘
            │ (d < 5m)
            ▼
 ┌──────────────────────┐
 │   MINI-GAME          │
 │   Desenterrar:       │
 │   - swipe rápido     │
 │   - estourar bolha   │
 │   - sequência de     │
 │     toques           │
 └──────────┬───────────┘
            │ (sucesso)
            ▼
 ┌──────────────────────┐
 │   COLETA             │
 │   prêmio calculado   │
 │   notifica criador   │
 │   P2P: "achou!"      │
 └──────────────────────┘
```

### 4.2 Estados do Detector

```
 ┌────────────┐
 │ DESLIGADO  │ ← modo padrão (economia de bateria)
 └─────┬──────┘
       │ [jogador ativa detector]
       ▼
 ┌────────────┐
 │ VARRENDO   │ ← lê GPS, escuta peers, calcula distância
 │ (baixo     │    vibração intermitente
 │  consumo)  │
 └─────┬──────┘
       │ [d < 200m]
       ▼
 ┌────────────┐
 │ APROXIM.   │ ← vibração aumenta, som ativo
 │ (médio     │    tela com radar/sonar
 │  consumo)  │
 └─────┬──────┘
       │ [d < 5m]
       ▼
 ┌────────────┐
 │ CAVANDO    │ ← mini-game ativo
 │ (alto      │    câmera treme, partículas
 │  consumo)  │
 └────────────┘
```

---

## 5. NOTIFICAÇÃO POR PASSAGEM (BACKGROUND)

### 5.1 Fluxo

```
 [App em background / suspenso]
         │
         ▼
 ┌─────────────────────────────────────┐
 │ GPS em background (baixa precisão)  │
 │ Ciclo: a cada 60s (Android limit)  │
 └────────────────┬────────────────────┘
                  │
                  ▼
 ┌─────────────────────────────────────┐
 │ Compara posição atual com todos os  │
 │ tesouros ativos dos peers na região │
 │ (cache local atualizado via P2P)    │
 └────────────────┬────────────────────┘
                  │
          ┌───────┴───────┐
          │               │
    d < 50m.          d >= 50m
          │               │
          ▼               ▼
 ┌──────────────┐  ┌──────────────┐
 │ NOTIFICA     │  │ DORMIR       │
 │ push local:  │  │ (próximo     │
 │ "Algo brilha │  │  ciclo)      │
 │  perto de    │  └──────────────┘
 │  você! 🌐"   │
 │ + círculo    │
 │ no mapa      │
 └──────┬───────┘
        │
        ▼
 ┌──────────────────────────────────────┐
 │ Círculo de aproximação:              │
 │ - Raio inicial: 50m                 │
 │ - Jogador abre app → círculo aparece│
 │ - Conforme anda, círculo encolhe    │
 │ - Só revela ponto exato quando      │
 │   ativa o detector manualmente      │
 └──────────────────────────────────────┘
```

### 5.2 Regras

| Regra | Valor |
|---|---|
| Ciclo de GPS background | 60s |
| Raio inicial da notificação | 50m |
| Raio mínimo do círculo | 10m (depois revela) |
| Cooldown entre notificações no mesmo local | 30 min |
| Máx de notificações/dia | 10 (Android policy) |
| Bateria | Fallback para geofencing API |

---

## 6. SISTEMA DE PRÊMIO CRESCENTE

### 6.1 Fórmula do bônus

```
XP_base = valor fixo por raridade do tesouro
XP_bonus = XP_base × (horas_sem_achar / 24) × multiplicador_raridade
XP_total = XP_base + XP_bonus

Onde:
  multiplicador_raridade:
    comum    = 0.5
    raro     = 1.0
    épico    = 1.5
    lendário = 2.0

  XP_base:
    comum    = 50
    raro     = 150
    épico    = 400
    lendário = 1000
```

### 6.2 Tabela de referência

| Tempo sem achar | Comum | Raro | Épico | Lendário |
|---|---|---|---|---|
| 0h (base) | 50 XP | 150 XP | 400 XP | 1000 XP |
| 6h | 56 XP | 187 XP | 500 XP | 1500 XP |
| 12h | 62 XP | 225 XP | 600 XP | 2000 XP |
| 24h | 75 XP | 300 XP | 800 XP | 3000 XP |
| 48h | 100 XP | 450 XP | 1200 XP | 5000 XP |
| 72h | 125 XP | 600 XP | 1600 XP | 7000 XP |
| 7 dias | 225 XP | 1200 XP | 3200 XP | 15000 XP |

### 6.3 Bônus ao criador

```
Criador_recebe = XP_total_do_descobridor × 0.5
```

---

## 7. SISTEMA DE PEER DISCOVERY (P2P)

### 7.1 Hierarquia de conexão

```
 [Jogador A] entra na área
      │
      ▼
 ┌────────────────────────────────────┐
 │ TENTA WiFi Direct (WifiP2pManager) │
 │ Descobre peers próximos (até 100m) │
 └──────────────┬─────────────────────┘
                │ (falhou?)
          ┌─────┴─────┐
          │           │
        SIM          NÃO (conectado)
          │           │
          ▼           ▼
 ┌──────────────┐  ┌────────────────────┐
 │ BLE (Bluetooth│  │ Socket TCP/UDP     │
 │ Low Energy)   │  │ LiteNetLib         │
 │ fallback      │  │ Troca:            │
 └──────┬───────┘  │ - lista tesouros  │
        │          │ - confirmações     │
        ▼          │ - chat / emotes    │
 ┌──────────────┐  └────────────────────┘
 │ LAN (mesmo   │
 │ WiFi)        │
 │ último caso  │
 └──────────────┘
```

### 7.2 Protocolo de troca

```
[PEER A] → [PEER B] (broadcast na rede P2P)

Mensagens:
─────────────────────────────────────────────
| Tipo          | Conteúdo                  |
|---------------|---------------------------|
| HELLO        | ID, nick, versão app      |
| TREASURE_LIST| lista de tesouros ativos  |
|              | (coordenadas cripto + hash)|
| TREASURE_FOUND| ID_tesouro, timestamp    |
|              | proof (hash da validação)  |
| CLAIM        | reivindica prêmio         |
| CONFIRM      | confirma coleta           |
| CHAT         | mensagem texto/emote      |
| QUEST_LIST   | correntes ativas          |
| QUEST_STEP   | pista concluída           |
| QUEST_DONE   | corrente completa         |
| PING         | keepalive (30s)           |
─────────────────────────────────────────────

Timeout sem PING: 120s → peer removido
```

### 7.3 Criptografia de coordenadas

```
Para evitar snifagem de posição exata:
- A cada broadcast, coordenadas são ofuscadas com
  hash(HMAC(coord_secret + timestamp))
- Apenas quando o detector está a < 50m o peer
  dono revela a coordenada exata via mensagem criptografada
- Chave HMAC trocada no handshake HELLO
```

---

## 8. SISTEMA DE ANTI-CHEAT

### 8.1 Regras locais (validadas pelo próprio dispositivo)

| Regra | Ação |
|---|---|
| GPS desligado ou spoofado | Bloqueia caça, notifica |
| Velocidade > 30 km/h (de carro) | Ignora detecção |
| Distância mínima entre coleta e enterro | 20m |
| Mesmo IP WiFi + mesmo tesouro em <1s | Suspeito, marca peer |
| Mini-game falhou 3x seguidas | Tempo de espera 60s |

### 8.2 Regras de consenso (validadas entre peers)

```
 [Jogador A] acha tesouro do [Jogador B]
      │
      ▼
 A envia: "TREASURE_FOUND" + prova (hash do mini-game + GPS)
      │
      ▼
 B recebe e valida:
      │
      ├── hash mini-game confere?  → SIM/NÃO
      ├── timestamp é anterior ao TTL? → SIM/NÃO
      ├── distância entre enterro e coleta > 20m? → SIM/NÃO
      └── velocidade no momento da coleta < 30 km/h? → SIM/NÃO
      │
      ▼
 ┌──────────────────────────────────────┐
 │ Se TODAS as validações passam:       │
 │   B envia CONFIRM → A coleta prêmio  │
 │                                      │
 │ Se alguma falha:                     │
 │   B envia REJECT + motivo            │
 │   Tesouro continua ativo             │
 │   A marcado como "suspeito"          │
 │   (3 suspeitas = ban temporário)     │
 └──────────────────────────────────────┘
```

### 8.3 Flag de reputação

```
Cada peer mantém reputação local de outros peers:
- 1 descoberta válida = +1 reputação
- 1 rejeição = -1 reputação
- 3 rejeições = peer marcado como "não confiável"
- Peer não confiável tem tesouros ignorados
```

---

## 9. SISTEMA DE MONETIZAÇÃO

### 9.1 Loja — fluxo de compra

```
 [Jogador] → abre loja
      │
      ▼
 ┌────────────────────────────────────────────┐
 │ Lista de itens disponíveis                 │
 │ (cosméticos, passes, moedas premium)       │
 └────────────────────┬───────────────────────┘
                      │
                      ▼
 ┌────────────────────────────────────────────┐
 │ Seleciona item → confirma compra          │
 │ Pagamento: Google Play Billing            │
 └────────────────────┬───────────────────────┘
                      │
                      ▼
 ┌────────────────────────────────────────────┐
 │ Compra processada pela loja Google         │
 │ Token de compra salvo localmente           │
 │ Item desbloqueado (PEER_BROADCAST:         │
 │ "item_adquirido" para peers validarem)     │
 └────────────────────────────────────────────┘
```

### 9.2 Assinatura B2B (lojas parceiras)

```
 [Loja] → acessa portal web
      │
      ▼
 ┌────────────────────────────────────────────┐
 │ Escolhe plano: Básico / Profissional       │
 │ Pagamento: Stripe / PayPal                 │
 └────────────────────┬───────────────────────┘
                      │
                      ▼
 ┌────────────────────────────────────────────┐
 │ QR gerado exclusivamente para a loja       │
 │ - QR tem hash único + timestamp            │
 │ - Ativo enquanto assinatura vigente        │
 │ - Loja imprime e cola no local             │
 └────────────────────────────────────────────┘
```

### 9.3 Marketplace de pacotes (estúdios)

```
 [Estúdio] → submete pacote no portal
      │
      ▼
 ┌────────────────────────────────────────────┐
 │ Validação automática do .themepack/        │
 │ .questpack/.soundpack                      │
 │ (schema JSON, tamanho, assets)             │
 └────────────────────┬───────────────────────┘
                      │
                      ▼
 ┌────────────────────────────────────────────┐
 │ Publicado na loja do app                   │
 │ Usuário compra → 70% estúdio / 30% app     │
 │ Pagamento via Google Play                  │
 └────────────────────────────────────────────┘
```

---

## 10. SISTEMA DE PLUGINS (PACOTES DE ESTÚDIO)

### 10.1 Estrutura do .themepack

```
 .themepack
 ├── manifest.json
 │   {
 │     "id": "beto_carrero_2026",
 │     "name": "Mundo Beto Carrero",
 │     "version": "1.0",
 │     "studio": "Beto Carrero Park",
 │     "type": "theme",
 │     "price": 4990, // centavos (R$ 49,90 p/ usuário)
 │     "assets": {
 │       "map_overlay": "map_overlay.png",
 │       "chest_common": "chest_common.fbx",
 │       "chest_rare": "chest_rare.fbx",
 │       "chest_epic": "chest_epic.fbx",
 │       "chest_legendary": "chest_legendary.fbx",
 │       "detector_skin": "detector_skin.png",
 │       "ui_theme": "ui_theme.json",
 │       "particles": "particles/"
 │     },
 │     "requirements": {
 │       "min_app_version": "1.2.0",
 │       "free_disk_mb": 150
 │     }
 │   }
 ├── map_overlay.png      // textura sobre OSM
 ├── chest_common.fbx      // baú comum temático
 ├── chest_rare.fbx
 ├── chest_epic.fbx
 ├── chest_legendary.fbx
 ├── detector_skin.png     // skin do radar/detector
 ├── ui_theme.json         // cores, fontes, ícones
 └── particles/            // efeitos visuais
```

### 10.2 Estrutura do .questpack

```
 .questpack
 ├── manifest.json
 │   {
 │     "id": "disney_magic_quest",
 │     "name": "Disney Magic Quest",
 │     "version": "1.0",
 │     "studio": "Disney",
 │     "type": "quest",
 │     "price": 9990, // R$ 99,90
 │     "steps": [
 │       {
 │         "order": 1,
 │         "lat": -23.561, "lng": -46.656,
 │         "radius_m": 30,
 │         "challenge_type": "riddle",
 │         "challenge_data": {
 │           "text": "Onde o rato mais famoso do mundo mora?",
 │           "answer_hash": "a1b2c3..."
 │         },
 │         "hint": "Procure por orelhas grandes"
 │       },
 │       ...
 │     ],
 │     "final_reward": {
 │       "type": "qr_code_physical",
 │       "description": "10% de desconto na loja do parque",
 │       "partner_code": "DISNEY_STORE_001"
 │     }
 │   }
 ├── npc_avatar.fbx        // personagem guia
 ├── step_sounds/          // áudio por etapa
 └── final_animation.mp4   // animação de conclusão
```

### 10.3 Pipeline de carregamento

```
 [Usuário compra pacote]
      │
      ▼
 ┌──────────────────────────────────┐
 │ Download via Google Play         │
 │ AssetBundle / Addressables       │
 └──────────────┬───────────────────┘
                ▼
 ┌──────────────────────────────────┐
 │ Validação de integridade (hash)  │
 └──────────────┬───────────────────┘
                ▼
 ┌──────────────────────────────────┐
 │ Cache local (pasta /assets/      │
 │ pacotes/)                        │
 └──────────────┬───────────────────┘
                ▼
 ┌──────────────────────────────────┐
 │ Aplicação:                       │
 │ - Sobrescreve UI (cores/fontes)  │
 │ - Substitui modelos de baú       │
 │ - Altera textura do mapa         │
 │ - Troca skin do detector         │
 │ - Adiciona músicas SFX           │
 └──────────────┬───────────────────┘
                ▼
 ┌──────────────────────────────────┐
 │ Pronto! Tema ativo até o jogador │
 │ trocar manualmente ou expirar    │
 │ (se temporário)                  │
 └──────────────────────────────────┘
```

---

## 11. ECONOMIA DO JOGO

### 11.1 Moedas

```
 ┌──────────────────────┐     ┌─────────────────────────┐
 │   MOEDA AZUL         │     │   MOEDA DOURADA          │
 │   (grátis, farmável) │     │   (premium, comprável)   │
 ├──────────────────────┤     ├─────────────────────────┤
 │ Uso:                 │     │ Uso:                     │
 │ - Pás comuns         │     │ - Pás especiais          │
 │ - Skins básicas      │     │ - Skins raras/lendárias  │
 │ - Boost de XP        │     │ - Detector Premium       │
 │ - Criar correntes    │     │ - Mapa do Tesouro        │
 │ (até 5 pistas)       │     │ - Criar correntes (10)   │
 │                      │     │ - Tema de estúdio        │
 ├──────────────────────┤     ├─────────────────────────┤
 │ Farm:                │     │ Compra:                  │
 │ - 50 por tesouro     │     │ - 100 = R$ 4,99          │
 │ - 10 por pista       │     │ - 500 = R$ 19,90         │
 │ - 5 por indicação    │     │ - 1200 = R$ 39,90        │
 │ - Bônus diário       │     │ - 3000 = R$ 89,90        │
 └──────────────────────┘     └─────────────────────────┘
```

### 11.2 Economia circular

```
 [Jogador enterra tesouro]
      │ pode apostar moedas azuis
      ▼
 ┌──────────────────────────────┐
 │ Tesouro ativo (TTL)          │
 │                              │
 │ ┌──────────────────────────┐ │
 │ │ Se NINGUÉM achar no TTL: │ │
 │ │ criador GANHA dobro da   │ │
 │ │ aposta (moedas azuis)    │ │
 │ └──────────────────────────┘ │
 │                              │
 │ ┌──────────────────────────┐ │
 │ │ Se ALGUÉM achar:          │ │
 │ │ criador PERDE aposta     │ │
 │ │ descobridor GANHA aposta │ │
 │ └──────────────────────────┘ │
 └──────────────────────────────┘
```

### 11.3 Tabela de custos

| Ação | Custo (moeda azul) | Custo (moeda dourada) |
|---|---|---|
| Enterrar tesouro comum | 0 | 0 |
| Enterrar tesouro raro | 50 | — |
| Enterrar tesouro épico | 200 | — |
| Enterrar tesouro lendário | — | 50 |
| Criar corrente (3 pistas) | 100 | — |
| Criar corrente (7 pistas) | — | 100 |
| Apostar no tesouro | 10–500 | — |
| Detector turbo (5 min) | 50 | — |
| Detector Premium (7 dias) | — | 150 |
| Skin de baú | 200 | — |
| Skin premium | — | 100–500 |
| Passe do Caçador (30 dias) | — | 1000 |
| Mapa do Tesouro | — | 30 (ou anúncio) |
| Anúncio recompensado | 50 | — |

---

## 12. ÁRVORE DE DECISÃO DO DETECTOR

```
         ┌────────────────────────────────────┐
         │         ESTADO: CAÇANDO             │
         │  Detector ligado, GPS ativo         │
         └────────────────┬───────────────────┘
                          │
                          ▼
         ┌────────────────────────────────────┐
         │   LISTA DE TESOUROS PRÓXIMOS       │
         │   (ordenada por distância)         │
         └────────────────┬───────────────────┘
                          │ [para cada tesouro]
                          ▼
         ┌────────────────────────────────────┐
         │           d > 200m?                 │
         └──────┬──────────────────────┬──────┘
                │ SIM                  │ NÃO
                ▼                      ▼
         ┌──────────┐        ┌──────────────────┐
         │ IGNORA   │        │ d entre 50-200m? │
         │ (próx.   │        └──────┬───────────┘
         │  tesouro)│          SIM  │      NÃO
         └──────────┘               │
                │                   │
                ▼                   ▼
         ┌──────────┐        ┌──────────────────┐
         │ IGNORA   │        │ d entre 20-50m?  │
         │ (próx.   │        └──────┬───────────┘
         │  tesouro)│          SIM  │      NÃO
         └──────────┘               │
                │                   │
                ▼                   ▼
         ┌──────────┐        ┌──────────────────┐
         │ IGNORA   │        │ d entre 5-20m?   │
         │ (próx.   │        └──────┬───────────┘
         │  tesouro)│          SIM  │      NÃO (d < 5m)
         └──────────┘               │
                │                   │
                ▼                   ▼
         ┌──────────────────────────────────────────────┐
         │     MÚLTIPLOS TESOUROS NO RAIO?              │
         │     (caso haja mais de um no mesmo detector) │
         └──────────────────────┬───────────────────────┘
                                │
                    ┌───────────┴────────────┐
                    │                        │
                    ▼                        ▼
         ┌────────────────────┐   ┌──────────────────────┐
         │ APENAS 1           │   │ MÚLTIPLOS            │
         │ → foca nele        │   │ → menu radial        │
         │   (vibração + seta)│   │   jogador escolhe    │
         └────────────────────┘   │   qual cavar         │
                                 └──────────────────────┘
```

---

## 13. REGRAS DE NEGÓCIO (REGRAS FIXAS)

### Regras Globais

| # | Regra | Justificativa |
|---|---|---|
| R1 | Tesouro virtual não pode ser enterrado dentro de área residencial privada (sem via pública) | Segurança e privacidade |
| R2 | Distância mínima entre dois tesouros: **10m** | Evitar sobreposição |
| R3 | Distância mínima entre enterro do criador e local do tesouro: **20m** | Obriga sair de casa |
| R4 | TTL máximo: **30 dias** | Renovação do mapa |
| R5 | TTL mínimo: **1 hora** | Viabilidade |
| R6 | Corrente máxima de pistas: **10** (premium) / **5** (grátis) | Equilíbrio |
| R7 | Velocidade máxima para detectar: **30 km/h** | Anti-cheat (carro) |
| R8 | Nível mínimo para enterrar tesouro lendário: **10** | Progressão |
| R9 | Um jogador pode ter no máximo **10 tesouros ativos** simultaneamente | Evitar spam |
| R10 | Tesouro físico (loja) não ocupa limite de tesouros ativos do jogador | Regra de parceiro |

### Regras de Corrente

| # | Regra | Justificativa |
|---|---|---|
| C1 | Apenas o criador pode editar/remover a corrente | Propriedade |
| C2 | Corrente pode ser compartilhada (mesmo QR/P2P) entre jogadores simultâneos | Social |
| C3 | Cada jogador vê apenas a **próxima pista** da corrente | Progressão |
| C4 | Corrente com TTL expirado não pode ser continuada — volta ao estado "expirada" | Finalidade |
| C5 | Criador pode reativar corrente expirada pagando 50 moedas azuis | Economia |

### Regras de Peer

| # | Regra | Justificativa |
|---|---|---|
| P1 | Peer sem HELLO em 120s é removido da rede local | Limpeza |
| P2 | Peer com mais de 3 rejeições de validação é marcado "não confiável" | Anti-cheat |
| P3 | Tesouros de peers não confiáveis não aparecem no detector | Isolamento |
| P4 | Um peer pode servir tesouros de outros peers offline (cache) | Disponibilidade |
| P5 | Cache de tesouros expira em **24h** sem renovação do peer dono | Frescor do dado |

### Regras de Economia

| # | Regra | Justificativa |
|---|---|---|
| E1 | Moeda azul não pode ser comprada diretamente (só farm) | Equilíbrio |
| E2 | Moeda dourada pode ser comprada ou ganha em eventos | Monetização |
| E3 | Aposta máxima em tesouro: 500 moedas azuis | Limite de risco |
| E4 | XP máximo por dia: 10.000 | Anti-grind |
| E5 | Itens cosméticos comprados são permanentes | Propriedade |
| E6 | Passe do Caçador é renovação mensal (não cumulativo) | Modelo de receita |

---

## 14. SISTEMA DE FEEDBACK SENTIMENTAL

### 14.1 Fluxo do feedback

```
 [CRIADOR] enterra tesouro sentimental
    ├── anexa: história (texto), áudio (opcional), foto (opcional)
    └── salva + broadcast P2P + servidor
         │
         ▼
 [DESCOBRIDOR] acha o tesouro
    ├── lê história / ouve áudio / vê foto
    ├── é OBRIGADO a deixar feedback (mín 1 tipo)
    │   ├── 📝 texto (500 chars)
    │   ├── 🎤 áudio (15s)
    │   ├── 📸 foto (selfie com o objeto)
    │   └── 🎨 desenho (tela de toque livre)
    └── envia feedback → salva local + P2P
         │
         ▼
 [SERVIDOR] recebe feedback
    ├── notifica criador (push): "💌 Algo deixou uma mensagem pra você!"
    ├── credita XP dobrado para ambas as partes
    └── feedback visível no perfil do criador (só pra ele)
```

### 14.2 Tipos de feedback

| Tipo | Limite | Moderável? | Tamanho |
|---|---|---|---|
| Texto | 500 caracteres | Sim (palavrão filter) | ~2 KB |
| Áudio | 15 segundos | Sim (revisão) | ~50 KB |
| Foto | 1 foto | Sim (revisão) | ~500 KB |
| Desenho | Tela 400x400px | Sim (revisão) | ~50 KB |

### 14.3 Regras

| # | Regra |
|---|---|
| S1 | Feedback é obrigatório para tesouro sentimental |
| S2 | Prazo para enviar feedback: 24h após achar |
| S3 | Se passar do prazo, feedback é "OPA! Obrigado!" padrão do sistema |
| S4 | Criador pode responder ao feedback (1 resposta apenas) |
| S5 | Feedback é anônimo por padrão (nick só se ambas as partes permitirem) |
| S6 | Conteúdo impróprio → denúncia → ban de 7 dias |
| S7 | Feedback pode ser convertido em "post" público (com permissão) |

### 14.4 Estados do feedback

```
 PENDENTE (descobridor achou, ainda não respondeu)
    │ [24h]
    ├── RESPONDIDO (descobridor enviou)
    │       │ [criador vê]
    │       ├── LIDO (criador abriu)
    │       └── RESPONDIDO (criador respondeu)
    │
    └── EXPIRADO (24h passou, feedback automático)
```

---

## 15. SISTEMA DE TESOURO DE ALTO VALOR (BITCOIN / ITENS REAIS)

### 15.1 Hierarquia de tesouros por valor

```
 ┌──────────────────────────────────────────────┐
 │           HIERARQUIA DE TESOUROS              │
 │                                              │
 │  🌟 Comum       → 0 XP base                  │
 │  ⭐ Raro        → 150 XP base                │
 │  💫 Épico       → 400 XP base                │
 │  👑 Lendário    → 1000 XP base (virtual)     │
 │  🔥 REAL        → 10.000+ XP (valor real)    │
 │                                              │
 │  REAL = qualquer tesouro com valor material   │
 │  acima de R$ 20 declarado pelo criador       │
 └──────────────────────────────────────────────┘
```

### 15.2 Fluxo

```
 [CRIADOR] (nível mínimo 10, reputação positiva)
    │
    ├── compra "Baú Lendário Premium" (R$ 49,90)
    ├── declara:
    │   ├── tipo do prêmio (bitcoin, dinheiro, vale-presente, ingresso, objeto)
    │   ├── valor estimado (R$ 20 a R$ 10.000)
    │   ├── descrição do que o descobridor vai encontrar
    │   └── opcional: foto do prêmio escondido
    ├── DEPOSITA o prêmio real no local físico
    └── publica → tesouro visível apenas para níveis 5+
         │
         ▼
 [DESCOBRIDOR] (nível mínimo 5)
    │
    ├── vê baú dourado pulsando com ⭐ "VALOR REAL"
    ├── detector leva até o local
    ├── mini-game de desenterrar
    ├── confirmação visual: tira foto do prêmio
    ├── GPS valida + timestamp + foto
    └── envia confirmação ao servidor
         │
         ▼
 [SERVIDOR] recebe confirmação
    │
    ├── notifica criador: "🔥 Seu tesouro lendário foi encontrado!"
    ├── criador tem 72h para confirmar (ou discordar)
    ├── se confirmar:
    │   ├── descobridor ganha prêmio real (fora do app)
    │   ├── descobridor ganha badge "Caçador de Lendas"
    │   ├── descobridor ganha 200 moedas douradas
    │   ├── criador ganha badge "Mestre dos Tesouros" (vitalício)
    │   ├── criador ganha 10.000 XP
    │   ├── criador ganha 5% do valor declarado em moeda dourada
    │   └── ambos entram no Hall da Fama
    │
    └── se NÃO confirmar em 72h:
        ├── app assume que é verdadeiro
        ├── badges liberados para descobridor
        └── criador perde direito de contestar
```

### 15.3 Tabela de recompensas

| Item | Criador | Descobridor |
|---|---|---|
| Badge | "Mestre dos Tesouros" (coroa dourada vitalícia) | "Caçador de Lendas" (vitalício) |
| XP | 10.000 + 50% do XP do descobridor | XP_base × 5 (máx 15.000) |
| Moeda dourada | 5% do valor declarado | 200 |
| Hall da Fama | Entrada permanente | Entrada permanente |
| Perfil | Contador "Tesouros Reais: 1" | Selo "Achado Lendário" |
| Prêmio real | — | O valor declarado (físico/digital) |
| Notoriedade | "Fulano escondeu X na praça!" | "Fulano achou o tesouro de X!" |

### 15.4 Regras

| # | Regra |
|---|---|
| V1 | Nível mínimo para criar: 10 |
| V2 | Nível mínimo para caçar: 5 |
| V3 | Valor mínimo declarado: R$ 20 |
| V4 | Valor máximo declarado: R$ 10.000 |
| V5 | Máx 1 tesouro real ativo por criador |
| V6 | Reputação positiva obrigatória (0 denúncias) |
| V7 | Prazo de confirmação do criador: 72h |
| V8 | Fake comprovado → ban permanente |
| V9 | Tesouro real só pode ser criado ONLINE (segurança) |
| V10 | Denúncia de tesouro real revisada em < 1h |
| V11 | Comissão do app: 0% sobre valor real (só taxa fixa do baú) |
| V12 | Criador pode contratar "seguro" (R$ 4,99) contra falso achado |

---

## 16. SISTEMA OFFLINE

### 16.1 Estados de conectividade

```
 ┌────────────┐
 │  ONLINE    │ ← conectado à internet (padrão)
 └─────┬──────┘
       │ [perdeu sinal]
       ▼
 ┌────────────┐
 │  OFFLINE   │ ← sem internet, mas GPS + P2P funcionam
 │            │    Cache local ativo
 └─────┬──────┘
       │ [voltou sinal]
       ▼
 ┌────────────┐
 │ SINCRON.   │ ← enviando dados pendentes ao servidor
 │            │    Fila de pendências sendo processada
 └─────┬──────┘
       │ [fila vazia]
       ▼
 ┌────────────┐
 │  ONLINE    │ ← de volta ao normal
 └────────────┘
```

### 16.2 Fila de sincronização local

```
 Pendências salvas no dispositivo (SQLite criptografado):

 TESOUROS_PENDENTES (criados offline):
 ┌──────────┬────────────┬──────────┬──────────┐
 │ id_local │ dados JSON │ status   │ tentativas│
 ├──────────┼────────────┼──────────┼──────────┤
 │ abc123   │ {lat, lng, │ pending  │ 0        │
 │          │  tipo, ...}│          │          │
 └──────────┴────────────┴──────────┴──────────┘

 COLETAS_PENDENTES (achados offline):
 ┌──────────┬────────────┬──────────┬──────────┐
 │ id_local │ dados JSON │ status   │ tentativas│
 ├──────────┼────────────┼──────────┼──────────┤
 │ def456   │ {tesouro_id,│ pending  │ 0        │
 │          │  proof...} │          │          │
 └──────────┴────────────┴──────────┴──────────┘

 FEEDBACKS_PENDENTES (respostas sentimentais offline):
 ┌──────────┬────────────┬──────────┬──────────┐
 │ id_local │ dados JSON │ status   │ tentativas│
 ├──────────┼────────────┼──────────┼──────────┤
 │ ghi789   │ {texto,    │ pending  │ 0        │
 │          │  audio...} │          │          │
 └──────────┴────────────┴──────────┴──────────┘
```

### 16.3 Cache local (dados baixados para uso offline)

| Cache | O quê | Tamanho | Renovação |
|---|---|---|---|
| Mapa 3D | Tile OSM low-poly num raio de 2km | ~15 MB | 7 dias |
| Tesouros | Tesouros ativos + correntes na região | ~50 KB | 60 min (se online) |
| Perfil | Nick, nível, XP, moedas, badges | ~5 KB | Ao logar |
| Inventário | Itens comprados (skins, passes) | ~10 KB | Ao logar |
| Config | Preferências do jogador | ~1 KB | Ao logar |
| Assets | Modelos 3D, texturas, sons | ~50 MB | Na instalação |

### 16.4 Regras do modo offline

| # | Regra | Justificativa |
|---|---|---|
| O1 | Cache de tesouros expira em **7 dias** sem renovação | Dados ficam obsoletos |
| O2 | Tesouro criado offline (PENDENTE) precisa sincronizar em **72h** ou é descartado | Evita tesouro fantasma |
| O3 | Coleta offline (PENDENTE) precisa sincronizar em **7 dias** ou XP é perdido | Limite de validade |
| O4 | P2P offline (WiFi Direct) alcance máximo: **100m** | Limitação física |
| O5 | Feedback sentimental offline: salva local, sincroniza quando voltar | Prioridade baixa |
| O6 | Tesouro REAL só pode ser criado ONLINE | Segurança |
| O7 | App mostra indicador 🛜 "Offline — sincronizando quando houver sinal" | Transparência |
| O8 | Fila de sincronização é criptografada localmente (SQLite + AES-256) | Segurança de dados |
| O9 | Máx 10 tesouros PENDENTES na fila offline | Evitar abuso |
| O10 | Tentativas de sincronização: 5 (com backoff exponencial: 30s, 1min, 2min, 5min, 15min) | Economia de bateria |

### 16.5 O que funciona em cada estado

| Operação | ONLINE | OFFLINE | SINCRONIZANDO |
|---|---|---|---|
| Ver mapa | ✅ | ✅ (cache) | ✅ (cache) |
| Detector | ✅ | ✅ | ✅ |
| Mini-game | ✅ | ✅ | ✅ |
| Enterrar virtual | ✅ | ✅ (PENDENTE) | ✅ (enfileirado) |
| Enterrar real | ✅ | ❌ | ❌ |
| Enterrar sentimental | ✅ | ✅ (PENDENTE) | ✅ (enfileirado) |
| Caçar virtual | ✅ | ✅ (PENDENTE) | ✅ (enfileirado) |
| Caçar real | ✅ | ❌ | ❌ |
| Feedback sentimental | ✅ | ✅ (PENDENTE) | ✅ (enfileirado) |
| Loja / comprar | ✅ | ❌ (mostra "sem sinal") | ❌ |
| Ver leaderboard | ✅ | ❌ (mostra último cache) | ✅ (atualiza) |
| Sincronizar perfil | ✅ | ❌ | ✅ |
| P2P com peers | ✅ | ✅ (WiFi Direct/BT) | ✅ |

---

*Documento gerado em 11/06/2026.*
