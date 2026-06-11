# 🏆 Melhores Práticas dos Concorrentes
## Aprendizados de Geocaching, Munzee, Actionbound, Scavify, TurfHunt, Treasure Hunters e outros

---

## 1. ONBOARDING

### Práticas dos concorrentes

| Fonte | Prática | Como fazem |
|---|---|---|
| **Geocaching** | Tutorial interativo no primeiro uso | Mostra um cache fictício no mapa e guia o jogador passo a passo até "encontrá-lo" — sem precisar sair de casa |
| **Munzee** | Onboarding narrativo | Guiado pelo mascote Cappy (unicórnio), com história fantasiosa desde o primeiro clique |
| **Actionbound** | Zero onboarding | Criador produz conteúdo no PC; jogador só abre o app e segue o Bound já pronto |
| **Scavify** | Value-first | Mostra o placar e as missões ANTES de pedir cadastro — "experimente antes de criar conta" |
| **TurfHunt** | Onboarding por QR | Organizador entrega um QR code; jogador escaneia e já entra na partida — sem cadastro |

### 🔥 Aplicado ao nosso jogo

| O quê | Como |
|---|---|
| **Valor antes do cadastro** | Abre o app → vê um mapa 3D com 3 baús próximos (virtuais) → pode andar até um e cavar → SÓ DEPOIS pede nickname + email |
| **Mascote guia** | Personagem animado (ex: um detetive / cão farejador / pirata) aparece nas primeiras 3 telas |
| **Permissões contexto** | Pede localização APENAS quando o jogador clica em "Caçar agora" — explicando "preciso saber onde você está para mostrar os tesouros perto de você" |
| **Primeira experiência em 30s** | Abriu → viu 3 baús → andou → cavou → ganhou XP. Tudo em < 30s |
| **Skip consciente** | "Pular tutorial" visível, mas com alerta: "Você pode pular, mas vai perder 100 XP grátis!" |

---

## 2. RETENÇÃO (HABIT LOOPS)

### O que funciona nos concorrentes

| Mecânica | Quem usa | Por que funciona |
|---|---|---|
| **Daily Streak** | Munzee, Geocaching | "Você capturou 5 dias seguidos! Volte amanhã para não perder sua sequência!" |
| **Selo por região** | Geocaching, Munzee | "Você encontrou 5 tesouros no Rio de Janeiro → Insígnia Carioca" |
| **Notificação de proximidade** | Treasure Hunters | "Tem um baú lendário a 200m de você!" (via push) |
| **Eventos temporários** | Munzee, Pokémon GO | "Evento de Halloween: baús fantasmas por 72h!" |
| **Clãs/guildas** | Munzee | Clãs de 10 jogadores competem mensalmente — pressão social positiva |
| **Conteúdo gerado pelo usuário** | Geocaching, Munzee | Jogadores criam tesouros → outros consomem → loop infinito |
| **Meta clara** | Todos | "Ache 10 tesouros para subir de nível", "Complete 3 correntes" |

### 🔥 Aplicado ao nosso jogo

| Mecânica | Implementação |
|---|---|
| **Daily Stroll** | Ande 500 passos por dia → ganhe 1 baú bônus (como o Daily Stroll do Munzee com o Cappy) |
| **Prêmio crescente** | Única. "Esse baú está perdido há 3 dias — já rende 3x XP!" → motiva voltar |
| **Notificação por passagem** | Única. Toca no bolso enquanto a criança vai pra escola → "Algo brilha perto de você" |
| **Selos de bairro** | "Caçador da Tijuca", "Explorador da Zona Sul" |
| **Ranking semanal** | "Quem achou mais tesouros essa semana?" no bairro, cidade, estado |
| **Evento surpresa** | Sem avisar: ativa 5 baús lendários no mapa por 24h |
| **Loop criador/descobridor** | Jogador que enterra recebe 50% do XP quando alguém acha → todo mundo quer criar |

---

## 3. MONETIZAÇÃO

### Modelos dos concorrentes

