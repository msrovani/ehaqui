# Roadmap — EHAQUI / ISHERE

## Visão geral

```
FASE 1 ───► FASE 2 ───► FASE 3 ───► FASE 4 ───► FASE 5 ───► FASE 6 ───► FASE 7 ───► FASE 8 ───► FASE 9
  POC        MVP        Correntes    Monetiz.    3 Novos      B2B         Retenção    Polimento   Pós
  (4 sem)    (8 sem)    (4 sem)      (4 sem)     (6 sem)      (8 sem)     (4 sem)     (6 sem)     (contínuo)
```

**Dual naming:** Google Play → EHAQUI (🇧🇷) · App Store → ISHERE (🇺🇸)

---

## FASE 1 — Prova de Conceito (4 semanas)

**Objetivo:** 2 celulares se conectando via P2P, um enterra e outro acha.

| Semana | Entregáveis |
|---|---|
| 1 | LiteNetLib/ENet: handshake, HELLO, PING, mensagens básicas |
| 2 | GPS: captura de coordenadas, cálculo de distância (Haversine) |
| 3 | Detector v1: vibração progressiva em 4 estágios |
| 4 | Enterrar + achar via P2P, mini-game simples |

**Milestone:** ✅ Dois celulares — um enterra, outro acha. P2P puro, sem servidor.

---

## FASE 2 — MVP Funcional (8 semanas)

**Objetivo:** App jogável com mapa 3D, detector, servidor básico.

| Semana | Entregáveis |
|---|---|
| 1–2 | Mapa 3D low-poly (OSM + Mapbox) |
| 3 | UI principal: mapa, detector, botões de ação |
| 4 | Mini-game de desenterrar (swipe + animação) |
| 5 | TTL + prêmio crescente |
| 6–7 | Servidor enxuto (auth + API tesouros) |
| 8 | Perfil do jogador + integração |

**Milestone:** ✅ MVP jogável com mapa, detector, servidor.

---

## FASE 3 — Correntes + Notificação (4 semanas)

**Objetivo:** Correntes de pistas e notificação por passagem.

| Semana | Entregáveis |
|---|---|
| 1 | Editor de correntes (UI + validação) |
| 2 | Navegação por correntes (pista → pista) |
| 3 | Notificação por passagem (geofencing + push) |
| 4 | Anti-cheat P2P + sistema de denúncia |

**Milestone:** ✅ Correntes + notificação + anti-cheat.

---

## FASE 4 — Monetização (4 semanas)

**Objetivo:** Loja funcionando com dinheiro real.

| Semana | Entregáveis |
|---|---|
| 1 | Google Play Billing + validação de receipt |
| 2 | App Store IAP + validação de receipt |
| 3 | Loja no jogo (catálogo + compra + inventário) |
| 4 | Anúncio recompensado + Passe do Caçador |

**Milestone:** ✅ Loja, anúncios, passe mensal.

---

## FASE 5 — Sentimental + Alto Valor + Offline (6 semanas)

**Objetivo:** Três sistemas novos e complexos.

| Semana | Entregáveis |
|---|---|
| 1–2 | Tesouro Sentimental (história, áudio, feedback, moderação) |
| 3–4 | Tesouro de Alto Valor (Baú Premium, badges, Hall da Fama) |
| 5–6 | Modo Offline (cache SQLite, fila criptografada, sincronização) |

**Milestone:** ✅ Três novos sistemas entregues.

---

## FASE 6 — B2B + Plugins (8 semanas)

**Objetivo:** Lojas e estúdios podem entrar na plataforma.

| Semana | Entregáveis |
|---|---|
| 1–2 | Portal de lojas parceiras (web + QR + dashboard) |
| 3–4 | Sistema de .themepack (schema + download + aplicação) |
| 5 | Sistema de .questpack (schema + NPCs + animações) |
| 6 | Marketplace de pacotes (catálogo + compra + comissão) |
| 7 | Editor de correntes web |
| 8 | 3 lojas parceiras piloto |

**Milestone:** ✅ Plataforma B2B operacional.

---

## FASE 7 — Onboarding + Retenção (4 semanas)

**Objetivo:** Primeira experiência < 30s, hábito diário.

| Semana | Entregáveis |
|---|---|
| 1 | Onboarding value-first (3 baús, sem cadastro) |
| 2 | Mascote guia |
| 3 | Daily Stroll + streak |
| 4 | Notificações contextuais |

**Milestone:** ✅ Onboarding fluido, retenção otimizada.

---

## FASE 8 — Polimento + Lançamento (6 semanas)

**Objetivo:** Jogo pronto para publicação.

| Semana | Entregáveis |
|---|---|
| 1 | Playtest com 20 crianças |
| 2 | Correções de bugs + otimização de bateria |
| 3 | Revisão de segurança + geofencing |
| 4 | Traduções (PT-BR, EN, ES) |
| 5 | Publicação Google Play (EHAQUI) |
| 6 | Publicação App Store (ISHERE) |

**Milestone:** ✅ Jogo publicado nas duas lojas.

---

## FASE 9 — Pós-Lançamento (contínuo)

**Objetivo:** Crescimento e expansão.

| Atividade | Prazo |
|---|---|
| Eventos sazonais (Halloween, Natal) | Trimestral |
| Primeiros pacotes de estúdio | Mês 1 pós-lançamento |
| Expansão internacional | Mês 3 |
| Modo AR (câmera) | Mês 6 |
| Suporte iOS | Lançamento |
| Otimização P2P para grandes áreas | Contínuo |

---

## Timeline resumida

```
Mês 1  │ POC (P2P + GPS)
Mês 2  │ MVP (mapa + detector + servidor)
Mês 3  │ Correntes + notificação
Mês 4  │ Monetização
Mês 5  │ Sentimental + Alto Valor + Offline
Mês 6  │ 
Mês 7  │ B2B + Plugins
Mês 8  │ 
Mês 9  │ Onboarding + Retenção
Mês 10 │ Polimento + Lançamento
Mês 11 │ 
Mês 12+ │ Pós-lançamento
```

**Tempo total até o lançamento: ~10 meses** (42 semanas de desenvolvimento).

---

*Roadmap gerado em 11/06/2026.*
