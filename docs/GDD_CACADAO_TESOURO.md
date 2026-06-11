# 🗺️ Caça ao Tesouro: Mundo Aberto P2P
## Game Design Document v1.0

---

## 1. VISÃO GERAL

**Título provisório:** Trilha das Pistas / Rastros / Corrente do Tesouro
**Plataforma:** Android (iOS futuramente)
**Engine:** Unity (URP) ou Godot
**Rede:** P2P puro (LiteNetLib / ENet + WiFi Direct + Bluetooth)
**Custo de servidor:** Zero
**Público-alvo:** Crianças 6–14 anos, famílias, criadores de conteúdo

### Proposta única

Jogo de caça ao tesouro no mundo real onde celulares se conectam diretamente (P2P), sem servidor central. Tesouros são escondidos em locais físicos via GPS. Qualquer jogador pode enterrar tesouros virtuais; lojas parceiras podem ancorar tesouros físicos com brinde real. O prêmio dos tesouros aumenta conforme ficam mais tempo sem ser encontrados.

---

## 2. PILARES DO JOGO

1. **Exploração real** — a criança se movimenta pelo mundo físico
2. **P2P puro** — zero custo de infraestrutura
3. **Conteúdo gerado pelo usuário** — todo jogador cria tesouros
4. **Monetização ética** — só cosméticos, conveniência e parcerias locais
5. **Mundo aberto** — qualquer lugar do planeta é jogável

---

## 3. TIPOS DE TESOURO

| Característica | Virtual | Físico |
|---|---|---|
| **Quem cria** | Qualquer jogador | Loja parceira |
| **Onde** | GPS qualquer | Na loja (QR fixo) |
| **Duração** | TTL opcional (definido pelo criador) | Permanente (enquanto a promo durar) |
| **Prêmio** | XP, moedas, skins | Brinde real (desconto, item grátis) |
| **Descoberta** | Detector GPS + vibração | QR code + validação na loja |
| **Prêmio crescente** | Sim — +XP por tempo sem achar | Não (fixo) |

### 3.1 Prêmio crescente (virtuais)

- A cada hora sem ser encontrado, o tesouro acumula **bônus de XP**
- Após 24h: dobra o XP base
- Após 72h: triplica + moedas bônus
- Após 7 dias: lendário — maior premiação possível
- O criador vê no perfil: "Seu tesouro na praça está perdido há 3 dias — já rendeu +150%"

---

## 4. CORRENTE DE PISTAS

Tesouros podem ser individuais ou encadeados em **correntes** (sequência de pistas).

### 4.1 Estrutura de uma corrente

```
[Início] → QR/pista 1 → charada → [Local 2]
[Local 2] → QR/pista 2 → charada → [Local 3]
[Local 3] → QR/pista 3 → ENIGMA FINAL → prêmio
```

### 4.2 Tipos de pista

| Tipo | Exemplo | Dificuldade |
|---|---|---|
| Charada | "Quanto mais tiro, maior fico" (buraco) | Fácil |
| Enigma visual | QR mostra metade de um desenho | Médio |
| Matemática | "7 + 8 - 3" → rua de número 12 | Fácil |
| Desafio físico | "10 passos para o norte" | Médio |
| Observação | "O que está escrito na placa vermelha?" | Difícil |
| Foto-match | Tire foto do mesmo ângulo que o criador | Difícil |

### 4.3 Anti-cheat (P2P)

- GPS registra timestamp de cada pista
- Distância mínima entre pistas: 20m
- Mini-game por etapa gera hash local verificado entre peers
- Tentativa de pulo → corrente trava, criador notificado

---

## 5. NOTIFICAÇÃO POR PASSAGEM (BACKGROUND)

- Detecta aproximação por GPS em background
- Notificação push local com círculo: "Algo brilha nesta área 🌐"
- Círculo inicial: 20–50m de raio (não revela o ponto exato)
- Conforme o jogador se move, o círculo encolhe
- Posição exata só é revelada ao abrir o app + ativar detector
- Transforma qualquer caminhada cotidiana numa mini-aventura