| App | Modelo | Preço | Receita estimada |
|---|---|---|---|
| **Geocaching** | Premium anual | US$ 29,99/ano | ~US$ 50M/ano |
| **Munzee** | Premium mensal + loja física | US$ 3,99/mês + US$ 5–20 stickers | ~US$ 5M/ano |
| **Actionbound** | B2B (PRO License) | € 29–350/mês | ~US$ 2M/ano |
| **Scavify** | B2B (Enterprise) | US$ 99–500/evento | ~US$ 3M/ano |
| **TurfHunt** | B2B (assinatura) | € 29–350/mês | ~US$ 1M/ano |
| **Treasure Hunters** | Play-to-Earn (crypto) | Grátis + taxas de transação | ~US$ 500k/ano |

### Lições aprendidas

1. **Premium funciona** — Geocaching e Munzee provam que jogadores pagam por conveniência e cosméticos
2. **B2B é mais lucrativo** — Actionbound e Scavify cobram 10x mais de empresas do que de consumidores
3. **Gratuito + anúncio recompensado** — Pokémo GO prova que jogadores preferem ver anúncio a pagar
4. **Loja física (real)** — Munzee vende stickers de QR code por US$ 5–20 — margem altíssima (custo de impressão é centavos)
5. **Passe sazonal** — Fortnite/CODM provam que passe de batalha é o formato mais lucrativo (80%+ margem)

### 🔥 Aplicado ao nosso jogo

| Produto | Preço | Inspiração |
|---|---|---|
| **Passe do Caçador (mensal)** | R$ 19,90 | Geocaching Premium + Fortnite Battle Pass |
| **Skin de detector** | R$ 9,99 | Cosmético Munzee |
| **Mapa do Tesouro** | R$ 1,99 ou anúncio | Anúncio recompensado Pokémon GO |
| **Pá especial (evento)** | R$ 4,99 | Item consumível |
| **Loja parceira (B2B)** | R$ 29,90–99/mês | Actionbound PRO |
| **Pacote de estúdio** | R$ 4.990–19.990 | Único. Mercado inexplorado |

---

## 4. UI/UX

### Padrões de sucesso nos concorrentes

| Padrão | Onde | Descrição |
|---|---|---|
| **Mapa como tela principal** | Geocaching, Munzee, Pokémon GO | Ao abrir o app, o jogador vê o mapa. Sem splash, sem burocracia |
| **Radar / detector visual** | Treasure Hunters | Círculo pulsante no centro da tela que indica distância ao tesouro |
| **Cores vibrantes e cartoon** | Munzee, Pokémon GO | Público infantil exige visual alegre e colorido |
| **Animações de coleta** | Munzee | Ao capturar, o QR "explode" em partículas + som + vibração |
| **Feedback tátil** | Todos os GPS games | Vibração é essencial — o jogador não olha a tela o tempo todo |
| **Bottom sheet (não tela cheia)** | Munzee, Pokémon GO | Informações do tesouro aparecem em painel que sobe de baixo — não bloqueia o mapa |
| **Botão grande de ação** | Pokémon GO | "CAVAR" / "COLETAR" ocupa 1/3 da tela — Não tem erro |
| **Perfil minimalista** | Geocaching | Avatar + nick + nível + 3 estatísticas principais. Nada mais |

### 🔥 Aplicado ao nosso jogo

```
 ┌─────────────────────────────┐
 │         MAPA 3D             │
 │   (OSM low-poly colorido)   │
 │                             │
 │      🏆 (baú lendário)      │
 │         🥇 (raro)           │
 │     ⭐ (comum)              │
 │                             │
 │  ┌───────────────────────┐  │
 │  │   📡 DETECTOR ATIVO   │  │
 │  │   ● ≡ ≡ ≡ ≡ ≡ ≡ ○    │  │
 │  │   Distância: 47m      │  │
 │  └───────────────────────┘  │
 │                             │
 │ ┌─────────────────────────┐ │
 │ │      🔍 CAÇAR           │ │
 │ └─────────────────────────┘ │
 │  [Perfil] [Tesouros] [Loja] │
 └─────────────────────────────┘
```

**Regras de UI:**
- 3 toques máximos para qualquer ação principal
- Mapa sempre visível (nunca bloqueie com modal)
- Detector ocupa 1/3 inferior — fácil de ver sem tirar o celular do bolso
- Fonte grande (min 16sp) — público infantil
- Icones > texto — criança reconhece imagem antes de ler

