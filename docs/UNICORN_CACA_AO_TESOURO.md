# 🦄 Caça ao Tesouro P2P
## O Unicórnio da Caça ao Tesouro

> **Um jogo, duas revoluções:**
> *P2P puro = zero custo de servidor*
> *Mundo real + virtual = engajamento infinito*

---

## 📋 ÍNDICE

1. [A Visão](#1-a-vis%C3%A3o)
2. [O Problema](#2-o-problema)
3. [A Solução](#3-a-solu%C3%A7%C3%A3o)
4. [Como Funciona](#4-como-funciona)
5. [Mapa vs Concorrência](#5-mapa-vs-concorr%C3%AAncia)
6. [Públicos](#6-p%C3%BAblicos)
7. [Monetização](#7-monetiza%C3%A7%C3%A3o)
8. [Arquitetura](#8-arquitetura)
9. [Mecânicas do Jogo](#9-mec%C3%A2nicas-do-jogo)
10. [Fios Lógicos](#10-fios-l%C3%B3gicos)
11. [Melhores Práticas](#11-melhores-pr%C3%A1ticas)
12. [Roadmap](#12-roadmap)
13. [Por que Vamos Vencer](#13-por-que-vamos-vencer)

---

## 1. A VISÃO

**O primeiro jogo de caça ao tesouro que funciona sem servidor.**

Onde qualquer criança do mundo pode:
- Esconder um tesouro virtual em qualquer lugar
- Encontrar tesouros de outros jogadores usando o celular como detector
- Seguir correntes de pistas com charadas pelo bairro
- Receber notificação quando passar perto de um tesouro sem nem estar no jogo
- Ganhar prêmios reais em lojas parceiras

Tudo isso com:
- **Custo de servidor: ~US$ 30/mês** (contra US$ 5k+/mês de um jogo tradicional)
- **Zero servidor de jogo** — o P2P faz o trabalho pesado
- **Conteúdo infinito** — gerado pelos próprios jogadores

---

## 2. O PROBLEMA

### 2.1 Para a criança
- Jogos GPS são sérios demais (Geocaching: "logbook", "coordenadas UTM")
- Nenhum jogo transforma a volta da escola numa aventura
- Nenhum jogo avisa "você está passando perto de um tesouro!"
- Tela cheia de informação, pouca diversão tátil

### 2.2 Para o estúdio indie
- Servidor de jogo custa US$ 2k–10k/mês para 100k usuários
- Manter mundo aberto exige infraestrutura pesada
- Conteúdo novo depende da equipe de desenvolvimento
- Monetização agressiva afasta o público infantil

### 2.3 Para a loja local
- Não existe plataforma simples pra atrair criança até a loja
- Marketing digital é caro e não alcança a vizinhança
- QR code de promoção não tem gamificação

### 2.4 Para os estúdios grandes
- Não tem como "entrar" num jogo GPS existente com tema próprio
- Criar um jogo dedicado custa milhões
- Disney, Beto Carrero, Shopping centers não têm presença em jogos GPS

---

## 3. A SOLUÇÃO

```
 ┌─────────────────────────────────────────────────────────────┐
 │                    CAÇA AO TESOURO P2P                       │
 │                                                             │
 │   🎮 Jogo: mundo real é o mapa                              │
 │   📡 Rede: P2P (WiFi Direct + Bluetooth)                    │
 │   🖥 Servidor: só autenticação + compras                    │
 │   💰 Custo: ~US$ 30/mês                                     │
 │   👶 Público: crianças 6–14 anos                            │
 │   🏪 B2B: lojas locais pagam R$ 29/mês                      │
 │   🎨 Plugins: Disney, Beto Carrero vendem temas             │
 │                                                             │
 │   "O Pokémon GO das brincadeiras de criança"                │
 └─────────────────────────────────────────────────────────────┘
```

---

## 4. COMO FUNCIONA

### 4.1 Ciclo do jogador

```
 ABRIU O APP → vê mapa 3D do bairro com baús brilhando
     ↓
 ESCOLHEU UM BAÚ → detector liga, vibração aumenta
     ↓
 CAMINHOU → vibração forte, "CAVE AQUI!"
     ↓
 MINI-GAME → desenterrou → XP + moedas
     ↓
 CRIOU TESOURO → enterrou perto da escola pra outro achar
     ↓
 NOTIFICAÇÃO → "Algo brilha perto de você!" (background)
     ↓
 VOLTA AMANHÃ → streak diário + prêmio crescente
```

### 4.2 Tipos de tesouro

```
 ┌────────────────────────────────────────────────────────────────┐
 │                     TESOURO VIRTUAL                            │
 │  Quem cria: qualquer jogador                                   │
 │  Onde: GPS qualquer lugar                                      │
 │  Duração: TTL opcional (1h a 30 dias)                          │
 │  Prêmio: XP, moedas, skins                                     │
 │  Bônus: prêmio CRESCE quanto mais tempo fica perdido           │
 │  Descoberta: detector por vibração + mini-game                 │
 └────────────────────────────────────────────────────────────────┘

 ┌────────────────────────────────────────────────────────────────┐
 │                     TESOURO FÍSICO (LOJA)                      │
 │  Quem cria: loja parceira (assinante B2B)                     │
 │  Onde: QR code fixo na loja                                   │
 │  Duração: enquanto a assinatura estiver ativa                 │
 │  Prêmio: brinde real (desconto, bala, pão na chapa)           │
 │  Descoberta: escaneia QR + valida GPS + loja confirma         │
 └────────────────────────────────────────────────────────────────┘

 ┌────────────────────────────────────────────────────────────────┐
 │                     CORRENTE DE PISTAS                         │
 │  Quem cria: qualquer jogador (premium: + pistas)              │
 │  Estrutura: pista 1 → charada → pista 2 → charada → prêmio    │
 │  Tipos: charada, enigma visual, matemática, desafio físico    │
 │  Limite: 3–10 pistas                                          │
 │  Distância mínima entre pistas: 20m                           │
 └────────────────────────────────────────────────────────────────┘
```

---

## 5. MAPA VS CONCORRÊNCIA

| Característica | Geocaching | Munzee | Actionbound | Scavify | Treasure Hunters | **NOSSO JOGO** |
|---|---|---|---|---|---|---|
| **P2P (zero servidor)** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Detector por vibração** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Prêmio crescente (TTL)** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Corrente de pistas** | ❌ | ❌ | parcial | ❌ | ❌ | **✅** |
| **Notificação por passagem** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Tesouro virtual + físico** | ✅ | ✅ | ✅ | ❌ | ❌ | **✅** |
| **Loja B2B parceira** | ❌ | ❌ | ✅ (caro) | ✅ (caro) | ❌ | **✅ (R$ 29/mês)** |
| **Plugins de estúdio** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Economia criador/descobridor** | ❌ | ❌ | ❌ | ❌ | parcial | **✅** |
| **Foco infantil** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Funciona sem servidor** | ❌ | ❌ | ❌ | ❌ | ❌ | **✅** |
| **Custo servidor/mês** | US$ 50k+ | US$ 20k+ | US$ 5k+ | US$ 5k+ | US$ 10k+ | **~US$ 30** |

> **Nossa vantagem:** não apenas features únicas — é um paradigma diferente. P2P permite o que nenhum concorrente consegue: custo de servidor irrelevante.

---

## 6. PÚBLICOS

### 6.1 Jogador (B2C)
- Crianças 6–14 anos
- Pais que querem filhos ativos (ao ar livre)
- Jovens que brincam na rua, vão pra escola a pé

### 6.2 Loja local (B2B)
- Padaria, sorveteria, papelaria, mercado
- Precisa atrair famílias pra loja física
- Não quer pagar caro (R$ 29/mês é acessível)

### 6.3 Estúdio / Marca (B2B Premium)
- Disney, Beto Carrero, Shopping centers, parques temáticos
- Quer presença em jogo GPS sem desenvolver o próprio
- Compra pacotes de tema: R$ 4.990–19.990

### 6.4 Criador de conteúdo
- YouTubers, TikTokers, streamers mirins
- Código criador: ganha comissão de seguidores
- Evento exclusivo: "Caçada do Criador"

---

## 7. MONETIZAÇÃO

### 7.1 Receitas

| Fonte | Produto | Preço | Margem |
|---|---|---|---|
| **Passe do Caçador** | Mensal: baús lendários, pás ilimitadas | R$ 19,90/mês | 90% |
| **Cosméticos** | Skin de detector, baú, pá, avatar | R$ 4,99–9,99 | 95% |
| **Consumíveis** | Pá especial, detector turbo, mapa do tesouro | R$ 1,99–4,99 | 95% |
| **Anúncio recompensado** | Detector turbo 5 min | Grátis (CMP ~R$ 0,02/views) | — |
| **B2B loja** | Assinatura mensal loja parceira | R$ 29,90–99/mês | 97% |
| **B2B estúdio** | Pacote de tema/corrente/mapa | R$ 4.990–19.990 | 85% |
| **Marketplace 70/30** | Comissão sobre pacotes de terceiros | 30% | 100% |

### 7.2 Projeção financeira (ano 2)

| Cenário | Conservador | Moderado | Otimista |
|---|---|---|---|
| MAU (Monthly Active Users) | 50k | 200k | 500k |
| Passe do Caçador (2% conversão) | 1k × R$ 19,90 | 4k × R$ 19,90 | 10k × R$ 19,90 |
| Receita passe | R$ 19.900 | R$ 79.600 | R$ 199.000 |
| Cosméticos (5% conversão, R$ 7/ticket) | R$ 17.500 | R$ 70.000 | R$ 175.000 |
| Anúncios (R$ 2 CPM, 10 sessões/mês) | R$ 1.000 | R$ 4.000 | R$ 10.000 |
| B2B lojas (50 lojas) | R$ 1.495 | R$ 2.990 | R$ 4.485 |
| B2B estúdio (2 pacotes/mês) | R$ 9.980 | R$ 24.980 | R$ 39.980 |
| **Receita total mensal** | **~R$ 50k** | **~R$ 181k** | **~R$ 428k** |
| **Custo servidor** | ~R$ 150 | ~R$ 300 | ~R$ 600 |
| **Margem operacional** | **99,7%** | **99,8%** | **99,9%** |

---

## 8. ARQUITETURA

### 8.1 Híbrida (P2P + Servidor Enxuto)

```
 ┌─────────────────────────────────────────────────────────────────────┐
 │                           ARQUITETURA                                │
 │                                                                     │
 │  ┌─────────────────────────────────────────────────────────────┐    │
 │  │                     SERVIDOR (Cloud Run)                     │    │
 │  │  ~US$ 30/mês                                                 │    │
 │  │                                                              │    │
 │  │  Autenticação  │  Compra (receipt)  │  B2B  │  Catalogos     │    │
 │  │  Leaderboard   │  Analytics         │  Push │  Denúncias     │    │
 │  └─────────────────────────────────────────────────────────────┘    │
 │                               │                                      │
 │                               ▼                                      │
 │  ┌─────────────────────────────────────────────────────────────┐    │
 │  │                       APP CLIENTE                            │    │
 │  │  Unity / Godot                                               │    │
 │  │  Mapa 3D  │  Detector  │  Mini-game  │  Loja  │  Perfil     │    │
 │  └───────────┬─────────────────────────────────────────────────┘    │
 │              │                                                      │
 │              ▼                                                      │
 │  ┌─────────────────────────────────────────────────────────────┐    │
 │  │              REDE P2P (LiteNetLib / ENet)                    │    │
 │  │  WiFi Direct → Bluetooth → LAN (fallback)                   │    │
 │  │                                                              │    │
 │  │  Troca tesouros  │  Validação P2P  │  Chat  │  Cache        │    │
 │  │  (custo ZERO)    │  (anti-cheat)   │        │  (offline)     │    │
 │  └─────────────────────────────────────────────────────────────┘    │
 └─────────────────────────────────────────────────────────────────────┘
 ```

 ### 8.2 Stack

 | Camada | Tecnologia | Custo |
 |---|---|---|
 | Servidor | Cloud Run (GCP) / Fly.io | ~US$ 15/mês |
 | Banco | Supabase (PostgreSQL) | Grátis–US$ 10/mês |
 | Autenticação | Firebase Auth / Supabase Auth | Grátis até 50k usuários |
 | Push | Firebase Cloud Messaging | Grátis |
 | Storage assets | Cloudflare R2 | ~US$ 0,01/GB |
 | P2P | LiteNetLib (Unity) / ENet (Godot) | Grátis |
 | GPS | FusedLocationProvider + Geofencing | Grátis |
 | Pagamento | Google Play Billing + App Store IAP | 15–30% comissão |
 | Engine | Unity (URP) ou Godot | Grátis até US$ 200k receita |

 **Custo total estimado (100k MAU): ~US$ 30–40/mês**

---

## 9. MECÂNICAS DO JOGO

### 9.1 Detector por vibração progressiva

```
 Distância     Vibração          Som             Tela
 ──────────────────────────────────────────────────────────
 200m+         ❌ silêncio        ❌               Mapa normal
 50–200m       Fraca (2s ciclo)   ❌               Borda pulsa
 20–50m        Média (1s ciclo)   Tom baixo       Seta direcional
 5–20m         Forte (0.3s ciclo) Tom médio       Radar pulsante
 0–5m          Contínua           Alerta sonoro    "CAVE AQUI!"
```

Jogável **sem olhar para a tela**. Criança anda com celular no bolso e sente quando está perto.

### 9.2 Prêmio crescente

```
 Fórmula: XP_total = XP_base × (1 + horas_sem_achar / 24 × raridade)

 Exemplo (tesouro lendário):
   Base:       1.000 XP
   24h depois: 3.000 XP
   7 dias:    15.000 XP
```

Quanto mais tempo perdido, maior a recompensa. Motiva jogadores a revisitar áreas.

### 9.3 Notificação por passagem (background)

- GPS em background (ciclo 60s)
- Detecta aproximação < 50m de um tesouro
- Envia push: "✨ Algo brilha perto de você!"
- Círculo de aproximação no mapa (50m → encolhe até revelar)
- Cooldown: 30 min por local

### 9.4 Corrente de pistas

```
 [Criador]
   ├── Pista 1: "Onde o leão dorme?" → Parque
   ├── Pista 2: "Lugar gelado onde a língua gruda" → Sorveteria
   ├── Pista 3: "Planta que toca o céu" → Árvore grande
   └── FINAL: QR code na árvore → prêmio

 [Jogador]
   Só vê a pista 1 no mapa
   Resolve → pista 2 aparece
   Resolve → pista 3 aparece
   Resolve → FINAL → prêmio
```

### 9.5 Anti-cheat P2P

- Hash do mini-game + GPS + timestamp
- Validação cruzada entre peers
- Distância mínima entre enterro e coleta: 20m
- Velocidade máxima para detectar: 30 km/h
- Reputação do peer (3 rejeições = não confiável)

---

## 10. FIOS LÓGICOS (RESUMO)

### 10.1 Máquina de estado do jogador

```
 OFFLINE (menu)
    ↓ [entrar no mapa]
 LIVRE
    ├── [escolheu tesouro] → NAVEGANDO (rota)
    ├── [ativou detector]  → CAÇANDO (detector + vibração)
    │                         └── [achou] → VALIDANDO (mini-game)
    │                                          └── COLETADO
    ├── [criar tesouro]    → ENTERRANDO (escolhe local + TTL)
    └── [criar corrente]   → CRIANDO CORRENTE (editor)
```

### 10.2 Ciclo de vida do tesouro virtual

```
 ENTERRAR (criador define local + TTL + raridade)
    ↓
 SALVO (no dispositivo do criador, broadcast P2P)
    ↓
 ATIVO (visível para peers na área, prêmio começa a crescer)
    ↓
 ┌──── ACHADO (jogador descobre, valida P2P)
 │        ↓
 │    COLETADO (XP + moedas, criador ganha 50%)
 │
 └──── EXPIRADO (TTL estourou)
         ↓
     REVERTIDO (criador recebe bônus)
```

### 10.3 Economia

| Moeda Azul (grátis, farmável) | Moeda Dourada (premium, comprável) |
|---|---|
| Farm: 50 por tesouro, 10 por pista | Compra: R$ 4,99–89,90 |
| Uso: pá comum, skin básica, criar corrente (5 pistas) | Uso: pá especial, skin rara, detector premium, passe |
| Aposta em tesouro (10–500) | Tema de estúdio |

### 10.4 Regras de negócio (principais)

| # | Regra |
|---|---|
| R1 | Distância mínima entre tesouros: 10m |
| R2 | TTL máximo: 30 dias |
| R3 | Velocidade máxima para detectar: 30 km/h |
| R4 | Máx 10 tesouros ativos por jogador |
| R5 | Peer sem HELLO em 120s é removido |
| R6 | Tesouro físico (loja) não conta no limite |

---

## 11. MELHORES PRÁTICAS (RESUMO)

### 11.1 Onboarding

- Primeira ação em **< 30s** sem cadastro
- Mascote guia (pirata / cão farejador)
- Pedir permissão de localização **depois** de mostrar valor
- 3 baús virtuais próximos na primeira tela

### 11.2 Retenção

- Daily Stroll: ande 500 passos → baú bônus
- Notificação por passagem (único no mercado)
- Prêmio crescente (FOMO natural)
- Selos de bairro e região
- Evento surpresa semanal

### 11.3 UI/UX

- Mapa é a tela principal (nunca bloquear)
- Detector ocupa 1/3 inferior
- 3 toques máx para qualquer ação
- Fonte 16sp+, ícones > texto
- Feedback tátil em todas as ações

### 11.4 Segurança

- Geofencing automático (evita áreas perigosas)
- Limite de 10 tesouros por jogador
- Distância mínima de 10m entre tesouros
- Denúncia P2P com reputação
- Modo treino em casa (segurança + onboarding)

---

## 12. ROADMAP

```
 FASE 1 — PROVA DE CONCEITO (4 semanas)
 ├── Protótipo P2P (LiteNetLib + WiFi Direct)
 ├── GPS + detector por vibração
 └── Enterrar e achar tesouro virtual (2 jogadores)

 FASE 2 — MVP FUNCIONAL (8 semanas)
 ├── Mapa 3D low-poly (OSM)
 ├── Mini-game de desenterrar
 ├── TTL + prêmio crescente
 ├── Servidor enxuto (auth + sync)
 └── Playtest com 10 pessoas

 FASE 3 — CORRENTES + NOTIFICAÇÃO (4 semanas)
 ├── Editor de correntes de pistas
 ├── Validação de pistas (GPS + charadas)
 ├── Notificação por passagem (background)
 └── Anti-cheat P2P

 FASE 4 — MONETIZAÇÃO (4 semanas)
 ├── Loja (cosméticos + passes)
 ├── Google Play Billing + App Store IAP
 ├── Anúncio recompensado
 └── Passe do Caçador

 FASE 5 — B2B + PLUGINS (8 semanas)
 ├── Portal de lojas parceiras
 ├── QR code físico com validação
 ├── Sistema de .themepack / .questpack
 ├── Marketplace de pacotes
 └── 3 lojas parceiras piloto

 FASE 6 — LANÇAMENTO BETA (4 semanas)
 ├── Região piloto (1 cidade)
 ├── Campanha TikTok/YouTube
 ├── 5 criadores de conteúdo onboard
 └── 1 parque temático parceiro

 FASE 7 — EXPANSÃO (contínuo)
 ├── Traduções (EN, ES)
 ├── Android + iOS
 ├── Otimização P2P para grandes áreas
 └── Eventos sazonais
```

---

## 13. POR QUE VAMOS VENCER

### 13.1 Vantagens estruturais

| Vantagem | Explicação | Barreira para concorrentes |
|---|---|---|
| **P2P puro** | Custo de servidor = US$ 30/mês. Concorrentes gastam US$ 20k+ | Migrar de servidor central pra P2P exige reescrever o jogo inteiro |
| **Detector por vibração** | Jogável sem olhar a tela. Nenhum concorrente tem | Mecânica proprietária de interação |
| **Prêmio crescente** | Engajamento orgânico baseado em tempo | Fácil de copiar, mas amarrado ao P2P que eles não têm |
| **Corrente de pistas** | Transforma o jogo em escape room ao ar livre | Concorrentes têm "pontos de interesse", não têm narrativa encadeada |
| **Plugins de estúdio** | Disney, Beto Carrero viram parceiros pagantes | Exige arquitetura de pacotes que nenhum concorrente tem |
| **Custo zero de conteúdo** | Jogadores criam tesouros — não precisamos de equipe de level design | Concorrentes (Actionbound, Scavify) dependem de criadores pagos |

### 13.2 Oceano azul

```
              CONCORRENTES EXISTENTES
         ┌──────────────────────────────┐
         │     SERVER-BASED             │
         │   (custo alto, pesado)       │
         │                              │
         │  Geocaching  Munzee           │
         │  Actionbound Scavify          │
         │  TurfHunt   Treasure Hunters  │
         │  GooseChase  Cosmotrove       │
         └──────────────────────────────┘

              NOSSO ESPAÇO
         ┌──────────────────────────────┐
         │        P2P + HÍBRIDO         │
         │   (custo zero, leve, novo)   │
         │                              │
         │  ★ Detector por vibração     │
         │  ★ Prêmio crescente          │
         │  ★ Corrente de pistas        │
         │  ★ Notificação por passagem  │
         │  ★ Plugins de estúdio        │
         │  ★ Foco infantil             │
         └──────────────────────────────┘
```

### 13.3 O que torna único

1. **P2P reduz custo de servidor em 99%** vs qualquer concorrente
2. **Detector vibratório** cria experiência tátil que nenhum GPS game tem
3. **Prêmio crescente** transforma abandono em ativo — tesouro velho vale mais
4. **Notificação por passagem** resolve o maior problema de jogos GPS: "esqueço de abrir"
5. **Plugin de estúdio** abre mercado B2B que não existe hoje
6. **R$ 29/mês pra loja local** é 10x mais barato que qualquer alternativa B2B

### 13.4 Citação

> *"O Pokémon GO das brincadeiras de criança. Mas sem servidor, sem custo, e com a nostalgia de esconder e achar tesouros que toda criança sente."*

---

## 📁 ARQUIVOS DO PROJETO

| Arquivo | Conteúdo |
|---|---|
| `GDD_CACADAO_TESOURO.md` | Game Design Document completo |
| `FIOS_LOGICOS.md` | Máquinas de estado, regras, sistemas, economia |
| `ARQUITETURA_HIBRIDA.md` | Arquitetura P2P + servidor, APIs, banco de dados |
| `MELHORES_PRATICAS.md` | Análise de concorrentes, onboarding, retenção, UI/UX |
| `PESQUISA_CONCORRENCIA.md` | 10 concorrentes analisados, matriz comparativa |

---

## 14. NOVAS CAMADAS (PÓS-CONCEPÇÃO)

### 14.1 Tesouro Sentimental

- Qualquer jogador pode enterrar um **objeto físico de valor sentimental** (chupeta, bilhete, dente de leite, pulseira)
- Anexa história (texto), áudio (15s), foto ao tesouro
- Descobridor é **obrigado a deixar feedback**: texto, áudio, foto ou desenho
- Criador recebe notificação: "💌 Algo deixou uma mensagem pra você!"
- XP dobrado para ambas as partes
- Gera conexão emocional real entre desconhecidos — viralização orgânica

### 14.2 Tesouro de Alto Valor (Bitcoin / Real)

- Criador compra "Baú Lendário Premium" (R$ 49,90) e deposita algo de valor real no local
- Requer nível mínimo 10 e reputação positiva
- Descobridor ganha o **prêmio real** (Bitcoin, dinheiro, vale-presente)
- Criador ganha badge "Mestre dos Tesouros" (vitalício), 10.000 XP, 5% do valor em moeda dourada, entrada no Hall da Fama
- Descobridor ganha badge "Caçador de Lendas", 200 moedas douradas, entrada no Hall da Fama
- App cobra 0% sobre valor real — só a taxa fixa do baú

### 14.3 Modo Offline Completo

- Enterrar tesouro: funciona offline (salva como "PENDENTE", sincroniza quando voltar)
- Caçar tesouro: funciona offline (GPS + P2P WiFi Direct sem internet)
- Feedback sentimental: funciona offline (sincroniza depois)
- Cache local do mapa num raio de 2km (~15 MB)
- Fila de sincronização criptografada (AES-256) com backoff exponencial
- Tesouro REAL só pode ser criado online (segurança)
- App mostra indicador 🛜 "Offline" quando sem sinal

---

## 📁 ARQUIVOS DO PROJETO

| Arquivo | Conteúdo |
|---|---|
| `GDD_CACADAO_TESOURO.md` | Game Design Document completo |
| `FIOS_LOGICOS.md` | Máquinas de estado, regras, sistemas, economia |
| `ARQUITETURA_HIBRIDA.md` | Arquitetura P2P + servidor, APIs, banco de dados |
| `MELHORES_PRATICAS.md` | Análise de concorrentes, onboarding, retenção, UI/UX |
| `PESQUISA_CONCORRENCIA.md` | 10 concorrentes analisados, matriz comparativa |

---

*Documento mestre gerado em 11/06/2026.*
*Por um time que acredita que brincadeira de criança ainda é o melhor jogo do mundo.*
