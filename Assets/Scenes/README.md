# 🗺️ Scenes — Ctrl+Alt+Delas · Ecto-Escape

Fases do jogo. Cada arquivo `.unity` é uma cena independente.

---

## Cenas existentes

| Arquivo | Descrição |
|---|---|
| `Ecto-Scape.unity` | Cena principal jogável do projeto |
| `SampleScene.unity` | Cena base/minimalista preservada apenas como template URP |

---

## 🎯 Hierarchy da cena principal jogável (`Ecto-Scape.unity`)

```
Scene
├── Main Camera
├── EventSystem
│
├── Canvas                        ← HUD
│   ├── ScoreText (TMP)           ← contagem de Data Cores
│   └── GameOverPanel             ← painel de vitória (inativo no início)
│       └── BtnReiniciar          ← chama GameManager.RestartGame()
│
├── GameManager                   ← script GameManager.cs
├── ESP32SerialReader             ← script ESP32SerialReader.cs
│
├── Player (Blink)                ← PlayerController + Rigidbody2D + Animator + PlayerInput
│   └── GroundCheck               ← Transform filho nos pés do Blink
│
└── Level
    ├── Ground                    ← Sprite + BoxCollider2D + Layer: Ground
    ├── Platforms                 ← plataformas estáticas e móveis (MovingPlatform.cs)
    ├── Cenário                   ← decoração visual (sem Collider)
    ├── Collectibles              ← Data Cores
    │   ├── DataCore_01           ← Collectible.cs + Collider2D Is Trigger
    │   ├── DataCore_02
    │   └── DataCore_03
    ├── Obstacles                 ← obstáculos do laboratório
    │   ├── LaserSeguranca        ← Obstacle.cs + Collider2D Is Trigger
    │   ├── DronePatrulha
    │   └── AcidoFluorescente
    └── LevelEnd                  ← LevelEnd.cs + Collider2D Is Trigger (portal de fuga)
```

---

## ⚙️ Configurações obrigatórias

### Tags e Layers

| Objeto | Configuração |
|---|---|
| `Player` (Blink) | **Tag:** `Player` |
| `Ground` e todas as plataformas | **Layer:** `Ground` |

### Inspector — GameManager

| Campo | O que arrastar / escrever |
|---|---|
| `Game Over Panel` | GameObject `GameOverPanel` do Canvas |
| `Scene Name` | nome exato da cena (confira em File → Build Settings) |

### Inspector — ESP32SerialReader

| Campo | Valor |
|---|---|
| `Port Name` | Windows: `COM3` · Mac/Linux: `/dev/tty.usbserial-XXXX`, `/dev/ttyUSB0`, `/dev/ttyACM0` ou vazio para auto-detect |
| `Baud Rate` | `115200` |
| `Auto Connect On Start` | ✅ para tentar hardware automaticamente, `off` para rodar só local |
| `Auto Detect Port On Unix` | ✅ recomendado |

### Inspector — PlayerController (no Blink)

| Campo | O que configurar |
|---|---|
| `Ground Layer` | selecionar Layer `Ground` |
| `Ground Check` | arrastar o filho `GroundCheck` |
| `Force Keyboard Fallback` | ✅ marcar para testar sem ESP32 |

### Inspector — LevelEnd (portal)

| Campo | O que arrastar / escrever |
|---|---|
| `Game Over Panel` | mesmo painel do Canvas |
| `Scene Name` | mesmo nome da cena |

---

## ✅ Teste antes de considerar a cena pronta

1. **Play** → Blink aparece sobre o chão sem cair.
2. Teclas `A`/`D` ou setas movem o Blink no Unix mesmo sem ESP32 conectado.
3. `Espaço` faz o Blink pular.
4. Coletar os 3 Data Cores exibe o painel de vitória e pausa o jogo.
5. Tocar num obstáculo chama `SendDamage()` (verifique na Console).
6. Chegar no `LevelEnd` exibe o painel de vitória.

---

## ➕ Como criar uma nova cena

1. `File → New Scene` → template **2D (Built-in)**.
2. Salve nesta pasta com nome descritivo (ex: `Level02.unity`).
3. Adicione em `File → Build Settings` para poder referenciar pelo nome.

> ⚠️ O nome da cena em `GameManager` e `LevelEnd` deve bater **exatamente** com o de Build Settings — incluindo maiúsculas/minúsculas.

> ℹ️ No estado atual do projeto, apenas `Ecto-Scape.unity` fica no Build Settings. `SampleScene.unity` foi mantida apenas como template/base de referência.
