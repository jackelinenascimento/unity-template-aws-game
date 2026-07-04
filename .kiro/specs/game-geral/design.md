# Design — Ecto-Escape
**Time:** Ctrl+Alt+Delas · Connect Byte × AWS

---

## 1. Arquitetura Geral

```
┌─────────────────────────────────────────────────────────┐
│                      UNITY (C#)                         │
│                                                         │
│  PlayerController ──── ESP32SerialReader (Singleton)    │
│         │                    │           │              │
│    [botões físicos]    [lê Serial]  [escreve Serial]    │
│         │                    │           │              │
│  Collectible ──────── GameManager   SendTreasure/Damage │
│  LevelEnd   ──────────────┘                             │
└──────────────────┬──────────────────────┬───────────────┘
                   │ USB Serial (115200)  │
┌──────────────────▼──────────────────────▼───────────────┐
│                    ESP32 (C++ Arduino)                  │
│                                                         │
│  loop() ──── Serial.read()  ──── showIdle/Treasure/Damage│
│           └── lê botões     ──── Serial.println(L1/R1/U) │
│                                        │                │
│                                   OLED SSD1306          │
│                                  (I2C GPIO21/22)        │
└─────────────────────────────────────────────────────────┘
```

---

## 2. Padrões de Design Utilizados

| Padrão | Onde | Por quê |
|---|---|---|
| **Singleton** | `ESP32SerialReader`, `GameManager` | Acesso global sem necessidade de referências diretas — qualquer script chama via `.Instance` |
| **Observer implícito** | `Collectible` → `GameManager.OnItemCollected()` | Desacopla o item da lógica de vitória |
| **Command Buffer** | `_jumpBuffered` no `PlayerController` | Evita perda do sinal de pulo quando `FixedUpdate` roda antes do `Update` no mesmo frame |
| **Producer-Consumer** | `ConcurrentQueue` no `ESP32SerialReader` | Thread de leitura Serial (producer) entrega para a main thread (consumer) sem race condition |
| **DontDestroyOnLoad** | `ESP32SerialReader` | Mantém a conexão Serial aberta entre cenas |

---

## 3. Diagrama de Classes

```
MonoBehaviour
├── PlayerController
│   ├── Rigidbody2D       _rb
│   ├── Animator          _animator
│   ├── bool              _isGrounded
│   ├── bool              _jumpBuffered
│   ├── Vector2           _moveInput          ← New Input System
│   ├── float             moveSpeed, jumpForce
│   ├── LayerMask         groundLayer
│   ├── Transform         groundCheck
│   ├── Update()          ← ground check + captura JumpPressed
│   ├── FixedUpdate()     ← aplica velocidade + pulo + animator + flip
│   ├── OnMove(), OnJump() ← callbacks do PlayerInput
│   └── TriggerTreasureReaction/DamageReaction/IdleReaction()
│
├── ESP32SerialReader  [Singleton, DontDestroyOnLoad]
│   ├── SerialPort        _serial
│   ├── Thread            _readThread
│   ├── ConcurrentQueue   _messageQueue
│   ├── bool              ButtonLeft, ButtonRight, ButtonDown
│   ├── bool              JumpPressed, _jumpPending
│   ├── OpenSerial(), CloseSerial(), ReadLoop()
│   ├── ParseIncoming()   ← L1/L0/R1/R0/D1/D0/U
│   └── SendIdle/Treasure/Damage() ← WriteChar(I/T/D)
│
├── GameManager  [Singleton]
│   ├── int               _totalCollectibles, _collectedCount
│   ├── GameObject        gameOverPanel
│   ├── Awake()           ← FindObjectsByType<Collectible>
│   ├── OnItemCollected() ← chamado por Collectible
│   ├── ShowGameOver()    ← painel + timeScale=0 + SendTreasure
│   └── RestartGame()     ← timeScale=1 + LoadScene
│
├── Collectible
│   ├── GameObject        collectEffect (opcional)
│   ├── AudioClip         collectSound  (opcional)
│   └── OnTriggerEnter2D() ← GameManager + SendTreasure + efeito + Destroy
│
├── LevelEnd
│   ├── GameObject        gameOverPanel
│   ├── OnTriggerEnter2D() ← painel + timeScale=0 + SendTreasure
│   └── RestartGame()
│
└── MovingPlatform
    ├── Rigidbody2D       _rb  [Kinematic]
    ├── Vector2           _pointA, _pointB, _target
    ├── float             moveDistance, speed, waitTime
    └── FixedUpdate()     ← MovePosition + inversão de direção
```

---

## 4. Design da Cena Principal

### Hierarchy

