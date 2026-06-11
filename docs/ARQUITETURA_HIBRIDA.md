# 🏗️ Arquitetura Híbrida: Servidor Central + P2P

---

## Filosofia

> **P2P** resolve o jogo (barato, escalável, offline)
> **Servidor** resolve o dinheiro (confiável, seguro, auditável)

O servidor é **enxuto**: não precisa rodar partidas, não precisa sincronizar posições, não precisa rotear mensagens. Só faz o que o P2P **não pode fazer** com segurança.

---

## 1. DIVISÃO DE RESPONSABILIDADES

### 🟢 P2P (LiteNetLib / WiFi Direct / Bluetooth)

| Responsabilidade | Por que P2P? |
|---|---|
| Troca de tesouros ativos entre peers | Baixa latência, zero custo de servidor |
| Validação de descoberta (hash + timestamp) | Confiança entre pares locais |
| Chat / emotes entre jogadores próximos | Tempo real, sem mediação |
| Cache de tesouros de peers offline | Disponibilidade local |
| Broadcast de HELLO / PING / keepalive | Descoberta de rede |

### 🔵 Servidor Central (leve, Cloud Run / AWS Lambda / Fly.io)

| Responsabilidade | Por que servidor? |
|---|---|
| **Autenticação** (login, conta, nick) | Segurança — não confie no P2P para identidade |
| **Compra in-app** (validação Google Play / App Store) | Apple/Google exigem servidor para receipt |
| **Entrega de itens comprados** (skins, passes, moedas) | Persistência — não pode ficar no device |
| **Assinatura B2B** (lojas parceiras) | Gestão de pagamento recorrente |
| **Marketplace de pacotes** (estúdios) | Catálogo, entrega, comissão 70/30 |
| **Leaderboard global** (opcional) | Dados consolidados |
| **Analytics** (retenção, DAU, conversão) | Métricas de negócio |
| **Sincronia multi-device** | Mesma conta em vários celulares |
| **Denúncia / moderação** | Anti-cheate global, ban de conta |
| **Push notification** (via FCM/APNs) | Notificação por passagem |

---

## 2. FLUXO HÍBRIDO — DESCOBERTA DE TESOURO

```
                    ┌──────────────────┐
                    │   SERVIDOR       │
                    │  (Cloud Run)     │
                    └────────┬─────────┘
                             │ [login] [compra] [itens]
                             │ [push] [leaderboard]
                             │
                    ┌────────▼─────────┐
                    │    APP CLIENTE    │
                    │  (Unity / Godot)  │
                    └────────┬─────────┘
                             │
              ┌──────────────┴──────────────┐
              │                              │
     ┌────────▼────────┐           ┌─────────▼────────┐
     │  PEER A         │◄─P2P──►   │  PEER B           │
     │  enterra baú    │           │  descobre baú     │
     │  na praça       │           │  valida localmente│
     └─────────────────┘           └─────────┬─────────┘
                                              │
                                              │ (coleta confirmada)
                                              ▼
                                     ┌─────────────────┐
                                     │   SERVIDOR       │
                                     │  registra coleta │
                                     │  credita XP/moed.│
                                     │  notifica criador│
                                     └─────────────────┘
```

### Passo a passo:

1. **Peer A** enterra tesouro → salva localmente + broadcast P2P aos peers próximos
2. **Servidor** recebe via API: "tesouro criado" (só metadados: criador_id, lat/lng hash, TTL, raridade)
3. **Peer B** entra na área → recebe P2P dos peers próximos a lista de tesouros
4. **Peer B** caminha, detector vibra, chega a < 5m → mini-game de desenterrar
5. **Peer B** valida P2P com Peer A (hash do mini-game + GPS)
6. **Peer B** envia confirmação ao **servidor**: "tesouro X coletado, proof: hash"
7. **Servidor** valida proof, credita XP/moedas na conta de B, notifica A via push
8. **Servidor** marca tesouro como coletado (não aparece mais)

---

## 3. FLUXO HÍBRIDO — MONETIZAÇÃO

```
 ┌───────────────────┐
 │   GOOGLE PLAY     │
 │   Billing API     │
 └────────┬──────────┘
          │ [compra: receipt token]
          ▼
 ┌───────────────────┐
 │   SERVIDOR        │
 │   valida receipt  │
 │   (Google/Apple)  │
 └────────┬──────────┘
          │ [receipt válido]
          ▼
 ┌───────────────────┐
 │   SERVIDOR        │
 │   credita item na │
 │   conta do jogador│
 │   skin / passe /  │
 │   moeda dourada   │
 └────────┬──────────┘
          │ [push sync]
          ▼
 ┌───────────────────┐
 │   APP CLIENTE     │
 │   item liberado   │
 │   broadcast P2P:  │
 │   "skin ativa"    │
 │   (cosmético)     │
 └───────────────────┘
```

