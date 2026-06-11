# EHAQUI / ISHERE — Caça ao Tesouro no Mundo Real

**pt-BR:** EHAQUI — É aqui! O primeiro jogo de caça ao tesouro P2P.  
**EN:** ISHERE — It's here! The first P2P treasure hunt game.

[![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)]()
[![Plataforma](https://img.shields.io/badge/plataforma-Android-brightgreen)]()
[![Engine](https://img.shields.io/badge/engine-Unity%20URP%2FGodot-blue)]()
[![Rede](https://img.shields.io/badge/rede-P2P%20%2B%20H%C3%ADbrido-purple)]()
[![i18n](https://img.shields.io/badge/i18n-pt--BR%20%7C%20EN-orange)]()

Jogo mobile onde crianças exploram o mundo real escondendo e encontrando tesouros usando o celular como **detector por vibração**. Tudo conectado via P2P — sem custo de servidor.

| 🇧🇷 EHAQUI | 🇺🇸 ISHERE |
|---|---|
| "É aqui!" — público Brasil | "It's here!" — público internacional |
| Documentação, UI, loja em pt-BR | App Store (en), documentação técnica |
| `i18n/pt-BR.json` | `i18n/en.json` |

---

## Sobre

### O problema

Jogos GPS existentes (Geocaching, Munzee) exigem servidor caro, não têm foco infantil e não funcionam offline.

### A solução

- **P2P puro** para troca de tesouros em tempo real — custo zero de servidor
- **Detector por vibração progressiva** — jogável sem olhar pra tela
- **Prêmio crescente** — tesouros valem mais quanto mais tempo ficam perdidos
- **Corrente de pistas** — transforma o jogo em escape room ao ar livre
- **Notificação por passagem** — avisa quando a criança passa perto de um tesouro
- **Modo offline completo** — funciona em parques, sítios, acampamentos
- **Tesouro sentimental** — feedback emocional entre desconhecidos
- **Plugins de estúdio** — Disney, Beto Carrero podem vender temas

### Público

Crianças 6–14 anos, famílias, criadores de conteúdo.

---

## Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                         ARQUITETURA                          │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐    │
│  │               SERVIDOR (Cloud Run)                   │    │
│  │  Auth  │  Compra  │  B2B  │  Push  │  Leaderboard   │    │
│  │  ~US$ 30/mês                                        │    │
│  └─────────────────────────────────────────────────────┘    │
│                          │                                    │
│                          ▼                                    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   APP CLIENTE                        │    │
│  │  Unity / Godot  │  Mapa  │  Detector  │  Mini-game   │    │
│  └──────────────────┬──────────────────────────────────┘    │
│                     │                                        │
│                     ▼                                        │
│  ┌─────────────────────────────────────────────────────┐    │
│  │               REDE P2P (LiteNetLib)                  │    │
│  │  WiFi Direct  │  Bluetooth  │  LAN  │  Cache local   │    │
│  │  Troca tesouros  │  Validação  │  Chat  │  Offline   │    │
│  │  CUSTO ZERO                                          │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

**Híbrido:** P2P para o jogo (grátis), servidor só para autenticação e compras (enxuto).

---

## Stack

| Camada | Tecnologia |
|---|---|
| **Engine** | Unity (URP) ou Godot 4 |
| **P2P** | LiteNetLib (Unity) / ENet (Godot) |
| **Servidor** | Cloud Run (GCP) ou Fly.io |
| **Banco** | Supabase (PostgreSQL) |
| **Autenticação** | Firebase Auth ou Supabase Auth |
| **Push** | Firebase Cloud Messaging |
| **Storage** | Cloudflare R2 |
| **GPS** | Android FusedLocationProvider + Geofencing |
| **Pagamento** | Google Play Billing + App Store IAP |
| **Mapa** | OpenStreetMap + Mapbox Unity SDK |
| **Criptografia** | ECDSA (secp256k1) + keccak256 |
| **Blockchain (opcional)** | Polygon L2 + Web3Auth |

---

## Estrutura

```
ehaqui/
├── .github/workflows/     # CI/CD
├── docs/                   # Documentação do jogo
├── i18n/                   # Internacionalização
│   ├── pt-BR.json          # 🇧🇷 EHAQUI
│   └── en.json             # 🇺🇸 ISHERE
├── src/Ehaqui/             # Código do jogo (engine TBD)
├── scripts/                # Scripts auxiliares
├── assets/brand/           # Logos, cores, assets de marca
├── README.md
├── TODO.md
├── ROADMAP.md
├── CHANGELOG.md
├── CONTRIBUTING.md
├── LICENSE
└── .gitignore
```

---

## i18n

O projeto usa **JSON flat aninhado** para internacionalização, consumível por qualquer engine.

| Arquivo | App | Locale |
|---|---|---|
| `i18n/pt-BR.json` | **EHAQUI** | `pt-BR` |
| `i18n/en.json` | **ISHERE** | `en` |

Para adicionar um idioma:
1. Copie `i18n/en.json` → `i18n/{locale}.json`
2. Traduza todos os valores (mantendo as chaves)
3. O CI valida se as chaves são idênticas entre idiomas

---

## Documentação

| Arquivo | Conteúdo |
|---|---|
| `docs/UNICORN_CACA_AO_TESOURO.md` | Documento mestre — visão, monetização, roadmap |
| `docs/GDD_CACADAO_TESOURO.md` | Game Design Document completo |
| `docs/FIOS_LOGICOS.md` | Máquinas de estado, regras, economia |
| `docs/ARQUITETURA_HIBRIDA.md` | Stack, API, banco, P2P Contract |
| `docs/MELHORES_PRATICAS.md` | Concorrentes, onboarding, retenção |
| `docs/PESQUISA_CONCORRENCIA.md` | 10 concorrentes analisados |

---

## Licença

MIT © 2026 EHAQUI / ISHERE

---

*Feito com 💜 no Brasil — brincadeira de criança ainda é o melhor jogo do mundo.*