---

## 6. ARQUITETURA P2P

### 6.1 Descoberta de peers

- WiFi Direct (`WifiP2pManager`) — principal
- Bluetooth clássico — fallback
- LAN (mesmo WiFi) — alternativa

### 6.2 Fluxo de comunicação

```
[Peer A] esconde baú na praça
    ↓ broadcast P2P
[Peer B] entra na área
    ↓ pergunta peers locais
[Peer A] responde: "baú ativo em (lat,lng criptografado)"
    ↓
[Peer B] caminha até o local, vibração aumenta
    ↓
[Peer B] acha o baú, envia confirmação P2P ao Peer A
    ↓
[Peer A] marca como achado, baú desaparece
```

### 6.3 Stack sugerida

- Unity + LiteNetLib (transporte P2P UDP)
- GPS nativo (Android `FusedLocationProviderClient`)
- AR Foundation (modo AR opcional)
- OpenStreetMap + geração procedural low-poly

---

## 7. MAPA E VISUAL

- **Base:** OpenStreetMap convertido em terreno 3D low-poly
- **Estilo visual:** vibrante, colorido (tipo Zelda: Breath of the Wild / Fall Guys)
- **Modo AR (opcional):** baús flutuam no mundo real com partículas
- **Clima dinâmico:** se chove de verdade (API de clima), chove no jogo
- **Baús lendários:** condições especiais (noite, lua cheia, garoa)

---

## 8. PROGRESSÃO

- **XP por tesouro achado** → sobe de nível → desbloqueia baús melhores
- **Selos de região:** ache 5 tesouros no mesmo bairro → insígnia
- **Correntes:** quem acha um tesouro seu entra na sua "corrente de descobertas"
- **Ranking:** semanal de descobertas, correntes completadas, XP acumulado

---

## 9. MONETIZAÇÃO

### 9.1 Jogador final

| Item | Preço |
|---|---|
| Detector Premium (dourado + alcance +10%) | R$ 9,99 |
| Pás especiais (eventos) | R$ 4,99 |
| Mapa do Tesouro (revela região de 1 lendário/dia) | R$ 1,99 ou anúncio |
| Passe do Caçador (mensal: baús lendários, pás ilimitadas) | R$ 19,90/mês |
| Anúncio recompensado (detector turbo 5 min) | Grátis (ver 30s) |
| Skin de baú (Halloween, Natal, pixel) | R$ 3,99 |
| Criar corrente premium (7 pistas + itens físicos) | R$ 4,99 |

### 9.2 Loja parceira (B2B)

| Plano | Preço | Benefícios |
|---|---|---|
| **Básico** | R$ 29,90/mês | 1 QR físico + 1 corrente/mês |
| **Profissional** | R$ 99/mês | QRs ilimitados + correntes + relatórios |
| **Evento** | R$ 299 | Campanha de 15 dias com destaque no mapa |

### 9.3 Estúdios parceiros (DLC / Pacotes)

| Pacote | Conteúdo | Preço ao estúdio |
|---|---|---|
| **Tema Visual** | Skins de mapa, baú, detector, UI | R$ 4.990 |
| **Trilha Sonora** | Músicas e SFX temáticos | R$ 2.990 |
| **Corrente Oficial** | Corrente de 5+ pistas, visível a todos | R$ 9.990 |
| **Personagem** | Skin de avatar + animações | R$ 3.990 |
| **Evento Temporário** | Ativa por 15–30 dias | R$ 14.990 |
| **Mapa Exclusivo** | Overlay 3D estilizado sobre OSM | R$ 19.990 |

**Comissão:** 70% estúdio / 30% plataforma

---

## 10. INCENTIVOS A CRIADORES

### 10.1 Para quem enterra tesouros (qualquer jogador)