**Importante:** itens comprados ficam **no servidor** (conta). O app baixa o inventário ao logar. Cosméticos são aplicados localmente e broadcast P2P para peers (ex: "olha minha skin lendária").

---

## 4. CUSTO DO SERVIDOR (ESTIMATIVA)

### Cenário: 100k MAU, 500k requisições/dia

| Recurso | Serviço | Custo/mês |
|---|---|---|
| API REST | Cloud Run (2 instâncias, 256MB) | ~US$ 15 |
| Banco de dados | Firestore / Neon (10GB) | ~US$ 10 |
| Push notification | FCM / APNs | Grátis |
| Autenticação | Firebase Auth / Clerk | Grátis (10k usuários) |
| Storage (assets) | Cloud Storage / R2 (50GB) | ~US$ 5 |
| Total | | **~US$ 30/mês** |

Comparado a servidor de jogo tradicional (que precisa manter conexão TCP por partida): **10x–50x mais barato**.

---

## 5. BANCO DE DADOS (MODELO SIMPLIFICADO)

```sql
-- Usuários
usuarios (
  id UUID PK,
  nick VARCHAR(20),
  email VARCHAR(255),
  auth_provider VARCHAR(20), -- google, apple, email
  created_at TIMESTAMP,
  nivel INT DEFAULT 1,
  xp_total BIGINT DEFAULT 0,
  moeda_azul INT DEFAULT 0,
  moeda_dourada INT DEFAULT 0,
  reputacao INT DEFAULT 0
)

-- Itens comprados (cosméticos, passes)
itens_usuarios (
  id UUID PK,
  usuario_id UUID FK,
  item_id VARCHAR(50), -- skin_baú_ouro, detector_premium, passe_cacador
  tipo VARCHAR(20), -- skin, passe, ferramenta
  adquirido_em TIMESTAMP,
  expira_em TIMESTAMP NULL, -- NULL = permanente
  origem VARCHAR(20) -- compra, evento, conquista
)

-- Tesouros (apenas metadados, validação é P2P)
tesouros (
  id UUID PK,
  criador_id UUID FK,
  tipo VARCHAR(10), -- virtual, fisico
  lat DOUBLE,
  lng DOUBLE,
  hash_coord VARCHAR(64), -- HMAC para validação
  raridade VARCHAR(10), -- comum, raro, epico, lendario
  ttl_horas INT NULL, -- NULL = sem TTL
  criado_em TIMESTAMP,
  coletado_em TIMESTAMP NULL,
  coletor_id UUID FK NULL,
  status VARCHAR(10) -- ativo, coletado, expirado
)

-- Compras (receipt validation)
compras (
  id UUID PK,
  usuario_id UUID FK,
  produto_id VARCHAR(50),
  receipt_token TEXT,
  plataforma VARCHAR(10), -- google, apple
  status VARCHAR(10), -- pendente, confirmado, reembolsado
  created_at TIMESTAMP
)

-- Lojas parceiras (B2B)
lojas_parceiras (
  id UUID PK,
  nome VARCHAR(100),
  cnpj VARCHAR(18),
  plano VARCHAR(20), -- basico, profissional, evento
  assinatura_ativa BOOLEAN,
  qr_code_hash VARCHAR(64) UNIQUE,
  premio_descricao TEXT,
  next_billing DATE
)

-- Correntes de pistas
correntes (
  id UUID PK,
  criador_id UUID FK,
  titulo VARCHAR(100),
  num_pistas INT,
  ttl_horas INT NULL,
  status VARCHAR(10), -- rascunho, ativa, concluida, expirada
  premio_final_descricao TEXT,
  criado_em TIMESTAMP
)

-- Estatísticas para leaderboard
estatisticas_diarias (
  usuario_id UUID FK,
  data DATE,
  tesouros_achados INT DEFAULT 0,
  tesouros_criados INT DEFAULT 0,
  pistas_completadas INT DEFAULT 0,
  xp_ganho INT DEFAULT 0,
  PRIMARY KEY (usuario_id, data)
)
```

---

## 6. API DO SERVIDOR (ENDPOINTS)

### Autenticação
| Método | Rota | Descrição |
|---|---|---|
| POST | `/auth/register` | Criar conta |
| POST | `/auth/login` | Login (JWT) |
| POST | `/auth/anonymous` | Login anônimo (migrável depois) |