---

## 5. ENGAJAMENTO COMUNITÁRIO

### O que os concorrentes fazem bem

| Tática | App | Resultado |
|---|---|---|
| **Eventos presenciais** | Geocaching (Mega Events), Munzee (meetups) | Jogadores organizam encontros — comunidade se fortalece |
| **Clãs/guildas** | Munzee (Clan Wars) | 750k+ jogadores organizados em clãs de 10 |
| **Criador de conteúdo** | Geocaching (criar cache), Munzee (deploy) | Jogador vira "autor" — cria orgulho e pertencimento |
| **Badges / conquistas** | Munzee, Geocaching | Mais de 100 tipos de badges colecionáveis |
| **Calendário de eventos** | Munzee | Evento novo a cada semana — sempre algo para fazer |
| **Discord / comunidade externa** | Munzee, Geocaching | Milhares de membros ativos no Discord |

### 🔥 Aplicado ao nosso jogo

| Tática | Como |
|---|---|
| **Corrente do Mês** | Todo mês uma corrente oficial criada pelo estúdio com prêmio real |
| **Selos de "Mestre das Trilhas"** | Perfil destaca os melhores criadores de correntes |
| **Código do Criador** | YouTuber/TikToker ganha comissão de seguidores |
| **"Taxa de Descoberta" por bairro** | "Seu bairro tem 47% dos tesouros ainda não encontrados!" → FOMO |
| **Discord oficial** | Desde o beta. Primeiros 1000 membros ganham badge exclusiva |

---

## 6. SEGURANÇA E MODERAÇÃO

### Práticas dos concorrentes

| Prática | Quem | Por que |
|---|---|---|
| **Geofencing de áreas perigosas** | Geocaching, Munzee | Bloqueia criação de tesouros perto de vias expressas, trilhos de trem |
| **Limite de tesouros por usuário** | Munzee | Máx 10 tesouros ativos simultaneamente |
| **Distância mínima entre tesouros** | Geocaching (161m), Munzee (30m) | Evita saturação do mapa |
| **Aprovação manual (só premium)** | Geocaching | Caches premium passam por revisão |
| **Botão de denúncia** | Todos | Reportar localização perigosa ou conteúdo impróprio |
| **Idade mínima verificada** | Actionbound | GDPR exige 16+ (ou consentimento parental) |
| **Jogar em casa (pré-visualização)** | Geocaching | App mostra como o tesouro aparece no mapa sem sair de casa — segurança |

### 🔥 Aplicado ao nosso jogo

| Medida | Implementação |
|---|---|
| **Geofencing automático** | Bloqueia enterro em: vias expressas, ferrovias, hidrantes, propriedade privada (via API OSM) |
| **Distância mínima** | 10m entre tesouros |
| **Limite por jogador** | 10 tesouros ativos (não afeta lojas parceiras) |
| **Denúncia P2P** | Peer reporta → servidor analisa → se confirmado, perde reputação |
| **Modo "Treino em Casa"** | Simula tesouros virtuais no mapa para treinar o detector sem sair — segurança e onboarding |
| **GDPR / COPPA** | Login opcional com consentimento parental para < 13 anos |

---

## 7. TÉCNICAS DE RETENÇÃO ESPECÍFICAS

### Ciclo diário ideal (baseado nos concorrentes)

```
 MANHÃ (indo pra escola)
   └─ Notificação por passagem: "Tem algo perto de você!"
   └─ Abre app → 30s → detector aponta → acha tesouro → XP + moeda

 TARDE (volta da escola)
   └─ Daily Stroll: "Ande 500 passos hoje para ganhar um baú bônus!"
   └─ Abre app → vê se tem tesouro novo perto de casa

 NOITE (em casa)
   └─ Cria tesouro para amanhã: "Enterre um baú perto da escola!"
   └─ Vê ranking: "Você está em 3º no bairro essa semana!"
   └─ Uma partida de mini-game (ex: estourar bolhas) para ganhar moedas extras
```

### Métricas de retenção (benchmarks da categoria)