- **XP por descoberta:** 50% do XP de quem achou
- **Bônus por TTL longo:** tesouros de 7 dias dão mais XP
- **Status "Mestre das Trilhas":** selo, coroa, destaque no mapa
- **Ranking semanal** de correntes mais resolvidas
- **Tesouro Bônus:** aposte moedas — se ninguém achar no TTL, você ganha o dobro

### 10.2 Para criadores de conteúdo (YouTubers, TikTokers, streamers)

- **Código Criador:** % das moedas premium gastas por seguidores
- **Corrente Patrocinada:** correntes mundiais com prêmio real pago pelo estúdio
- **Evento Exclusivo:** "Caçada do Criador" — 10 tesouros lendários
- **Mapa Revelado:** estatísticas de todos os tesouros ativos na região
- **Skin Personalizada:** pino/baú com a marca do criador
- **Comissão de Loja:** 10% da assinatura de lojas que ele indicar

---

## 11. ARQUITETURA DE PLUGINS (PACOTES DE CONTEÚDO)

```
[Core do Jogo]
  ├── Mapa base (OpenStreetMap low-poly)
  ├── Motor P2P (LiteNetLib)
  ├── Detector (GPS + vibração)
  └── Loja (compra dentro do app)

[Pacotes de Conteúdo - baixados sob demanda]
  ├── .themepack (JSON + assets)
  │   ├── theme.json (cores, fontes, ícones)
  │   ├── map_overlay.png (textura do mapa)
  │   ├── chest_models/ (baús temáticos)
  │   └── detector_skin/ (bússola, radar, luneta)
  ├── .soundpack (arquivos de áudio)
  ├── .questpack (JSON com correntes, pistas, NPCs)
  └── .avatar (boneco animado + emotes)
```

### Editor de Correntes (web)

- Drag & drop no mapa
- Define pistas, charadas, prêmio
- Gera `.questpack` pronto para publicação
- Preview: simula a corrente como um jogador veria

---

## 12. EXEMPLO DE USO REAL: PADARIA

1. Padaria assina **Básico** (R$ 29,90/mês)
2. Cola QR na vitrine → "O que acorda todo mundo de manhã?"
3. Criança responde "Galo" → próximo QR no galinheiro do parque ao lado
4. Última pista: volta pra padaria → "Pão quentinho"
5. Prêmio: **1 pão na chapa grátis** (custo: R$ 2,00)
6. Criança trouxe os pais → gastaram R$ 30 em café da manhã

**Resultado:** padaria paga R$ 2 de brinde, fatura R$ 30. O app não cobrou nada por descoberta.

---

## 13. EXEMPLO DE USO REAL: PARQUE TEMÁTICO (BETO CARREIRO)

1. Compra pacote **"Mundo Beto Carrero"** (R$ 9.990)
2. Mapa vira o parque, baús viram "ovos de dinossauro", detector vira "mapa do pirata"
3. Corrente Oficial: 5 pistas (Montanha Russa → Castelo → Ilha dos Piratas → ...)
4. Última pista → loja de souvenir → QR → 10% de desconto
5. Visitante que completa entra em sorteio de ingresso cortesia
6. Retorno: aumento de tempo no parque + idas à loja + viral TikTok

---

## 14. ANTI-CHEAT E SEGURANÇA

- GPS registra timestamp de cada interação
- Distância mínima entre pistas consecutivas: 20m
- Mini-game de validação gera hash local verificado entre peers
- Tentativa de burla → corrente/bloqueio notificado ao criador
- Tesouros físicos validados por timestamp + localização + QR único

---

## 15. PRÓXIMOS PASSOS (SUGESTÃO)