### Jogador
| Método | Rota | Descrição |
|---|---|---|
| GET | `/player/profile` | Perfil (nick, nivel, XP, moedas) |
| GET | `/player/inventory` | Itens comprados |
| PUT | `/player/nick` | Alterar nick |

### Tesouros
| Método | Rota | Descrição |
|---|---|---|
| POST | `/treasures` | Registrar novo tesouro (criador envia metadados) |
| POST | `/treasures/:id/claim` | Reivindicar coleta (enviar proof P2P) |
| GET | `/treasures/recent?lat=X&lng=Y&radius=Z` | Tesouros ativos próximos (fallback se P2P indisponível) |

### Compras
| Método | Rota | Descrição |
|---|---|---|
| POST | `/purchases/verify` | Validar receipt Google/Apple |
| GET | `/store/catalog` | Catálogo de itens disponíveis |
| POST | `/store/consume` | Consumir item (ex: pá, detector turbo) |

### Lojas Parceiras (B2B)
| Método | Rota | Descrição |
|---|---|---|
| POST | `/partners/register` | Cadastrar loja |
| POST | `/partners/qr/:id/validate` | Validar QR escaneado |
| GET | `/partners/dashboard` | Estatísticas da loja |

### Pacotes de Estúdio
| Método | Rota | Descrição |
|---|---|---|
| GET | `/packs/list` | Listar pacotes disponíveis |
| POST | `/packs/buy` | Comprar pacote |
| GET | `/packs/:id/assets` | Baixar assets do pacote |

### Admin
| Método | Rota | Descrição |
|---|---|---|
| GET | `/admin/analytics` | DAU, conversão, receita |
| POST | `/admin/ban` | Banir jogador |
| PUT | `/admin/packs` | Publicar/remover pacote |

---

## 7. POR QUE NÃO FAZER TUDO P2P?

| Funcionalidade | Só P2P | Problema |
|---|---|---|
| Autenticação | ❌ | Qualquer um pode forjar identidade |
| Compra in-app | ❌ | Receipt validation **exige** servidor (Apple/Google) |
| Inventário | ⚠️ | Perde se trocar de celular |
| Leaderboard global | ❌ | Sem servidor não tem dado central |
| Ban de jogador | ❌ | Peer malicioso volta sempre |
| Push notification | ❌ | FCM/APNs exigem servidor |
| Assinatura B2B | ❌ | Loja precisa de recibo fiscal |

### O que o servidor NÃO faz (e por isso é barato):

- ❌ Não mantém conexão TCP com jogadores em tempo real
- ❌ Não processa física de jogo
- ❌ Não valida posição GPS em tempo real (quem valida é o P2P)
- ❌ Não roteia mensagens entre jogadores
- ❌ Não hospeda partidas

---

## 8. FALLBACK: SE O P2P FALHAR

| Situação | Comportamento |
|---|---|
| Sem WiFi Direct | Fallback para Bluetooth |
| Sem Bluetooth | Fallback para servidor (API `/treasures/recent`) |
| Sem internet (offline total) | Cache local de tesouros da última vez online + detector funciona com GPS |
| Servidor offline | Jogo funciona normalmente (P2P puro); só não compra nem sincroniza |
| Peer não confiável | Marca como suspeito, usa fallback do servidor para validar |

---

## 9. STACK SUGERIDA

| Camada | Tecnologia | Custo |
|---|---|---|
| **Servidor** | Cloud Run (GCP) ou Fly.io | ~US$ 15/mês |
| **Banco** | Supabase (PostgreSQL) ou Neon | Grátis–US$ 10/mês |
| **Auth** | Firebase Auth ou Supabase Auth | Grátis até 50k users |
| **Push** | Firebase Cloud Messaging | Grátis |
| **Storage assets** | Cloudflare R2 ou AWS S3 | ~US$ 0,01/GB |
| **P2P transporte** | LiteNetLib (Unity) / ENet (Godot) | Grátis |
| **GPS / background** | Android FusedLocationProvider + Geofencing | Grátis |
| **Pagamento** | Google Play Billing + App Store IAP | 15–30% comissão |

**Custo total mensal (100k MAU): ~US$ 25–40/mês**

---

## RESUMO

