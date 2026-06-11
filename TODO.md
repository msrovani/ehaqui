# TODO — EHAQUI / ISHERE

> Lista de tarefas organizadas por fase e prioridade.
> Legendas: 🔴 Alta · 🟡 Média · 🟢 Baixa · ✅ Concluído · 🔄 Em andamento

---

## FASE 0 — FUNDAÇÃO (PRÉ-DESENVOLVIMENTO)

- [x] ✅ Game Design Document (GDD)
- [x] ✅ Fios lógicos (máquinas de estado, regras, economia)
- [x] ✅ Arquitetura híbrida (P2P + servidor)
- [x] ✅ Pesquisa de concorrência
- [x] ✅ Melhores práticas dos concorrentes
- [x] ✅ Documento mestre (Unicórnio)
- [x] ✅ Nome definido: EHAQUI (🇧🇷) / ISHERE (🇺🇸)
- [x] ✅ i18n base pt-BR + EN
- [x] ✅ Estrutura do repositório + .gitignore
- [x] ✅ CI/CD (GitHub Actions)
- [ ] 🔴 **Definir engine: Unity URP vs Godot 4**
- [ ] 🟡 **Definir naming conventions (C# / GDScript)**
- [ ] 🟢 **Criar logo EHAQUI + ISHERE**

---

## FASE 1 — PROVA DE CONCEITO (P2P + GPS)

### Prioridade 🔴 Crítica

- [ ] 🔴 **P2P básico entre 2 dispositivos**
  - [ ] Implementar LiteNetLib (Unity) ou ENet (Godot)
  - [ ] Handshake HELLO / PING / PONG
  - [ ] Troca de mensagens simples (texto)
  - [ ] Broadcast local (WiFi Direct)
- [ ] 🔴 **GPS básico**
  - [ ] Capturar coordenadas (FusedLocationProvider)
  - [ ] Exibir posição no mapa
  - [ ] Calcular distância entre dois pontos (Haversine)
- [ ] 🔴 **Detector por vibração v1**
  - [ ] Mapear distância → intensidade de vibração
  - [ ] Vibração progressiva (4 estágios)
  - [ ] Testar com 2 celulares lado a lado

### Prioridade 🟡 Média

- [ ] 🟡 **Enterrar tesouro (P2P)**
  - [ ] Selecionar local no mapa
  - [ ] Salvar localmente + broadcast P2P
  - [ ] Outro peer recebe e armazena em cache
- [ ] 🟡 **Achar tesouro (P2P)**
  - [ ] Detector aponta para tesouro do peer
  - [ ] Mini-game simples de desenterrar (swipe)
  - [ ] Validação: hash do mini-game + GPS
- [ ] 🟡 **Cache local**
  - [ ] SQLite com tesouros ativos
  - [ ] Sincronizar quando voltar online
- [ ] 🟢 **Logar distância e precisão do GPS em tela de debug**

### Milestone Fase 1 ✅

> Dois celulares: um enterra, outro acha. Tudo P2P, sem servidor.

---

## FASE 2 — MVP FUNCIONAL

### Prioridade 🔴 Crítica

- [ ] 🔴 **Mapa 3D low-poly (OpenStreetMap)**
  - [ ] Download de tiles OSM
  - [ ] Gerar terreno 3D simplificado
  - [ ] Posicionar jogador e tesouros no mapa
- [ ] 🔴 **UI principal**
  - [ ] Tela de mapa com detector embutido
  - [ ] Bottom sheet do tesouro selecionado
  - [ ] Botão "CAÇAR" / "ENTERRAR"
- [ ] 🔴 **Mini-game de desenterrar v1**
  - [ ] Swipe rápido para cavar
  - [ ] Animação de tesouro sendo desenterrado
  - [ ] Partículas + som de coleta

### Prioridade 🟡 Média

- [ ] 🟡 **TTL + prêmio crescente**
  - [ ] Timer local por tesouro
  - [ ] Cálculo de XP bonus = f(horas_sem_achar)
  - [ ] Exibir "prêmio atual" no detector
- [ ] 🟡 **Perfil do jogador**
  - [ ] Nick, nível, XP, moedas
  - [ ] Badges conquistadas
  - [ ] Histórico de tesouros criados/achados
- [ ] 🟡 **Servidor enxuto v1**
  - [ ] API de autenticação (Firebase Auth)
  - [ ] POST /treasures (registrar metadados)
  - [ ] POST /treasures/claim (coleta)
  - [ ] Supabase banco + migrations

### Prioridade 🟢 Baixa

- [ ] 🟢 **Tela de carregamento**
- [ ] 🟢 **Música de fundo (loop calmo)**
- [ ] 🟢 **Efeito sonoro do detector (aproximação)**

### Milestone Fase 2 ✅

> MVP funcional: mapa 3D, detector, enterrar/achar, servidor básico.

---

## FASE 3 — CORRENTES + NOTIFICAÇÃO

### Prioridade 🔴 Crítica

- [ ] 🔴 **Editor de correntes de pistas**
  - [ ] UI de criação (pin no mapa + tipo de pista)
  - [ ] Selecionar tipo: charada, enigma, matemática, desafio físico
  - [ ] Validar distância mínima entre pistas (20m)
  - [ ] Salvar corrente + broadcast P2P
- [ ] 🔴 **Navegação por correntes**
  - [ ] Jogador vê só a primeira pista
  - [ ] Cada pista resolvida revela a próxima
  - [ ] Validação de resposta (hash da charada)
  - [ ] Tela de "corrente em andamento"
- [ ] 🔴 **Notificação por passagem (background)**
  - [ ] Geofencing API (Android)
  - [ ] Ciclo de GPS background (60s)
  - [ ] Círculo de aproximação (50m → encolhe)
  - [ ] Push local FCM

### Prioridade 🟡 Média

- [ ] 🟡 **Anti-cheat P2P**
  - [ ] Hash do mini-game + GPS + timestamp
  - [ ] Validação cruzada entre peers
  - [ ] Sistema de reputação do peer
  - [ ] Bloqueio por velocidade > 30 km/h
- [ ] 🟡 **Sistema de denúncia**
  - [ ] Reportar tesouro perigoso
  - [ ] Reportar peer suspeito
  - [ ] Moderação semi-automática

### Prioridade 🟢 Baixa

- [ ] 🟢 **Selo de bairro/região**
- [ ] 🟢 **Ranking semanal (servidor)**
- [ ] 🟢 **Evento surpresa: baús lendários por 24h**

### Milestone Fase 3 ✅

> Correntes funcionando + notificação por passagem + anti-cheat.

---

## FASE 4 — MONETIZAÇÃO

### Prioridade 🔴 Crítica

- [ ] 🔴 **Google Play Billing**
  - [ ] Integrar Android Billing Library
  - [ ] Validação de receipt no servidor
  - [ ] Produtos: cosméticos, passes, moedas
- [ ] 🔴 **App Store IAP**
  - [ ] Integrar StoreKit
  - [ ] Validação de receipt no servidor
- [ ] 🔴 **Loja no jogo**
  - [ ] Catálogo de itens
  - [ ] Compra + confirmação
  - [ ] Inventário do jogador

### Prioridade 🟡 Média

- [ ] 🟡 **Anúncio recompensado**
  - [ ] AdMob / Unity Ads
  - [ ] "Ver anúncio → detector turbo 5 min"
  - [ ] "Ver anúncio → +1 pá"
- [ ] 🟡 **Passe do Caçador (mensal)**
  - [ ] UI do passe
  - [ ] Recompensas diárias/semanais
  - [ ] Renovação automática

### Prioridade 🟢 Baixa

- [ ] 🟢 **Skins de detector, baú, pá**
- [ ] 🟢 **Efeitos especiais de coleta**
- [ ] 🟢 **Emotes para P2P chat**

### Milestone Fase 4 ✅

> Loja funcionando com compras reais + anúncio recompensado + passe.

---

## FASE 5 — TESOURO SENTIMENTAL + REAL + OFFLINE

### Prioridade 🔴 Crítica

- [ ] 🔴 **Tesouro Sentimental**
  - [ ] Campo de história (500 chars)
  - [ ] Gravação de áudio (15s)
  - [ ] Foto do objeto
  - [ ] Feedback obrigatório: texto, áudio, foto, desenho
  - [ ] Notificação ao criador quando receber feedback
  - [ ] Moderação de conteúdo
- [ ] 🔴 **Tesouro de Alto Valor**
  - [ ] Baú Lendário Premium (R$ 49,90 / $14.99)
  - [ ] Validação de nível mínimo (10)
  - [ ] Confirmação do criador (72h)
  - [ ] Badge "Mestre dos Tesouros" / "Caçador de Lendas"
  - [ ] Hall da Fama
- [ ] 🔴 **Modo Offline Completo**
  - [ ] Cache local SQLite (mapa + tesouros)
  - [ ] Fila de sincronização criptografada
  - [ ] Enterrar offline (PENDENTE)
  - [ ] Caçar offline (PENDENTE)
  - [ ] Feedback offline (PENDENTE)
  - [ ] Indicador "Offline" na UI
  - [ ] Backoff exponencial de sincronização

### Prioridade 🟡 Média

- [ ] 🟡 **P2P Contract (criptográfico)**
  - [ ] Geração de par de chaves ECDSA
  - [ ] Assinatura do contrato do tesouro
  - [ ] Validação de claim entre peers
  - [ ] Cadeia de testemunhas
- [ ] 🟡 **Smart Contract Polygon (opcional)**
  - [ ] NFT Proof of Discovery
  - [ ] NFT Legendary Hider
  - [ ] Integração Web3Auth

### Milestone Fase 5 ✅

> Três sistemas novos: sentimental, alto valor, offline.

---

## FASE 6 — B2B + PLUGINS

### Prioridade 🔴 Crítica

- [ ] 🔴 **Portal de lojas parceiras (web)**
  - [ ] Cadastro de loja
  - [ ] Geração de QR code único
  - [ ] Dashboard de estatísticas
  - [ ] Pagamento recorrente (Stripe)
- [ ] 🔴 **Sistema de .themepack**
  - [ ] Schema JSON do pacote
  - [ ] Download + cache de assets
  - [ ] Aplicação de tema no mapa, baús, detector
- [ ] 🔴 **Sistema de .questpack**
  - [ ] Schema JSON da corrente oficial
  - [ ] NPCs, diálogos, animações de conclusão

### Prioridade 🟡 Média

- [ ] 🟡 **Marketplace de pacotes**
  - [ ] Catálogo no app
  - [ ] Compra + download
  - [ ] Comissão 70/30
- [ ] 🟡 **Editor de correntes web**
  - [ ] Drag & drop no mapa
  - [ ] Preview da corrente
  - [ ] Exportar .questpack

### Prioridade 🟢 Baixa

- [ ] 🟢 **3 lojas parceiras piloto**
  - [ ] Padaria
  - [ ] Sorveteria
  - [ ] Papelaria

### Milestone Fase 6 ✅

> Lojas podem criar tesouros físicos. Estúdios vendem temas.

---

## FASE 7 — ONBOARDING + RETENÇÃO

### Prioridade 🔴 Crítica

- [ ] 🔴 **Onboarding < 30s**
  - [ ] Primeira tela: mapa com 3 baús próximos
  - [ ] Andar até um → cavar → ganhar XP
  - [ ] SÓ DEPOIS pede cadastro
- [ ] 🔴 **Mascote guia**
  - [ ] Personagem animado nas primeiras telas
  - [ ] Voz/diálogo guiando o primeiro passo
- [ ] 🔴 **Daily Stroll**
  - [ ] "Ande 500 passos hoje → baú bônus"
  - [ ] Contador de passos (Pedometer API)

### Prioridade 🟡 Média

- [ ] 🟡 **Streak de dias consecutivos**
  - [ ] "Você caçou por 5 dias seguidos!"
  - [ ] Recompensa crescente por streak
- [ ] 🟡 **Notificação contextual**
  - [ ] "Volte amanhã, seu streak vai perder!"
  - [ ] "Tem um tesouro lendário perto de você!"

### Milestone Fase 7 ✅

> Onboarding fluido + hábito diário.

---

## FASE 8 — POLIMENTO + LANÇAMENTO

### Prioridade 🔴 Crítica

- [ ] 🔴 **Testes com 20 crianças (playtest)**
- [ ] 🔴 **Otimização de bateria (GPS background)**
- [ ] 🔴 **Revisão de segurança (geofencing)**
- [ ] 🔴 **Tradução expandida: PT-BR + EN + ES**
- [ ] 🔴 **Publicação Google Play (EHAQUI)**
- [ ] 🔴 **Publicação App Store (ISHERE)**

### Prioridade 🟡 Média

- [ ] 🟡 **Site do jogo (ehaqui.com / ishere.app)**
- [ ] 🟡 **Vídeo de trailer / gameplay**
- [ ] 🟡 **Discord oficial**
- [ ] 🟡 **Campanha TikTok com 5 criadores**

### Prioridade 🟢 Baixa

- [ ] 🟢 **Suporte a tablets**
- [ ] 🟢 **Modo daltônico**
- [ ] 🟢 **Acessibilidade (fontes maiores, contraste)**

### Milestone Fase 8 ✅

> Jogo publicado e rodando com usuários reais.

---

## FASE 9 — PÓS-LANÇAMENTO

- [ ] **Eventos sazonais** (Halloween, Natal, férias)
- [ ] **Pacotes de estúdio** (primeiros parceiros)
- [ ] **Expansão internacional** (mais traduções)
- [ ] **Otimização P2P para grandes áreas**
- [ ] **Modo AR (câmera)** — ver baú flutuando no mundo real
- [ ] **Suporte a iOS**

---

## LEGENDA DE STATUS

```
✅ Concluído
🔄 Em andamento
🔴 Prioridade alta (próximo a fazer)
🟡 Prioridade média
🟢 Prioridade baixa
❌ Bloqueado
```

---

*Última atualização: 11/06/2026*