1. Validação técnica — protótipo P2P com LiteNetLib + GPS (1 semana)
2. MVP funcional — mapa OSM low-poly + detector + enterrar/achar virtual (3 semanas)
3. Playtest local — grupo de 10 pessoas na mesma região (1 semana)
4. Correntes de pistas — editor + validação (2 semanas)
5. Notificação por passagem — background GPS + push local (1 semana)
6. Loja + monetização — cosméticos, passes, assinaturas (2 semanas)
7. Parcerias B2B — plataforma de lojas + QR físico (3 semanas)
8. Pacotes de estúdio — sistema de `.themepack/.questpack` (4 semanas)
9. Lançamento beta — região piloto (1 cidade) (2 semanas)
10. Expansão global — traduções + otimização P2P (contínuo)

---

## 16. TESOURO SENTIMENTAL (MENSAGEM NA GARRAFA)

### Conceito

Nem todo tesouro tem valor material. Uma criança pode esconder algo de **valor sentimental** — uma chupeta, um dente de leite, um bilhete, uma pulseirinha, um desenho. O app permite marcar esse local e anexar uma **história** ao tesouro.

### Fluxo

```
 CRIADOR (criança)
   ├── esconde objeto físico real (ex: chupeta debaixo do banco da praça)
   ├── abre app → "Tesouro Sentimental"
   ├── escreve: "Esta foi minha chupeta favorita. Chamei ela de Rosinha."
   ├── opcional: grava áudio de 15s contando a história
   └── opcional: tira foto do objeto no local

 DESCOBRIDOR (outra criança)
   ├── acha o tesouro (detector vibra, mini-game)
   ├── lê a história / ouve o áudio / vê a foto
   └── pode **responder com feedback**:
        ├── 📝 texto: "Que legal! Vou cuidar bem dela!"
        ├── 🎤 áudio: grava mensagem de voz
        ├── 📸 foto: selfie segurando o objeto
        └── 🎨 desenho: rabisco na tela

 CRIADOR recebe notificação:
   "💌 Seu tesouro foi encontrado!
    Alguém deixou uma mensagem pra você!"
```

### Regras

| Regra | Valor |
|---|---|
| Tamanho máximo do texto da história | 500 caracteres |
| Áudio máximo | 15s |
| Foto opcional | 1 por tesouro |
| Feedback do descobridor | Obrigatório (mín 1 dos 4 tipos) |
| Prazo para o descobridor responder | 24h após achar |
| Notificação ao criador | Imediata ao receber feedback |
| Tesouro sentimental expira? | TTL opcional (padrão 7 dias) |
| XP para o descobridor | Dobrado (por ser sentimental) |
| XP para o criador | Dobrado quando receber feedback |

### Monetização

- **Papel de carta virtual** (R$ 1,99): template decorado para o feedback
- **Tema "Cápsula do Tempo"**: skins sazonais para tesouros sentimentais
- **Modo anônimo**: feedback sem revelar o nick (grátis 3x, depois R$ 0,99)

### Por que funciona

- Gera **conexão emocional** entre estranhos — raro em jogos
- Conteúdo viral: "achei a chupeta do João e ele me mandou um áudio agradecendo 😭"
- Pais se emocionam → compartilham → baixam o app

---

## 17. TESOURO DE ALTO VALOR (BITCOIN / ITENS REAIS)

### Conceito

Um jogador (ou marca) pode esconder algo de **valor real** — Bitcoin, vale-presente, ingresso, NFT, dinheiro físico. O app coordena a caça, mas o prêmio real está fora do app.

### Fluxo

```
 CRIADOR
   ├── compra "Baú Lendário Premium" no app (R$ 49,90)
   ├── define: o que está escondendo e onde
   │   (ex: "Papel com chave privada de 0.01 BTC dentro do livro X na biblioteca")
   ├── deposita o valor real no local físico
   ├── ou: registra o prêmio digital (caso o prêmio seja online)
   └── publica o tesouro como "LENDÁRIO — VALOR REAL"

 DESCOBRIDOR
   ├── vê no mapa: baú dourado pulsante com ⭐ "VALOR REAL"
   ├── segue detector até o local
   ├── mini-game + confirmação visual (tira foto do prêmio)
   ├── app valida: GPS + foto + timestamp
   └── ganha o prêmio REAL

 CRIADOR recebe:
   ├── NOTIFICAÇÃO: "🔥 Seu tesouro foi encontrado!"
   ├── XP ÉPICO: 10.000 XP
   ├── Badge vitalício: "MESTRE DOS TESOUROS — edição limitada"
   ├── Entrada no Hall da Fama do app
   ├── Selo "Provedor de Lendas" no perfil (coroa dourada)
   └── 5% do valor do tesouro em moeda dourada (se o criador marcou o valor em reais)
```