```
Scene
├── Main Camera
├── EventSystem
├── Canvas
│   ├── ScoreText (TMP)          ← "Data Cores: 0/3"
│   └── GameOverPanel (inactive)
│       ├── TitleText (TMP)      ← "ESCAPOU!"
│       └── BtnReiniciar         ← OnClick → GameManager.RestartGame()
├── GameManager
├── ESP32SerialReader
├── Player (Blink)
│   ├── SpriteRenderer           ← personagem.png
│   ├── Rigidbody2D              ← GravityScale:3, FreezeRotZ
│   ├── CapsuleCollider2D        ← Material: Material.physicsMaterial2D
│   ├── Animator                 ← Player.controller
│   ├── PlayerController
│   ├── PlayerInput              ← InputSystem_Actions, Send Messages
│   └── GroundCheck (empty)      ← posicionado nos pés
└── Level
    ├── Ground                   ← BoxCollider2D, Layer:Ground
    ├── Platforms
    │   ├── Plataforma_01
    │   ├── Plataforma_02 (MovingPlatform)
    │   └── Plataforma_03
    ├── Cenário (visual only)
    ├── Collectibles
    │   ├── DataCore_01          ← Collectible.cs, Collider2D IsTrigger
    │   ├── DataCore_02
    │   └── DataCore_03
    ├── Obstacles
    │   ├── LaserSeguranca       ← Obstacle.cs, Collider2D IsTrigger
    │   ├── DronePatrulha
    │   └── AcidoFluorescente
    └── LevelEnd (portal)        ← LevelEnd.cs, Collider2D IsTrigger
```

### Layout da Fase (progressão sugerida)

```
[INÍCIO] → DataCore_01 → LaserSeguranca → Plataforma_02(moving)
        → DataCore_02 → DronePatrulha   → AcidoFluorescente
        → DataCore_03 → [PORTAL DE FUGA / LevelEnd]
```

---

## 5. Design Visual — Universo Ecto-Escape

### Paleta de cores

| Elemento | Cor principal | Cor de destaque |
|---|---|---|
| Blink (corpo) | Branco `#FFFFFF` | Azul claro `#A0D8EF` |
| Laboratório (fundo) | Azul escuro `#0D1B2A` | Roxo `#3A2A5E` |
| Data Cores | Ciano `#00FFFF` | Branco `#FFFFFF` |
| Lasers | Vermelho `#FF2B2B` | Laranja `#FF7700` |
| Neons / UI | Verde `#00FF88` | Azul `#4DFFFF` |
| Plataformas | Cinza metálico `#4A5568` | Borda `#718096` |

### Estilo dos assets

- **Flat vector** com bordas suaves — sem texturas realistas
- Fundo transparente em todos os sprites
- Iluminação sugerida por gradientes e halos nos neons
- Blink: forma arredondada, olhos expressivos grandes, sem membros

---

## 6. Protocolo de Comunicação Serial

### ESP32 → Unity

| Mensagem | Tipo | Significado |
|---|---|---|
| `L1` | estado | Botão Esquerda pressionado |
| `L0` | estado | Botão Esquerda solto |
| `R1` | estado | Botão Direita pressionado |
| `R0` | estado | Botão Direita solto |
| `D1` | estado | Botão Baixo pressionado (reservado) |
| `D0` | estado | Botão Baixo solto (reservado) |
| `U` | evento | Pulo (borda — um disparo por pressão) |

### Unity → ESP32

| Char | Expressão OLED | Quando enviar |
|---|---|---|
| `I` | Neutra `(O_O)` | Jogo iniciando / estado padrão |
| `T` | Feliz `(^u^)` | Data Core coletado / vitória |
| `D` | Assustada `(>_<)` | Blink colide com obstáculo |

### Diagrama de ligação OLED (I2C)

```
OLED VCC  →  ESP32 3V3
OLED GND  →  ESP32 GND
OLED SDA  →  ESP32 GPIO21
OLED SCL  →  ESP32 GPIO22
```

---

## 7. Firmware ESP32 — Estrutura

```cpp
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

Adafruit_SSD1306 display(128, 64, &Wire, -1);

void setup() {
    Serial.begin(115200);
    display.begin(SSD1306_SWITCHCAPVCC, 0x3C);
    showIdle();
}

void loop() {
    // 1. Lê reações do Unity → OLED
    if (Serial.available()) {
        char c = Serial.read();
        if      (c == 'I') showIdle();
        else if (c == 'T') showTreasure();
        else if (c == 'D') showDamage();
    }

    // 2. Lê botões físicos → Unity
    // btnLeft  → Serial.println("L1") / Serial.println("L0")
    // btnRight → Serial.println("R1") / Serial.println("R0")
    // btnJump  → Serial.println("U")  (apenas na borda de pressão)
}

// 3 funções de expressão — clearDisplay + desenha + display()
void showIdle()     { /* boca reta */ }
void showTreasure() { /* boca sorrindo */ }
void showDamage()   { /* boca invertida */ }
```

---

## 8. Decisões de Design

| Decisão | Alternativa descartada | Motivo |
|---|---|---|
| Thread dedicada para Serial | Polling no Update | Evita travar o frame quando Serial está lenta |
| `ConcurrentQueue` para mensagens | `lock` manual | API mais simples e thread-safe nativa |
| Buffer de pulo (`_jumpBuffered`) | Ler `JumpPressed` direto no FixedUpdate | FixedUpdate pode rodar antes do Update e perder o sinal |
| `Physics2D.OverlapCircle` para chão | `OnCollisionEnter2D` | Mais confiável em plataformers — não depende de evento de colisão |
| `Singleton` para GameManager e ESP32 | Referências diretas via Inspector | Qualquer script pode chamar sem dependência de configuração manual |
| `CapsuleCollider2D` no Blink | `BoxCollider2D` | Bordas arredondadas — menos travamento nas quinas das plataformas |