```
┌──────────────────────────────────────────────────────────┐
│                    ARQUITETURA HÍBRIDA                    │
│                                                          │
│  P2P = jogo em tempo real (grátis, offline, escalável)   │
│  └─ descoberta de peers                                  │
│  └─ troca de tesouros                                    │
│  └─ validação local (hash + GPS)                         │
│  └─ chat / emotes                                        │
│                                                          │
│  SERVIDOR = dinheiro e persistência (enxuto, ~US$ 30/m)  │
│  └─ autenticação                                         │
│  └─ compras (receipt validation)                         │
│  └─ entrega de itens                                     │
│  └─ assinaturas B2B                                      │
│  └─ marketplace de pacotes                               │
│  └─ leaderboard / analytics                              │
│                                                          │
│  JOGO FUNCIONA SEM SERVIDOR (sem compras)                │
│  CUSTO DE SERVIDOR = 1/10 de um jogo tradicional         │
└──────────────────────────────────────────────────────────┘
```

---

## 10. CONTRATO DE TESOURO (CRIPTOGRÁFICO / BLOCKCHAIN)

### 10.1 O conceito

Cada tesouro enterrado carrega um **"contrato"** — um conjunto de regras criptografadas que determinam:

- **O quê:** recompensa (XP, moedas, badge, token real)
- **Onde:** localização (hash da coordenada + salt)
- **Quando:** TTL, data de ativação
- **Quem:** condições para o descobridor (nível mínimo, reputação)
- **Prova:** o que prova que o tesouro foi encontrado (hash do mini-game + GPS + timestamp + testemunhas P2P)

### 10.2 Dois modelos

| Modelo | Blockchain | P2P Contract (nosso) |
|---|---|---|
| **Onde roda** | Ethereum, Solana, Polygon | No próprio dispositivo + peers |
| **Custo por transação** | US$ 0,001–50 (ETH) / ~US$ 0,0001 (Solana) | **GRÁTIS** |
| **Velocidade** | Segundos a minutos | Instantâneo (P2P local) |
| **Requer internet** | Sim | Não (offline) |
| **Complexidade** | Alta (Solidity, wallet, gas) | Baixa (ECDSA + hash local) |
| **Público infantil** | ❌ (criança não tem wallet) | ✅ (transparente) |
| **Valor real (Bitcoin)** | ✅ (escrow on-chain) | ⚠️ (requer confiança) |

### 10.3 Nosso modelo: P2P Contract (recomendado)

Cada tesouro é um **contrato criptográfico** assinado pelo dispositivo do criador. A validação é feita pelos peers.

```
 ┌────────────────────────────────────────────────────────────┐
 │                P2P TREASURE CONTRACT                        │
 ├────────────────────────────────────────────────────────────┤
 │ {                                                          │
 │   "treasure_id": "a1b2c3d4",                              │
 │   "version": "1.0",                                        │
 │   "hider_pubkey": "0x3f8c...",                            │
 │   "location_hash": "keccak256(lat|lng|salt)",            │
 │   "salt": "abc123xyz",                                     │
 │   "rarity": "legendary",                                   │
 │   "ttl": 2592000,            // 30 dias em segundos        │
 │   "reward": {                                              │
 │     "type": "xp",                                          │
 │     "base_amount": 1000,                                   │
 │     "bonus_per_hour": 42                                   │
 │   },                                                       │
 │   "conditions": {                                          │
 │     "min_player_level": 1,                                 │
 │     "requires_witnesses": 2,   // peers que testemunham    │
 │     "max_distance_m": 5,       // raio de validação GPS    │
 │     "proof_type": ["gps", "minigame_hash", "witness_sig"]  │
 │   },                                                       │
 │   "hider_signature": "0x7a9b..."  // assinatura do criador │
 │ }                                                          │
 └────────────────────────────────────────────────────────────┘
```

### 10.4 Fluxo P2P Contract

```
 [CRIADOR]
   ├── gera par de chaves ECDSA (uma vez, no primeiro uso)
   ├── cria tesouro → monta JSON do contrato
   ├── assina com chave privada
   ├── broadcast P2P: "novo contrato: a1b2c3d4"
   └── peers validam: assinatura confere? → cache local

 [DESCOBRIDOR]
   ├── chega no local (GPS confere com location_hash + salt)
   ├── completa mini-game → gera hash_proof
   ├── coleta assinaturas de testemunhas (peers próximos)
   ├── monta "Claim": {
   │     treasure_id, hash_proof, witnesses: [sig1, sig2]
   │   }
   ├── assina com sua chave privada
   └── broadcast P2P: "claim: a1b2c3d4"

 [PEERS] validam o Claim:
   ├── GPS do descobridor está a < 5m do location_hash?
   ├── hash_proof do mini-game confere?
   ├── testemunhas são peers reais (não bots)?
   └── se SIM → broadcast "CONFIRM: a1b2c3d4"

 [CRIADOR] recebe confirmação
   ├── valida Claim
   └── broadcast "SETTLE: a1b2c3d4" → tesouro coletado

 CADEIA DE CONFIANÇA:
   hider_sig → claim_sig → witness_sig_x3 → settlement
   Tudo criptografado, verificável, sem servidor
```