### Incentivos para ambas as partes

| | Quem enterrou (criador) | Quem achou (descobridor) |
|---|---|---|
| **Prêmio principal** | Satisfação de criar uma lenda | O BITCOIN / item real 🤑 |
| **XP** | 10.000 XP + bônus de 50% do XP do descobridor | XP máximo da raridade (até 15.000) |
| **Badge** | "Mestre dos Tesouros" (vitalício, único) | "Caçador de Lendas" (vitalício) |
| **Hall da Fama** | Entrada permanente com data + valor + local | Entrada permanente com data |
| **Moeda dourada** | 5% do valor declarado em moeda dourada | Bônus de 200 moedas douradas |
| **Perfil** | Coroa dourada + contador "Tesouros Reais: 1" | Selo "Achado Lendário" |
| **Notoriedade** | "Fulano escondeu 0.1 BTC na Praça da Matriz!" | "Fulano achou o tesouro de 0.1 BTC!" |

### Regras

| Regra | Valor |
|---|---|
| Custo para criar tesouro real | R$ 49,90 (Baú Lendário Premium) |
| Valor mínimo declarado | R$ 20,00 |
| Valor máximo declarado | R$ 10.000,00 |
| Comissão do app | 0% sobre o valor real (só a taxa fixa) |
| Validação de descoberta | GPS + foto do prêmio + confirmação manual do criador |
| Prazo para o criador confirmar | 72h após o achado |
| Se criador não confirmar | App assume que é verdadeiro, libera badge para descobridor |
| Tesouro não encontrado no TTL | Criador pode reenterrar grátis 1x |
| Fake (criador mente que escondeu algo) | Ban permanente + perda do valor do baú |

### Curadoria de segurança

- Criador precisa ter **nível mínimo 10** para criar tesouro real
- Criador precisa ter reputação positiva (sem denúncias)
- Máximo de 1 tesouro real ativo por vez
- Tesouro real aparece no mapa só para jogadores nível 5+
- Denúncia prioritária: qualquer denúncia de tesouro real é revisada em 1h

### Monetização adicional

| Item | Preço |
|---|---|
| Baú Lendário Premium (criar tesouro real) | R$ 49,90 |
| Seguro contra falso achado (criador recebe R$ 20 se descobridor mentir) | R$ 4,99 |
| Destaque no mapa por 24h (mais visibilidade) | R$ 9,90 |
| Certificado digital de "Prova de Tesouro Real" | R$ 2,99 |

### Por que funciona

- Transforma o jogo numa **plataforma de caça ao tesouro real**
- Atrai mídia: "Criança achou Bitcoin escondido na praça pelo app X"
- Marcas podem usar: "Escondemos 10 vales-presente na cidade!"
- Criador vira **celebridade local** — "o homem que escondeu 1 BTC"

---

## 18. MODO OFFLINE COMPLETO

### Filosofia

> **O mundo real não tem internet. O jogo também não precisa.**

Crianças brincam em parques, praças, sítios, acampamentos — lugares sem sinal. O app precisa funcionar **100% offline** para as operações essenciais.

### O que funciona offline