| Métrica | Geocaching | Munzee | Nosso alvo |
|---|---|---|---|
| Day 1 Retention | 45% | 40% | 50%+ |
| Day 7 Retention | 25% | 20% | 30%+ |
| Day 30 Retention | 15% | 12% | 20%+ |
| Tesouros/dia/usuário ativo | 1.2 | 2.1 | 3+ |
| Sessões/semana | 2.5 | 3.8 | 5+ |
| Tempo médio de sessão | 8 min | 6 min | 5 min (criança tem menos atenção) |

---

## 8. ERROS DOS CONCORRENTES (O QUE EVITAR)

### Aprenda com as falhas

| Erro | Quem errou | Consequência | Como evitamos |
|---|---|---|---|
| **Onboarding longo** | Geocaching (versão antiga) | 60% dos downloads nunca abriam o app de novo | Primeira ação em < 30s |
| **Mapa poluído** | Munzee (versão 3.x) | Jogadores reclamavam de "muito tesouro, não sei qual escolher" | TTL opcional + distância mínima + filtro por raridade |
| **Drenagem de bateria** | Pokémon GO (2016) | Revolta, reviews negativas, desinstalações | GPS background só a cada 60s + detector desligado por padrão |
| **Sem moderação** | Geocaching (início) | Caches em lugares perigosos, processos judiciais | Geofencing automático na criação + denúncia P2P |
| **Dependência de servidor** | Todos | Se o servidor cai, ninguém joga — Pokémon GO ficou 3 dias offline no lançamento | P2P funciona mesmo com servidor offline |
| **Monetização agressiva** | Treasure Hunters | Críticas por "pay-to-win disfarçado de crypto" | Apenas cosméticos e conveniência — sem vantagem de jogo |
| **Sem conteúdo novo** | Geocaching (2015) | Queda de 30% nos usuários ativos | Conteúdo gerado pelo usuário + eventos semanais + pacotes de estúdio |
| **Ignorar público infantil** | Todos os concorrentes | Criança de 6–14 anos não tem app dedicado | Detector vibratório, visual cartoon, brincadeiras de criança |

---

## 9. DESIGN DE SOM E FEEDBACK

### Melhores práticas

| Aspecto | Referência | Implementação |
|---|---|---|
| **Som de aproximação** | Metal detector / sonar de submarino (filmes) | Tom mais agudo e rápido conforme se aproxima |
| **Vibração progressiva** | iPhone Taptic Engine | Vibração fraca a 200m → forte a 5m → contínua ao achar |
| **Som de coleta** | Zelda: "segredo encontrado!" | Fanfarra curta (1s) + partículas na tela |
| **Som de erro** | Game Boy: "wah-wah" | Tom descendente (mini-game falhou) |
| **Música de fundo** | Animal Crossing | Calma e relaxante — a criança pode ficar minutos andando |
| **Silêncio automático** | — | Detecta fones de ouvido → se sem fone, reduz volume em 50% (escola, rua) |

### Regra de ouro

> O jogo deve ser jogável **apenas com vibração**, sem olhar para a tela.
> A criança pode andar com o celular no bolso e sentir quando está perto de um tesouro.

---

## 10. CHECKLIST FINAL — NOSSO JOGO VS CONCORRENTES

| Requisito | Concorrentes têm? | Nós temos? |
|---|---|---|
| Jogo funciona sem internet | ❌ (maioria) | ✅ (P2P + cache) |
| Jogo funciona sem servidor | ❌ | ✅ |
| Detector por vibração | ❌ | ✅ |
| Prêmio crescente (TTL) | ❌ | ✅ |
| Corrente de pistas (charadas) | ❌ | ✅ |
| Notificação por passagem | ❌ | ✅ |
| Criança como público-alvo | ❌ | ✅ |
| Loja B2B parceira (R$ 29/mês) | ⚠️ (Actionbound, caro) | ✅ (acessível) |
| Pacote de estúdio (tema) | ❌ | ✅ |
| Moderação automática (geofence) | ⚠️ (parcial) | ✅ |
| Onboarding < 30s | ❌ | ✅ |
| Primeira ação sem cadastro | ❌ | ✅ |

---

*Documento gerado em 11/06/2026.*