### 10.5 O contrato serve como

| Função | Como o P2P Contract resolve |
|---|---|
| **Prova de criação** | Contrato assinado pelo criador (ECDSA) |
| **Prova de descoberta** | Claim assinado pelo descobridor + testemunhas |
| **Anti-replay** | Nonce único + timestamp |
| **Anti-spoof** | GPS + hash do mini-game + testemunhas locais |
| **Finalidade** | Settlement assinado por ambas as partes |
| **Histórico** | Cadeia de assinaturas mantida localmente |

### 10.6 E se quisermos blockchain de verdade (para tesouros de alto valor)?

Para tesouros **REAIS** (Bitcoin, R$ 500+), o P2P Contract não basta — precisa de **escrow on-chain** para garantir que o prêmio não suma.

#### Modelo Híbrido com Blockchain

```
 TESOURO REAL (R$ 500+)
   │
   ├── ❶ CRIADOR compra Baú Premium (R$ 49,90)
   ├── ❷ CRIADOR deposita o valor REAL no local físico
   ├── ❸ CRIADOR cria P2P Contract + CONTRATO ON-CHAIN
   │      ├── P2P Contract → coordena a descoberta
   │      └── Smart Contract → segura o "selo de verificação"
   │          (não o valor real, mas um NFT de garantia)
   │
   ├── ❹ DESCOBRIDOR acha → validação P2P
   ├── ❺ SERVIDOR confirma (GPS + foto + timestamp)
   ├── ❻ SMART CONTRACT emite:
   │      ├── NFT "Proof of Discovery" para o descobridor
   │      └── NFT "Legendary Hider" para o criador
   │
   └── ❼ Prêmio real é transferido fora da chain
         (combinado entre as partes, o app só coordena)
```

#### Por que NÃO colocar o valor real na chain?

| Razão | Explicação |
|---|---|
| Criança não tem wallet | 99% do público não tem MetaMask |
| Gas fee > prêmio pequeno | R$ 20 em ETH pode custar R$ 50 de gas |
| GPS não é oracle confiável | Chainlink não valida GPS |
| Complexidade destrói o produto | Nosso diferencial é SIMPLICIDADE |
| Fraude por spoofing | Smart contract não detecta GPS falso |

#### Quando valeria a pena usar blockchain?

| Cenário | Recomendação |
|---|---|
| Tesouro < R$ 100 | Só P2P Contract (grátis, instantâneo) |
| Tesouro R$ 100–R$ 1.000 | P2P + Servidor (validação central confiável) |
| Tesouro R$ 1.000+ | P2P + Servidor + NFT on-chain (Polygon) |
| Parceria com marca (ex: Disney) | NFT colecionável na Polygon |

### 10.7 Stack recomendada para contratos

| Componente | Tecnologia | Custo |
|---|---|---|
| Chave criptográfica | ECDSA (secp256k1) — mesma curva do Bitcoin | Grátis |
| Hash de localização | keccak256 (mesmo do Ethereum) | Grátis |
| Assinatura de contrato | `sign(keccak256(contrato_json), privkey)` | Grátis |
| Verificação P2P | `ecrecover(hash, signature) == pubkey` | Grátis |
| Blockchain (opcional) | **Polygon** (L2, gas ~US$ 0,001) | ~US$ 0,001/tx |
| Wallet (opcional) | Web3Auth (social login → wallet) | Grátis até 10k users |
| NFT (opcional) | ERC-721 na Polygon + IPFS pra metadados | ~US$ 0,01/mint |

### 10.8 Conclusão

| Modelo | Pra quê | Custo |
|---|---|---|
| **P2P Contract** | 99% dos tesouros (virtuais, sentimentais, correntes) | **GRÁTIS** |
| **Servidor + P2P** | Tesouros reais até R$ 1.000 (validação central) | ~US$ 30/mês |
| **Blockchain (Polygon)** | Tesouros REAIS R$ 1.000+ / NFTs de marca / Parcerias | ~US$ 0,001/tx |

O **P2P Contract** é o verdadeiro diferencial: nenhum concorrente tem um protocolo criptográfico de tesouro que funciona 100% offline e sem custo. É o nosso "Ethereum das brincadeiras de criança".

---

*Documento gerado em 11/06/2026.*