| Operação | GPS (nativo) | P2P (WiFi Direct / BT) | Funciona sem internet? |
|---|---|---|---|
| Ver mapa com tesouros em cache | ✅ (cache local) | — | **✅ SIM** |
| Detector vibratório | ✅ | — | **✅ SIM** |
| Mini-game de desenterrar | — | — | **✅ SIM** |
| Enterrar tesouro | ✅ | — | **✅ SIM** (pendente) |
| Validação P2P (achar tesouro) | ✅ | ✅ | **✅ SIM** |
| Trocar tesouros entre peers | — | ✅ | **✅ SIM** |
| Notificação por passagem | ✅ (geofence) | — | **✅ SIM** |
| Feedback sentimental | — | — | ✅ (salva local, sincroniza depois) |
| Loja / compra | — | — | ❌ (precisa de internet) |
| Leaderboard global | — | — | ❌ (precisa de internet) |

### Fluxo offline — Enterrar sem internet

```
 1. Criança está no parque (sem sinal)
 2. Abre app → "Enterrar tesouro"
 3. GPS captura coordenadas (funciona sem internet)
 4. Escolhe: virtual / sentimental / real
 5. Escreve história, tira foto, grava áudio (tudo local)
 6. Confirma → salvo como "PENDENTE" no dispositivo
 7. App guarda em fila local:
    {
      id: "local_abc123",
      lat: -23.5505, lng: -46.6333,
      tipo: "sentimental",
      historia: "Esta foi minha chupeta favorita...",
      foto: "chupeta.jpg",
      status: "pending",  // ← ainda não sincronizado
      criado_em: "2026-06-11T14:30:00Z"
    }
 8. Quando o celular pegar sinal:
    ├── App sincroniza automaticamente com servidor
    ├── Broadcast P2P para peers próximos
    └── Tesouro muda de "PENDENTE" para "ATIVO"
```

### Fluxo offline — Caçar sem internet

```
 1. Criança estava em casa (com internet)
    → App baixou cache de tesouros num raio de 2km
    → Guardou mapa + dados dos tesouros localmente

 2. Criança vai pro parque (sem sinal)
 3. Abre app → detector funciona com GPS (sem internet)
 4. Vibração aumenta conforme se aproxima
 5. Acha o tesouro → mini-game funciona (sem internet)
 6. Validação P2P com peer mais próximo (WiFi Direct)
 7. Salva como "COLETADO_PENDENTE" localmente
 8. Quando voltar a ter sinal:
    ├── Envia confirmação ao servidor
    ├── Credita XP/moedas
    └── Notifica criador
```

### Cache local

| Dado | Tamanho estimado | Renovação |
|---|---|---|
| Mapa 3D do raio de 2km | ~15 MB | A cada 7 dias ou quando entrar em nova área |
| Tesouros ativos na região | ~50 KB | A cada 60 minutos (se online) |
| Perfil do jogador | ~5 KB | Ao logar |
| Inventário de itens | ~10 KB | Ao logar |
| Histórico de feedbacks pendentes | ~1 KB | Até sincronizar |

### Regras do modo offline

| # | Regra |
|---|---|
| O1 | Cache de tesouros expira em 7 dias sem renovação |
| O2 | Tesouro "PENDENTE" (criado offline) precisa sincronizar em até 72h ou é descartado |
| O3 | Coleta "PENDENTE" (achado offline) precisa sincronizar em até 7 dias ou XP é perdido |
| O4 | P2P offline (WiFi Direct) alcance máximo: 100m entre peers |
| O5 | Feedback sentimental offline: salva local, sincroniza quando voltar |
| O6 | Tesouro real SÓ pode ser enterrado online (segurança) |
| O7 | App mostra indicador 🛜 "Offline — sincronizando quando houver sinal" |
| O8 | Fila de sincronização é criptografada localmente (SQLite + AES) |

---

## 19. POSSÍVEIS NOMES

- Trilha das Pistas
- Corrente do Tesouro
- Rastros
- Baús Perdidos
- Rota do Tesouro
- Pistas do Mundo
- Tesouro P2P

---

*Documento gerado em 11/06/2026.*
